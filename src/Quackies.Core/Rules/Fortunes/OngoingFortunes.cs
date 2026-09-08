using System.Collections.Generic;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules.Fortunes
{
    public static partial class SetOneFortunes
    {
        public static IEnumerable<IRoundEventRule> CreateOngoingBatch()
        {
            yield return new LuckyDevil();
            yield return new ItsShiningExtraBright();
            yield return new SeasonedPerfectly();
            yield return new ThePotIsFull();
            yield return new LivingInLuxury();
            yield return new RollTheDie();
            yield return new PumpkinPatchParty();
            yield return new MagicPotion();
        }

        private sealed class LuckyDevil : RoundEventRule
        {
            internal LuckyDevil() : base("lucky-devil", "Lucky Devil",
                "A ruby scoring space also awards two victory points, even after an explosion.") { }
            public override void OnEvaluationComplete(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds)
                    if (context.ScoringSpaceHasRuby(playerId)) context.GainPoints(playerId, 2);
            }
        }

        private sealed class ItsShiningExtraBright : RoundEventRule
        {
            internal ItsShiningExtraBright() : base("its-shining-extra-bright", "It's Shining Extra Bright",
                "A ruby scoring space awards one additional ruby.") { }
            public override void OnEvaluationComplete(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds)
                    if (context.ScoringSpaceHasRuby(playerId)) context.GainRubies(playerId, 1);
            }
        }

        private sealed class SeasonedPerfectly : RoundEventRule
        {
            internal SeasonedPerfectly() : base("seasoned-perfectly", "Seasoned Perfectly",
                "A white-chip total of exactly seven at round end advances the droplet one space.") { }
            public override void OnEvaluationComplete(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds)
                    if (context.WhiteTotal(playerId) == 7) context.AdvanceDroplet(playerId, 1);
            }
        }

        private sealed class ThePotIsFull : RoundEventRule
        {
            internal ThePotIsFull() : base("the-pot-is-full", "The Pot Is Full",
                "Every player entitled to the evaluation die rolls it twice.") { }
            public override void OnRevealed(RoundEventContext context) => context.SetBonusDieRolls(2);
        }

        private sealed class LivingInLuxury : RoundEventRule
        {
            internal LivingInLuxury() : base("living-in-luxury", "Living in Luxury",
                "The white-chip explosion threshold is nine for this round.") { }
            public override void OnRevealed(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds) context.SetExplosionThreshold(playerId, 9);
            }
        }

        private sealed class RollTheDie : RoundEventRule
        {
            internal RollTheDie() : base("roll-the-die", "Roll the Die",
                "Every player immediately rolls the bonus die once and receives the reward.") { }
            public override void OnRevealed(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds) context.RollDie(playerId);
            }
        }

        private sealed class PumpkinPatchParty : RoundEventRule
        {
            internal PumpkinPatchParty() : base("pumpkin-patch-party", "Pumpkin Patch Party",
                "Every orange chip placed this round travels one additional space.") { }
            public override void OnChipPlaced(RoundEventContext context, string playerId, Token chip, ChipPlacementSource source)
            {
                if (chip.Color == TokenColor.Orange) context.AdvanceLastChip(playerId, 1);
            }
        }

        private sealed class MagicPotion : RoundEventRule
        {
            internal MagicPotion() : base("magic-potion", "Magic Potion",
                "Every player's flask refills for free at the end of the round.") { }
            public override void OnRoundEnded(RoundEventContext context)
            {
                foreach (var playerId in context.PlayerIds) context.RefillFlask(playerId);
            }
        }
    }
}
