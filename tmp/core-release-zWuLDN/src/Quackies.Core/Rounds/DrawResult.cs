using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quackies.Core.Cauldrons;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rounds
{
    public sealed class DrawResult
    {
        public DrawResult(
            Token drawnToken,
            PlacedToken placedToken,
            int newCauldronPosition,
            int whiteChipTotal,
            bool hasExploded,
            int remainingBagCount,
            IEnumerable<DrawEvent> events)
        {
            if (drawnToken == null)
            {
                throw new ArgumentNullException(nameof(drawnToken));
            }

            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            if (placedToken == null)
            {
                throw new ArgumentNullException(nameof(placedToken));
            }

            DrawnToken = drawnToken;
            PlacedToken = placedToken;
            NewCauldronPosition = newCauldronPosition;
            WhiteChipTotal = whiteChipTotal;
            HasExploded = hasExploded;
            RemainingBagCount = remainingBagCount;
            Events = new ReadOnlyCollection<DrawEvent>(new List<DrawEvent>(events));
        }

        public Token DrawnToken { get; }

        public PlacedToken PlacedToken { get; }

        public int NewCauldronPosition { get; }

        public int WhiteChipTotal { get; }

        public bool HasExploded { get; }

        public int RemainingBagCount { get; }

        public IReadOnlyList<DrawEvent> Events { get; }
    }
}
