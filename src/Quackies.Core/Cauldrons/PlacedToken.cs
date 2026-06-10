using System;
using Quackies.Core.Tokens;

namespace Quackies.Core.Cauldrons
{
    public sealed class PlacedToken
    {
        public PlacedToken(Token token, int position, int drawIndex)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (drawIndex <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(drawIndex), "Draw index must be greater than zero.");
            }

            Token = token;
            Position = position;
            DrawIndex = drawIndex;
        }

        public Token Token { get; }

        public int Position { get; }

        public int DrawIndex { get; }

        public override string ToString()
        {
            return $"#{DrawIndex}: {Token} at {Position}";
        }
    }
}
