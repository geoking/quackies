using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Ducks.AI;
using Quackies.Core.Ducks.Definitions;
using Quackies.Core.Ducks.Runtime;
using Quackies.Core.Match;
using Xunit;
using Xunit.Abstractions;

namespace Quackies.Core.Tests;

public sealed class DuckNormalPolicyTests
{
    private readonly DuckNormalPolicy _policy = new();
    private readonly ITestOutputHelper _output;

    public DuckNormalPolicyTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Exact_preview_changes_the_decision_but_unknown_bag_order_does_not()
    {
        var seedPreview = AdventureScenario(position: 3, exhaustion: 4, bag: new[] { "seeds", "grumpy_goose" },
            knownDefinitionId: "seeds");
        var goosePreview = AdventureScenario(position: 3, exhaustion: 4, bag: new[] { "seeds", "grumpy_goose" },
            knownDefinitionId: "grumpy_goose");

        Assert.Equal(GameActionKind.Explore, Choose(seedPreview).Kind);
        var gooseDecision = Evaluate(goosePreview);
        Assert.Equal(GameActionKind.Settle, gooseDecision.Action.Kind);
        Assert.Contains("Signpost preview", gooseDecision.Reason, StringComparison.Ordinal);

        var firstUnknown = AdventureScenario(position: 3, exhaustion: 4,
            bag: new[] { "seeds", "grumpy_goose" });
        var reversedUnknown = AdventureScenario(position: 3, exhaustion: 4,
            bag: new[] { "seeds", "grumpy_goose" });
        reversedUnknown.Runtime.Player("human").BagPhysicalChipIds.Reverse();

        Assert.Equal(
            Evaluate(firstUnknown).Action.Id,
            Evaluate(reversedUnknown).Action.Id);
        Assert.Equal(
            firstUnknown.Match.GetSnapshot("human").OwnBag.Select(chip => chip.DefinitionId),
            reversedUnknown.Match.GetSnapshot("human").OwnBag.Select(chip => chip.DefinitionId));
    }

    [Fact]
    public void Protected_Goose_still_wears_out_when_its_Exhaustion_exceeds_five()
    {
        var scenario = AdventureScenario(position: 3, exhaustion: 5,
            bag: new[] { "grumpy_goose" }, knownDefinitionId: "grumpy_goose");
        scenario.Runtime.Player("human").SplashProtectionArmed = true;

        var decision = Evaluate(scenario);

        Assert.Equal(GameActionKind.Settle, decision.Action.Kind);
        Assert.Contains("Exhaustion 5", decision.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public void Policy_keeps_a_safe_haven_but_draws_from_the_neighbouring_ordinary_space_into_it()
    {
        var atHaven = AdventureScenario(position: 4, exhaustion: 0,
            bag: new[] { "seeds" }, knownDefinitionId: "seeds");
        var beforeHaven = AdventureScenario(position: 3, exhaustion: 0,
            bag: new[] { "seeds" }, knownDefinitionId: "seeds");

        Assert.Equal(GameActionKind.Settle, Choose(atHaven).Kind);
        var explore = Choose(beforeHaven);
        Assert.Equal(GameActionKind.Explore, explore.Kind);
        beforeHaven.Match.Execute("human", explore);
        Assert.Equal(4, beforeHaven.Runtime.Player("human").Position);
    }

    [Fact]
    public void Early_safe_plateau_does_not_trap_the_policy_after_one_placement()
    {
        var plateau = AdventureScenario(position: 1, exhaustion: 0,
            bag: new[] { "seeds" }, knownDefinitionId: "seeds");

        var decision = Evaluate(plateau);

        Assert.Equal(GameActionKind.Explore, decision.Action.Kind);
        Assert.Contains("improves", decision.Reason, StringComparison.Ordinal);
        plateau.Match.Execute("human", decision.Action);
        Assert.Equal(2, plateau.Runtime.Player("human").Position);
    }

    [Fact]
    public void Finished_opponents_public_rest_can_change_the_Most_Rested_decision()
    {
        var defendLead = AdventureScenario(position: 3, exhaustion: 4,
            bag: new[] { "seeds", "grumpy_goose" });
        var cannotLead = AdventureScenario(position: 3, exhaustion: 4,
            bag: new[] { "seeds", "grumpy_goose" });
        SetFinishedOpponent(defendLead.Runtime, position: 1);
        SetFinishedOpponent(cannotLead.Runtime, position: 9);

        var defendDecision = Evaluate(defendLead);
        var chaseDecision = Evaluate(cannotLead);

        Assert.Equal(GameActionKind.Settle, defendDecision.Action.Kind);
        Assert.Equal(GameActionKind.Explore, chaseDecision.Action.Kind);
        Assert.Contains("Keeping", defendDecision.Reason, StringComparison.Ordinal);
        Assert.Contains("improves", chaseDecision.Reason, StringComparison.Ordinal);
    }

    [Fact]
    public void Early_Dream_prefers_movement_while_late_Dream_prefers_direct_Reeds_Twigs()
    {
        var early = DreamScenario(day: 2, sleep: 20);
        var late = DreamScenario(day: 8, sleep: 20);

        var earlyActions = PurchaseComparisonActions(early.Match, "human");
        var lateActions = PurchaseComparisonActions(late.Match, "human");

        Assert.Equal("tailwind_4", _policy.Choose(early.Match.GetSnapshot("human"), earlyActions).DefinitionId);
        Assert.Equal("reeds_1", _policy.Choose(late.Match.GetSnapshot("human"), lateActions).DefinitionId);
    }

    [Fact]
    public void Purchase_choice_respects_issued_legal_types_and_available_budget()
    {
        var scenario = DreamScenario(day: 4, sleep: 7);
        var view = scenario.Match.GetSnapshot("human");
        var issued = scenario.Match.GetLegalActions("human");

        var decision = _policy.Evaluate(view, issued);

        Assert.Contains(issued, action => ReferenceEquals(action, decision.Action));
        Assert.Equal(GameActionKind.BuyEncounter, decision.Action.Kind);
        Assert.True(decision.Action.Cost <= view.Players.Single(player => player.Id == "human").RemainingSleep);
        Assert.Contains(decision.Action.DefinitionId,
            new[] { "seeds", "tailwind_2", "signpost", "splash", "reeds_1", "companion", "wildflowers" });
        scenario.Match.Execute("human", decision.Action);
        Assert.DoesNotContain(scenario.Match.GetLegalActions("human"), action =>
            action.Kind == GameActionKind.BuyEncounter
            && DuckRules.V1.ShopOffer(action.DefinitionId).ShopType ==
                DuckRules.V1.ShopOffer(decision.Action.DefinitionId).ShopType);
    }

    [Fact]
    public void Pending_Day_ten_commit_does_not_change_the_other_ducks_policy_input_decision()
    {
        var runtime = DuckMatchRuntime.Create(seed: 313);
        runtime.State.Day = 10;
        runtime.State.FinalDayDecisionBeat = 1;
        PrepareAdventurePlayer(runtime, "human", position: 3, exhaustion: 0, new[] { "seeds", "fallen_log" });
        PrepareAdventurePlayer(runtime, "ai", position: 3, exhaustion: 0, new[] { "seeds", "fallen_log" });
        var match = new MatchSession<DuckMatchView>(runtime);
        var beforeView = match.GetSnapshot("ai");
        var before = _policy.Choose(beforeView, match.GetLegalActions("ai"));

        var humanCommit = match.GetLegalActions("human").Single(action => action.Kind == GameActionKind.Explore);
        match.Execute("human", humanCommit);
        var pendingView = match.GetSnapshot("ai");
        var pending = _policy.Choose(pendingView, match.GetLegalActions("ai"));

        Assert.False(beforeView.AwaitingFinalDayDecisions);
        Assert.True(pendingView.AwaitingFinalDayDecisions);
        Assert.Equal(before.Id, pending.Id);
        Assert.Equal(before.Kind, pending.Kind);
        Assert.Equal(
            beforeView.Players.Select(PublicPlayerTuple),
            pendingView.Players.Select(PublicPlayerTuple));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(42)]
    [InlineData(941)]
    public void Normal_policy_completes_seeded_games_with_real_stops_and_portfolio_choices(int seed)
    {
        var first = PlayNormalGame(seed);
        var repeat = PlayNormalGame(seed);

        Assert.Equal(DuckPhase.Finished, first.View.Phase);
        Assert.NotNull(first.View.FinalResult);
        Assert.InRange(first.ActionCount, 1, 3999);
        Assert.True(first.ActionKinds.GetValueOrDefault(GameActionKind.Explore) > 0);
        Assert.True(first.ActionKinds.GetValueOrDefault(GameActionKind.Settle) > 0);
        Assert.True(first.ActionKinds.GetValueOrDefault(GameActionKind.BuyEncounter) > 0);
        Assert.True(first.PurchaseDefinitionIds.Distinct(StringComparer.Ordinal).Count() >= 2);
        Assert.Equal(first.ActionKinds.OrderBy(pair => pair.Key), repeat.ActionKinds.OrderBy(pair => pair.Key));
        Assert.Equal(first.PurchaseDefinitionIds, repeat.PurchaseDefinitionIds);
        Assert.Equal(
            first.View.FinalResult!.Standings.Select(FinalTuple),
            repeat.View.FinalResult!.Standings.Select(FinalTuple));
        _output.WriteLine(
            "seed {0}: {1} actions, Explore {2}, Settle {3}, purchases {4} ({5} types), winners {6}",
            seed,
            first.ActionCount,
            first.ActionKinds.GetValueOrDefault(GameActionKind.Explore),
            first.ActionKinds.GetValueOrDefault(GameActionKind.Settle),
            first.ActionKinds.GetValueOrDefault(GameActionKind.BuyEncounter),
            first.PurchaseDefinitionIds.Distinct(StringComparer.Ordinal).Count(),
            string.Join(" & ", first.View.FinalResult.WinnerIds));
    }

    private GameAction Choose(AdventureTestScenario scenario) =>
        _policy.Choose(scenario.Match.GetSnapshot("human"), scenario.Match.GetLegalActions("human"));

    private DuckPolicyDecision Evaluate(AdventureTestScenario scenario) =>
        _policy.Evaluate(scenario.Match.GetSnapshot("human"), scenario.Match.GetLegalActions("human"));

    private static AdventureTestScenario AdventureScenario(
        int position,
        int exhaustion,
        string[] bag,
        string? knownDefinitionId = null)
    {
        var runtime = DuckMatchRuntime.Create(seed: 211);
        runtime.State.WorldEventDeckDefinitionIds[runtime.State.CurrentEventIndex] = "home_before_dark";
        runtime.Player("human").GuideProtectionAvailable = false;
        PrepareAdventurePlayer(runtime, "human", position, exhaustion, bag);
        var player = runtime.Player("human");
        if (knownDefinitionId != null)
        {
            var known = player.BagPhysicalChipIds.Single(id =>
                player.Inventory.Single(chip => chip.PhysicalChipId == id).DefinitionId == knownDefinitionId);
            player.BagPhysicalChipIds.Remove(known);
            player.BagPhysicalChipIds.Insert(0, known);
            player.KnownNextPhysicalChipIds.Add(known);
        }
        return new AdventureTestScenario(runtime, new MatchSession<DuckMatchView>(runtime));
    }

    private static void PrepareAdventurePlayer(
        DuckMatchRuntime runtime,
        string playerId,
        int position,
        int exhaustion,
        string[] bag)
    {
        var player = runtime.Player(playerId);
        player.Inventory.Clear();
        player.BagPhysicalChipIds.Clear();
        player.KnownNextPhysicalChipIds.Clear();
        player.PlacedChips.Clear();
        player.PlacedHelpfulTypes.Clear();
        player.Position = position;
        player.Exhaustion = exhaustion;
        player.SafeExhaustionMaximum = 5;
        player.HasFinishedDay = false;
        player.IsWornOut = false;
        var placedId = runtime.State.NextPhysicalChipId++;
        player.Inventory.Add(new DuckPhysicalChipState(placedId, "seeds"));
        player.PlacedChips.Add(new DuckPlacedChipState(placedId, position, nuisanceSuppressed: false));
        player.PlacedHelpfulTypes.Add(DuckEncounterType.Seeds);
        foreach (var definitionId in bag)
        {
            var physicalId = runtime.State.NextPhysicalChipId++;
            player.Inventory.Add(new DuckPhysicalChipState(physicalId, definitionId));
            player.BagPhysicalChipIds.Add(physicalId);
        }
    }

    private static void SetFinishedOpponent(DuckMatchRuntime runtime, int position)
    {
        var opponent = runtime.Player("ai");
        opponent.Position = position;
        opponent.HasFinishedDay = true;
        opponent.IsWornOut = false;
        opponent.PlacedChips.Clear();
    }

    private static DreamTestScenario DreamScenario(int day, int sleep)
    {
        var runtime = DuckMatchRuntime.Create(seed: 227);
        runtime.State.Day = day;
        runtime.State.Phase = DuckPhase.Night;
        foreach (var player in runtime.State.Players)
        {
            player.FrozenSleep = sleep;
            player.RemainingSleep = sleep;
            player.IsSleepFrozen = true;
            player.HasFinishedDream = false;
        }
        return new DreamTestScenario(runtime, new MatchSession<DuckMatchView>(runtime));
    }

    private static IReadOnlyList<GameAction> PurchaseComparisonActions(
        MatchSession<DuckMatchView> match,
        string playerId)
    {
        return match.GetLegalActions(playerId).Where(action =>
            action.Kind == GameActionKind.FinishDream
            || action.DefinitionId == "tailwind_4"
            || action.DefinitionId == "reeds_1").ToArray();
    }

    private NormalGameResult PlayNormalGame(int seed)
    {
        var match = MatchSession.CreateDuck(seed);
        var kinds = new Dictionary<GameActionKind, int>();
        var purchases = new List<string>();
        for (var step = 0; step < 4000; step++)
        {
            var view = match.GetSnapshot("human");
            if (view.Phase == DuckPhase.Finished)
                return new NormalGameResult(view, kinds.Values.Sum(), kinds, purchases);
            var acted = false;
            foreach (var playerId in new[] { "human", "ai" })
            {
                var actions = match.GetLegalActions(playerId);
                if (actions.Count == 0) continue;
                var action = _policy.Choose(match.GetSnapshot(playerId), actions);
                Assert.Contains(actions, issued => ReferenceEquals(issued, action));
                kinds[action.Kind] = kinds.GetValueOrDefault(action.Kind) + 1;
                if (action.Kind == GameActionKind.BuyEncounter) purchases.Add(action.DefinitionId);
                match.Execute(playerId, action);
                acted = true;
            }
            if (!acted) throw new InvalidOperationException("Policy game stalled before final scoring.");
        }
        throw new InvalidOperationException("Policy game exceeded its action bound.");
    }

    private static (string Id, int Position, int Exhaustion, int BagCount, bool Finished, bool Worn)
        PublicPlayerTuple(DuckPlayerView player) =>
        (player.Id, player.Position, player.Exhaustion, player.BagCount, player.HasFinishedDay, player.IsWornOut);

    private static (string PlayerId, int Rank, int Twigs, int Sleep, int DreamTwigs, bool Winner)
        FinalTuple(DuckFinalStanding standing) =>
        (standing.PlayerId, standing.Rank, standing.TotalTwigs, standing.FrozenNightTenSleep,
            standing.DreamTwigs, standing.IsWinner);

    private sealed record AdventureTestScenario(DuckMatchRuntime Runtime, MatchSession<DuckMatchView> Match);
    private sealed record DreamTestScenario(DuckMatchRuntime Runtime, MatchSession<DuckMatchView> Match);
    private sealed record NormalGameResult(
        DuckMatchView View,
        int ActionCount,
        IReadOnlyDictionary<GameActionKind, int> ActionKinds,
        IReadOnlyList<string> PurchaseDefinitionIds);
}
