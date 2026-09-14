using System;
using System.Globalization;

namespace Quackies.Core.Ducks.Definitions
{
    /// <summary>One occupiable and scorable route reward. The separate nest at zero is not a row.</summary>
    public sealed class DuckBoardSpace
    {
        public DuckBoardSpace(
            int space,
            DuckBiome biome,
            int sleep,
            int twigs,
            int feathers,
            string? havenName)
        {
            if (space < 1 || space > 43) throw new ArgumentOutOfRangeException(nameof(space));
            if (!Enum.IsDefined(typeof(DuckBiome), biome)) throw new ArgumentOutOfRangeException(nameof(biome));
            if (sleep < 0) throw new ArgumentOutOfRangeException(nameof(sleep));
            if (twigs < 0) throw new ArgumentOutOfRangeException(nameof(twigs));
            if (feathers < 0) throw new ArgumentOutOfRangeException(nameof(feathers));
            if (havenName != null && (havenName.Length == 0 || havenName.Trim() != havenName))
                throw new ArgumentException("A haven name must be non-empty and have no surrounding whitespace.", nameof(havenName));
            if (havenName == null && feathers != 0)
                throw new ArgumentException("Only a haven may award Feathers.", nameof(feathers));

            Space = space;
            Biome = biome;
            Sleep = sleep;
            Twigs = twigs;
            Feathers = feathers;
            HavenName = havenName;
        }

        public string DefinitionId => "space_" + Space.ToString("00", CultureInfo.InvariantCulture);
        public int Space { get; }
        public DuckBiome Biome { get; }
        public int Sleep { get; }
        public int Twigs { get; }
        public int Feathers { get; }
        public string? HavenName { get; }
        public bool IsHaven => HavenName != null;
    }
}
