namespace Quackies.Core.Randomness
{
    public interface IRandomSource
    {
        int NextInt(int exclusiveMax);
    }
}
