using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules.Fortunes
{
    /// <summary>Stateless Set 1 fortunes that require dedicated brewing-round state.</summary>
    public static partial class SetOneFortunes
    {
        public static IEnumerable<IRoundEventRule> CreateFinalBatch()
        {
            yield return new WellStirred();
            yield return new StrongIngredient();
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

        private sealed class StrongIngredient : RoundEventRule
        {
            internal StrongIngredient() : base("strong-ingredient", "Strong Ingredient",
                "In start-player order, each non-exploded player may preview up to five chips and place one final chip.") { }

            public override void OnPlayerStopped(RoundEventContext context, string playerId)
            {
                if (!context.AllPlayersFinishedBrewing || !context.TryUseOnce(Id)) return;
                var eligible = context.PlayerIdsInStartOrder
                    .Where(id => !context.Exploded(id) && context.CanPlaceFortuneChip(id))
                    .ToArray();
                context.OfferSequentialFortuneBagSelections(eligible, 5, Title);
            }
        }
    }
}
