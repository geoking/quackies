namespace Quackies.Core.Rules
{
    public enum ChipPlacementSource
    {
        BagDraw,
        IngredientSelection,
        FortuneCard
    }

    /// <summary>
    /// A fortune card owns only its event-specific behavior. The match controls
    /// card order and phase transitions, and supplies a narrow mutation context.
    /// </summary>
    public interface IRoundEventRule
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        void OnRevealed(RoundEventContext context);
        void OnBrewingStarted(RoundEventContext context);
        bool CanExplodeOnPlacement(RoundEventContext context, string playerId, Tokens.Token chip, ChipPlacementSource source);
        void OnChipPlaced(RoundEventContext context, string playerId, Tokens.Token chip, ChipPlacementSource source);
        void OnPlayerStopped(RoundEventContext context, string playerId);
        void OnEvaluationStarted(RoundEventContext context);
        void OnEvaluationComplete(RoundEventContext context);
        void OnRoundEnded(RoundEventContext context);
    }

    public abstract class RoundEventRule : IRoundEventRule
    {
        protected RoundEventRule(string id, string title, string description)
        {
            Id = id;
            Title = title;
            Description = description;
        }

        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public virtual void OnRevealed(RoundEventContext context) { }
        public virtual void OnBrewingStarted(RoundEventContext context) { }
        public virtual bool CanExplodeOnPlacement(RoundEventContext context, string playerId, Tokens.Token chip,
            ChipPlacementSource source) => true;
        public virtual void OnChipPlaced(RoundEventContext context, string playerId, Tokens.Token chip, ChipPlacementSource source) { }
        public virtual void OnPlayerStopped(RoundEventContext context, string playerId) { }
        public virtual void OnEvaluationStarted(RoundEventContext context) { }
        public virtual void OnEvaluationComplete(RoundEventContext context) { }
        public virtual void OnRoundEnded(RoundEventContext context) { }
    }
}
