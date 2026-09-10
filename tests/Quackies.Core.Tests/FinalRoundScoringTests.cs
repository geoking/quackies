using System;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class FinalRoundScoringTests
{
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(4, 3, 3)]
    [InlineData(5, 3, 4)]
    [InlineData(14, 3, 5)]
    [InlineData(35, 15, 22)]
    public void SafeFinalPotReceivesPointsAndRoundedDownCoinsWithoutShopping(int coins, int points, int expected)
    {
        var match = CreateFinalRound(coins, points);
        var before = Human(match).VictoryPoints;
        Act(match, "human", "stop");
        Act(match, "ai", "stop");

        Assert.Equal(before + expected + 1, Human(match).VictoryPoints); // Fixed die face awards one VP.
        AssertFinalActions(match);
        var settled = Human(match).VictoryPoints;
        FinishRubies(match);
        Assert.Equal(MatchPhase.Finished, match.GetSnapshot("human").Phase);
        Assert.Equal(settled, Human(match).VictoryPoints);
    }

    [Theory]
    [InlineData(9, 1, 1)]
    [InlineData(15, 1, 3)]
    [InlineData(15, 5, 5)]
    [InlineData(14, 0, 2)]
    public void ExplodedFinalPotAutomaticallyReceivesBetterSingleReward(int coins, int points, int expected)
    {
        var match = CreateFinalRound(coins, points);
        var before = Human(match).VictoryPoints;
        Act(match, "ai", "stop");
        for (var i = 0; i < 6; i++) Act(match, "human", "draw");

        Assert.True(Human(match).Exploded);
        Assert.Equal(before + expected, Human(match).VictoryPoints);
        AssertFinalActions(match);
        Assert.DoesNotContain(match.GetSnapshot("human").DieRolls,
            roll => roll.Round == 9 && roll.PlayerId == "human");
        var settled = Human(match).VictoryPoints;
        FinishRubies(match);
        Assert.Equal(settled, Human(match).VictoryPoints); // No second conversion at the phase boundary.
    }

    [Fact]
    public void EarlierExplosionStillOffersPointsOrShoppingCoins()
    {
        var match = MatchSession.Create(new ZeroRandom(), RuleSet.SetOne(Array.Empty<IRoundEventRule>()));
        Act(match, "ai", "stop");
        for (var i = 0; i < 6; i++) Act(match, "human", "draw");

        Assert.Equal(MatchPhase.Evaluation, match.GetSnapshot("human").Phase);
        Assert.Contains(match.GetLegalActions("human"), action => action.Id.EndsWith(":take-points", StringComparison.Ordinal));
        Assert.Contains(match.GetLegalActions("human"), action => action.Id.EndsWith(":take-coins", StringComparison.Ordinal));
    }

    private static MatchSession CreateFinalRound(int coins, int points)
    {
        var standard = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var track = new BoardTrack(Enumerable.Range(0, 54)
            .Select(position => new TrackSpaceView(position, coins, points, false)));
        var match = MatchSession.Create(new ZeroRandom(),
            new RuleSet(track, standard.Ingredients.Values, standard.ShopChips));
        for (var round = 1; round < 9; round++)
        {
            Act(match, "human", "stop");
            Act(match, "ai", "stop");
            foreach (var id in new[] { "human", "ai" })
                if (match.GetLegalActions(id).Any(action => action.Id == "finish-shopping"))
                    Act(match, id, "finish-shopping");
            // The starting shopper alternates each round.
            foreach (var id in new[] { "human", "ai" })
                if (match.GetLegalActions(id).Any(action => action.Id == "finish-shopping"))
                    Act(match, id, "finish-shopping");
            FinishRubies(match);
            Act(match, "human", "next-round");
        }
        return match;
    }

    private static void AssertFinalActions(MatchSession match)
    {
        Assert.Equal(MatchPhase.RubySpending, match.GetSnapshot("human").Phase);
        foreach (var id in new[] { "human", "ai" })
        {
            Assert.Equal(0, match.GetSnapshot(id).Players.Single(player => player.Id == id).Coins);
            Assert.All(match.GetLegalActions(id), action =>
                Assert.True(action.Kind == GameActionKind.ConvertRubies || action.Kind == GameActionKind.FinishRubySpending));
        }
    }

    private static PlayerView Human(MatchSession match) =>
        match.GetSnapshot("human").Players.Single(player => player.Id == "human");

    private static void FinishRubies(MatchSession match)
    {
        Act(match, "human", "finish-ruby-spending");
        Act(match, "ai", "finish-ruby-spending");
    }

    private static void Act(MatchSession match, string id, string actionId) =>
        match.Execute(id, match.GetLegalActions(id).Single(action => action.Id == actionId));

    private sealed class ZeroRandom : IRandomSource
    {
        public int NextInt(int exclusiveMax) => 0;
    }
}
