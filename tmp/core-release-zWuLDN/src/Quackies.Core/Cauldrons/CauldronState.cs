using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quackies.Core.Tokens;

namespace Quackies.Core.Cauldrons
{
    public sealed class CauldronState
    {
        private readonly List<PlacedToken> _placedTokens;

        public CauldronState()
        {
            _placedTokens = new List<PlacedToken>();
        }

        public int CurrentPosition { get; private set; }

        public int WhiteChipTotal { get; private set; }

        public bool HasExploded { get; private set; }

        public bool IsStopped { get; private set; }

        public PlacedToken? LastPlacedToken { get; private set; }

        public IReadOnlyList<PlacedToken> PlacedTokens
        {
            get { return new ReadOnlyCollection<PlacedToken>(_placedTokens); }
        }

        public PlacedToken PlaceToken(Token token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (IsStopped)
            {
                throw new InvalidOperationException("Cannot place another token after the cauldron has stopped.");
            }

            if (HasExploded)
            {
                throw new InvalidOperationException("Cannot place another token after the cauldron has exploded.");
            }

            CurrentPosition += token.Value;

            var placedToken = new PlacedToken(token, CurrentPosition, _placedTokens.Count + 1);
            _placedTokens.Add(placedToken);
            LastPlacedToken = placedToken;

            if (token.Color == TokenColor.White)
            {
                WhiteChipTotal += token.Value;
                HasExploded = WhiteChipTotal > 7;
            }

            return placedToken;
        }

        public void Stop()
        {
            IsStopped = true;
        }
    }
}
