using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quackies.Core.Randomness;
using Quackies.Core.Tokens;

namespace Quackies.Core.Bags
{
    public sealed class Bag
    {
        private readonly List<Token> _tokens;

        public Bag(IEnumerable<Token> tokens)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            _tokens = new List<Token>();

            foreach (var token in tokens)
            {
                if (token == null)
                {
                    throw new ArgumentException("Bag cannot contain null tokens.", nameof(tokens));
                }

                _tokens.Add(token);
            }
        }

        public int Count
        {
            get { return _tokens.Count; }
        }

        public bool IsEmpty
        {
            get { return _tokens.Count == 0; }
        }

        public IReadOnlyList<Token> RemainingTokens
        {
            get { return new ReadOnlyCollection<Token>(_tokens); }
        }

        public Token DrawToken(IRandomSource random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (_tokens.Count == 0)
            {
                throw new InvalidOperationException("Cannot draw a token from an empty bag.");
            }

            var index = random.NextInt(_tokens.Count);

            if (index < 0 || index >= _tokens.Count)
            {
                throw new InvalidOperationException("Random source returned an index outside the bag range.");
            }

            var token = _tokens[index];
            _tokens.RemoveAt(index);

            return token;
        }
    }
}
