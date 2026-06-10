using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quackies.Core.Bags;
using Quackies.Core.Randomness;
using Quackies.Core.Rounds;
using Quackies.Core.Tokens;

namespace Quackies.Core.Players
{
    public sealed class PlayerState
    {
        private readonly List<PlacedToken> _drawnTokens;

        public PlayerState(Bag bag)
        {
            if (bag == null)
            {
                throw new ArgumentNullException(nameof(bag));
            }

            Bag = bag;
            _drawnTokens = new List<PlacedToken>();
        }

        public Bag Bag { get; }

        public int CurrentCauldronPosition { get; private set; }

        public int WhiteChipTotal { get; private set; }

        public bool HasExploded { get; private set; }

        public IReadOnlyList<PlacedToken> DrawnTokens
        {
            get { return new ReadOnlyCollection<PlacedToken>(_drawnTokens); }
        }

        public DrawResult DrawToken(IRandomSource random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (HasExploded)
            {
                throw new InvalidOperationException("Cannot draw another token after the player has exploded.");
            }

            var token = Bag.DrawToken(random);
            var events = new List<DrawEvent>
            {
                new DrawEvent(DrawEventType.TokenDrawn, $"Drew {token}.")
            };

            CurrentCauldronPosition += token.Value;
            _drawnTokens.Add(new PlacedToken(token, CurrentCauldronPosition));

            if (token.Color == TokenColor.White)
            {
                WhiteChipTotal += token.Value;
                events.Add(new DrawEvent(DrawEventType.WhiteChipTotalChanged, $"White chip total is now {WhiteChipTotal}."));
            }

            HasExploded = WhiteChipTotal > 7;

            if (HasExploded)
            {
                events.Add(new DrawEvent(DrawEventType.Explosion, "The cauldron exploded."));
            }

            return new DrawResult(
                token,
                CurrentCauldronPosition,
                WhiteChipTotal,
                HasExploded,
                Bag.Count,
                events);
        }
    }
}
