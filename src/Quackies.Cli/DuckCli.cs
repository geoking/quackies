using Quackies.Core.Ducks.Definitions;
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
            var match = MatchSession.CreateDuck(options.Seed);
            Console.WriteLine($"Quackies ducks · 10 Days · seed {options.Seed} · all ducks start at the nest");
            Show(match.GetSnapshot("human"));
            if (options.InspectOnly)
            {
                ShowCatalogue(match.GetSnapshot("human"));
                return 0;
            }
            if (options.DemoDay) return DemoDay(match);
            Console.WriteLine("Developer two-duck controls. Normal AI arrives in C5.");
            Console.WriteLine("Enter human:1 or ai:1 to choose that duck's numbered action. Enter view:human / view:ai to inspect its private information; q quits.");
            var viewer = "human";
            while (match.GetSnapshot(viewer).Phase != DuckPhase.Finished)
            {
                var issued = new Dictionary<string, IReadOnlyList<GameAction>>();
                foreach (var id in new[] { "human", "ai" })
                {
                    issued[id] = match.GetLegalActions(id);
                    for (var i = 0; i < issued[id].Count; i++) Console.WriteLine($"{id}:{i + 1}. {issued[id][i].Label}");
                }
                if (issued.Values.All(actions => actions.Count == 0))
                {
                    Console.WriteLine("The C3 daily slice ends here. Full ten-Day continuation is the next checkpoint.");
                    return 0;
                }
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == null) { Console.WriteLine("Input closed. Quitting."); return 0; }
                if (input.Equals("q", StringComparison.OrdinalIgnoreCase)) return 0;
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
                Show(match.GetSnapshot(viewer));
            }
            return 0;
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
    }

    private static int DemoDay(MatchSession<DuckMatchView> match)
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
                Show(match.GetSnapshot("human"));
                acted = true;
                if (match.GetSnapshot("human").Day == 2) break;
            }
            if (!acted) throw new InvalidOperationException("The daily-cycle demo has no legal action; the current checkpoint is incomplete.");
        }
        throw new InvalidOperationException("The daily-cycle demonstration exceeded its action bound.");
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
