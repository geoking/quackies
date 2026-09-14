using System;

namespace Quackies.Core.Ducks.Runtime
{
    /// <summary>Immutable v1 Duck match setup. Both players receive the same starting trail.</summary>
    public sealed class DuckMatchSettings
    {
        public const int StandardDays = 10;
        public static DuckMatchSettings Standard { get; } = new DuckMatchSettings();

        public DuckMatchSettings(int startingFeathers = 0)
        {
            if (startingFeathers < 0 || startingFeathers > 3)
                throw new ArgumentOutOfRangeException(nameof(startingFeathers), "Starting Feathers must be between zero and three.");

            StartingFeathers = startingFeathers;
        }

        public int Days => StandardDays;
        public int StartingFeathers { get; }
    }
}
