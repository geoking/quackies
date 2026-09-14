using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Ducks.Definitions;
using Quackies.Core.Ducks.Runtime;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class DuckNightResolutionTests
{
    private static readonly DuckRuleDefinitions Rules = DuckRules.V1;

    [Fact]
    public void Every_board_reward_is_used_for_safe_and_worn_ducks()
    {
        foreach (var space in Rules.BoardSpaces)
        {
            var safe = Calculate(State(1, "rain_softened_seeds",
                Player("safe", space.Space)), "safe");
            var worn = Calculate(State(1, "rain_softened_seeds",
                Player("worn", space.Space, worn: true)), "worn");

            Assert.Equal(space.Sleep, safe.PrintedSleep);
            Assert.Equal(space.Twigs, safe.PrintedTwigs);
            Assert.Equal(space.Sleep, safe.FrozenSleep);
            Assert.Equal(space.Twigs, safe.TotalTwigsEarned);
            Assert.Equal(space.IsHaven ? space.Feathers : 0, safe.FeathersAwarded);
            Assert.True(safe.IsMostRested);

            Assert.Equal(space.Sleep, worn.PrintedSleep);
            Assert.Equal(space.Twigs, worn.PrintedTwigs);
            Assert.Equal(space.Sleep / 2, worn.FrozenSleep);
            Assert.Equal(space.Twigs, worn.TotalTwigsEarned);
            Assert.Equal(0, worn.FeathersAwarded);
            Assert.False(worn.IsMostRested);
        }
    }

    [Fact]
    public void Resolve_keeps_banked_twigs_and_does_not_grant_immediate_reeds_or_pocket_twice()
    {
        const int earlierBank = 9;
        const int immediateReedsAndPocket = 4;
        var player = Player("duck", 4, finalDefinitionId: "brambles",
            bankedTwigs: earlierBank + immediateReedsAndPocket,
            reedsTwigs: 3, eventTwigs: 1);
        var state = State(1, "a_pocket_of_driftwood", player);

        var calculated = Calculate(state, player.Id);

        Assert.Equal(1, calculated.PrintedTwigs);
        Assert.Equal(3, calculated.ReedsTwigs);
        Assert.Equal(1, calculated.EventTwigs);
        Assert.Equal(1, calculated.BramblesPenalty);
        Assert.Equal(4, calculated.TotalTwigsEarned);

        DuckNightResolver.Resolve(state, Rules);

        Assert.Equal(earlierBank + immediateReedsAndPocket, player.TotalTwigs);
        Assert.Equal(4, player.LastNightOutcome!.TotalTwigsEarned);
    }

    [Fact]
    public void Final_pebbles_deducts_sleep_before_worn_out_halving()
    {
        var outcome = Calculate(State(1, "rain_softened_seeds",
            Player("duck", 4, finalDefinitionId: "loose_pebbles", worn: true)), "duck");

        Assert.Equal(6, outcome.PrintedSleep);
        Assert.Equal(1, outcome.PebblesPenalty);
        Assert.Equal(5, outcome.SleepBeforeWear);
        Assert.Equal(2, outcome.FrozenSleep);
    }

    [Theory]
    [InlineData("brambles")]
    [InlineData("loose_pebbles")]
    public void Protected_final_penalty_chip_does_not_reduce_rewards(string definitionId)
    {
        var outcome = Calculate(State(1, "rain_softened_seeds",
            Player("duck", 4, finalDefinitionId: definitionId, nuisanceSuppressed: true)), "duck");

        Assert.Equal(0, outcome.BramblesPenalty);
        Assert.Equal(0, outcome.PebblesPenalty);
        Assert.Equal(1, outcome.TotalTwigsEarned);
        Assert.Equal(6, outcome.FrozenSleep);
    }

    [Fact]
    public void Flowers_award_sleep_only_to_safe_ducks_at_havens()
    {
        var safeHaven = Player("safe-haven", 4, flowers: 2);
        var safeOrdinary = Player("safe-ordinary", 5, flowers: 2);
        var wornHaven = Player("worn-haven", 4, worn: true, flowers: 2);
        var outcomes = DuckNightResolver.Calculate(
            State(1, "rain_softened_seeds", safeHaven, safeOrdinary, wornHaven), Rules);

        Assert.Equal(4, outcomes[safeHaven.Id].FlowerSleep);
        Assert.Equal(10, outcomes[safeHaven.Id].FrozenSleep);
        Assert.Equal(0, outcomes[safeOrdinary.Id].FlowerSleep);
        Assert.Equal(5, outcomes[safeOrdinary.Id].FrozenSleep);
        Assert.Equal(0, outcomes[wornHaven.Id].FlowerSleep);
        Assert.Equal(3, outcomes[wornHaven.Id].FrozenSleep);
    }

    [Fact]
    public void Safe_flock_leaders_tie_and_a_larger_worn_flock_is_excluded()
    {
        var firstLeader = Player("first", 5, flock: 2);
        var secondLeader = Player("second", 6, flock: 2);
        var smallerSafeFlock = Player("smaller", 4, flock: 1);
        var wornLargerFlock = Player("worn", 10, worn: true, flock: 5);
        var outcomes = DuckNightResolver.Calculate(State(1, "rain_softened_seeds",
            firstLeader, secondLeader, smallerSafeFlock, wornLargerFlock), Rules);

        Assert.Equal(2, outcomes[firstLeader.Id].FlockSleep);
        Assert.Equal(2, outcomes[secondLeader.Id].FlockSleep);
        Assert.Equal(0, outcomes[smallerSafeFlock.Id].FlockSleep);
        Assert.Equal(0, outcomes[wornLargerFlock.Id].FlockSleep);
    }

    [Fact]
    public void Most_rested_uses_frozen_sleep_not_position_and_keeps_safe_ties()
    {
        var lowerComfortable = Player("lower", 4);
        var fartherLowerSleep = Player("farther", 5);
        var tiedAtAnotherPosition = Player("tie", 6);
        var outcomes = DuckNightResolver.Calculate(State(1, "rain_softened_seeds",
            lowerComfortable, fartherLowerSleep, tiedAtAnotherPosition), Rules);

        Assert.True(outcomes[lowerComfortable.Id].IsMostRested);
        Assert.False(outcomes[fartherLowerSleep.Id].IsMostRested);
        Assert.True(outcomes[tiedAtAnotherPosition.Id].IsMostRested);
        Assert.Equal(1, outcomes[lowerComfortable.Id].NextDayTemporaryStep);
        Assert.Equal(1, outcomes[tiedAtAnotherPosition.Id].NextDayTemporaryStep);
    }

    [Fact]
    public void All_worn_ducks_leave_most_rested_unawarded()
    {
        var first = Player("first", 4, worn: true);
        var second = Player("second", 10, worn: true);
        var outcomes = DuckNightResolver.Calculate(
            State(1, "rain_softened_seeds", first, second), Rules);

        Assert.All(outcomes.Values, outcome =>
        {
            Assert.False(outcome.IsMostRested);
            Assert.Equal(0, outcome.NextDayTemporaryStep);
        });
    }

    [Fact]
    public void All_tucked_in_requires_every_duck_safe_at_a_haven()
    {
        var pass = DuckNightResolver.Calculate(State(1, "all_tucked_in",
            Player("one", 4), Player("two", 10)), Rules);
        var fail = DuckNightResolver.Calculate(State(1, "all_tucked_in",
            Player("one", 4), Player("two", 11)), Rules);

        Assert.All(pass.Values, outcome => Assert.Equal(2, outcome.CollectiveEventSleep));
        Assert.All(fail.Values, outcome => Assert.Equal(0, outcome.CollectiveEventSleep));
    }

    [Fact]
    public void Home_before_dark_requires_every_duck_safe()
    {
        var pass = DuckNightResolver.Calculate(State(1, "home_before_dark",
            Player("one", 4), Player("two", 11)), Rules);
        var fail = DuckNightResolver.Calculate(State(1, "home_before_dark",
            Player("one", 4), Player("two", 11, worn: true)), Rules);

        Assert.All(pass.Values, outcome => Assert.Equal(1, outcome.CollectiveEventSleep));
        Assert.All(fail.Values, outcome => Assert.Equal(0, outcome.CollectiveEventSleep));
    }

    [Fact]
    public void Shared_supper_counts_a_worn_ducks_seed_and_adds_sleep_before_halving()
    {
        var safe = Player("safe", 5, seedPlaced: true);
        var worn = Player("worn", 4, worn: true, seedPlaced: true);
        var pass = DuckNightResolver.Calculate(State(1, "shared_supper", safe, worn), Rules);
        var fail = DuckNightResolver.Calculate(State(1, "shared_supper",
            Player("safe", 5, seedPlaced: true), Player("missing", 4, worn: true)), Rules);

        Assert.Equal(1, pass[safe.Id].CollectiveEventSleep);
        Assert.Equal(6, pass[safe.Id].FrozenSleep);
        Assert.Equal(1, pass[worn.Id].CollectiveEventSleep);
        Assert.Equal(3, pass[worn.Id].FrozenSleep);
        Assert.All(fail.Values, outcome => Assert.Equal(0, outcome.CollectiveEventSleep));
    }

    [Fact]
    public void Restless_night_reduces_only_safe_haven_subtotal_after_flowers_and_final_bonus()
    {
        var havenLeader = Player("haven", 4, flowers: 2, flock: 2);
        var ordinary = Player("ordinary", 5, flowers: 3, flock: 1);
        var wornHaven = Player("worn", 10, worn: true, flowers: 3, flock: 4);
        var outcomes = DuckNightResolver.Calculate(State(10, "restless_night",
            havenLeader, ordinary, wornHaven), Rules);

        var haven = outcomes[havenLeader.Id];
        Assert.Equal(4, haven.FlowerSleep);
        Assert.Equal(2, haven.FinalHavenSleep);
        Assert.Equal(1, haven.RestlessNightPenalty);
        Assert.Equal(2, haven.FlockSleep);
        Assert.Equal(13, haven.FrozenSleep);
        Assert.Equal(4, haven.DreamTwigs);
        Assert.Equal(0, haven.NextDayTemporaryStep);

        Assert.Equal(0, outcomes[ordinary.Id].FlowerSleep);
        Assert.Equal(0, outcomes[ordinary.Id].FinalHavenSleep);
        Assert.Equal(0, outcomes[ordinary.Id].RestlessNightPenalty);
        Assert.Equal(5, outcomes[ordinary.Id].FrozenSleep);

        Assert.Equal(0, outcomes[wornHaven.Id].FlowerSleep);
        Assert.Equal(0, outcomes[wornHaven.Id].FinalHavenSleep);
        Assert.Equal(0, outcomes[wornHaven.Id].RestlessNightPenalty);
        Assert.Equal(5, outcomes[wornHaven.Id].FrozenSleep);
    }

    [Fact]
    public void Final_day_endpoint_awards_two_feathers_and_exposes_complete_conversion_fields()
    {
        var endpoint = Player("endpoint", 43, bankedTwigs: 20);
        var worn = Player("worn", 42, finalDefinitionId: "loose_pebbles", worn: true,
            bankedTwigs: 30);
        var state = State(10, "rain_softened_seeds", endpoint, worn);

        var calculated = DuckNightResolver.Calculate(state, Rules);

        Assert.Equal(21, calculated[endpoint.Id].PrintedSleep);
        Assert.Equal(9, calculated[endpoint.Id].PrintedTwigs);
        Assert.Equal(2, calculated[endpoint.Id].FinalHavenSleep);
        Assert.Equal(23, calculated[endpoint.Id].FrozenSleep);
        Assert.Equal(2, calculated[endpoint.Id].FeathersAwarded);
        Assert.True(calculated[endpoint.Id].IsMostRested);
        Assert.Equal(6, calculated[endpoint.Id].DreamTwigs);
        Assert.Equal(0, calculated[endpoint.Id].NextDayTemporaryStep);

        Assert.Equal(13, calculated[worn.Id].PrintedSleep);
        Assert.Equal(1, calculated[worn.Id].PebblesPenalty);
        Assert.Equal(12, calculated[worn.Id].SleepBeforeWear);
        Assert.Equal(6, calculated[worn.Id].FrozenSleep);
        Assert.Equal(1, calculated[worn.Id].DreamTwigs);

        DuckNightResolver.Resolve(state, Rules);

        Assert.Equal(29, endpoint.TotalTwigs);
        Assert.Equal(2, endpoint.PermanentFeatherTrail);
        Assert.False(endpoint.PendingMostRestedStep);
        Assert.Equal(DuckPhase.Night, state.Phase);
    }

    [Fact]
    public void Calculate_and_resolve_reject_night_before_every_duck_has_finished_with_a_chip()
    {
        var finished = Player("finished", 4);
        var unfinished = Player("unfinished", 4);
        unfinished.HasFinishedDay = false;
        var state = State(1, "rain_softened_seeds", finished, unfinished);

        Assert.Throws<InvalidOperationException>(() => DuckNightResolver.Calculate(state, Rules));
        Assert.Throws<InvalidOperationException>(() => DuckNightResolver.Resolve(state, Rules));

        unfinished.HasFinishedDay = true;
        unfinished.PlacedChips.Clear();
        Assert.Throws<InvalidOperationException>(() => DuckNightResolver.Calculate(state, Rules));
    }

    [Fact]
    public void Resolve_is_single_application_and_duplicate_attempt_adds_no_awards()
    {
        var player = Player("duck", 43, bankedTwigs: 7);
        var state = State(1, "rain_softened_seeds", player);

        DuckNightResolver.Resolve(state, Rules);
        var totalTwigs = player.TotalTwigs;
        var trail = player.PermanentFeatherTrail;
        var awards = state.PublicAwards.ToArray();
        var history = state.History.ToArray();

        Assert.Throws<InvalidOperationException>(() => DuckNightResolver.Resolve(state, Rules));
        Assert.Equal(totalTwigs, player.TotalTwigs);
        Assert.Equal(trail, player.PermanentFeatherTrail);
        Assert.Equal(awards, state.PublicAwards);
        Assert.Equal(history, state.History);
    }

    private static DuckNightOutcome Calculate(DuckMatchState state, string playerId) =>
        DuckNightResolver.Calculate(state, Rules)[playerId];

    private static DuckMatchState State(int day, string eventDefinitionId, params DuckPlayerState[] players)
    {
        var state = new DuckMatchState(DuckMatchSettings.Standard)
        {
            Day = day,
            Phase = DuckPhase.Adventure,
            CurrentEventIndex = 0,
            NextPhysicalChipId = 100
        };
        state.WorldEventDeckDefinitionIds.Add(eventDefinitionId);
        state.Players.AddRange(players);
        return state;
    }

    private static DuckPlayerState Player(
        string id,
        int position,
        string finalDefinitionId = "splash",
        bool worn = false,
        bool nuisanceSuppressed = false,
        int bankedTwigs = 0,
        int reedsTwigs = 0,
        int eventTwigs = 0,
        int flowers = 0,
        int flock = 0,
        bool seedPlaced = false)
    {
        var player = new DuckPlayerState(id, id, 0)
        {
            Position = position,
            HasFinishedDay = true,
            IsWornOut = worn,
            TotalTwigs = bankedTwigs,
            DayReedsTwigs = reedsTwigs,
            DayEventTwigs = eventTwigs,
            FlowersPlaced = flowers,
            ActiveFlock = flock
        };

        var nextId = 1;
        if (seedPlaced && finalDefinitionId != "seeds")
        {
            player.Inventory.Add(new DuckPhysicalChipState(nextId, "seeds"));
            player.PlacedChips.Add(new DuckPlacedChipState(nextId, Math.Max(1, position - 1), false));
            player.PlacedHelpfulTypes.Add(DuckEncounterType.Seeds);
            nextId++;
        }

        player.Inventory.Add(new DuckPhysicalChipState(nextId, finalDefinitionId));
        player.PlacedChips.Add(new DuckPlacedChipState(nextId, position, nuisanceSuppressed));
        if (seedPlaced || finalDefinitionId == "seeds")
            player.PlacedHelpfulTypes.Add(DuckEncounterType.Seeds);
        return player;
    }
}
