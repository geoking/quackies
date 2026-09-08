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
