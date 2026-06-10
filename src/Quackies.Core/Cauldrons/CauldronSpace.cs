using System;

namespace Quackies.Core.Cauldrons
{
    public sealed class CauldronSpace
    {
        public CauldronSpace(int position, int buyingPower, int victoryPoints, bool hasRuby)
        {
            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative.");
            }

            if (buyingPower < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(buyingPower), "Buying power cannot be negative.");
            }

            if (victoryPoints < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(victoryPoints), "Victory points cannot be negative.");
            }

            Position = position;
            BuyingPower = buyingPower;
            VictoryPoints = victoryPoints;
            HasRuby = hasRuby;
        }

        public int Position { get; }

        public int BuyingPower { get; }

        public int VictoryPoints { get; }

        public bool HasRuby { get; }
    }
}
