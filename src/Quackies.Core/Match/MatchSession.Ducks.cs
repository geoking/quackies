using Quackies.Core.Ducks.Runtime;

namespace Quackies.Core.Match
{
    public sealed partial class MatchSession
    {
        public static MatchSession<DuckMatchView> CreateDuck(int seed, DuckMatchSettings? settings = null)
        {
            return new MatchSession<DuckMatchView>(DuckMatchRuntime.Create(seed, settings));
        }
    }
}
