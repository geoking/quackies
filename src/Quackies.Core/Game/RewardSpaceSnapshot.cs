using Quackies.Core.Cauldrons;

namespace Quackies.Core.Game
{
    public sealed class RewardSpaceSnapshot
    {
        public RewardSpaceSnapshot(int position, int buyingPower, int victoryPoints, bool hasRuby)
        {
            Position = position;
            BuyingPower = buyingPower;
            VictoryPoints = victoryPoints;
            HasRuby = hasRuby;
        }

        public int Position { get; }

        public int BuyingPower { get; }

        public int VictoryPoints { get; }

        public bool HasRuby { get; }

        internal static RewardSpaceSnapshot FromRewardSpace(CauldronSpace space)
        {
            return new RewardSpaceSnapshot(
                space.Position,
                space.BuyingPower,
                space.VictoryPoints,
                space.HasRuby);
        }
    }
}
