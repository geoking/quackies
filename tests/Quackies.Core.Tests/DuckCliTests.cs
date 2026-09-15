using System.Diagnostics;
using Xunit;

namespace Quackies.Core.Tests;

public sealed partial class DuckCliTests
{
    [Fact]
    public void Default_route_is_ducks_and_classic_requires_an_explicit_profile()
    {
        var ducks = Run("", "--seed", "42", "--inspect");
        Assert.Equal(0, ducks.ExitCode);
        Assert.Contains("Quackies ducks · 10 Days · seed 42", ducks.Output);

        var classic = Run("q\n", "--profile", "classic", "--seed", "42");
        Assert.Equal(0, classic.ExitCode);
        Assert.Contains("Quackies classic reference · nine-round Set 1 match · seed 42", classic.Output);
    }

    [Fact]
    public void Launch_help_exits_without_creating_or_loading_a_save()
    {
        using var files = new SaveFiles();
        var result = Run("", "--save", files.Path, "--continue", "--help");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("The current ten-Day duck game is the default", result.Output);
        Assert.False(File.Exists(files.Path));
        Assert.False(File.Exists(files.Path + ".bak"));
    }

    [Fact]
    public void Invalid_profile_fails_before_starting_a_game()
    {
        var result = Run("", "--profile", "unknown");
        Assert.Equal(2, result.ExitCode);
        Assert.Contains("--profile must be ducks or classic", result.Error);
        Assert.DoesNotContain("Quackies ducks", result.Output);
        Assert.DoesNotContain("classic reference", result.Output);
    }

    [Fact]
    public void Interactive_references_are_observation_driven_and_keep_the_AI_view_private()
    {
        var input = "status\nboard 4\nbag\ntokens\nshop\nevent\nnight\nhistory\nview:ai\nq\n";
        var result = Run(input, "--seed", "42", "--no-save");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("4. Wetlands", result.Output);
        Assert.Contains("HAVEN: Reed hammock", result.Output);
        Assert.Contains("Remaining bag", result.Output);
        Assert.Contains("All 16 encounter variants", result.Output);
        Assert.Contains("reeds_3: Nesting reeds 3", result.Output);
        Assert.Contains("Dream shop", result.Output);
        Assert.Contains("World Event · Day 1: Thick Morning Mist", result.Output);
        Assert.Contains("No Night has resolved yet", result.Output);
        Assert.Contains("Public match history", result.Output);
        Assert.Contains("AI's private view is unavailable", result.Output);
        Assert.DoesNotContain("Private Signpost preview for ai", result.Output);
    }

    [Fact]
    public void Interactive_shop_for_a_revision_one_save_uses_its_live_prices()
    {
        using var files = new SaveFiles();
        var fixture = Path.Combine(RepositoryRoot(), "tests", "Quackies.Core.Tests", "Fixtures", "Duck", "seed20-day10-revision1.json");
        File.Copy(fixture, files.Path);
        var result = Run("shop\nq\n", "--continue", "--two-player", "--save", files.Path);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Dream shop", result.Output);
        Assert.Contains("tailwind_2: 5 Sleep", result.Output);
        Assert.Contains("reeds_3: 16 Sleep", result.Output);
        Assert.DoesNotContain("reeds_3: 20 Sleep", result.Output);
    }

    [Fact]
    public void Explicit_seed_replays_the_same_catalogue_output()
    {
        var first = Run("", "--seed", "42", "--inspect");
        var second = Run("", "--profile=ducks", "--seed=42", "--inspect");
        Assert.Equal(first.Output, second.Output);
    }

    [Fact]
    public void Daily_demo_runs_real_issued_actions_through_Night_purchases_and_next_Dawn()
    {
        var result = Run("", "--profile", "ducks", "--seed", "42", "--demo-day");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Day 1/10 · Adventure", result.Output);
        Assert.Contains("Day 1/10 · Night", result.Output);
        Assert.Contains("Night 1:", result.Output);
        Assert.Contains("frozen Sleep", result.Output);
        Assert.Contains("Twigs: printed", result.Output);
        Assert.Contains("human: Buy Seeds for 3 Sleep", result.Output);
        Assert.Contains("ai: Buy Seeds for 3 Sleep", result.Output);
        Assert.Contains("Day 2/10 · Adventure", result.Output);
        Assert.Contains("bag 14", result.Output);
        Assert.Contains("Dawn deficit", result.Output);
        Assert.Contains("Day 1 → Night 1 → Day 2", result.Output);
        Assert.DoesNotContain("Day 3/10", result.Output);
        Assert.DoesNotContain("Private preview for ai", result.Output);
    }

    [Fact]
    public void Inspect_exposes_the_duck_profile_without_classic_currencies()
    {
        var result = Run("", "--profile", "ducks", "--seed", "42", "--inspect");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("43 rewards · 8 havens · 16 encounter variants · 11 shop offers · 10 World Events", result.Output);
        Assert.Contains("bag 13", result.Output);
        Assert.Contains("reeds_3: 20 Sleep · movement 1 · Twig yield 3", result.Output);
        Assert.DoesNotContain("rubies", result.Output);
        Assert.DoesNotContain("coins", result.Output);
        Assert.DoesNotContain("Private preview", result.Output);
    }

    [Theory]
    [InlineData("--starting-feathers", "4")]
    [InlineData("--starting-feathers", "3")]
    [InlineData("--seed", "not-an-integer")]
    [InlineData("--unexpected", "option")]
    public void Invalid_duck_options_fail_clearly(string option, string value)
    {
        var result = Run("", "--profile", "ducks", option, value);
        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Usage: Quackies.Cli --profile ducks", result.Error);
        Assert.DoesNotContain("Quackies ducks ·", result.Output);
    }

    private static (int ExitCode, string Output, string Error) Run(string input, params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = RepositoryRoot(),
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        foreach (var argument in new[] { "run", "--project", "src/Quackies.Cli", "--configuration", "Release", "--verbosity", "quiet", "--" }.Concat(arguments))
            start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        process.StandardInput.Write(input);
        process.StandardInput.Close();
        if (!process.WaitForExit(30000))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("Duck CLI did not exit.");
        }
        return (process.ExitCode, output.GetAwaiter().GetResult(), error.GetAwaiter().GetResult());
    }

    private static string RepositoryRoot()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root != null && !File.Exists(Path.Combine(root.FullName, "Quackies.sln"))) root = root.Parent;
        Assert.NotNull(root);
        return root.FullName;
    }
}
