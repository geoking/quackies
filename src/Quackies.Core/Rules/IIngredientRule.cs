using Quackies.Core.Tokens;

namespace Quackies.Core.Rules
{
    /// <summary>Ingredient books are stateless definitions; per-match changes go through the supplied context.</summary>
    public interface IIngredientRule
    {
        TokenColor Color { get; }
        string Description { get; }
        void OnPlaced(IngredientContext context, Token chip);
        void OnEvaluation(IngredientContext context);
    }

    public abstract class IngredientRule : IIngredientRule
    {
        protected IngredientRule(TokenColor color, string description) { Color = color; Description = description; }
        public TokenColor Color { get; }
        public string Description { get; }
        public virtual void OnPlaced(IngredientContext context, Token chip) { }
        public virtual void OnEvaluation(IngredientContext context) { }
    }
}
