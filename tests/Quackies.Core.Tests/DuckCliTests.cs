using System.Diagnostics;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class DuckCliTests
{
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
        Assert.Contains("reeds_3: 16 Sleep · movement 1 · Twig yield 3", result.Output);
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
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root != null && !File.Exists(Path.Combine(root.FullName, "Quackies.sln"))) root = root.Parent;
        Assert.NotNull(root);
        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = root.FullName,
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
}
