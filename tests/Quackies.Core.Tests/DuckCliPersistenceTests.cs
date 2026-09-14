using System.Text.Json;
using Quackies.Core.Ducks.Persistence;
using Quackies.Core.Ducks.Runtime;
using Quackies.Core.Match;
using Xunit;

namespace Quackies.Core.Tests;

public sealed partial class DuckCliTests
{
    private static readonly JsonSerializerOptions SaveJson = new() { IncludeFields = true };

    [Fact]
    public void Default_duck_game_runs_Normal_AI_and_does_not_allow_its_private_view()
    {
        var result = Run("view:ai\nq\n", "--profile", "ducks", "--seed", "42", "--no-save");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Human versus Normal AI.", result.Output);
        Assert.Contains("AI: Explore", result.Output);
        Assert.DoesNotContain("Private preview for ai", result.Output);
        Assert.DoesNotContain("Developer two-duck controls.", result.Output);
    }

    [Fact]
    public void CLI_autosaves_each_action_and_Continue_preserves_the_exact_next_draw()
    {
        using var files = new SaveFiles();
        var first = Run("human:1\nq\n", "--profile", "ducks", "--seed", "42", "--two-player", "--new-game", "--save", files.Path);
        Assert.Equal(0, first.ExitCode);
        var saved = files.Read();
        Assert.Single(saved.Players.Single(player => player.Id == "human").PlacedChips);
        Assert.Empty(files.ReadBackup().Players.Single(player => player.Id == "human").PlacedChips);
        var originalBytes = File.ReadAllBytes(files.Path);

        var review = Run("q\n", "--profile", "ducks", "--continue", "--two-player", "--save", files.Path);
        Assert.Equal(0, review.ExitCode);
        Assert.Contains("continuing saved game", review.Output);
        Assert.Equal(originalBytes, File.ReadAllBytes(files.Path));

        var expected = DuckSaves.Restore(saved);
        expected.Execute("human", expected.GetLegalActions("human").Single(action => action.Kind == GameActionKind.Explore));
        var next = Run("human:1\nq\n", "--profile", "ducks", "--continue", "--two-player", "--save", files.Path);
        Assert.Equal(0, next.ExitCode);
        Assert.Equal(JsonSerializer.Serialize(DuckSaves.Capture(expected), SaveJson), JsonSerializer.Serialize(files.Read(), SaveJson));
        Assert.Empty(Directory.GetFiles(files.Directory, ".quackies-save-*.tmp*"));
    }

    [Fact]
    public void CLI_purchase_is_saved_without_refunding_Sleep_or_reopening_the_purchase_slot()
    {
        using var files = new SaveFiles();
        var match = MatchSession.CreateDuck(0);
        foreach (var id in new[] { "human", "ai" }) ExecuteKind(match, id, GameActionKind.Explore);
        foreach (var id in new[] { "human", "ai" }) ExecuteKind(match, id, GameActionKind.Settle);
        files.Write(DuckSaves.Capture(match));
        var before = match.GetSnapshot("human").Players.Single(player => player.Id == "human");

        var result = Run("human:1\nq\n", "--profile", "ducks", "--continue", "--two-player", "--save", files.Path);
        Assert.Equal(0, result.ExitCode);
        var restored = DuckSaves.Restore(files.Read());
        var buyer = restored.GetSnapshot("human").Players.Single(player => player.Id == "human");
        Assert.Equal(new[] { "seeds" }, buyer.PurchasedEncounterDefinitionIds);
        Assert.Equal(before.RemainingSleep - 3, buyer.RemainingSleep);
        Assert.Equal(before.FrozenSleep, buyer.FrozenSleep);
        Assert.Single(restored.GetLegalActions("human"), action => action.Kind == GameActionKind.FinishDream);
        Assert.DoesNotContain(restored.GetLegalActions("human"), action => action.Kind == GameActionKind.BuyEncounter);
    }

    [Fact]
    public void CLI_saves_a_pending_final_Day_commit_then_reveals_the_same_two_draws_after_Continue()
    {
        using var files = new SaveFiles();
        var expected = AtFinalDawn();
        files.Write(DuckSaves.Capture(expected));
        ExecuteKind(expected, "human", GameActionKind.Explore);

        var pending = Run("human:1\nq\n", "--profile", "ducks", "--continue", "--two-player", "--save", files.Path);
        Assert.Equal(0, pending.ExitCode);
        var saved = files.Read();
        Assert.Equal("human", Assert.Single(saved.FinalDayCommits).PlayerId);
        Assert.All(saved.Players, player => Assert.Empty(player.PlacedChips));
        Assert.Equal(JsonSerializer.Serialize(DuckSaves.Capture(expected), SaveJson), JsonSerializer.Serialize(saved, SaveJson));

        ExecuteKind(expected, "ai", GameActionKind.Explore);
        var reveal = Run("ai:1\nq\n", "--profile", "ducks", "--continue", "--two-player", "--save", files.Path);
        Assert.Equal(0, reveal.ExitCode);
        Assert.Empty(files.Read().FinalDayCommits);
        Assert.Equal(JsonSerializer.Serialize(DuckSaves.Capture(expected), SaveJson), JsonSerializer.Serialize(files.Read(), SaveJson));
    }

    [Fact]
    public void Full_Normal_demo_saves_final_results_and_Continue_does_not_award_them_again()
    {
        using var files = new SaveFiles();
        var result = Run("", "--profile", "ducks", "--seed", "42", "--demo-game", "--save", files.Path);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Complete ten-Day match finished.", result.Output);
        Assert.Contains("Final standings", result.Output);
        Assert.Contains("Day 10/10 · Finished · Nest level 3", result.Output);
        Assert.DoesNotContain("Day 11/10", result.Output);
        var saved = files.Read();
        Assert.Equal(DuckPhase.Finished, saved.Phase);
        Assert.NotNull(saved.FinalResult);
        Assert.NotEmpty(saved.FinalResult!.WinnerIds);
        var finalBytes = File.ReadAllBytes(files.Path);

        var review = Run("", "--profile", "ducks", "--continue", "--inspect", "--save", files.Path);
        Assert.Equal(0, review.ExitCode);
        Assert.Contains("Final standings", review.Output);
        Assert.Equal(finalBytes, File.ReadAllBytes(files.Path));
    }

    [Fact]
    public void Corrupt_primary_recovers_the_previous_action_with_an_explicit_message()
    {
        using var files = new SaveFiles();
        files.Write(DuckSaves.Capture(MatchSession.CreateDuck(42)), backup: true);
        File.WriteAllText(files.Path, "not a save");
        var result = Run("", "--profile", "ducks", "--continue", "--inspect", "--save", files.Path);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Recovered the previous saved action from backup", result.Error);
        Assert.Contains("Human: 0 Twigs · space 0", result.Output);
        Assert.Equal("not a save", File.ReadAllText(files.Path));
    }

    [Fact]
    public void Corrupt_save_without_a_valid_backup_fails_without_starting_a_new_game()
    {
        using var files = new SaveFiles();
        File.WriteAllText(files.Path, "{}");
        var result = Run("", "--profile", "ducks", "--continue", "--save", files.Path);
        Assert.Equal(3, result.ExitCode);
        Assert.Contains("Save/Continue failed", result.Error);
        Assert.DoesNotContain("all ducks start at the nest", result.Output);
        Assert.Equal("{}", File.ReadAllText(files.Path));
    }

    [Fact]
    public void Failed_backup_replacement_preserves_the_previous_primary_save()
    {
        using var files = new SaveFiles();
        files.Write(DuckSaves.Capture(MatchSession.CreateDuck(42)));
        var bytes = File.ReadAllBytes(files.Path);
        Directory.CreateDirectory(files.Path + ".bak");
        var result = Run("human:1\nq\n", "--profile", "ducks", "--continue", "--two-player", "--save", files.Path);
        Assert.Equal(3, result.ExitCode);
        Assert.Contains("Save/Continue failed", result.Error);
        Assert.Equal(bytes, File.ReadAllBytes(files.Path));
        Assert.Empty(Directory.GetFiles(files.Directory, ".quackies-save-*.tmp*"));
    }

    [Fact]
    public void Restart_creates_a_new_zero_start_game_and_saves_it()
    {
        using var files = new SaveFiles();
        var result = Run("human:1\nr\nq\n", "--profile", "ducks", "--seed", "42", "--two-player", "--new-game", "--save", files.Path);
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("New game · seed 42", result.Output);
        Assert.All(files.Read().Players, player =>
        {
            Assert.Equal(0, player.Position);
            Assert.Equal(0, player.TotalTwigs);
            Assert.Equal(0, player.PermanentFeatherTrail);
            Assert.Empty(player.PlacedChips);
        });
    }

    private static MatchSession<DuckMatchView> AtFinalDawn()
    {
        var match = MatchSession.CreateDuck(123);
        for (var step = 0; step < 1000; step++)
        {
            foreach (var id in new[] { "human", "ai" })
            {
                if (match.GetSnapshot("human").Day == 10) return match;
                var actions = match.GetLegalActions(id);
                if (actions.Count == 0) continue;
                var action = actions.FirstOrDefault(item => item.Kind == GameActionKind.Settle)
                    ?? actions.FirstOrDefault(item => item.Kind == GameActionKind.FinishDream)
                    ?? actions[0];
                match.Execute(id, action);
            }
        }
        throw new InvalidOperationException("Final Dawn was not reached.");
    }

    private static void ExecuteKind(MatchSession<DuckMatchView> match, string id, GameActionKind kind) =>
        match.Execute(id, match.GetLegalActions(id).Single(action => action.Kind == kind));

    private sealed class SaveFiles : IDisposable
    {
        internal SaveFiles()
        {
            Directory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "quackies-cli-test-" + Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(Directory);
            Path = System.IO.Path.Combine(Directory, "duck.json");
        }
        internal string Directory { get; }
        internal string Path { get; }
        internal DuckSaveData Read() => JsonSerializer.Deserialize<DuckSaveData>(File.ReadAllText(Path), SaveJson)!;
        internal DuckSaveData ReadBackup() => JsonSerializer.Deserialize<DuckSaveData>(File.ReadAllText(Path + ".bak"), SaveJson)!;
        internal void Write(DuckSaveData data, bool backup = false) => File.WriteAllText(Path + (backup ? ".bak" : ""), JsonSerializer.Serialize(data, SaveJson));
        public void Dispose() => System.IO.Directory.Delete(Directory, recursive: true);
    }
}
