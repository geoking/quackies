using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Quackies.Core.Rules.Fortunes;
using Quackies.Core.Rules.Ingredients;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class DieRollObservationTests
{
    [Theory]
    [InlineData(0, "1 victory point")]
    [InlineData(1, "1 victory point")]
    [InlineData(2, "2 victory points")]
    [InlineData(3, "1 ruby")]
    [InlineData(4, "Orange 1 chip")]
    [InlineData(5, "Advance droplet 1 space")]
    public void FortuneDieHistoryRecordsEveryPhysicalFace(int face, string description)
    {
        var match = CreateRollTheDieMatch(new FixedRandom(0, face, face));
        var rolls = match.GetSnapshot("human").DieRolls;

        Assert.Equal(2, rolls.Count);
        Assert.Collection(rolls,
            roll => AssertRoll(roll, 1, "human", face, DieRollReason.Fortune, true, description),
            roll => AssertRoll(roll, 2, "ai", face, DieRollReason.Fortune, true, description));
    }

    [Fact]
    public void TiedNonExplodedLeadersEachReceiveAnObservedRoundBonusRoll()
    {
        var match = MatchSession.Create(new FixedRandom(2, 3), RuleSet.SetOne(Array.Empty<IRoundEventRule>()));

        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);

        var rolls = match.GetSnapshot("human").DieRolls;
        Assert.Collection(rolls,
            roll => AssertRoll(roll, 1, "human", 2, DieRollReason.RoundBonus, true, "2 victory points"),
            roll => AssertRoll(roll, 2, "ai", 3, DieRollReason.RoundBonus, true, "1 ruby"));
    }

    [Fact]
    public void ExplodedPlayerIsExcludedFromRoundBonusDieHistory()
    {
        var match = MatchSession.Create(new FixedRandom(0, 0, 0, 0, 0, 0, 2), RuleSet.SetOne(Array.Empty<IRoundEventRule>()));
        ExecuteKind(match, "ai", GameActionKind.Stop);
        for (var draw = 0; draw < 6; draw++) ExecuteKind(match, "human", GameActionKind.Draw);

        var roll = Assert.Single(match.GetSnapshot("human").DieRolls);
        AssertRoll(roll, 1, "ai", 2, DieRollReason.RoundBonus, true, "2 victory points");
    }

    [Fact]
    public void DieHistoryIsDetachedReadOnlyAndIncludesLaterRollsOnlyInLaterSnapshots()
    {
        var match = CreateRollTheDieMatch(new ZeroRandom());
        var beforeEvaluation = match.GetSnapshot("human").DieRolls;

        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);

        var afterEvaluation = match.GetSnapshot("human").DieRolls;
        Assert.Equal(2, beforeEvaluation.Count);
        Assert.Equal(4, afterEvaluation.Count);
        Assert.Throws<NotSupportedException>(() => ((IList<DieRollView>)afterEvaluation).Add(afterEvaluation[0]));
    }

    [Fact]
    public void OrangeFaceRecordsUnappliedWhenTheSupplyIsEmpty()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var card = SetOneFortunes.CreateOngoingBatch().Single(candidate => candidate.Id == "roll-the-die");
        var rules = new RuleSet(baseline.Track, SetOneIngredients.Create(), Array.Empty<ShopChipDefinition>(), new[] { card });
        var match = MatchSession.Create(new FixedRandom(0, 4, 4), rules);

        Assert.All(match.GetSnapshot("human").DieRolls, roll => Assert.False(roll.RewardApplied));
    }

    [Fact]
    public void DropletFaceRecordsUnappliedAtTheEndOfTheTrack()
    {
        var rollTheDie = SetOneFortunes.CreateOngoingBatch().Single(candidate => candidate.Id == "roll-the-die");
        var events = new IRoundEventRule[] { new AdvanceDropletsToEnd(), rollTheDie };
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips, events);
        var match = MatchSession.Create(new FixedRandom(0, 0, 0, 0, 5, 5), rules);
        FinishRoundAndAdvance(match);

        var fortuneRolls = match.GetSnapshot("human").DieRolls.Where(roll => roll.Reason == DieRollReason.Fortune).ToArray();
        Assert.Equal(2, fortuneRolls.Length);
        Assert.All(fortuneRolls, roll =>
        {
            Assert.Equal(5, roll.Face);
            Assert.False(roll.RewardApplied);
        });
    }

    private static MatchSession CreateRollTheDieMatch(IRandomSource random)
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var card = SetOneFortunes.CreateOngoingBatch().Single(candidate => candidate.Id == "roll-the-die");
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips, new[] { card });
        return MatchSession.Create(random, rules);
    }

    private static void AssertRoll(DieRollView roll, int sequence, string playerId, int face, DieRollReason reason,
        bool applied, string description)
    {
        Assert.Equal(sequence, roll.Sequence);
        Assert.Equal(1, roll.Round);
        Assert.Equal(playerId, roll.PlayerId);
        Assert.Equal(face, roll.Face);
        Assert.Equal(reason, roll.Reason);
        Assert.Equal(applied, roll.RewardApplied);
        Assert.Equal(description, roll.Description);
    }

    private static void FinishRoundAndAdvance(MatchSession match)
    {
        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);
        while (match.Phase != MatchPhase.RoundComplete)
        {
            var action = new[] { "human", "ai" }
                .SelectMany(playerId => match.GetLegalActions(playerId).Select(candidate => (playerId, candidate)))
                .First(item => item.candidate.Kind == GameActionKind.FinishShopping ||
                    item.candidate.Kind == GameActionKind.FinishRubySpending);
            match.Execute(action.playerId, action.candidate);
        }
        ExecuteKind(match, "human", GameActionKind.NextRound);
    }

    private static void ExecuteKind(MatchSession match, string playerId, GameActionKind kind)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Kind == kind);
        match.Execute(playerId, action);
    }

    private sealed class AdvanceDropletsToEnd : RoundEventRule
    {
        internal AdvanceDropletsToEnd() : base("advance-to-end", "Advance to end", "Test setup.") { }
        public override void OnRevealed(RoundEventContext context)
        {
            foreach (var playerId in context.PlayerIds) context.AdvanceDroplet(playerId, 100);
        }
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
