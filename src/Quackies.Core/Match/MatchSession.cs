using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Quackies.Core.Tokens;

namespace Quackies.Core.Match
{
    /// <summary>
    /// Coordinates a two-player match. Phase handlers own phase rules; the
    /// session owns state, action validation and transitions between them.
    /// </summary>
    public sealed class MatchSession
    {
        private static readonly HashSet<int> RatBoundaries = new HashSet<int>
        { 1, 4, 7, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48 };

        private readonly List<PlayerRoundState> _players;
        private readonly Dictionary<ShopChipDefinition, int> _supply;
        private readonly List<IRoundEventRule> _eventDeck;
        private readonly List<MatchLogEntry> _history = new List<MatchLogEntry>();
        private readonly List<DieRollView> _dieRolls = new List<DieRollView>();
        private readonly IReadOnlyList<Token> _startingBag;
        private readonly Dictionary<string, GameActionKind> _roundNineCommits = new Dictionary<string, GameActionKind>(StringComparer.Ordinal);
        private readonly HashSet<string> _roundCapabilitiesUsed = new HashSet<string>(StringComparer.Ordinal);
        private readonly BrewingPhaseHandler _brewing;
        private readonly EvaluationPhaseHandler _evaluation;
        private readonly ShoppingPhaseHandler _shopping;
        private readonly RubySpendingPhaseHandler _rubySpending;
        private IRoundEventRule? _currentEvent;
        private long _nextChoiceSequence;
        private int _bonusDieRolls = 1;

        private MatchSession(IRandomSource random, RuleSet rules, MatchSettings settings)
        {
            Random = random ?? throw new ArgumentNullException(nameof(random));
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _players = new List<PlayerRoundState>
            {
                new PlayerRoundState("human", "Human", settings.StartingRubies),
                new PlayerRoundState("ai", "AI", settings.StartingRubies)
            };
            _startingBag = MatchView.Freeze(_players[0].Inventory.Select(chip => new Token(chip.Color, chip.Value)));
            _supply = rules.ShopChips.ToDictionary(chip => chip, chip => chip.Stock);
            _eventDeck = rules.RoundEvents.ToList();
            _brewing = new BrewingPhaseHandler(this);
            _evaluation = new EvaluationPhaseHandler(this);
            _shopping = new ShoppingPhaseHandler(this);
            _rubySpending = new RubySpendingPhaseHandler(this);
            StartRound(1);
        }

        public static MatchSession Create(IRandomSource random, RuleSet? rules = null) =>
            new MatchSession(random, rules ?? RuleSet.SetOne(), MatchSettings.Standard);

        public static MatchSession Create(IRandomSource random, MatchSettings settings, RuleSet? rules = null) =>
            new MatchSession(random, rules ?? RuleSet.SetOne(), settings);

        public int Round { get; private set; }
        public MatchPhase Phase { get; private set; }
        public MatchSettings Settings { get; }
        internal IRandomSource Random { get; }
        internal RuleSet Rules { get; }
        internal IReadOnlyList<PlayerRoundState> Players => _players;
        internal int StartPlayerIndex => (Round - 1) % _players.Count;

        public MatchView GetSnapshot(string viewerId)
        {
            var viewer = Player(viewerId);
            var playerViews = _players.Select(player => new PlayerView(
                player.Id, player.Name, player.Points, player.Rubies, player.Coins, player.WhiteTotal,
                player.ExplosionThreshold, player.Position, player.Droplet, player.RatPosition,
                player.Bag.Count, player.Inventory.Count, player.FlaskFull, player.Stopped, player.Exploded,
                player.Pot.Select(chip => new PlacedChipView(chip.Token.Color, chip.Token.Value, chip.Position)),
                player.ScoringSpaceAtStop ?? Rules.Track.ScoringSpace(player.Position)));
            var offers = Rules.ShopChips
                .Where(chip => chip.AvailableFromRound <= Round)
                .Select(chip => new ShopOffer(chip.Color, chip.Value, chip.Price, _supply[chip], Rules.Ingredients[chip.Color].Description));
            var winners = Phase == MatchPhase.Finished ? DetermineWinners().Select(player => player.Id) : Array.Empty<string>();

            return new MatchView(Round, Phase, viewer.Id, _currentEvent?.Id ?? string.Empty,
                _currentEvent?.Title ?? string.Empty, _currentEvent?.Description ?? string.Empty,
                playerViews, viewer.Bag.ToArray(), _startingBag, offers, _history, _dieRolls, winners,
                _roundNineCommits.Count > 0);
        }

        public IReadOnlyList<GameAction> GetLegalActions(string playerId)
        {
            var player = Player(playerId);
            IEnumerable<GameAction> actions;
            if (player.Choices.Count > 0)
            {
                actions = ChoiceActions(player);
            }
            else
            {
                switch (Phase)
                {
                    case MatchPhase.Brewing:
                        if (Round == 9 && _players.Any(candidate => candidate.Choices.Count > 0)) actions = Array.Empty<GameAction>();
                        else if (_roundNineCommits.ContainsKey(player.Id)) actions = Array.Empty<GameAction>();
                        else actions = _brewing.GetLegalActions(player);
                        break;
                    case MatchPhase.Shopping: actions = _shopping.GetLegalActions(player); break;
                    case MatchPhase.RubySpending: actions = _rubySpending.GetLegalActions(player); break;
                    case MatchPhase.RoundComplete:
                        actions = new[] { new GameAction("next-round", GameActionKind.NextRound, "Start next round") };
                        break;
                    default: actions = Array.Empty<GameAction>(); break;
                }
            }
            return new ReadOnlyCollection<GameAction>(actions.ToList());
        }

        public MatchView Execute(string playerId, GameAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var player = Player(playerId);
            var legal = GetLegalActions(playerId).SingleOrDefault(candidate => string.Equals(candidate.Id, action.Id, StringComparison.Ordinal));
            if (legal == null) throw new InvalidOperationException($"Action '{action.Id}' is not legal for {playerId} in {Phase}.");

            if (player.Choices.Count > 0)
            {
                ExecuteChoice(player, legal);
                ContinueAfterChoice();
            }
            else
            {
                switch (Phase)
                {
                    case MatchPhase.Brewing: ExecuteBrewing(player, legal); break;
                    case MatchPhase.Shopping: _shopping.Execute(player, legal); break;
                    case MatchPhase.RubySpending: _rubySpending.Execute(player, legal); break;
                    case MatchPhase.RoundComplete: StartRound(Round + 1); break;
                    default: throw new InvalidOperationException($"No actions can be executed during {Phase}.");
                }
            }
            return GetSnapshot(playerId);
        }

        internal PlayerRoundState Player(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return _players.SingleOrDefault(player => string.Equals(player.Id, id, StringComparison.Ordinal))
                ?? throw new ArgumentException($"Unknown player ID '{id}'.", nameof(id));
        }

        internal PlayerRoundState OpponentOf(PlayerRoundState player) => _players.Single(candidate => candidate != player);

        internal void AddLog(string message) => AddLog(string.Empty, message);

        internal void AddLog(string actorId, string message)
        {
            _history.Add(new MatchLogEntry(Round, actorId, message));
        }

        internal void GainPoints(PlayerRoundState player, int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            player.Points += amount;
            if (amount > 0) AddLog(player.Id, $"{player.Name} gained {amount} victory point(s).");
        }

        internal void GainRubies(PlayerRoundState player, int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            player.Rubies += amount;
            if (amount > 0) AddLog(player.Id, $"{player.Name} gained {amount} ruby/rubies.");
        }

        internal void AdvanceDroplet(PlayerRoundState player, int spaces)
        {
            if (spaces < 0) throw new ArgumentOutOfRangeException(nameof(spaces));
            var previous = player.Droplet;
            player.Droplet = Math.Min(player.Droplet + spaces, Rules.Track.LastChipPosition);
            if (player.Droplet > previous) AddLog(player.Id, $"{player.Name} advanced their droplet {player.Droplet - previous} space(s).");
        }

        internal void AdvanceLastChip(PlayerRoundState player, int spaces)
        {
            if (spaces < 0) throw new ArgumentOutOfRangeException(nameof(spaces));
            if (player.Pot.Count == 0) throw new InvalidOperationException("There is no placed chip to advance.");
            var previous = player.Pot[player.Pot.Count - 1].Position;
            player.Pot[player.Pot.Count - 1].Position = Math.Min(player.Pot[player.Pot.Count - 1].Position + spaces,
                Rules.Track.LastChipPosition);
            var advanced = player.Pot[player.Pot.Count - 1].Position - previous;
            if (advanced > 0) AddLog(player.Id, $"{player.Name}'s last chip advanced {advanced} extra space(s).");
        }

        internal void RefillFlask(PlayerRoundState player)
        {
            if (player.FlaskFull) return;
            player.FlaskFull = true;
            AddLog(player.Id, $"{player.Name}'s flask refilled.");
        }

        internal TrackSpaceView ScoringSpace(PlayerRoundState player) =>
            player.ScoringSpaceAtStop ?? Rules.Track.ScoringSpace(player.Position);

        internal void SetExplosionThreshold(PlayerRoundState player, int threshold)
        {
            if (threshold < 0) throw new ArgumentOutOfRangeException(nameof(threshold));
            player.ExplosionThreshold = threshold;
            AddLog(player.Id, $"{player.Name}'s explosion threshold is {threshold} this round.");
        }

        internal void SetBonusDieRolls(int rolls)
        {
            if (rolls < 1) throw new ArgumentOutOfRangeException(nameof(rolls));
            _bonusDieRolls = rolls;
        }

        internal int BonusDieRolls => _bonusDieRolls;
        internal void RollDie(PlayerRoundState player, DieRollReason reason, bool addRewardChipToCurrentBag) =>
            _evaluation.RollDie(player, reason, addRewardChipToCurrentBag);

        internal void RecordDieRoll(PlayerRoundState player, int face, DieRollReason reason, bool rewardApplied,
            string description)
        {
            _dieRolls.Add(new DieRollView(_dieRolls.Count + 1, Round, player.Id, face, reason, rewardApplied, description));
        }

        internal bool TryGiveSupplyChip(PlayerRoundState player, TokenColor color, int value, bool addToCurrentBag = false)
        {
            var definition = Rules.ShopChips.SingleOrDefault(chip => chip.Color == color && chip.Value == value);
            if (definition == null || definition.AvailableFromRound > Round || _supply[definition] == 0) return false;
            _supply[definition]--;
            var chip = new Token(color, value);
            player.Inventory.Add(chip);
            if (addToCurrentBag) player.Bag.Add(chip);
            AddLog(player.Id, $"{player.Name} received {chip}.");
            return true;
        }

        internal bool CanTakeSupplyChip(TokenColor color, int value)
        {
            var definition = Rules.ShopChips.SingleOrDefault(chip => chip.Color == color && chip.Value == value);
            return definition != null && definition.AvailableFromRound <= Round && _supply[definition] > 0;
        }

        internal bool CanExchangeRubyForSupplyChip(PlayerRoundState player, TokenColor color, int value) =>
            player.Rubies > 0 && CanTakeSupplyChip(color, value);

        internal bool TryExchangeRubyForSupplyChip(PlayerRoundState player, TokenColor color, int value)
        {
            if (!CanExchangeRubyForSupplyChip(player, color, value)) return false;
            player.Rubies--;
            if (TryGiveSupplyChip(player, color, value, addToCurrentBag: true))
            {
                AddLog(player.Id, $"{player.Name} exchanged 1 ruby for a {color} {value} chip.");
                return true;
            }
            player.Rubies++;
            return false;
        }

        internal IReadOnlyList<Token> PreviewBag(PlayerRoundState player, int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            var remaining = player.Bag.ToList();
            var preview = new List<Token>();
            while (preview.Count < count && remaining.Count > 0)
            {
                var index = Random.NextInt(remaining.Count);
                preview.Add(remaining[index]);
                remaining.RemoveAt(index);
            }
            AddLog(player.Id, $"{player.Name} previewed {preview.Count} chip(s) from their bag.");
            return MatchView.Freeze(preview);
        }

        internal bool HasHigherSupplyChip(TokenColor color, int value) => NextSupplyChip(color, value) != null;

        internal bool CanUpgradeBagChip(PlayerRoundState player, TokenColor color, int value)
        {
            var next = NextSupplyChip(color, value);
            return next != null && next.AvailableFromRound <= Round && _supply[next] > 0 &&
                player.Bag.Any(chip => chip.Color == color && chip.Value == value);
        }

        internal bool TryUpgradeBagChip(PlayerRoundState player, TokenColor color, int value)
        {
            var next = NextSupplyChip(color, value);
            if (next == null || !CanUpgradeBagChip(player, color, value)) return false;
            var oldChip = player.Bag.First(chip => chip.Color == color && chip.Value == value);
            var oldDefinition = Rules.ShopChips.SingleOrDefault(chip => chip.Color == color && chip.Value == value);
            player.Bag.Remove(oldChip);
            player.Inventory.Remove(oldChip);
            if (oldDefinition != null) _supply[oldDefinition]++;
            _supply[next]--;
            var upgraded = new Token(next.Color, next.Value);
            player.Inventory.Add(upgraded);
            player.Bag.Add(upgraded);
            AddLog(player.Id, $"{player.Name} exchanged {oldChip} for {upgraded}.");
            return true;
        }

        private ShopChipDefinition? NextSupplyChip(TokenColor color, int value) => Rules.ShopChips
            .Where(chip => chip.Color == color && chip.Value > value)
            .OrderBy(chip => chip.Value)
            .FirstOrDefault();

        internal bool RemoveInventoryChip(PlayerRoundState player, TokenColor color, int value)
        {
            var chip = player.Inventory.FirstOrDefault(candidate => candidate.Color == color && candidate.Value == value);
            if (chip == null) return false;
            player.Inventory.Remove(chip);
            player.Bag.Remove(chip);
            var definition = Rules.ShopChips.SingleOrDefault(candidate => candidate.Color == color && candidate.Value == value);
            if (definition != null) _supply[definition]++;
            AddLog(player.Id, $"{player.Name} removed {chip} from their bag.");
            return true;
        }

        internal bool TryUseOnceThisRound(PlayerRoundState player, string capabilityId)
        {
            if (string.IsNullOrWhiteSpace(capabilityId))
                throw new ArgumentException("A round capability needs a stable ID.", nameof(capabilityId));
            return _roundCapabilitiesUsed.Add($"{player.Id}:{capabilityId}");
        }

        internal bool TryUseOnceThisRound(string capabilityId)
        {
            if (string.IsNullOrWhiteSpace(capabilityId))
                throw new ArgumentException("A round capability needs a stable ID.", nameof(capabilityId));
            return _roundCapabilitiesUsed.Add($"match:{capabilityId}");
        }

        internal void ReturnLastPlacedChip(PlayerRoundState player)
        {
            if (player.Pot.Count == 0) throw new InvalidOperationException("There is no placed chip to return.");
            var last = player.Pot[player.Pot.Count - 1];
            player.Pot.RemoveAt(player.Pot.Count - 1);
            player.Bag.Add(last.Token);
            if (last.Token.Color == TokenColor.White) player.WhiteTotal -= last.Token.Value;
            player.Exploded = false;
            player.Stopped = false;
            player.MayUseFlask = false;
            AddLog(player.Id, $"{player.Name} returned {last.Token} to the bag without using their flask.");
        }

        internal void OfferSequentialFortuneBagSelections(IEnumerable<PlayerRoundState> orderedPlayers, int count, string title)
        {
            if (orderedPlayers == null) throw new ArgumentNullException(nameof(orderedPlayers));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            var remainingPlayers = new Queue<PlayerRoundState>(orderedPlayers);

            void OfferNext()
            {
                if (remainingPlayers.Count == 0) return;
                var player = remainingPlayers.Dequeue();
                var candidates = new List<Token>();
                for (var index = 0; index < count && player.Bag.Count > 0; index++)
                {
                    var bagIndex = Random.NextInt(player.Bag.Count);
                    candidates.Add(player.Bag[bagIndex]);
                    player.Bag.RemoveAt(bagIndex);
                }
                if (candidates.Count == 0)
                {
                    OfferNext();
                    return;
                }

                var options = new List<ChoiceOption>();
                for (var index = 0; index < candidates.Count; index++)
                {
                    var selectedIndex = index;
                    var selected = candidates[index];
                    options.Add(new ChoiceOption($"fortune-bag-{index}", $"Place {selected}", () =>
                    {
                        for (var candidateIndex = 0; candidateIndex < candidates.Count; candidateIndex++)
                            if (candidateIndex != selectedIndex) player.Bag.Add(candidates[candidateIndex]);
                        _brewing.PlaceChip(player, selected, resolveIngredient: false, mayExplode: false,
                            ChipPlacementSource.FortuneCard);
                        OfferNext();
                    }, selected.Color, selected.Value));
                }
                options.Add(new ChoiceOption("fortune-none", "Return all previewed chips", () =>
                {
                    player.Bag.AddRange(candidates);
                    OfferNext();
                }));
                Offer(player, title, options.ToArray());
            }

            OfferNext();
        }

        internal int RatStepsForCurrentRound(PlayerRoundState player)
        {
            if (player.RatStepEntitlement.HasValue) return player.RatStepEntitlement.Value;
            if (Round < 2) return 0;
            return CountRatTails(player.Points, _players.Max(candidate => candidate.Points));
        }

        internal void SetRatStepsForCurrentRound(PlayerRoundState player, int steps)
        {
            if (steps < 0) throw new ArgumentOutOfRangeException(nameof(steps));
            player.RatStepEntitlement = steps;
        }

        internal int Remaining(ShopChipDefinition chip) => _supply[chip];
        internal void TakeFromSupply(ShopChipDefinition chip) => _supply[chip]--;

        internal void Offer(PlayerRoundState player, string title, params ChoiceOption[] choices)
        {
            if (choices == null || choices.Length == 0) throw new ArgumentException("At least one choice is required.", nameof(choices));
            if (choices.GroupBy(choice => choice.Id, StringComparer.Ordinal).Any(group => group.Count() != 1))
                throw new ArgumentException("Choice option IDs must be unique.", nameof(choices));
            player.Choices.Enqueue(new PendingChoice(++_nextChoiceSequence, title, choices));
        }

        internal void OfferBagSelection(PlayerRoundState player, int count, string title, bool optional)
        {
            var candidates = new List<Token>();
            for (var index = 0; index < count && player.Bag.Count > 0; index++)
            {
                var bagIndex = Random.NextInt(player.Bag.Count);
                candidates.Add(player.Bag[bagIndex]);
                player.Bag.RemoveAt(bagIndex);
            }
            if (candidates.Count == 0) return;

            var options = new List<ChoiceOption>();
            for (var index = 0; index < candidates.Count; index++)
            {
                var selectedIndex = index;
                var selected = candidates[index];
                options.Add(new ChoiceOption($"bag-{index}", $"Place {selected}", () =>
                {
                    for (var candidateIndex = 0; candidateIndex < candidates.Count; candidateIndex++)
                        if (candidateIndex != selectedIndex) player.Bag.Add(candidates[candidateIndex]);
                    _brewing.PlaceChip(player, selected, resolveIngredient: true, mayExplode: true);
                }, selected.Color, selected.Value));
            }
            if (optional)
                options.Add(new ChoiceOption("none", "Return all previewed chips", () => player.Bag.AddRange(candidates)));
            Offer(player, title, options.ToArray());
        }

        internal void FinishShoppingIfReady()
        {
            if (_players.All(player => player.ShoppingDone)) EnterRubySpending();
        }

        internal void FinishRubySpendingIfReady()
        {
            if (!_players.All(player => player.RubiesDone)) return;
            _currentEvent?.OnRoundEnded(new RoundEventContext(this));
            Phase = Round == 9 ? MatchPhase.Finished : MatchPhase.RoundComplete;
            AddLog(Round == 9 ? "The match is complete." : $"Round {Round} is complete.");
        }

        private IEnumerable<GameAction> ChoiceActions(PlayerRoundState player)
        {
            var choice = player.Choices.Peek();
            return choice.Options.Where(option => option.IsAvailable()).Select(option => new GameAction($"choose:{choice.Sequence}:{option.Id}", GameActionKind.Choose,
                option.Label, choice.Title, option.Color, option.Value));
        }

        private void ExecuteChoice(PlayerRoundState player, GameAction action)
        {
            var choice = player.Choices.Dequeue();
            var prefix = $"choose:{choice.Sequence}:";
            if (!action.Id.StartsWith(prefix, StringComparison.Ordinal))
                throw new InvalidOperationException("That choice belongs to an earlier decision.");
            var optionId = action.Id.Substring(prefix.Length);
            var option = choice.Options.Single(candidate => string.Equals(candidate.Id, optionId, StringComparison.Ordinal));
            AddLog(player.Id, $"{player.Name} chose: {option.Label}.");
            option.Apply();
        }

        private void ContinueAfterChoice()
        {
            if (_players.Any(player => player.Choices.Count > 0)) return;
            if (Phase == MatchPhase.Preparation) FinishPreparation();
            else if (Phase == MatchPhase.Brewing) FinishBrewingIfReady();
            else if (Phase == MatchPhase.Evaluation) _evaluation.FinishIfReady();
        }

        private void ExecuteBrewing(PlayerRoundState player, GameAction action)
        {
            if (Round != 9 || action.Kind == GameActionKind.UseFlask || _players.Count(candidate => !candidate.Stopped && !candidate.Exploded) < 2)
            {
                _brewing.Execute(player, action);
                FinishBrewingIfReady();
                return;
            }

            _roundNineCommits.Add(player.Id, action.Kind);
            var active = _players.Where(candidate => !candidate.Stopped && !candidate.Exploded).ToArray();
            if (!active.All(candidate => _roundNineCommits.ContainsKey(candidate.Id))) return;

            var committed = PlayersInStartOrder().Where(candidate => !candidate.Stopped && !candidate.Exploded)
                .Select(candidate => (Player: candidate, Kind: _roundNineCommits[candidate.Id])).ToArray();
            _roundNineCommits.Clear();
            foreach (var item in committed)
                _brewing.Execute(item.Player, new GameAction(item.Kind == GameActionKind.Draw ? "draw" : "stop", item.Kind,
                    item.Kind == GameActionKind.Draw ? "Draw a chip" : "Stop brewing"));
            FinishBrewingIfReady();
        }

        private void FinishBrewingIfReady()
        {
            if (_players.Any(player => player.Choices.Count > 0)) return;
            if (_players.All(player => player.Stopped || player.Exploded))
            {
                Phase = MatchPhase.Evaluation;
                _evaluation.Begin();
            }
        }

        private void StartRound(int round)
        {
            if (round < 1 || round > 9) throw new ArgumentOutOfRangeException(nameof(round));
            Round = round;
            Phase = MatchPhase.Preparation;
            _bonusDieRolls = 1;
            _roundNineCommits.Clear();
            _roundCapabilitiesUsed.Clear();
            foreach (var player in _players)
            {
                if (round == 6) player.Inventory.Add(new Token(TokenColor.White, 1));
                player.ResetRound();
            }
            _currentEvent = DrawEvent();
            AddLog($"Round {round} begins. {PlayersInStartOrder().First().Name} is start player.");
            _currentEvent?.OnRevealed(new RoundEventContext(this));
            if (_players.All(player => player.Choices.Count == 0)) FinishPreparation();
        }

        private IRoundEventRule? DrawEvent()
        {
            if (_eventDeck.Count == 0) return null;
            var index = Random.NextInt(_eventDeck.Count);
            var card = _eventDeck[index];
            _eventDeck.RemoveAt(index);
            return card;
        }

        private void FinishPreparation()
        {
            foreach (var player in _players)
            {
                player.TemporaryRatCount = RatStepsForCurrentRound(player);
                player.RatPosition = Math.Min(player.Droplet + player.TemporaryRatCount, Rules.Track.LastChipPosition);
            }
            Phase = MatchPhase.Brewing;
            _currentEvent?.OnBrewingStarted(new RoundEventContext(this));
        }

        internal void NotifyChipPlaced(PlayerRoundState player, Token chip, ChipPlacementSource source) =>
            _currentEvent?.OnChipPlaced(new RoundEventContext(this), player.Id, chip, source);

        internal void NotifyPlayerStopped(PlayerRoundState player) =>
            _currentEvent?.OnPlayerStopped(new RoundEventContext(this), player.Id);

        internal void NotifyEvaluationStarted() => _currentEvent?.OnEvaluationStarted(new RoundEventContext(this));
        internal void NotifyEvaluationComplete() => _currentEvent?.OnEvaluationComplete(new RoundEventContext(this));

        private static int CountRatTails(int trailingPoints, int leadingPoints)
        {
            var total = 0;
            for (var score = trailingPoints; score < leadingPoints; score++)
                if (RatBoundaries.Contains(PositiveModulo(score, 50))) total++;
            return total;
        }

        private static int PositiveModulo(int value, int divisor)
        {
            var result = value % divisor;
            return result < 0 ? result + divisor : result;
        }

        private void EnterRubySpending()
        {
            Phase = MatchPhase.RubySpending;
            foreach (var player in _players)
            {
                player.Coins = 0;
                player.RubiesDone = false;
            }
        }

        internal void EnterShopping()
        {
            if (Round == 9)
            {
                foreach (var player in _players)
                {
                    var converted = player.Coins / 5;
                    player.Points += converted;
                    player.Coins %= 5;
                    if (converted > 0) AddLog(player.Id, $"{player.Name} converted coins to {converted} victory point(s).");
                }
                EnterRubySpending();
                return;
            }
            Phase = MatchPhase.Shopping;
        }

        internal IEnumerable<PlayerRoundState> PlayersInStartOrder()
        {
            for (var offset = 0; offset < _players.Count; offset++)
                yield return _players[(StartPlayerIndex + offset) % _players.Count];
        }

        private IReadOnlyList<PlayerRoundState> DetermineWinners()
        {
            var bestPoints = _players.Max(player => player.Points);
            var finalists = _players.Where(player => player.Points == bestPoints).ToArray();
            var bestPosition = finalists.Max(player => player.Position);
            return finalists.Where(player => player.Position == bestPosition).ToArray();
        }
    }
}
