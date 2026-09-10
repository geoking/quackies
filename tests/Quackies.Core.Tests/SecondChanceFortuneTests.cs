using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Quackies.Core.Rules.Fortunes;
using Quackies.Core.Tokens;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class SecondChanceFortuneTests
{
    [Fact]
    public void FirstFivePlacementsAreProtectedAndTheNextOrdinaryDrawCanExplode()
    {
        var match = CreateMatch(new FixedRandom(0, 6, 4, 4, 0, 0, 0));
        for (var draw = 0; draw < 5; draw++) ExecuteKind(match, "human", GameActionKind.Draw);

        var afterFive = Player(match, "human");
        Assert.Equal(9, afterFive.WhiteTotal);
        Assert.False(afterFive.Exploded);
        Assert.Contains(match.GetLegalActions("human"), action =>
            action.Id.EndsWith(":keep-pot", StringComparison.Ordinal));
        ExecuteSuffix(match, "human", ":keep-pot");
        Assert.False(Player(match, "human").Exploded);

        ExecuteKind(match, "human", GameActionKind.Draw);

        Assert.True(Player(match, "human").Exploded);
        Assert.Equal(10, Player(match, "human").WhiteTotal);
    }

    [Fact]
    public void RestartRestoresOpeningPotBagAndFlaskAndDoesNotOfferAgain()
    {
        var match = CreateMatch(new ZeroRandom());
        ExecuteKind(match, "human", GameActionKind.Draw);
        ExecuteKind(match, "human", GameActionKind.UseFlask);
        for (var draw = 0; draw < 4; draw++) ExecuteKind(match, "human", GameActionKind.Draw);

        Assert.False(Player(match, "human").FlaskFull);
        ExecuteSuffix(match, "human", ":restart-brewing");

        var restarted = Player(match, "human");
        Assert.Empty(restarted.PlacedChips);
        Assert.Equal(9, restarted.BagCount);
        Assert.Equal(0, restarted.WhiteTotal);
        Assert.True(restarted.FlaskFull);
        Assert.False(restarted.Stopped);
        Assert.False(restarted.Exploded);

        for (var draw = 0; draw < 5; draw++) ExecuteKind(match, "human", GameActionKind.Draw);

        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.Kind == GameActionKind.Choose);
        Assert.Equal(5, Player(match, "human").PlacedChips.Count);
    }

    [Fact]
    public void FifthBlueResolvesItsPreviewBeforeSecondChanceIsOffered()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var secondChance = Card();
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips,
            new IRoundEventRule[] { new GiveHumanBlue(), secondChance });
        var random = new FixedRandom(0, 0, 0, 0, 0, 0, 0, 0, 5, 0);
        var match = MatchSession.Create(random, rules);
        FinishRoundAndAdvance(match);
        for (var draw = 0; draw < 5; draw++) ExecuteKind(match, "human", GameActionKind.Draw);

        Assert.All(match.GetLegalActions("human"), action => Assert.Equal("Crow skull: choose up to one chip", action.ChoiceTitle));
        ExecuteSuffix(match, "human", ":none");

        Assert.All(match.GetLegalActions("human"), action => Assert.Equal("A Second Chance", action.ChoiceTitle));
        Assert.Contains(match.GetLegalActions("human"), action =>
            action.Id.EndsWith(":restart-brewing", StringComparison.Ordinal));
    }

    [Fact]
    public void RestartDoesNotRestoreTheRandomStreamToTheSameDrawSequence()
    {
        var random = new RecordingRandom();
        var match = CreateMatch(random);
        for (var draw = 0; draw < 5; draw++) ExecuteKind(match, "human", GameActionKind.Draw);
        var callsBeforeRestart = random.CallCount;

        ExecuteSuffix(match, "human", ":restart-brewing");
        ExecuteKind(match, "human", GameActionKind.Draw);

        Assert.Equal(callsBeforeRestart + 1, random.CallCount);
    }

    private static MatchSession CreateMatch(IRandomSource random)
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips, new[] { Card() });
        return MatchSession.Create(random, rules);
    }

    private static IRoundEventRule Card() =>
        SetOneFortunes.CreateFinalBatch().Single(candidate => candidate.Id == "a-second-chance");

    private static PlayerView Player(MatchSession match, string playerId) =>
        match.GetSnapshot(playerId).Players.Single(player => player.Id == playerId);

    private static void ExecuteKind(MatchSession match, string playerId, GameActionKind kind)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Kind == kind);
        match.Execute(playerId, action);
    }

    private static void ExecuteSuffix(MatchSession match, string playerId, string suffix)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Id.EndsWith(suffix, StringComparison.Ordinal));
        match.Execute(playerId, action);
    }

    private static void FinishRoundAndAdvance(MatchSession match)
    {
        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);
        while (match.Phase != MatchPhase.RoundComplete)
        {
            var progressed = false;
            foreach (var playerId in new[] { "human", "ai" })
            {
                var action = match.GetLegalActions(playerId).FirstOrDefault(candidate =>
                    candidate.Kind == GameActionKind.FinishShopping || candidate.Kind == GameActionKind.FinishRubySpending);
                if (action == null) continue;
                match.Execute(playerId, action);
                progressed = true;
                if (match.Phase == MatchPhase.RoundComplete) break;
            }
            Assert.True(progressed);
        }
        ExecuteKind(match, "human", GameActionKind.NextRound);
    }

    private sealed class GiveHumanBlue : RoundEventRule
    {
        internal GiveHumanBlue() : base("give-human-blue", "Give blue", "Test setup.") { }
        public override void OnRevealed(RoundEventContext context) => context.TryGiveChip("human", TokenColor.Blue, 1);
    }

    private sealed class ZeroRandom : IRandomSource
    {
        public int NextInt(int exclusiveMax) => 0;
    }

    private sealed class FixedRandom : IRandomSource
    {
        private readonly Queue<int> _values;
        internal FixedRandom(params int[] values) => _values = new Queue<int>(values);
        public int NextInt(int exclusiveMax)
        {
            var value = _values.Count == 0 ? 0 : _values.Dequeue();
            if (value < 0 || value >= exclusiveMax)
                throw new InvalidOperationException($"Fixed value {value} is invalid for {exclusiveMax}.");
            return value;
        }
    }

    private sealed class RecordingRandom : IRandomSource
    {
        public int CallCount { get; private set; }
        public int NextInt(int exclusiveMax)
        {
            CallCount++;
            return 0;
        }
    }
}
