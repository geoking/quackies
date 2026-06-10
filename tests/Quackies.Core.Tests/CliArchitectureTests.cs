using System;
using System.IO;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class CliArchitectureTests
{
    [Fact]
    public void CliUsesGameFacadeInsteadOfOwningRuleOrchestration()
    {
        var programText = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "src", "Quackies.Cli", "Program.cs"));

        Assert.Contains("QuackiesGame.CreateSinglePlayer", programText, StringComparison.Ordinal);
        Assert.DoesNotContain("DefaultBagFactory", programText, StringComparison.Ordinal);
        Assert.DoesNotContain("new PlayerState", programText, StringComparison.Ordinal);
        Assert.DoesNotContain("CauldronRewardResolver", programText, StringComparison.Ordinal);
        Assert.DoesNotContain("ApplyEndRoundReward", programText, StringComparison.Ordinal);
        Assert.DoesNotContain("StopRound();", programText.Replace("game.StopRound();", string.Empty, StringComparison.Ordinal), StringComparison.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Quackies.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate Quackies.sln.");
    }
}
