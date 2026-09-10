using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Quackies.Core.Rules.Fortunes;
using Quackies.Core.Rules.Ingredients;
using Quackies.Core.Tokens;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class InteractiveFortuneTests
{
    [Fact]
    public void InteractiveBatchContainsTheThreeDistinctCards()
    {
        var cards = SetOneFortunes.CreateInteractiveBatch().ToArray();

        Assert.Equal(3, cards.Length);
        Assert.Equal(3, cards.Select(card => card.Id).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void LessIsMoreReturnsPreviewsAndRewardsTheUniqueLowestTotal()
    {
        var random = new FixedRandom(0, 0, 0, 0, 0, 0, 4, 4, 4, 4, 4);
        var match = CreateMatch("less-is-more", random);

        var view = match.GetSnapshot("human");
        var human = Player(view, "human");
        var ai = Player(view, "ai");
        Assert.Equal(MatchPhase.Brewing, view.Phase);
        Assert.Equal(10, human.InventoryCount);
        Assert.Equal(10, human.BagCount);
        Assert.Contains(view.OwnBag, chip => chip.Color == TokenColor.Blue && chip.Value == 2);
        Assert.Equal(1, human.Rubies);
        Assert.Equal(9, ai.InventoryCount);
        Assert.Equal(9, ai.BagCount);
        Assert.Equal(2, ai.Rubies);
    }

    [Fact]
    public void LessIsMoreRewardsEveryPlayerTiedForLowest()
    {
        var match = CreateMatch("less-is-more", new ZeroRandom());
        var view = match.GetSnapshot("human");

        Assert.All(view.Players, player =>
        {
            Assert.Equal(10, player.InventoryCount);
            Assert.Equal(1, player.Rubies);
        });
        Assert.Equal(8, view.ShopOffers.Single(offer => offer.Color == TokenColor.Blue && offer.Value == 2).Remaining);
    }

    [Fact]
    public void OpportunisticMomentUpgradesOnlyAPreviewedChipAndReturnsTheOldChipToSupply()
    {
        var random = new FixedRandom(0, 8, 0, 0, 0, 0, 0, 0, 0);
        var match = CreateMatch("an-opportunistic-moment", random);

        Assert.Equal(MatchPhase.Preparation, match.Phase);
        Assert.Contains(match.GetLegalActions("human"), action =>
            action.Id.EndsWith(":upgrade-green-1", StringComparison.Ordinal));
        ExecuteSuffix(match, "human", ":upgrade-green-1");

        var view = match.GetSnapshot("human");
        Assert.Equal(MatchPhase.Brewing, view.Phase);
        Assert.DoesNotContain(view.OwnBag, chip => chip.Color == TokenColor.Green && chip.Value == 1);
        Assert.Contains(view.OwnBag, chip => chip.Color == TokenColor.Green && chip.Value == 2);
        Assert.Equal(9, Player(view, "human").InventoryCount);
        Assert.Equal(10, Player(view, "ai").InventoryCount);
        Assert.Equal(13, view.ShopOffers.Single(offer => offer.Color == TokenColor.Green && offer.Value == 1).Remaining);
        Assert.Equal(9, view.ShopOffers.Single(offer => offer.Color == TokenColor.Green && offer.Value == 2).Remaining);
    }

    [Fact]
    public void OpportunisticMomentRechecksSharedStockAndFallsBackToGreenOne()
    {
        var shop = new[]
        {
            new ShopChipDefinition(TokenColor.Green, 1, 4, 0, 1),
            new ShopChipDefinition(TokenColor.Green, 2, 8, 1, 1)
        };
        var random = new FixedRandom(0, 8, 0, 0, 0, 8, 0, 0, 0);
        var match = MatchSession.Create(random, RulesWithShop(shop, Card("an-opportunistic-moment")));
        var staleAiUpgrade = match.GetLegalActions("ai").Single(action =>
            action.Id.EndsWith(":upgrade-green-1", StringComparison.Ordinal));

        ExecuteSuffix(match, "human", ":upgrade-green-1");

        Assert.DoesNotContain(match.GetLegalActions("ai"), action =>
            action.Id.EndsWith(":upgrade-green-1", StringComparison.Ordinal));
        Assert.Contains(match.GetLegalActions("ai"), action =>
            action.Id.EndsWith(":fallback-green-1", StringComparison.Ordinal));
        Assert.Throws<InvalidOperationException>(() => match.Execute("ai", staleAiUpgrade));
        ExecuteSuffix(match, "ai", ":fallback-green-1");

        var view = match.GetSnapshot("ai");
        Assert.Equal(MatchPhase.Brewing, view.Phase);
        Assert.Contains(view.OwnBag, chip => chip.Color == TokenColor.Green && chip.Value == 1);
        Assert.Equal(0, view.ShopOffers.Single(offer => offer.Color == TokenColor.Green && offer.Value == 1).Remaining);
    }

    [Fact]
    public void SchadenfreudeGivesTheOtherPlayerAnUnlockedTwoChipAfterExplosion()
    {
        var match = CreateMatch("schadenfreude", new ZeroRandom());
        ExecuteKind(match, "ai", GameActionKind.Stop);
        for (var draw = 0; draw < 6; draw++) ExecuteKind(match, "human", GameActionKind.Draw);

        var choices = match.GetLegalActions("ai");
        Assert.Contains(choices, action => action.Color == TokenColor.Blue && action.Value == 2);
        Assert.DoesNotContain(choices, action => action.Color == TokenColor.Yellow);
        ExecuteColor(match, "ai", TokenColor.Blue, 2);

        var view = match.GetSnapshot("ai");
        Assert.True(Player(view, "human").Exploded);
        Assert.Contains(view.OwnBag, chip => chip.Color == TokenColor.Blue && chip.Value == 2);
        Assert.Equal(10, Player(view, "ai").InventoryCount);
    }

    [Fact]
    public void SchadenfreudeDoesNothingWhenPlayersStopSafely()
    {
        var match = CreateMatch("schadenfreude", new ZeroRandom());

        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);

        Assert.All(match.GetSnapshot("human").Players, player => Assert.Equal(9, player.InventoryCount));
    }

    private static MatchSession CreateMatch(string cardId, IRandomSource random)
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips, new[] { Card(cardId) });
        return MatchSession.Create(random, rules);
    }

    private static RuleSet RulesWithShop(IEnumerable<ShopChipDefinition> shop, params IRoundEventRule[] events) =>
        new RuleSet(BoardTrack.Standard(), SetOneIngredients.Create(), shop, events);

    private static IRoundEventRule Card(string id) =>
        SetOneFortunes.CreateInteractiveBatch().Single(card => string.Equals(card.Id, id, StringComparison.Ordinal));

    private static PlayerView Player(MatchView view, string playerId) =>
        view.Players.Single(player => string.Equals(player.Id, playerId, StringComparison.Ordinal));

    private static void ExecuteSuffix(MatchSession match, string playerId, string suffix)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Id.EndsWith(suffix, StringComparison.Ordinal));
        match.Execute(playerId, action);
    }

    private static void ExecuteColor(MatchSession match, string playerId, TokenColor color, int value)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Color == color && candidate.Value == value);
        match.Execute(playerId, action);
    }

    private static void ExecuteKind(MatchSession match, string playerId, GameActionKind kind)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Kind == kind);
        match.Execute(playerId, action);
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
