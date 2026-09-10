using System;

namespace Quackies.Core.Match
{
    public enum DieRollReason
    {
        RoundBonus,
        Fortune
    }

    /// <summary>Immutable observation of one resolved bonus-die roll.</summary>
    public sealed class DieRollView
    {
        internal DieRollView(int sequence, int round, string playerId, int face, DieRollReason reason,
            bool rewardApplied, string description)
        {
            Sequence = sequence;
            Round = round;
            PlayerId = playerId ?? throw new ArgumentNullException(nameof(playerId));
            Face = face;
            Reason = reason;
            RewardApplied = rewardApplied;
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public int Sequence { get; }
        public int Round { get; }
        public string PlayerId { get; }
        public int Face { get; }
        public DieRollReason Reason { get; }
        public bool RewardApplied { get; }
        public string Description { get; }
    }
}
