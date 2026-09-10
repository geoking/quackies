using System;

namespace Quackies.Core.Match
{
    public sealed class MatchLogEntry
    {
        internal MatchLogEntry(int round, string actorId, string message)
        {
            Round = round;
            ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public int Round { get; }
        public string ActorId { get; }
        public string Message { get; }
    }
}
