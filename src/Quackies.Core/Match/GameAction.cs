using Quackies.Core.Tokens;

namespace Quackies.Core.Match
{
    public enum GameActionKind
    {
        Draw, Stop, UseFlask, Choose, BuyIngredient, FinishShopping,
        MoveDroplet, RefillFlask, ConvertRubies, FinishRubySpending, NextRound
    }

    /// <summary>A command offered by the current state. Execute validates its ID again; stale actions are rejected.</summary>
    public sealed class GameAction
    {
        private object? _scope;
        private string? _playerId;
        private long _revision;
        private string? _window;

        internal GameAction Issue(object scope, string playerId, long revision, string window)
        {
            var issued = new GameAction(Id, Kind, Label, ChoiceTitle, Color, Value, Cost);
            issued._scope = scope;
            issued._playerId = playerId;
            issued._revision = revision;
            issued._window = window;
            return issued;
        }

        internal bool WasIssued(object scope, string playerId, long revision, string window) =>
            ReferenceEquals(_scope, scope) && _playerId == playerId && _revision == revision && _window == window;

        internal GameAction(string id, GameActionKind kind, string label, string choiceTitle = "", TokenColor? color = null, int value = 0, int cost = 0)
        { Id = id; Kind = kind; Label = label; ChoiceTitle = choiceTitle; Color = color; Value = value; Cost = cost; }
        public string Id { get; }
        public GameActionKind Kind { get; }
        public string Label { get; }
        public string ChoiceTitle { get; }
        public TokenColor? Color { get; }
        public int Value { get; }
        public int Cost { get; }
    }
}
