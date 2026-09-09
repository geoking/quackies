using System.Collections.Generic;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules.Fortunes
{
    /// <summary>Stateless Set 1 fortunes that require dedicated brewing-round state.</summary>
    public static partial class SetOneFortunes
    {
        public static IEnumerable<IRoundEventRule> CreateFinalBatch()
        {
            yield return new WellStirred();
        }

        private sealed class WellStirred : RoundEventRule
        {
            internal WellStirred() : base("well-stirred", "Well Stirred",
                "You may return the first white chip you draw this round without spending your flask.") { }

            public override void OnChipPlaced(RoundEventContext context, string playerId, Token chip,
                ChipPlacementSource source)
            {
                if (source == ChipPlacementSource.FortuneCard || chip.Color != TokenColor.White ||
                    !context.TryUseOnce(playerId, Id)) return;

                context.OfferChoice(playerId, Title,
                    new RoundEventChoice("return-white", $"Return {chip} to your bag",
                        (round, id) => round.ReturnLastPlacedChip(id)),
                    new RoundEventChoice("keep-white", $"Keep {chip} in your pot", (round, id) => { }));
            }
        }
    }
}
