using System.Collections.Generic;
using Quackies.Core.Match;

namespace Quackies.Core.AI
{
    /// <summary>Compatibility entry point; the initial game's opponent now uses Normal's no-risk policy.</summary>
    public sealed class BalancedPolicy : IPlayerPolicy
    {
        private readonly NormalPolicy _normal = new NormalPolicy();
        public GameAction Choose(MatchView observation, IReadOnlyList<GameAction> legalActions) =>
            _normal.Choose(observation, legalActions);
    }
}
