using System;
using Quackies.Core.Tokens;

namespace Quackies.Core.Players
{
    public sealed class PlacedToken
    {
        public PlacedToken(Token token, int position)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            Token = token;
            Position = position;
        }

        public Token Token { get; }

        public int Position { get; }

        public override string ToString()
        {
            return $"{Token} at {Position}";
        }
    }
}
