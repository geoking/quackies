using Quackies.Core.Rewards;

namespace Quackies.Core.Game
{
    public sealed class EndRoundRewardSnapshot
    {
        public EndRoundRewardSnapshot(
            int position,
            int buyingPower,
            int victoryPoints,
            bool hasRuby,
            bool playerExploded,
            bool mayTakeBothVictoryPointsAndBuyingPower,
            bool mustChooseVictoryPointsOrBuyingPower)
        {
            Position = position;
            BuyingPower = buyingPower;
            VictoryPoints = victoryPoints;
            HasRuby = hasRuby;
            PlayerExploded = playerExploded;
            MayTakeBothVictoryPointsAndBuyingPower = mayTakeBothVictoryPointsAndBuyingPower;
            MustChooseVictoryPointsOrBuyingPower = mustChooseVictoryPointsOrBuyingPower;
        }

        public int Position { get; }

        public int BuyingPower { get; }

        public int VictoryPoints { get; }

        public bool HasRuby { get; }

        public bool PlayerExploded { get; }

        public bool MayTakeBothVictoryPointsAndBuyingPower { get; }

        public bool MustChooseVictoryPointsOrBuyingPower { get; }

        internal static EndRoundRewardSnapshot FromReward(EndRoundRewardResult reward)
        {
            return new EndRoundRewardSnapshot(
                reward.Position,
                reward.BuyingPower,
                reward.VictoryPoints,
                reward.HasRuby,
                reward.PlayerExploded,
                reward.MayTakeBothVictoryPointsAndBuyingPower,
                reward.MustChooseVictoryPointsOrBuyingPower);
        }
    }
}
