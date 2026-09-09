using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules.Fortunes
{
    /// <summary>Stateless Set 1 fortune definitions concerning rat steps and exchanges.</summary>
    public static partial class SetOneFortunes
    {
        public static IEnumerable<IRoundEventRule> CreateRatBatch()
        {
            yield return new AGoodStart();
            yield return new RatInfestation();
            yield return new RatsAreYourFriends();
            yield return new WheelAndDeal();
        }

        private sealed class AGoodStart : RoundEventRule
        {
            internal AGoodStart() : base("a-good-start", "A Good Start",
                "Keep your ordinary rat steps, or exchange up to three of them for the same number of rubies.") { }

            public override void OnRevealed(RoundEventContext context)
            {
                FreezeRatEntitlements(context);
                foreach (var playerId in context.PlayerIds)
                {
                    var ratSteps = context.RatSteps(playerId);
                    var choices = new List<RoundEventChoice>
                    {
                        new RoundEventChoice("keep-rats", $"Keep {ratSteps} rat step{PluralSuffix(ratSteps)}", (round, id) => { })
                    };
                    for (var tradedSteps = 1; tradedSteps <= System.Math.Min(3, ratSteps); tradedSteps++)
                    {
                        var amount = tradedSteps;
                        choices.Add(new RoundEventChoice($"trade-{amount}-rats",
                            $"Exchange {amount} rat step{PluralSuffix(amount)} for {amount} rub{(amount == 1 ? "y" : "ies")}",
                            (round, id) =>
                            {
                                round.SetRatSteps(id, ratSteps - amount);
                                round.GainRubies(id, amount);
                            }));
                    }
                    context.OfferChoice(playerId, Title, choices.ToArray());
                }
            }
        }

        private sealed class RatInfestation : RoundEventRule
        {
            internal RatInfestation() : base("rat-infestation", "Rat Infestation",
                "Every player receives twice their ordinary number of rat steps this round.") { }

            public override void OnRevealed(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds)
                    context.SetRatSteps(playerId, context.RatSteps(playerId) * 2);
            }
        }

        private sealed class RatsAreYourFriends : RoundEventRule
        {
            internal RatsAreYourFriends() : base("rats-are-your-friends", "Rats Are Your Friends",
                "Choose any available 4 chip, or gain one victory point for each rat step due this round.") { }

            public override void OnRevealed(RoundEventContext context)
            {
                var pointRewards = context.PlayerIds.ToDictionary(playerId => playerId, context.RatSteps);
                foreach (var playerId in context.PlayerIds)
                {
                    var ratSteps = pointRewards[playerId];
                    var choices = context.AvailableChips(4)
                        .Select(chip => GiveFourChip($"{chip.Color.ToString().ToLowerInvariant()}-4",
                            $"Take a {chip.Color} 4 chip", chip.Color, 4))
                        .ToList();
                    choices.Add(new RoundEventChoice("rat-points",
                        $"Gain {ratSteps} victory point{PluralSuffix(ratSteps)}",
                        (round, id) => round.GainPoints(id, ratSteps)));
                    context.OfferChoice(playerId, Title, choices.ToArray());
                }
            }
        }

        private sealed class WheelAndDeal : RoundEventRule
        {
            internal WheelAndDeal() : base("wheel-and-deal", "Wheel and Deal",
                "You may exchange one ruby for any available 1 chip except purple or black.") { }

            public override void OnRevealed(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds)
                {
                    var choices = context.AvailableChips(1)
                        .Where(chip => chip.Color != TokenColor.Purple && chip.Color != TokenColor.Black)
                        .Where(chip => context.CanExchangeRubyForChip(playerId, chip.Color, 1))
                        .Select(chip => ExchangeRubyForChip(playerId, chip.Color, 1))
                        .ToList();
                    choices.Add(new RoundEventChoice("keep-ruby", "Keep your rubies", (round, id) => { }));
                    context.OfferChoice(playerId, Title, choices.ToArray());
                }
            }
        }

        private static void FreezeRatEntitlements(RoundEventContext context)
        {
            var entitlements = context.PlayerIds.ToDictionary(playerId => playerId, context.RatSteps);
            foreach (var entitlement in entitlements)
                context.SetRatSteps(entitlement.Key, entitlement.Value);
        }

        private static RoundEventChoice GiveFourChip(string id, string label, TokenColor color, int value) =>
            new RoundEventChoice(id, label, (round, playerId) => round.TryGiveChip(playerId, color, value), color, value,
                round => round.CanGiveChip(color, value));

        private static RoundEventChoice ExchangeRubyForChip(string playerId, TokenColor color, int value) =>
            new RoundEventChoice($"exchange-{color.ToString().ToLowerInvariant()}-{value}",
                $"Exchange 1 ruby for a {color} {value} chip",
                (round, id) => round.TryExchangeRubyForChip(id, color, value), color, value,
                round => round.CanExchangeRubyForChip(playerId, color, value));

        private static string PluralSuffix(int amount) => amount == 1 ? "" : "s";
    }
}
