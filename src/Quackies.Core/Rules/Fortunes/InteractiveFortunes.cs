using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules.Fortunes
{
    /// <summary>Stateless Set 1 fortune definitions using previews, exchanges and opponent choices.</summary>
    public static partial class SetOneFortunes
    {
        public static IEnumerable<IRoundEventRule> CreateInteractiveBatch()
        {
            yield return new LessIsMore();
            yield return new AnOpportunisticMoment();
            yield return new Schadenfreude();
        }

        private sealed class LessIsMore : RoundEventRule
        {
            internal LessIsMore() : base("less-is-more", "Less Is More",
                "Everyone previews five chips. The lowest total receives a blue 2 chip; everyone else receives one ruby.") { }

            public override void OnRevealed(RoundEventContext context)
            {
                var totals = context.PlayerIds.ToDictionary(playerId => playerId,
                    playerId => context.PreviewBag(playerId, 5).Sum(chip => chip.Value));
                var lowest = totals.Values.Min();
                foreach (var playerId in context.PlayerIds)
                {
                    if (totals[playerId] == lowest) context.TryGiveChip(playerId, TokenColor.Blue, 2);
                    else context.GainRubies(playerId, 1);
                }
            }
        }

        private sealed class AnOpportunisticMoment : RoundEventRule
        {
            internal AnOpportunisticMoment() : base("an-opportunistic-moment", "An Opportunistic Moment",
                "Preview four chips and optionally exchange one for the next value of the same color. If no exchange is possible, receive a green 1 chip.") { }

            public override void OnRevealed(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds)
                {
                    var candidates = context.PreviewBag(playerId, 4)
                        .Select(chip => (chip.Color, chip.Value))
                        .Distinct()
                        .Where(chip => context.HasHigherChip(chip.Color, chip.Value))
                        .ToArray();
                    if (!candidates.Any(chip => context.CanUpgradeBagChip(playerId, chip.Color, chip.Value)))
                    {
                        context.TryGiveChip(playerId, TokenColor.Green, 1);
                        continue;
                    }

                    var choices = candidates.Select(chip => UpgradeChoice(playerId, chip.Color, chip.Value)).ToList();
                    choices.Add(new RoundEventChoice("keep-preview", "Return all previewed chips",
                        (round, id) => { }));
                    choices.Add(new RoundEventChoice("fallback-green-1", "Receive a green 1 chip",
                        (round, id) => round.TryGiveChip(id, TokenColor.Green, 1), TokenColor.Green, 1,
                        round => !candidates.Any(chip => round.CanUpgradeBagChip(playerId, chip.Color, chip.Value))));
                    context.OfferChoice(playerId, Title, choices.ToArray());
                }
            }

            private static RoundEventChoice UpgradeChoice(string playerId, TokenColor color, int value)
            {
                var colorId = color.ToString().ToLowerInvariant();
                return new RoundEventChoice($"upgrade-{colorId}-{value}", $"Upgrade {color} {value}",
                    (round, id) => round.TryUpgradeBagChip(id, color, value), color, value,
                    round => round.CanUpgradeBagChip(playerId, color, value));
            }
        }

        private sealed class Schadenfreude : RoundEventRule
        {
            internal Schadenfreude() : base("schadenfreude", "Schadenfreude",
                "When a player's pot explodes, their left neighbor chooses an available 2 chip.") { }

            public override void OnPlayerStopped(RoundEventContext context, string playerId)
            {
                if (!context.Exploded(playerId)) return;
                var neighborId = context.PlayerIds.Single(candidate => !string.Equals(candidate, playerId, StringComparison.Ordinal));
                var choices = context.AvailableChips(2).Select(chip => GiveChip(neighborId, chip.Color, chip.Value)).ToArray();
                if (choices.Length > 0) context.OfferChoice(neighborId, Title, choices);
            }

            private static RoundEventChoice GiveChip(string playerId, TokenColor color, int value) =>
                new RoundEventChoice($"take-{color.ToString().ToLowerInvariant()}-{value}", $"Take a {color} {value} chip",
                    (round, id) => round.TryGiveChip(id, color, value), color, value,
                    round => round.CanGiveChip(color, value));
        }
    }
}
