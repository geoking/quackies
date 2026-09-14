using System.Collections.Generic;
using Quackies.Core.Ducks.Runtime;
using Quackies.Core.Match;

namespace Quackies.Core.Ducks.AI
{
    /// <summary>A Duck policy sees only one detached observation and its currently issued actions.</summary>
    public interface IDuckPlayerPolicy
    {
        GameAction Choose(DuckMatchView observation, IReadOnlyList<GameAction> legalActions);
    }
}
