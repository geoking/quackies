using Quackies.Core.Ducks.Definitions;
using Quackies.Core.Ducks.AI;
using Quackies.Core.Ducks.Runtime;
using Quackies.Core.Match;

namespace Quackies.Cli;

internal static class DuckCli
{
    internal static int Run(string[] arguments)
    {
        try
        {
            var options = DuckCliOptions.Parse(arguments);
            var store = options.ShouldSave || options.ContinueGame ? new DuckSaveStore(options.EffectiveSavePath) : null;
            var resume = options.ContinueGame;
            if (!resume && !options.NewGame && !options.DemoDay && !options.DemoGame && store?.HasSavedGame == true)
            {
                Console.WriteLine("Saved game found. Enter c to Continue, n for a new game, or q to quit.");
                var choice = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (choice != "c" && choice != "n") return 0;
                resume = choice == "c";
            }
            var recoveredBackup = false;
            var match = resume ? store!.Load(out recoveredBackup) : MatchSession.CreateDuck(options.Seed);
            if (recoveredBackup) Console.Error.WriteLine("Recovered the previous saved action from backup; the latest primary save could not be read.");
            Console.WriteLine(resume
                ? "Quackies ducks · 10 Days · continuing saved game"
                : $"Quackies ducks · 10 Days · seed {options.Seed} · all ducks start at the nest");
            Show(match.GetSnapshot("human"));
            if (options.InspectOnly)
            {
                ShowCatalogue(match.GetSnapshot("human"));
                return 0;
            }
            if (options.DemoDay && match.GetSnapshot("human").Day != 1)
                throw new ArgumentException("--demo-day starts or continues Day 1 only; use --demo-game for the full match.");
            if (options.ShouldSave && !resume) store!.Save(match);
            if (options.ShouldSave) Console.WriteLine("Autosave: " + store!.SavePath);
            void SaveAfterAction() { if (options.ShouldSave) store!.Save(match); }
            if (options.DemoDay) return DemoDay(match, SaveAfterAction);
            if (options.DemoGame) return DemoGame(match, SaveAfterAction);
            Console.WriteLine(options.TwoPlayer ? "Developer two-duck controls." : "Human versus Normal AI.");
            Console.WriteLine(options.TwoPlayer
                ? "Enter human:1 or ai:1 to choose an action; view:human / view:ai inspects that duck. r restarts; q quits."
                : "Enter an action number. view:human reviews your board; r restarts; q quits.");
            var viewer = "human";
            var normal = new DuckNormalPolicy();
            while (true)
            {
                if (!options.TwoPlayer)
                {
                    do
                    {
                        var aiActions = match.GetLegalActions("ai");
                        if (aiActions.Count == 0 || aiActions.All(action => action.Kind == GameActionKind.NextDay)) break;
                        var before = match.GetSnapshot("ai");
                        var action = normal.Choose(before, aiActions);
                        match.Execute("ai", action);
                        SaveAfterAction();
                        var after = match.GetSnapshot("human");
                        Console.WriteLine(before.Day == 10 && before.Phase == DuckPhase.Adventure && after.AwaitingFinalDayDecisions
                            ? "AI committed its hidden final-Day decision."
                            : "AI: " + action.Label);
                        Show(after);
                    } while (match.GetLegalActions("human").Count == 0
                        && match.GetSnapshot("human").Phase != DuckPhase.Finished);
                }
                var issued = new Dictionary<string, IReadOnlyList<GameAction>>();
                foreach (var id in options.TwoPlayer ? new[] { "human", "ai" } : new[] { "human" })
                {
                    issued[id] = match.GetLegalActions(id);
                    for (var i = 0; i < issued[id].Count; i++) Console.WriteLine($"{id}:{i + 1}. {issued[id][i].Label}");
                }
                if (issued.Values.All(actions => actions.Count == 0))
                {
                    if (match.GetSnapshot(viewer).Phase != DuckPhase.Finished)
                        throw new InvalidOperationException("No active duck has a legal action before the game is finished.");
                    Console.WriteLine("Game complete. Enter r to restart or q to quit.");
                }
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == null) { Console.WriteLine("Input closed. Quitting."); return 0; }
                if (input.Equals("q", StringComparison.OrdinalIgnoreCase)) return 0;
                if (input.Equals("r", StringComparison.OrdinalIgnoreCase))
                {
                    match = MatchSession.CreateDuck(options.Seed);
                    SaveAfterAction();
                    Console.WriteLine("New game · seed " + options.Seed);
                    Show(match.GetSnapshot(viewer));
                    continue;
                }
                if (!options.TwoPlayer && int.TryParse(input, out _)) input = "human:" + input;
                var parts = input.Split(':', 2);
                if (parts.Length == 2 && parts[0] == "view" && issued.ContainsKey(parts[1]))
                {
                    viewer = parts[1];
                    Show(match.GetSnapshot(viewer));
                    continue;
                }
                if (parts.Length != 2 || !issued.TryGetValue(parts[0], out var actions)
                    || !int.TryParse(parts[1], out var number) || number < 1 || number > actions.Count)
                {
                    Console.WriteLine("Choose one of the listed player:action entries.");
                    continue;
                }
                match.Execute(parts[0], actions[number - 1]);
                SaveAfterAction();
                Show(match.GetSnapshot(viewer));
            }
        }
        catch (ArgumentException exception)
        {
            Console.Error.WriteLine(exception.Message);
            Console.Error.WriteLine(DuckCliOptions.Usage);
            return 2;
        }
        catch (InvalidOperationException exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
        catch (Exception exception) when (exception is IOException or InvalidDataException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            Console.Error.WriteLine("Save/Continue failed: " + exception.Message);
            return 3;
        }
    }

    private static int DemoDay(MatchSession<DuckMatchView> match, Action saveAfterAction)
    {
        Console.WriteLine("Deterministic daily-cycle smoke demonstration; these are scripted choices, not Normal AI.");
        for (var step = 0; step < 200; step++)
        {
            var view = match.GetSnapshot("human");
            if (view.Day == 2)
            {
                Console.WriteLine("Verified CLI boundary reached: Day 1 → Night 1 → Day 2.");
                return 0;
            }
            var acted = false;
            foreach (var id in new[] { "human", "ai" })
            {
                view = match.GetSnapshot(id);
                var player = view.Players.Single(p => p.Id == id);
                var actions = match.GetLegalActions(id);
                var chosen = actions.FirstOrDefault(action => action.Kind == GameActionKind.NextDay)
                    ?? actions.FirstOrDefault(action => action.Kind == GameActionKind.Settle && player.PlacedChips.Count >= (id == "human" ? 4 : 3))
                    ?? actions.FirstOrDefault(action => action.Kind == GameActionKind.Explore)
                    ?? actions.FirstOrDefault(action => action.Kind == GameActionKind.BuyEncounter)
                    ?? actions.FirstOrDefault(action => action.Kind == GameActionKind.FinishDream);
                if (chosen == null) continue;
                Console.WriteLine($"{id}: {chosen.Label}");
                match.Execute(id, chosen);
                saveAfterAction();
                Show(match.GetSnapshot("human"));
                acted = true;
                if (match.GetSnapshot("human").Day == 2) break;
            }
            if (!acted) throw new InvalidOperationException("The daily-cycle demo has no legal action; the current checkpoint is incomplete.");
        }
        throw new InvalidOperationException("The daily-cycle demonstration exceeded its action bound.");
    }

    private static int DemoGame(MatchSession<DuckMatchView> match, Action saveAfterAction)
    {
        var normal = new DuckNormalPolicy();
        Console.WriteLine("Ten-Day demonstration: Normal policy controls both ducks using their own observations.");
        for (var step = 0; step < 4000; step++)
        {
            if (match.GetSnapshot("human").Phase == DuckPhase.Finished)
            {
                Console.WriteLine("Complete ten-Day match finished.");
                return 0;
            }
            var acted = false;
            foreach (var id in new[] { "human", "ai" })
            {
                var actions = match.GetLegalActions(id);
                if (actions.Count == 0) continue;
                var before = match.GetSnapshot(id);
                var chosen = normal.Choose(before, actions);
                match.Execute(id, chosen);
                saveAfterAction();
                var after = match.GetSnapshot("human");
                Console.WriteLine(before.Day == 10 && before.Phase == DuckPhase.Adventure && after.AwaitingFinalDayDecisions
                    ? id + " committed a hidden final-Day decision."
                    : id + ": " + chosen.Label);
                Show(after);
                acted = true;
            }
            if (!acted) throw new InvalidOperationException("The match has no legal action before final scoring.");
        }
        throw new InvalidOperationException("The ten-Day demonstration exceeded its action bound.");
    }

    private static void Show(DuckMatchView view)
    {
        Console.WriteLine($"Day {view.Day}/10 · {view.Phase} · World Event: {view.CurrentEvent.Name}");
        foreach (var player in view.Players)
        {
            Console.WriteLine($"{player.Name}: {player.TotalTwigs} Twigs · space {player.Position} · Exhaustion {player.Exhaustion}/{player.SafeExhaustionMaximum} · bag {player.BagCount}" +
                (player.IsWornOut ? " · WORN OUT" : player.HasFinishedDay ? " · resting" : ""));
            Console.WriteLine($"  Feather trail {player.PermanentFeatherTrail} · today's start {player.EffectiveStart} · active flock {player.ActiveFlock}" +
                (player.ActiveMostRestedStep ? " · active zzz +1" : "") + (player.PendingMostRestedStep ? " · zzz for tomorrow" : ""));
            if (player.LastNightOutcome is { } night)
            {
                Console.WriteLine($"  Night {night.Day}: {night.FrozenSleep} frozen Sleep; {player.RemainingSleep} available now. Twigs earned {night.TotalTwigsEarned}; Feathers {night.FeathersAwarded}" +
                    (night.IsMostRested ? " · Most Rested" : ""));
                Console.WriteLine($"  Sleep: printed {night.PrintedSleep}, Flowers +{night.FlowerSleep}, final haven +{night.FinalHavenSleep}, event +{night.CollectiveEventSleep}, flock +{night.FlockSleep}, Restless Night -{night.RestlessNightPenalty}, Pebbles -{night.PebblesPenalty}; before wear {night.SleepBeforeWear}.");
                Console.WriteLine($"  Twigs: printed {night.PrintedTwigs}, Reeds +{night.ReedsTwigs}, event +{night.EventTwigs}, Brambles -{night.BramblesPenalty}.");
            }
        }
        if (view.KnownNextChips.Count > 0)
            Console.WriteLine($"Private preview for {view.ViewerId}, in order: " + string.Join(", ", view.KnownNextChips.Select(chip => chip.DefinitionId)));
        foreach (var entry in view.History.TakeLast(4)) Console.WriteLine($"  D{entry.Day} {entry.ActorId}: {entry.Message}");
        if (view.FinalResult is { } result)
        {
            Console.WriteLine("Final standings · total Twigs, then Night 10 retained Sleep:");
            foreach (var standing in result.Standings)
                Console.WriteLine($"  {standing.Rank}. {standing.PlayerName}: {standing.TotalTwigs} Twigs (including {standing.DreamTwigs} Dream Twigs), {standing.FrozenNightTenSleep} Sleep");
            Console.WriteLine(result.WinnerIds.Count == 1
                ? "Winner: " + result.Standings.Single(standing => standing.IsWinner).PlayerName
                : "Draw: " + string.Join(", ", result.Standings.Where(standing => standing.IsWinner).Select(standing => standing.PlayerName)));
        }
        Console.WriteLine();
    }

    private static void ShowCatalogue(DuckMatchView view)
    {
        Console.WriteLine($"Catalogue: {DuckRules.V1.BoardSpaces.Count} rewards · {DuckRules.V1.BoardSpaces.Count(s => s.IsHaven)} havens · {DuckRules.V1.EncounterDefinitions.Count} encounter variants · {view.ShopOffers.Count} shop offers · {DuckRules.V1.WorldEvents.Count} World Events");
        Console.WriteLine("Own inventory: " + string.Join(", ", view.OwnInventory.GroupBy(chip => chip.DefinitionId).Select(group => $"{group.Key} ×{group.Count()}")));
        foreach (var offer in view.ShopOffers)
            Console.WriteLine($"  {offer.DefinitionId}: {offer.SleepPrice} Sleep · movement {(offer.Encounter.BaseMovement?.ToString() ?? "flock-dependent")} · Twig yield {offer.Encounter.TwigYield}");
    }
}
