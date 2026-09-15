using System;
using System.IO;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class CliArchitectureTests
{
    [Fact]
    public void CliUsesMatchSessionInsteadOfOwningRuleOrchestration()
    {
        var cliDirectory = Path.Combine(FindRepositoryRoot(), "src", "Quackies.Cli");
        var sources = Directory.GetFiles(cliDirectory, "*.cs")
            .ToDictionary(path => Path.GetFileName(path)!, File.ReadAllText, StringComparer.Ordinal);
        var allText = string.Join("\n", sources.Values);

        Assert.Contains("CliEntry.Run(args)", sources["Program.cs"], StringComparison.Ordinal);
        Assert.Contains("MatchSession.CreateDuck", sources["DuckCli.cs"], StringComparison.Ordinal);
        Assert.Contains("MatchSession.Create", sources["ClassicCli.cs"], StringComparison.Ordinal);
        Assert.DoesNotContain("QuackiesGame.CreateSinglePlayer", allText, StringComparison.Ordinal);
        Assert.DoesNotContain("DefaultBagFactory", allText, StringComparison.Ordinal);
        Assert.DoesNotContain("new PlayerState", allText, StringComparison.Ordinal);
        Assert.DoesNotContain("CauldronRewardResolver", allText, StringComparison.Ordinal);
        Assert.DoesNotContain("ApplyEndRoundReward", allText, StringComparison.Ordinal);
        Assert.DoesNotContain("StopRound();", allText.Replace("game.StopRound();", string.Empty, StringComparison.Ordinal), StringComparison.Ordinal);
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
