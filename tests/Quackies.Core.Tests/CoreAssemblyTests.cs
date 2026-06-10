using Quackies.Core;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class CoreAssemblyTests
{
    [Fact]
    public void CoreAssemblyCanBeLoaded()
    {
        Assert.Equal("Quackies.Core", typeof(CoreAssemblyMarker).Assembly.GetName().Name);
    }
}
