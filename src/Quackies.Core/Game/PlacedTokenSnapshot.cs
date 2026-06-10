using Quackies.Core.Cauldrons;
using Quackies.Core.Tokens;

namespace Quackies.Core.Game
{
    public sealed class PlacedTokenSnapshot
    {
        public PlacedTokenSnapshot(TokenColor color, int value, string tokenText, int position, int drawIndex)
        {
            Color = color;
            Value = value;
            TokenText = tokenText;
            Position = position;
            DrawIndex = drawIndex;
        }

        public TokenColor Color { get; }

        public int Value { get; }

        public string TokenText { get; }

        public int Position { get; }

        public int DrawIndex { get; }

        internal static PlacedTokenSnapshot FromPlacedToken(PlacedToken placedToken)
        {
            return new PlacedTokenSnapshot(
                placedToken.Token.Color,
                placedToken.Token.Value,
                placedToken.Token.ToString(),
                placedToken.Position,
                placedToken.DrawIndex);
        }
    }
}
