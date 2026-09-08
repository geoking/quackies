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
        public int RatSteps(string playerId) => _session.Player(playerId).TemporaryRatCount;
        public IReadOnlyList<Token> Bag(string playerId) => new ReadOnlyCollection<Token>(_session.Player(playerId).Bag.ToList());
        public void GainPoints(string playerId, int amount) => _session.GainPoints(_session.Player(playerId), amount);
        public void GainRubies(string playerId, int amount) => _session.GainRubies(_session.Player(playerId), amount);
        public void AdvanceDroplet(string playerId, int spaces) => _session.AdvanceDroplet(_session.Player(playerId), spaces);
        public bool TryGiveChip(string playerId, TokenColor color, int value) =>
            _session.TryGiveSupplyChip(_session.Player(playerId), color, value, addToCurrentBag: true);
        public bool CanGiveChip(TokenColor color, int value) => _session.CanTakeSupplyChip(color, value);
        public IReadOnlyList<ShopChipDefinition> AvailableChips(int value) =>
            new ReadOnlyCollection<ShopChipDefinition>(_session.Rules.ShopChips
                .Where(chip => chip.Value == value && chip.AvailableFromRound <= Round && _session.Remaining(chip) > 0).ToList());

        public bool RemoveFromBag(string playerId, TokenColor color, int value) =>
            _session.RemoveInventoryChip(_session.Player(playerId), color, value);

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
