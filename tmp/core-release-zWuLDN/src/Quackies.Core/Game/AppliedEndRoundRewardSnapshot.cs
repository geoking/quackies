using Quackies.Core.Rewards;

namespace Quackies.Core.Game
{
    public sealed class AppliedEndRoundRewardSnapshot
    {
        public AppliedEndRoundRewardSnapshot(
            int appliedVictoryPoints,
            int appliedBuyingPower,
            int appliedRubies,
            ExplodedRewardChoice? explodedRewardChoice)
        {
            AppliedVictoryPoints = appliedVictoryPoints;
            AppliedBuyingPower = appliedBuyingPower;
            AppliedRubies = appliedRubies;
            ExplodedRewardChoice = explodedRewardChoice;
        }

        public int AppliedVictoryPoints { get; }

        public int AppliedBuyingPower { get; }

        public int AppliedRubies { get; }

        public ExplodedRewardChoice? ExplodedRewardChoice { get; }

        internal static AppliedEndRoundRewardSnapshot FromAppliedReward(AppliedEndRoundRewardResult appliedReward)
        {
            return new AppliedEndRoundRewardSnapshot(
                appliedReward.AppliedVictoryPoints,
                appliedReward.AppliedBuyingPower,
                appliedReward.AppliedRubies,
                appliedReward.ExplodedRewardChoice);
        }
    }
}
