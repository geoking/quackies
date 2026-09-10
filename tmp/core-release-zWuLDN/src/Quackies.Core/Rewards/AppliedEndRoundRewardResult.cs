namespace Quackies.Core.Rewards
{
    public sealed class AppliedEndRoundRewardResult
    {
        public AppliedEndRoundRewardResult(
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
    }
}
