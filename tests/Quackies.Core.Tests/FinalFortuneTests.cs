using System;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Quackies.Core.Rules.Fortunes;
using Quackies.Core.Tokens;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class FinalFortuneTests
{
    [Fact]
    public void WellStirredReturnsTheFirstOrdinaryWhiteWithoutSpendingTheFlask()
    {
        var match = CreateMatch("well-stirred", new ZeroRandom());

        ExecuteKind(match, "human", GameActionKind.Draw);
        Assert.Contains(match.GetLegalActions("human"), action =>
            action.Id.EndsWith(":return-white", StringComparison.Ordinal));
        ExecuteSuffix(match, "human", ":return-white");

        var afterReturn = Player(match, "human");
        Assert.Empty(afterReturn.PlacedChips);
        Assert.Equal(0, afterReturn.WhiteTotal);
        Assert.Equal(9, afterReturn.BagCount);
        Assert.True(afterReturn.FlaskFull);
        Assert.False(afterReturn.Stopped);
        Assert.False(afterReturn.Exploded);

        ExecuteKind(match, "human", GameActionKind.Draw);
        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.Kind == GameActionKind.Choose);
        Assert.Contains(match.GetLegalActions("human"), action => action.Kind == GameActionKind.UseFlask);
    }

    [Fact]
    public void WellStirredCanKeepTheFirstWhiteAndOffersEachPlayerIndependently()
    {
        var match = CreateMatch("well-stirred", new ZeroRandom());

        ExecuteKind(match, "human", GameActionKind.Draw);
        ExecuteSuffix(match, "human", ":keep-white");
        ExecuteKind(match, "ai", GameActionKind.Draw);

        var human = Player(match, "human");
        Assert.Single(human.PlacedChips);
        Assert.Equal(1, human.WhiteTotal);
        Assert.True(human.FlaskFull);
        Assert.Contains(match.GetLegalActions("ai"), action =>
            action.Id.EndsWith(":return-white", StringComparison.Ordinal));
    }

    [Fact]
    public void WhitePlacedFromBlueSelectionConsumesTheWellStirredOffer()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var wellStirred = SetOneFortunes.CreateFinalBatch().Single(candidate => candidate.Id == "well-stirred");
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips,
            new IRoundEventRule[] { new GiveHumanBlue(), wellStirred });
        var match = MatchSession.Create(new FixedRandom(0, 0, 0, 0, 9, 0, 0), rules);
        FinishRoundAndAdvance(match);

        ExecuteKind(match, "human", GameActionKind.Draw);
        ExecuteSuffix(match, "human", ":bag-0");
        Assert.Contains(match.GetLegalActions("human"), action =>
            action.Id.EndsWith(":return-white", StringComparison.Ordinal));
        ExecuteSuffix(match, "human", ":keep-white");
        ExecuteKind(match, "human", GameActionKind.Draw);

        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.Kind == GameActionKind.Choose);
        Assert.Equal(2, Player(match, "human").WhiteTotal);
    }

    private static MatchSession CreateMatch(string cardId, IRandomSource random)
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var card = SetOneFortunes.CreateFinalBatch().Single(candidate => candidate.Id == cardId);
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips, new[] { card });
        return MatchSession.Create(random, rules);
    }

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
        public override void OnRevealed(RoundEventContext context) =>
            context.TryGiveChip("human", TokenColor.Blue, 1);
    }

    private sealed class ZeroRandom : IRandomSource
    {
        public int NextInt(int exclusiveMax) => 0;
    }

    private sealed class FixedRandom : IRandomSource
    {
        private readonly System.Collections.Generic.Queue<int> _values;
        internal FixedRandom(params int[] values) => _values = new System.Collections.Generic.Queue<int>(values);
        public int NextInt(int exclusiveMax)
        {
            var value = _values.Count == 0 ? 0 : _values.Dequeue();
            if (value < 0 || value >= exclusiveMax)
                throw new InvalidOperationException($"Fixed value {value} is invalid for {exclusiveMax}.");
            return value;
        }
    }
}
