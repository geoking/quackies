using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Quackies.Core.Rules.Fortunes;
using Quackies.Core.Rules.Ingredients;
using Quackies.Core.Tokens;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class MatchSettingsTests
{
    [Fact]
    public void StandardSettingsGiveEveryPlayerOneStartingRuby()
    {
        var match = MatchSession.Create(new ZeroRandom());

        Assert.Equal(1, MatchSettings.Standard.StartingRubies);
        Assert.Same(MatchSettings.Standard, match.Settings);
        Assert.All(match.GetSnapshot("human").Players, player => Assert.Equal(1, player.Rubies));
    }

    [Fact]
    public void HouseSettingsGiveEveryPlayerZeroStartingRubies()
    {
        var settings = new MatchSettings(startingRubies: 0);
        var match = MatchSession.Create(new ZeroRandom(), settings);

        Assert.Same(settings, match.Settings);
        Assert.All(match.GetSnapshot("human").Players, player => Assert.Equal(0, player.Rubies));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void StartingRubiesMustStayWithinTheSupportedZeroOrOneRange(int startingRubies)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new MatchSettings(startingRubies));

        Assert.Equal("startingRubies", exception.ParamName);
    }

    [Fact]
    public void StartingBagRemainsTheImmutableNineChipReferenceAfterGiftDrawAndPurchase()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var beginnersBonus = SetOneFortunes.CreatePreparationBatch()
            .Single(card => string.Equals(card.Id, "beginners-bonus", StringComparison.Ordinal));
        var shop = new[]
        {
            new ShopChipDefinition(TokenColor.Green, 1, 1, 2, 1),
            new ShopChipDefinition(TokenColor.Orange, 1, 1, 1, 1)
        };
        var rules = new RuleSet(baseline.Track, SetOneIngredients.Create(), shop, new[] { beginnersBonus });
        var match = MatchSession.Create(new FixedRandom(0, 9, 0), rules);
        var afterGift = match.GetSnapshot("human");
        var reference = afterGift.StartingBag.Select(chip => (chip.Color, chip.Value)).ToArray();

        Assert.Equal(MatchPhase.Brewing, afterGift.Phase);
        Assert.Equal(10, afterGift.OwnBag.Count);
        Assert.Equal(9, afterGift.StartingBag.Count);
        Assert.Equal(7, afterGift.StartingBag.Count(chip => chip.Color == TokenColor.White));
        Assert.Single(afterGift.StartingBag, chip => chip.Color == TokenColor.Orange && chip.Value == 1);
        Assert.Single(afterGift.StartingBag, chip => chip.Color == TokenColor.Green && chip.Value == 1);
        Assert.Throws<NotSupportedException>(() =>
            ((IList<Token>)afterGift.StartingBag).Add(new Token(TokenColor.Green, 1)));

        ExecuteKind(match, "ai", GameActionKind.Stop);
        ExecuteKind(match, "human", GameActionKind.Draw);
        Assert.Equal(reference, match.GetSnapshot("human").StartingBag.Select(chip => (chip.Color, chip.Value)).ToArray());
        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "human", GameActionKind.BuyIngredient, TokenColor.Orange);

        var afterPurchase = match.GetSnapshot("human");
        Assert.Equal(11, afterPurchase.Players.Single(player => player.Id == "human").InventoryCount);
        Assert.Equal(reference, afterPurchase.StartingBag.Select(chip => (chip.Color, chip.Value)).ToArray());
    }

    [Fact]
    public void CliLeavesCompletedRoundAdvanceToHumanAndQuitsCleanlyAtEndOfInput()
    {
        var result = RunCli("2\n1\n1\n", "--seed", "0");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("starting rubies 1", result.StandardOutput, StringComparison.Ordinal);
        Assert.Contains("Round 1/9 · RoundComplete", result.StandardOutput, StringComparison.Ordinal);
        Assert.Contains("1. Start next round", result.StandardOutput, StringComparison.Ordinal);
        Assert.Contains("Input closed. Quitting.", result.StandardOutput, StringComparison.Ordinal);
        Assert.DoesNotContain("Round 2/9", result.StandardOutput, StringComparison.Ordinal);
    }

    [Theory]
    [MemberData(nameof(InvalidCliArguments))]
    public void CliReturnsExitCodeTwoForInvalidStartingRubyArguments(string[] arguments)
    {
        var result = RunCli(string.Empty, arguments);

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Usage: Quackies.Cli [--starting-rubies 0|1]", result.StandardError, StringComparison.Ordinal);
    }

    public static IEnumerable<object[]> InvalidCliArguments()
    {
        yield return new object[] { new[] { "--starting-rubies", "2" } };
        yield return new object[] { new[] { "--starting-rubies=-1" } };
        yield return new object[] { new[] { "--starting-rubies" } };
    }

    private static void ExecuteKind(MatchSession match, string playerId, GameActionKind kind, TokenColor? color = null)
    {
        var action = match.GetLegalActions(playerId).Single(candidate =>
            candidate.Kind == kind && (!color.HasValue || candidate.Color == color));
        match.Execute(playerId, action);
    }

    private static CliResult RunCli(string standardInput, params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = FindRepositoryRoot()
        };
        start.ArgumentList.Add("run");
        start.ArgumentList.Add("--project");
        start.ArgumentList.Add(Path.Combine(start.WorkingDirectory, "src", "Quackies.Cli", "Quackies.Cli.csproj"));
        start.ArgumentList.Add("--configuration");
        start.ArgumentList.Add("Release");
        start.ArgumentList.Add("--verbosity");
        start.ArgumentList.Add("quiet");
        start.ArgumentList.Add("--");
        foreach (var argument in arguments) start.ArgumentList.Add(argument);

        using var process = Process.Start(start) ?? throw new InvalidOperationException("Could not start the Quackies CLI.");
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        process.StandardInput.Write(standardInput);
        process.StandardInput.Close();
        if (!process.WaitForExit(30_000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("The Quackies CLI did not exit within 30 seconds.");
        }
        return new CliResult(process.ExitCode, output.GetAwaiter().GetResult(), error.GetAwaiter().GetResult());
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Quackies.sln"))) return directory.FullName;
            directory = directory.Parent;
        }
        throw new InvalidOperationException("Could not locate Quackies.sln.");
    }

    private sealed class CliResult
    {
        internal CliResult(int exitCode, string standardOutput, string standardError)
        {
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            StandardError = standardError;
        }

        internal int ExitCode { get; }
        internal string StandardOutput { get; }
        internal string StandardError { get; }
    }

    private sealed class ZeroRandom : IRandomSource
    {
        public int NextInt(int exclusiveMax) => 0;
    }

    private sealed class FixedRandom : IRandomSource
    {
        private readonly Queue<int> _values;
        internal FixedRandom(params int[] values) { _values = new Queue<int>(values); }

        public int NextInt(int exclusiveMax)
        {
            var value = _values.Count == 0 ? 0 : _values.Dequeue();
            if (value < 0 || value >= exclusiveMax)
                throw new InvalidOperationException($"Fixed value {value} is invalid for {exclusiveMax}.");
            return value;
        }
    }
}
