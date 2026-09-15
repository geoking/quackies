using Quackies.Core.Ducks.Definitions;
using Quackies.Core.Ducks.Runtime;
using Quackies.Core.Match;

namespace Quackies.Cli;

internal static class DuckCliRenderer
{
    internal static void ShowStatus(DuckMatchView view, int recentHistory = 3)
    {
        Console.WriteLine($"Day {view.Day}/10 · {view.Phase} · Nest level {view.NestLevel} · World Event: {view.CurrentEvent.Name}");
        Console.WriteLine("  " + DuckReferenceText.Event(view.CurrentEvent.EventType));
        foreach (var player in view.Players)
        {
            var state = player.IsWornOut ? " · WORN OUT" : player.HasFinishedDay ? " · resting" : string.Empty;
            Console.WriteLine($"{player.Name}: {player.TotalTwigs} Twigs · space {player.Position} · Exhaustion {player.Exhaustion}/{player.SafeExhaustionMaximum} · bag {player.BagCount}{state}");
            Console.WriteLine($"  Feather trail {player.PermanentFeatherTrail} · today's start {player.EffectiveStart} · active flock {player.ActiveFlock}" +
                (player.ActiveMostRestedStep ? " · active zzz +1" : string.Empty) +
                (player.PendingMostRestedStep ? " · zzz for tomorrow" : string.Empty));
            var activeEffects = new List<string>();
            if (player.SplashProtectionArmed) activeEffects.Add("Splash protects the next placed chip");
            if (player.LogSlowdownPending) activeEffects.Add("Log will slow the next helpful chip");
            if (player.GuideProtectionAvailable) activeEffects.Add("Guide protects the first obstacle nuisance");
            if (activeEffects.Count > 0) Console.WriteLine("  Active: " + string.Join(" · ", activeEffects));
            if (view.Day > 1)
                Console.WriteLine($"  Dawn deficit {player.DawnTwigDeficit} · Dawn Feathers {player.DawnFeathersAwarded}");
            ShowPlaced(player);
        }

        var own = view.Players.Single(player => player.Id == view.ViewerId);
        if (own.Position > 0) ShowPrintedReward(DuckRules.V1.BoardSpaceAt(own.Position), "Your current printed reward");
        var nextHaven = DuckRules.V1.BoardSpaces.FirstOrDefault(space => space.IsHaven && space.Space > own.Position);
        Console.WriteLine(nextHaven == null
            ? "Nearest forthcoming haven: none; the route ends at space 43."
            : $"Nearest forthcoming haven: {nextHaven.Space} {nextHaven.HavenName} · printed {Reward(nextHaven)}");
        ShowPreview(view);
        foreach (var entry in view.History.TakeLast(recentHistory))
            Console.WriteLine($"  D{entry.Day} {(string.IsNullOrEmpty(entry.ActorId) ? "match" : entry.ActorId)}: {entry.Message}");
        ShowFinal(view);
        Console.WriteLine();
    }

    internal static void ShowBoard(DuckMatchView view, int? selectedSpace)
    {
        Console.WriteLine("Board rewards are printed values only; Core applies events, encounter bonuses, penalties and wear-out at Night.");
        var spaces = selectedSpace.HasValue
            ? DuckRules.V1.BoardSpaces.Where(space => space.Space == selectedSpace.Value)
            : DuckRules.V1.BoardSpaces;
        foreach (var space in spaces)
        {
            var markers = view.Players.Where(player => player.Position == space.Space).Select(player => player.Id == "human" ? "H" : "AI").ToArray();
            var marker = markers.Length == 0 ? "" : " [" + string.Join(",", markers) + "]";
            var haven = space.IsHaven ? " · HAVEN: " + space.HavenName : string.Empty;
            Console.WriteLine($"{space.Space,2}. {space.Biome,-9} · {Reward(space)}{haven}{marker}");
        }
        if (selectedSpace.HasValue && !spaces.Any())
            Console.WriteLine("Choose a board space from 1 to 43.");
        Console.WriteLine("Markers: H = human, AI = Normal opponent. Rewards pay only where the duck finally rests.");
        Console.WriteLine();
    }

    internal static void ShowBag(DuckMatchView view)
    {
        Console.WriteLine($"{Player(view).Name}'s private bag information");
        ShowComposition("Remaining bag", view.OwnBag);
        ShowComposition("Owned inventory", view.OwnInventory);
        Console.WriteLine("Remaining entries are counts sorted by name, never shuffled future order.");
        ShowPreview(view);
        Console.WriteLine();
    }

    internal static void ShowTokens()
    {
        Console.WriteLine("All 16 encounter variants");
        foreach (var encounter in DuckRules.V1.EncounterDefinitions)
            Console.WriteLine($"{encounter.DefinitionId}: {encounter.Name} · {DuckReferenceText.Encounter(encounter)}");
        Console.WriteLine();
    }

    internal static void ShowShop(DuckMatchView view, IReadOnlyList<GameAction> legalActions)
    {
        var player = Player(view);
        var available = legalActions.Where(action => action.Kind == GameActionKind.BuyEncounter)
            .Select(action => action.DefinitionId).ToHashSet(StringComparer.Ordinal);
        var slots = Math.Max(0, player.PurchaseLimit - player.PurchasedEncounterDefinitionIds.Count);
        Console.WriteLine($"Dream shop · {player.RemainingSleep} Sleep remaining · {slots}/{player.PurchaseLimit} purchase slots remaining");
        foreach (var offer in view.ShopOffers)
        {
            var shopClosed = view.Phase != DuckPhase.Night || player.HasFinishedDream;
            var status = available.Contains(offer.DefinitionId) ? "AVAILABLE NOW"
                : shopClosed ? "unavailable in the current phase"
                : player.PurchasedShopTypes.Contains(offer.ShopType) ? "already bought this type tonight"
                : slots == 0 ? "no purchase slots remaining"
                : offer.SleepPrice > player.RemainingSleep ? "not enough Sleep"
                : "not currently offered as a legal action";
            Console.WriteLine($"{offer.DefinitionId}: {offer.SleepPrice} Sleep · {offer.Encounter.Name} · {status}");
            Console.WriteLine("  " + DuckReferenceText.Encounter(offer.Encounter));
        }
        Console.WriteLine("Prices above come from this running match; variants of Tailwind or Reeds share a one-per-type limit.");
        Console.WriteLine();
    }

    internal static void ShowEvent(DuckMatchView view)
    {
        Console.WriteLine($"World Event · Day {view.Day}: {view.CurrentEvent.Name}");
        Console.WriteLine(DuckReferenceText.Event(view.CurrentEvent.EventType));
        Console.WriteLine("It applies to both ducks for this Day; collective conditions resolve after both finish.");
        Console.WriteLine();
    }

    internal static void ShowNight(DuckMatchView view)
    {
        var nights = view.Players.Where(player => player.LastNightOutcome != null).ToArray();
        if (nights.Length == 0)
        {
            Console.WriteLine("No Night has resolved yet.\n");
            return;
        }
        Console.WriteLine($"Night {nights[0].LastNightOutcome!.Day}:");
        foreach (var player in nights)
        {
            var night = player.LastNightOutcome!;
            Console.WriteLine($"{player.Name}: {night.FrozenSleep} frozen Sleep; {player.RemainingSleep} available now. Twigs earned {night.TotalTwigsEarned}; Feathers {night.FeathersAwarded}" +
                (night.IsMostRested ? " · Most Rested" : string.Empty));
            Console.WriteLine($"  Sleep: printed {night.PrintedSleep}, Flowers +{night.FlowerSleep}, final haven +{night.FinalHavenSleep}, event +{night.CollectiveEventSleep}, flock +{night.FlockSleep}, Restless Night -{night.RestlessNightPenalty}, Pebbles -{night.PebblesPenalty}; before wear {night.SleepBeforeWear}.");
            Console.WriteLine($"  Twigs: printed {night.PrintedTwigs}, Reeds +{night.ReedsTwigs}, event +{night.EventTwigs}, Brambles -{night.BramblesPenalty}.");
            if (night.DreamTwigs > 0) Console.WriteLine($"  Dream Twigs: {night.DreamTwigs}.");
        }
        Console.WriteLine();
    }

    internal static void ShowNewNight(DuckMatchView before, DuckMatchView after)
    {
        var previousDay = before.Players.Single(player => player.Id == "human").LastNightOutcome?.Day;
        var currentDay = after.Players.Single(player => player.Id == "human").LastNightOutcome?.Day;
        if (currentDay.HasValue && currentDay != previousDay) ShowNight(after);
    }

    internal static void ShowHistory(DuckMatchView view)
    {
        Console.WriteLine("Public match history");
        if (view.History.Count == 0) Console.WriteLine("  No completed actions yet.");
        foreach (var entry in view.History)
            Console.WriteLine($"  D{entry.Day} {(string.IsNullOrEmpty(entry.ActorId) ? "match" : entry.ActorId)}: {entry.Message}");
        Console.WriteLine();
    }

    internal static void ShowHelp(bool twoPlayer)
    {
        Console.WriteLine("Commands: action number, help, status, board [1-43], bag, tokens, shop, event, night, history, r/restart, q/quit");
        Console.WriteLine(twoPlayer
            ? "Developer controls: human:N or ai:N executes that duck's issued action; view:human and view:ai select its private observation."
            : "view:human reviews your observation. The AI's private view is unavailable.");
        Console.WriteLine("Most Twigs wins. Five Exhaustion is normally safe; use tokens, event and board before deciding to draw again.");
        Console.WriteLine("Informational commands and invalid input do not advance either duck or write a save.\n");
    }

    internal static void ShowCatalogue(DuckMatchView view)
    {
        Console.WriteLine($"Catalogue: {DuckRules.V1.BoardSpaces.Count} rewards · {DuckRules.V1.BoardSpaces.Count(space => space.IsHaven)} havens · {DuckRules.V1.EncounterDefinitions.Count} encounter variants · {view.ShopOffers.Count} shop offers · {DuckRules.V1.WorldEvents.Count} World Events");
        Console.WriteLine("Own inventory: " + string.Join(", ", view.OwnInventory.GroupBy(chip => chip.DefinitionId).Select(group => $"{group.Key} ×{group.Count()}")));
        foreach (var offer in view.ShopOffers)
            Console.WriteLine($"  {offer.DefinitionId}: {offer.SleepPrice} Sleep · movement {(offer.Encounter.BaseMovement?.ToString() ?? "flock-dependent")} · Twig yield {offer.Encounter.TwigYield}");
    }

    private static DuckPlayerView Player(DuckMatchView view) => view.Players.Single(player => player.Id == view.ViewerId);

    private static void ShowPlaced(DuckPlayerView player)
    {
        if (player.PlacedChips.Count == 0) return;
        var placed = player.PlacedChips.Select(chip =>
        {
            var name = DuckRules.V1.Encounter(chip.DefinitionId).Name;
            return $"{name}@{chip.Position}" + (chip.NuisanceSuppressed ? "(protected)" : string.Empty);
        });
        Console.WriteLine("  Placed route: " + string.Join(" → ", placed));
    }

    private static void ShowComposition(string title, IReadOnlyList<DuckPhysicalChipView> chips)
    {
        var groups = chips.GroupBy(chip => chip.DefinitionId)
            .Select(group => (Name: DuckRules.V1.Encounter(group.Key).Name, Count: group.Count()))
            .OrderBy(group => group.Name, StringComparer.Ordinal);
        Console.WriteLine($"{title} ({chips.Count}): " + string.Join(", ", groups.Select(group => $"{group.Name} ×{group.Count}")));
    }

    private static void ShowPreview(DuckMatchView view)
    {
        if (view.KnownNextChips.Count == 0) return;
        Console.WriteLine($"Private Signpost preview for {view.ViewerId}, in order: " + string.Join(", ",
            view.KnownNextChips.Select(chip => DuckRules.V1.Encounter(chip.DefinitionId).Name)));
    }

    private static string Reward(DuckBoardSpace space) =>
        $"{space.Sleep} Sleep · {space.Twigs} Twig{(space.Twigs == 1 ? "" : "s")}" +
        (space.Feathers > 0 ? $" · {space.Feathers} Feather{(space.Feathers == 1 ? "" : "s")}" : string.Empty);

    private static void ShowPrintedReward(DuckBoardSpace space, string title) =>
        Console.WriteLine($"{title}: space {space.Space} {space.HavenName ?? space.Biome.ToString()} · {Reward(space)} (bonuses and wear-out excluded)");

    private static void ShowFinal(DuckMatchView view)
    {
        if (view.FinalResult is not { } result) return;
        Console.WriteLine("Final standings · total Twigs, then Night 10 retained Sleep:");
        foreach (var standing in result.Standings)
            Console.WriteLine($"  {standing.Rank}. {standing.PlayerName}: {standing.TotalTwigs} Twigs (including {standing.DreamTwigs} Dream Twigs), {standing.FrozenNightTenSleep} Sleep");
        Console.WriteLine(result.WinnerIds.Count == 1
            ? "Winner: " + result.Standings.Single(standing => standing.IsWinner).PlayerName
            : "Draw: " + string.Join(", ", result.Standings.Where(standing => standing.IsWinner).Select(standing => standing.PlayerName)));
    }
}
