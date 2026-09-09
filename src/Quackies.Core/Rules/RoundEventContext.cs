using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules
{
    /// <summary>
    /// Narrow access granted while a fortune card is revealed. More lifecycle
    /// hooks can be added here without moving event behavior into MatchSession.
    /// </summary>
    public sealed class RoundEventContext
    {
        private readonly MatchSession _session;

        internal RoundEventContext(MatchSession session)
        {
            _session = session;
            PlayerIds = new ReadOnlyCollection<string>(session.Players.Select(player => player.Id).ToList());
        }

        public int Round => _session.Round;
        public IReadOnlyList<string> PlayerIds { get; }
        public int Points(string playerId) => _session.Player(playerId).Points;
        public int Rubies(string playerId) => _session.Player(playerId).Rubies;
        public bool Exploded(string playerId) => _session.Player(playerId).Exploded;
        public int RatSteps(string playerId) => _session.RatStepsForCurrentRound(_session.Player(playerId));
        public int WhiteTotal(string playerId) => _session.Player(playerId).WhiteTotal;
        public IReadOnlyList<Token> Bag(string playerId) => new ReadOnlyCollection<Token>(_session.Player(playerId).Bag.ToList());
        public IReadOnlyList<Token> PreviewBag(string playerId, int count) =>
            _session.PreviewBag(_session.Player(playerId), count);
        public void GainPoints(string playerId, int amount) => _session.GainPoints(_session.Player(playerId), amount);
        public void GainRubies(string playerId, int amount) => _session.GainRubies(_session.Player(playerId), amount);
        public void AdvanceDroplet(string playerId, int spaces) => _session.AdvanceDroplet(_session.Player(playerId), spaces);
        public bool TryGiveChip(string playerId, TokenColor color, int value) =>
            _session.TryGiveSupplyChip(_session.Player(playerId), color, value, addToCurrentBag: true);
        public bool CanGiveChip(TokenColor color, int value) => _session.CanTakeSupplyChip(color, value);
        public bool CanExchangeRubyForChip(string playerId, TokenColor color, int value) =>
            _session.CanExchangeRubyForSupplyChip(_session.Player(playerId), color, value);
        public bool TryExchangeRubyForChip(string playerId, TokenColor color, int value) =>
            _session.TryExchangeRubyForSupplyChip(_session.Player(playerId), color, value);
        public bool HasHigherChip(TokenColor color, int value) => _session.HasHigherSupplyChip(color, value);
        public bool CanUpgradeBagChip(string playerId, TokenColor color, int value) =>
            _session.CanUpgradeBagChip(_session.Player(playerId), color, value);
        public bool TryUpgradeBagChip(string playerId, TokenColor color, int value) =>
            _session.TryUpgradeBagChip(_session.Player(playerId), color, value);
        public IReadOnlyList<ShopChipDefinition> AvailableChips(int value) =>
            new ReadOnlyCollection<ShopChipDefinition>(_session.Rules.ShopChips
                .Where(chip => chip.Value == value && chip.AvailableFromRound <= Round && _session.Remaining(chip) > 0).ToList());

        public bool RemoveFromBag(string playerId, TokenColor color, int value) =>
            _session.RemoveInventoryChip(_session.Player(playerId), color, value);
        public bool ScoringSpaceHasRuby(string playerId) => _session.ScoringSpace(_session.Player(playerId)).HasRuby;
        public void AdvanceLastChip(string playerId, int spaces) => _session.AdvanceLastChip(_session.Player(playerId), spaces);
        public void RefillFlask(string playerId) => _session.RefillFlask(_session.Player(playerId));
        public void SetExplosionThreshold(string playerId, int threshold) => _session.SetExplosionThreshold(_session.Player(playerId), threshold);
        public void SetBonusDieRolls(int rolls) => _session.SetBonusDieRolls(rolls);
        public void SetRatSteps(string playerId, int steps) =>
            _session.SetRatStepsForCurrentRound(_session.Player(playerId), steps);
        public void RollDie(string playerId) =>
            _session.RollDie(_session.Player(playerId), DieRollReason.Fortune, addRewardChipToCurrentBag: true);

        public void OfferChoice(string playerId, string title, params RoundEventChoice[] choices)
        {
            if (choices == null || choices.Length == 0) throw new ArgumentException("An event choice needs at least one option.", nameof(choices));
            var player = _session.Player(playerId);
            _session.Offer(player, title, choices.Select(choice => new ChoiceOption(
                choice.Id, choice.Label, () => choice.Apply(this, playerId), choice.Color, choice.Value,
                () => choice.IsAvailable(this))).ToArray());
        }
    }

    public sealed class RoundEventChoice
    {
        private readonly Action<RoundEventContext, string> _apply;

        public RoundEventChoice(string id, string label, Action<RoundEventContext, string> apply, TokenColor? color = null,
            int value = 0, Func<RoundEventContext, bool>? isAvailable = null)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("A choice needs a stable ID.", nameof(id));
            Id = id;
            Label = label ?? throw new ArgumentNullException(nameof(label));
            _apply = apply ?? throw new ArgumentNullException(nameof(apply));
            Color = color;
            Value = value;
            _isAvailable = isAvailable ?? (_ => true);
        }

        public string Id { get; }
        public string Label { get; }
        public TokenColor? Color { get; }
        public int Value { get; }
        private readonly Func<RoundEventContext, bool> _isAvailable;
        internal bool IsAvailable(RoundEventContext context) => _isAvailable(context);
        internal void Apply(RoundEventContext context, string playerId) => _apply(context, playerId);
    }
}
