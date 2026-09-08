using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules.Fortunes
{
    /// <summary>Stateless Set 1 fortune definitions implemented by the preparation batch.</summary>
    public static class SetOneFortunes
    {
        public static IEnumerable<IRoundEventRule> CreatePreparationBatch()
        {
            yield return new JustInTime();
            yield return new ChooseWisely();
            yield return new YouOnlyGetToChooseOne();
            yield return new Charity();
            yield return new ThePotIsFillingUp();
            yield return new BeginnersBonus();
        }

        private sealed class JustInTime : RoundEventRule
        {
            internal JustInTime() : base("just-in-time", "Just in Time",
                "Choose four victory points or permanently remove one white 1 chip from your bag.") { }

            public override void OnRevealed(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds)
                {
                    var choices = new List<RoundEventChoice>
                    {
                        new RoundEventChoice("four-points", "Gain 4 victory points", (round, id) => round.GainPoints(id, 4))
                    };
                    if (context.Bag(playerId).Any(chip => chip.Color == TokenColor.White && chip.Value == 1))
                        choices.Add(new RoundEventChoice("remove-white-1", "Remove one white 1 chip", (round, id) =>
                            round.RemoveFromBag(id, TokenColor.White, 1), TokenColor.White, 1));
                    context.OfferChoice(playerId, Title, choices.ToArray());
                }
            }
        }

        private sealed class ChooseWisely : RoundEventRule
        {
            internal ChooseWisely() : base("choose-wisely", "Choose Wisely",
                "Choose two droplet steps or, once unlocked and available, a purple 1 chip.") { }

            public override void OnRevealed(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds)
                {
                    var choices = new List<RoundEventChoice>
                    {
                        new RoundEventChoice("droplet-two", "Move your droplet 2 spaces", (round, id) => round.AdvanceDroplet(id, 2))
                    };
                    if (context.CanGiveChip(TokenColor.Purple, 1))
                        choices.Add(new RoundEventChoice("purple-1", "Take a purple 1 chip", (round, id) =>
                            round.TryGiveChip(id, TokenColor.Purple, 1), TokenColor.Purple, 1,
                            round => round.CanGiveChip(TokenColor.Purple, 1)));
                    context.OfferChoice(playerId, Title, choices.ToArray());
                }
            }
        }

        private sealed class YouOnlyGetToChooseOne : RoundEventRule
        {
            internal YouOnlyGetToChooseOne() : base("you-only-get-to-choose-one", "You Only Get to Choose One",
                "Choose a black 1 chip, any available 2 chip, or three rubies.") { }

            public override void OnRevealed(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds)
                {
                    var choices = new List<RoundEventChoice>();
                    if (context.CanGiveChip(TokenColor.Black, 1))
                        choices.Add(GiveChip("black-1", "Take a black 1 chip", TokenColor.Black, 1));
                    choices.AddRange(context.AvailableChips(2).Select(chip => GiveChip(
                        $"{chip.Color.ToString().ToLowerInvariant()}-2", $"Take a {chip.Color} 2 chip", chip.Color, 2)));
                    choices.Add(new RoundEventChoice("three-rubies", "Gain 3 rubies", (round, id) => round.GainRubies(id, 3)));
                    context.OfferChoice(playerId, Title, choices.ToArray());
                }
            }

            private static RoundEventChoice GiveChip(string id, string label, TokenColor color, int value) =>
                new RoundEventChoice(id, label, (round, playerId) => round.TryGiveChip(playerId, color, value), color, value,
                    round => round.CanGiveChip(color, value));
        }

        private sealed class Charity : RoundEventRule
        {
            internal Charity() : base("charity", "Charity", "All players tied for the fewest rubies receive one ruby.") { }
            public override void OnRevealed(RoundEventContext context)
            {
                var fewest = context.PlayerIds.Min(context.Rubies);
                foreach (var playerId in context.PlayerIds.Where(id => context.Rubies(id) == fewest).ToArray())
                    context.GainRubies(playerId, 1);
            }
        }

        private sealed class ThePotIsFillingUp : RoundEventRule
        {
            internal ThePotIsFillingUp() : base("the-pot-is-filling-up", "The Pot Is Filling Up",
                "Every player advances their droplet one space.") { }
            public override void OnRevealed(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds) context.AdvanceDroplet(playerId, 1);
            }
        }

        private sealed class BeginnersBonus : RoundEventRule
        {
            internal BeginnersBonus() : base("beginners-bonus", "Beginner's Bonus",
                "All players tied for the fewest points receive a green 1 chip.") { }
            public override void OnRevealed(RoundEventContext context)
            {
                var fewest = context.PlayerIds.Min(context.Points);
                foreach (var playerId in context.PlayerIds.Where(id => context.Points(id) == fewest).ToArray())
                    context.TryGiveChip(playerId, TokenColor.Green, 1);
            }
        }
    }
}
