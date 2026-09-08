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
        private readonly List<string> _log = new List<string>();
        private readonly Dictionary<string, GameActionKind> _roundNineCommits = new Dictionary<string, GameActionKind>(StringComparer.Ordinal);
        private readonly BrewingPhaseHandler _brewing;
        private readonly EvaluationPhaseHandler _evaluation;
        private readonly ShoppingPhaseHandler _shopping;
        private readonly RubySpendingPhaseHandler _rubySpending;
        private IRoundEventRule? _currentEvent;
        private long _nextChoiceSequence;

        private MatchSession(IRandomSource random, RuleSet rules)
        {
            Random = random ?? throw new ArgumentNullException(nameof(random));
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _players = new List<PlayerRoundState>
            {
                new PlayerRoundState("human", "Human"),
                new PlayerRoundState("ai", "AI")
            };
            _supply = rules.ShopChips.ToDictionary(chip => chip, chip => chip.Stock);
            _eventDeck = rules.RoundEvents.ToList();
            _brewing = new BrewingPhaseHandler(this);
            _evaluation = new EvaluationPhaseHandler(this);
            _shopping = new ShoppingPhaseHandler(this);
            _rubySpending = new RubySpendingPhaseHandler(this);
            StartRound(1);
        }

        public static MatchSession Create(IRandomSource random, RuleSet? rules = null) =>
            new MatchSession(random, rules ?? RuleSet.SetOne());

        public int Round { get; private set; }
        public MatchPhase Phase { get; private set; }
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
                playerViews, viewer.Bag.ToArray(), offers, _log.TakeLast(16), winners,
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

        internal void AddLog(string message)
        {
            _log.Add(message);
            if (_log.Count > 80) _log.RemoveRange(0, _log.Count - 80);
        }

        internal void GainPoints(PlayerRoundState player, int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            player.Points += amount;
        }

        internal void GainRubies(PlayerRoundState player, int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            player.Rubies += amount;
        }

        internal void AdvanceDroplet(PlayerRoundState player, int spaces)
        {
            if (spaces < 0) throw new ArgumentOutOfRangeException(nameof(spaces));
            player.Droplet = Math.Min(player.Droplet + spaces, Rules.Track.LastChipPosition);
        }

        internal bool TryGiveSupplyChip(PlayerRoundState player, TokenColor color, int value, bool addToCurrentBag = false)
        {
            var definition = Rules.ShopChips.SingleOrDefault(chip => chip.Color == color && chip.Value == value);
            if (definition == null || definition.AvailableFromRound > Round || _supply[definition] == 0) return false;
            _supply[definition]--;
            var chip = new Token(color, value);
            player.Inventory.Add(chip);
            if (addToCurrentBag) player.Bag.Add(chip);
            return true;
        }

        internal bool CanTakeSupplyChip(TokenColor color, int value)
        {
            var definition = Rules.ShopChips.SingleOrDefault(chip => chip.Color == color && chip.Value == value);
            return definition != null && definition.AvailableFromRound <= Round && _supply[definition] > 0;
        }

        internal bool RemoveInventoryChip(PlayerRoundState player, TokenColor color, int value)
        {
            var chip = player.Inventory.FirstOrDefault(candidate => candidate.Color == color && candidate.Value == value);
            if (chip == null) return false;
            player.Inventory.Remove(chip);
            player.Bag.Remove(chip);
            var definition = Rules.ShopChips.SingleOrDefault(candidate => candidate.Color == color && candidate.Value == value);
            if (definition != null) _supply[definition]++;
            return true;
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

        private static void ExecuteChoice(PlayerRoundState player, GameAction action)
        {
            var choice = player.Choices.Dequeue();
            var prefix = $"choose:{choice.Sequence}:";
            if (!action.Id.StartsWith(prefix, StringComparison.Ordinal))
                throw new InvalidOperationException("That choice belongs to an earlier decision.");
            var optionId = action.Id.Substring(prefix.Length);
            choice.Options.Single(option => string.Equals(option.Id, optionId, StringComparison.Ordinal)).Apply();
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
            _roundNineCommits.Clear();
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
            var leaderPoints = _players.Max(player => player.Points);
            foreach (var player in _players)
            {
                player.TemporaryRatCount = CountRatTails(player.Points, leaderPoints);
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
                    player.Points += player.Coins / 5;
                    player.Coins %= 5;
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
