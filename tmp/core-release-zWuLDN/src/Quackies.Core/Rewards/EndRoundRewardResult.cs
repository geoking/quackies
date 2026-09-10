using System;
using Quackies.Core.Cauldrons;

namespace Quackies.Core.Rewards
{
    public sealed class EndRoundRewardResult
    {
        public EndRoundRewardResult(CauldronSpace rewardSpace, bool playerExploded)
        {
            if (rewardSpace == null)
            {
                throw new ArgumentNullException(nameof(rewardSpace));
            }

            RewardSpace = rewardSpace;
            Position = rewardSpace.Position;
            BuyingPower = rewardSpace.BuyingPower;
            VictoryPoints = rewardSpace.VictoryPoints;
            HasRuby = rewardSpace.HasRuby;
            PlayerExploded = playerExploded;
            MayTakeBothVictoryPointsAndBuyingPower = !playerExploded;
            MustChooseVictoryPointsOrBuyingPower = playerExploded;
        }

        public CauldronSpace RewardSpace { get; }

        public int Position { get; }

        public int BuyingPower { get; }

        public int VictoryPoints { get; }

        public bool HasRuby { get; }

        public bool PlayerExploded { get; }

        public bool MayTakeBothVictoryPointsAndBuyingPower { get; }

        public bool MustChooseVictoryPointsOrBuyingPower { get; }
    }
}
