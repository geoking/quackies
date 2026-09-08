using System;

namespace Quackies.Core.Match
{
    /// <summary>Immutable match setup options. Standard preserves the published base-game setup.</summary>
    public sealed class MatchSettings
    {
        public static MatchSettings Standard { get; } = new MatchSettings(1);

        public MatchSettings(int startingRubies = 1)
        {
            if (startingRubies < 0 || startingRubies > 1)
                throw new ArgumentOutOfRangeException(nameof(startingRubies), "Starting rubies must be zero or one.");
            StartingRubies = startingRubies;
        }

        public int StartingRubies { get; }
    }
}
