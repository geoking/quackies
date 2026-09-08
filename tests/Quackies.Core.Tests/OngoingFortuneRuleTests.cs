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

public sealed class OngoingFortuneRuleTests
{
    [Fact]
    public void OngoingBatchContainsAllEightDistinctCards()
    {
        var cards = SetOneFortunes.CreateOngoingBatch().ToArray();

        Assert.Equal(8, cards.Length);
        Assert.Equal(8, cards.Select(card => card.Id).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void LivingInLuxuryRaisesExplosionThresholdToNineForThisRound()
    {
        var match = CreateMatch("living-in-luxury", new FixedRandomSource(0, 0, 0, 0, 0, 0, 0));
        for (var draw = 0; draw < 6; draw++) ExecuteKind(match, "human", GameActionKind.Draw);

        var human = Player(match, "human");
        Assert.Equal(9, human.ExplosionThreshold);
        Assert.Equal(8, human.WhiteTotal);
        Assert.False(human.Exploded);
    }

    [Fact]
    public void PumpkinPatchPartyAdvancesOrangeOneExtraPhysicalSpace()
    {
        var match = CreateMatch("pumpkin-patch-party", new FixedRandomSource(0, 7));

        ExecuteKind(match, "human", GameActionKind.Draw);

        var chip = Assert.Single(Player(match, "human").PlacedChips);
        Assert.Equal(TokenColor.Orange, chip.Color);
        Assert.Equal(2, chip.Position);
    }

    [Fact]
    public void LuckyDevilAwardsTwoPointsOnRubySpaceAfterExplosion()
    {
        var match = CreateMatch("lucky-devil", new FixedRandomSource(0, 0, 0, 0, 0, 0, 0, 0));
        ExecuteKind(match, "ai", GameActionKind.Stop);
        for (var draw = 0; draw < 6; draw++) ExecuteKind(match, "human", GameActionKind.Draw);

        ExecuteSuffix(match, "human", ":take-coins");

        var human = Player(match, "human");
        Assert.True(human.Exploded);
        Assert.True(human.ScoringSpace.HasRuby);
        Assert.Equal(2, human.VictoryPoints);
    }

    [Fact]
    public void ShiningExtraBrightAddsRubyToNormalScoringRuby()
    {
        var match = CreateMatch("its-shining-extra-bright", new FixedRandomSource(0, 0, 0, 0, 0, 0));
        ExecuteKind(match, "ai", GameActionKind.Stop);
        for (var draw = 0; draw < 4; draw++) ExecuteKind(match, "human", GameActionKind.Draw);
        ExecuteKind(match, "human", GameActionKind.Stop);

        var human = Player(match, "human");
        Assert.True(human.ScoringSpace.HasRuby);
        Assert.Equal(3, human.Rubies);
    }

    [Fact]
    public void SeasonedPerfectlyAdvancesDropletAtWhiteTotalExactlySeven()
    {
        var match = CreateMatch("seasoned-perfectly", new FixedRandomSource(0, 0, 0, 0, 0, 2, 0));
        ExecuteKind(match, "ai", GameActionKind.Stop);
        for (var draw = 0; draw < 5; draw++) ExecuteKind(match, "human", GameActionKind.Draw);
        ExecuteKind(match, "human", GameActionKind.Stop);

        var human = Player(match, "human");
        Assert.Equal(7, human.WhiteTotal);
        Assert.Equal(1, human.DropletPosition);
    }

    [Fact]
    public void PotIsFullGivesEachTiedLeaderTwoDieRolls()
    {
        var match = CreateMatch("the-pot-is-full", new FixedRandomSource(0, 0, 0, 0, 0));

        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);

        Assert.All(match.GetSnapshot("human").Players, player => Assert.Equal(2, player.VictoryPoints));
    }

    [Fact]
    public void RollTheDieAppliesImmediateRewardAndMakesGiftDrawable()
    {
        var match = CreateMatch("roll-the-die", new FixedRandomSource(0, 4, 0));
        var view = match.GetSnapshot("human");
        var human = Player(match, "human");

        Assert.Equal(MatchPhase.Brewing, view.Phase);
        Assert.Equal(10, human.InventoryCount);
        Assert.Equal(10, human.BagCount);
        Assert.Equal(2, view.OwnBag.Count(chip => chip.Color == TokenColor.Orange));
        Assert.Equal(17, view.ShopOffers.Single(offer => offer.Color == TokenColor.Orange).Remaining);
    }

    [Fact]
    public void MagicPotionRefillsSpentFlaskForFreeAtRoundEnd()
    {
        var match = CreateMatch("magic-potion", new FixedRandomSource(0, 0, 0, 0));
        ExecuteKind(match, "human", GameActionKind.Draw);
        ExecuteKind(match, "human", GameActionKind.UseFlask);
        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);
        Assert.False(Player(match, "human").FlaskFull);

        FinishShoppingAndRubies(match);

        Assert.Equal(MatchPhase.RoundComplete, match.Phase);
        Assert.True(Player(match, "human").FlaskFull);
    }

    private static MatchSession CreateMatch(string cardId, IRandomSource random)
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var card = SetOneFortunes.CreateOngoingBatch().Single(candidate => candidate.Id == cardId);
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

    private static void FinishShoppingAndRubies(MatchSession match)
    {
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
    }

    private sealed class FixedRandomSource : IRandomSource
    {
        private readonly Queue<int> _values;
        internal FixedRandomSource(params int[] values) { _values = new Queue<int>(values); }
        public int NextInt(int exclusiveMax)
        {
            if (exclusiveMax <= 0) throw new ArgumentOutOfRangeException(nameof(exclusiveMax));
            var value = _values.Count == 0 ? 0 : _values.Dequeue();
            if (value < 0 || value >= exclusiveMax) throw new InvalidOperationException($"Fixed value {value} is invalid for {exclusiveMax}.");
            return value;
        }
    }
}
