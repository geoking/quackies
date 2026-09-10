namespace Quackies.Core.Rules
{
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
        public abstract void OnRevealed(RoundEventContext context);
    }
}
