namespace Quackies.Core.Game
{
    public sealed class GameSnapshot
    {
        public GameSnapshot(
            GamePhase phase,
            PlayerSnapshot currentPlayer,
            RewardSpaceSnapshot currentRewardSpace,
            EndRoundRewardSnapshot? pendingReward,
            AppliedEndRoundRewardSnapshot? appliedReward,
            string? roundEndReason)
        {
            Phase = phase;
            CurrentPlayer = currentPlayer;
            CurrentRewardSpace = currentRewardSpace;
            PendingReward = pendingReward;
            AppliedReward = appliedReward;
            RoundEndReason = roundEndReason;
        }

        public GamePhase Phase { get; }

        public PlayerSnapshot CurrentPlayer { get; }

        public RewardSpaceSnapshot CurrentRewardSpace { get; }

        public EndRoundRewardSnapshot? PendingReward { get; }

        public AppliedEndRoundRewardSnapshot? AppliedReward { get; }

        public string? RoundEndReason { get; }
    }
}
