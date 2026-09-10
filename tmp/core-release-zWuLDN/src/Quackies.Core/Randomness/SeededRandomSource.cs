using System;

namespace Quackies.Core.Randomness
{
    public sealed class SeededRandomSource : IRandomSource
    {
        private readonly Random _random;

        public SeededRandomSource(int seed)
        {
            _random = new Random(seed);
        }

        public int NextInt(int exclusiveMax)
        {
            if (exclusiveMax <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exclusiveMax), "Maximum value must be greater than zero.");
            }

            return _random.Next(exclusiveMax);
        }
    }
}
