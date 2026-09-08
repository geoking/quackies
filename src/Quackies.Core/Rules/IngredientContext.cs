using System;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules
{
    /// <summary>A narrow mutation capability supplied only while resolving an ingredient effect.</summary>
    public sealed class IngredientContext
    {
        private readonly MatchSession _session;
        private readonly PlayerRoundState _player;
        internal IngredientContext(MatchSession session, PlayerRoundState player) { _session = session; _player = player; }
        public int Round => _session.Round;
        public int CountPlaced(TokenColor color) => _player.Count(color);
        public int CountInLast(TokenColor color, int count) => _player.CountInLast(color, count);
        public int OpponentCount(TokenColor color) => _session.OpponentOf(_player).Count(color);
        public bool PreviousChipIsWhite => _player.Pot.Count >= 2 && _player.Pot[_player.Pot.Count - 2].Token.Color == TokenColor.White;
        public void GainPoints(int amount) { RequireNonnegative(amount); _session.GainPoints(_player, amount); }
        public void GainRubies(int amount) { RequireNonnegative(amount); _session.GainRubies(_player, amount); }
        public void AdvanceDroplet(int amount) { RequireNonnegative(amount); _session.AdvanceDroplet(_player, amount); }
        public void AdvanceLastChip(int spaces)
        {
            RequireNonnegative(spaces);
            _session.AdvanceLastChip(_player, spaces);
        }
        public void ReturnPreviousWhite()
        {
            if (!PreviousChipIsWhite) throw new InvalidOperationException("The previous chip is not white.");
            var index = _player.Pot.Count - 2;
            var chip = _player.Pot[index].Token;
            _player.Pot.RemoveAt(index); _player.Bag.Add(chip); _player.WhiteTotal -= chip.Value;
            _session.AddLog(_player.Id, $"{_player.Name} returned the preceding {chip} to the bag.");
        }
        public void SelectFromBag(int count) => _session.OfferBagSelection(_player, count, "Crow skull: choose up to one chip", true);
        public void OfferOptional(string title, string applyLabel, Action apply)
        {
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            _session.Offer(_player, title,
                new ChoiceOption("apply", applyLabel, apply), new ChoiceOption("skip", "Decline ingredient effect", () => { }));
        }
        private static void RequireNonnegative(int value)
        { if (value < 0) throw new ArgumentOutOfRangeException(nameof(value)); }
    }
}
