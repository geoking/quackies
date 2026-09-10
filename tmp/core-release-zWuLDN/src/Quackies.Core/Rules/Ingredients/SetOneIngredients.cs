using System.Collections.Generic;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules.Ingredients
{
    public static class SetOneIngredients
    {
        public static IEnumerable<IIngredientRule> Create()
        {
            yield return new PlainIngredient(TokenColor.White, "Cherry bombs: a white total above 7 explodes the pot.");
            yield return new PlainIngredient(TokenColor.Orange, "Pumpkin: advances one space; supports the red book.");
            yield return new CrowSkull(); yield return new Toadstool(); yield return new Mandrake();
            yield return new GardenSpider(); yield return new GhostBreath(); yield return new AfricanDeathHeadHawkMoth();
        }
        private sealed class PlainIngredient : IngredientRule
        { internal PlainIngredient(TokenColor color, string text) : base(color, text) { } }
        private sealed class CrowSkull : IngredientRule
        {
            internal CrowSkull() : base(TokenColor.Blue, "Look at 1/2/4 chips from your bag; place up to one and return the rest. Its effect applies.") { }
            public override void OnPlaced(IngredientContext context, Token chip) => context.SelectFromBag(chip.Value);
        }
        private sealed class Toadstool : IngredientRule
        {
            internal Toadstool() : base(TokenColor.Red, "Move 1 extra space with 1–2 pumpkins already in the pot, or 2 extra with at least 3.") { }
            public override void OnPlaced(IngredientContext context, Token chip)
            {
                var pumpkins = context.CountPlaced(TokenColor.Orange);
                var extra = pumpkins >= 3 ? 2 : pumpkins > 0 ? 1 : 0;
                if (extra > 0) context.AdvanceLastChip(extra);
            }
        }
        private sealed class Mandrake : IngredientRule
        {
            internal Mandrake() : base(TokenColor.Yellow, "If the immediately previous chip is white, you may return it to your bag. The yellow chip stays in place.") { }
            public override void OnPlaced(IngredientContext context, Token chip)
            {
                if (context.PreviousChipIsWhite) context.OfferOptional("Mandrake", "Return the preceding white chip", context.ReturnPreviousWhite);
            }
        }
        private sealed class GardenSpider : IngredientRule
        {
            internal GardenSpider() : base(TokenColor.Green, "At evaluation, each green chip among your last two chips gives 1 ruby, even after an explosion.") { }
            public override void OnEvaluation(IngredientContext context)
            {
                var count = context.CountInLast(TokenColor.Green, 2);
                if (count > 0) context.GainRubies(count);
            }
        }
        private sealed class GhostBreath : IngredientRule
        {
            internal GhostBreath() : base(TokenColor.Purple, "1 purple: 1 VP. 2: 1 VP and 1 ruby. 3+: 2 VP and advance your droplet once.") { }
            public override void OnEvaluation(IngredientContext context)
            {
                var count = context.CountPlaced(TokenColor.Purple);
                if (count == 0) return;
                context.GainPoints(count >= 3 ? 2 : 1);
                if (count == 2) context.GainRubies(1);
                if (count >= 3) context.AdvanceDroplet(1);
            }
        }
        private sealed class AfricanDeathHeadHawkMoth : IngredientRule
        {
            internal AfricanDeathHeadHawkMoth() : base(TokenColor.Black, "Two players: a positive tie gives each a droplet step; more black chips gives a droplet step and 1 ruby.") { }
            public override void OnEvaluation(IngredientContext context)
            {
                var own = context.CountPlaced(TokenColor.Black);
                var opponent = context.OpponentCount(TokenColor.Black);
                if (own == 0 || own < opponent) return;
                context.AdvanceDroplet(1);
                if (own > opponent) context.GainRubies(1);
            }
        }
    }
}
