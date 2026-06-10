using System;

namespace Quackies.Core.Tokens
{
    public sealed class Token
    {
        public Token(TokenColor color, int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Token value must be greater than zero.");
            }

            Color = color;
            Value = value;
        }

        public TokenColor Color { get; }

        public int Value { get; }

        public override string ToString()
        {
            return $"{Color} {Value}";
        }
    }
}
