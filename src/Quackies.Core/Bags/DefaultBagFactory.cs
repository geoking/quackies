using System.Collections.Generic;
using Quackies.Core.Tokens;

namespace Quackies.Core.Bags
{
    public static class DefaultBagFactory
    {
        public static Bag CreateStartingBag()
        {
            return new Bag(new[]
            {
                new Token(TokenColor.White, 1),
                new Token(TokenColor.White, 1),
                new Token(TokenColor.White, 1),
                new Token(TokenColor.White, 1),
                new Token(TokenColor.White, 2),
                new Token(TokenColor.White, 2),
                new Token(TokenColor.White, 3),
                new Token(TokenColor.Orange, 1)
            });
        }

        public static Bag CreateFromTokens(IEnumerable<Token> tokens)
        {
            return new Bag(tokens);
        }
    }
}
