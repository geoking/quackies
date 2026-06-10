using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quackies.Core.Cauldrons;

namespace Quackies.Core.Game
{
    public sealed class CauldronSnapshot
    {
        public CauldronSnapshot(
            int currentPosition,
            int whiteChipTotal,
            bool hasExploded,
            bool isStopped,
            PlacedTokenSnapshot? lastPlacedToken,
            IEnumerable<PlacedTokenSnapshot> placedTokens)
        {
            if (placedTokens == null)
            {
                throw new ArgumentNullException(nameof(placedTokens));
            }

            CurrentPosition = currentPosition;
            WhiteChipTotal = whiteChipTotal;
            HasExploded = hasExploded;
            IsStopped = isStopped;
            LastPlacedToken = lastPlacedToken;
            PlacedTokens = new ReadOnlyCollection<PlacedTokenSnapshot>(new List<PlacedTokenSnapshot>(placedTokens));
        }

        public int CurrentPosition { get; }

        public int WhiteChipTotal { get; }

        public bool HasExploded { get; }

        public bool IsStopped { get; }

        public PlacedTokenSnapshot? LastPlacedToken { get; }

        public IReadOnlyList<PlacedTokenSnapshot> PlacedTokens { get; }

        internal static CauldronSnapshot FromCauldron(CauldronState cauldron)
        {
            var placedTokens = new List<PlacedTokenSnapshot>();

            foreach (var placedToken in cauldron.PlacedTokens)
            {
                placedTokens.Add(PlacedTokenSnapshot.FromPlacedToken(placedToken));
            }

            return new CauldronSnapshot(
                cauldron.CurrentPosition,
                cauldron.WhiteChipTotal,
                cauldron.HasExploded,
                cauldron.IsStopped,
                cauldron.LastPlacedToken == null ? null : PlacedTokenSnapshot.FromPlacedToken(cauldron.LastPlacedToken),
                placedTokens);
        }
    }
}
