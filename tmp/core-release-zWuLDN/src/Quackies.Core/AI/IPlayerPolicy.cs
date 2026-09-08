using System.Collections.Generic;
using Quackies.Core.Match;

namespace Quackies.Core.AI
{
    /// <summary>A policy can see only a player's detached observation and currently legal actions.</summary>
    public interface IPlayerPolicy
    {
        GameAction Choose(MatchView observation, IReadOnlyList<GameAction> legalActions);
    }
}
