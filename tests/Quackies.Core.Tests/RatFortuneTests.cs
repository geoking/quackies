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

public sealed class RatFortuneTests
{
    [Fact]
    public void RoundOneHasNoRatsAfterAsymmetricJustInTimePointsButRoundTwoDoes()
    {
        var match = MatchSession.Create(new ZeroRandom(), RulesWith(
            PreparationCard("just-in-time"), new NoOpEvent("round-two")));

        ExecuteSuffix(match, "human", ":four-points");
        ExecuteSuffix(match, "ai", ":remove-white-1");

        Assert.Equal(MatchPhase.Brewing, match.Phase);
        Assert.All(match.GetSnapshot("human").Players, player => Assert.Equal(player.DropletPosition, player.RatPosition));

        AdvanceOneRound(match);

        Assert.Equal(2, RatSteps(match, "ai"));
        Assert.Equal(0, RatSteps(match, "human"));
    }

    [Fact]
    public void GoodStartOffersKeepAndAtMostThreeRatTrades()
    {
        var match = StartRoundTwoWithPointGap(RatCard("a-good-start"));
        var aiActions = match.GetLegalActions("ai");

        Assert.Contains(aiActions, action => action.Id.EndsWith(":keep-rats", StringComparison.Ordinal));
        Assert.Contains(aiActions, action => action.Id.EndsWith(":trade-1-rats", StringComparison.Ordinal));
        Assert.Contains(aiActions, action => action.Id.EndsWith(":trade-2-rats", StringComparison.Ordinal));
        Assert.Contains(aiActions, action => action.Id.EndsWith(":trade-3-rats", StringComparison.Ordinal));
        Assert.DoesNotContain(aiActions, action => action.Id.EndsWith(":trade-4-rats", StringComparison.Ordinal));

        ExecuteSuffix(match, "human", ":keep-rats");
        ExecuteSuffix(match, "ai", ":keep-rats");

        Assert.Equal(9, RatSteps(match, "ai"));
    }

    [Fact]
    public void GoodStartTradeChangesOnlyTheCurrentRoundsRatEntitlement()
    {
        var match = StartRoundTwoWithPointGap(RatCard("a-good-start"), new NoOpEvent("round-three"));
        var rubiesBefore = Player(match, "ai").Rubies;

        ExecuteSuffix(match, "human", ":keep-rats");
        ExecuteSuffix(match, "ai", ":trade-3-rats");

        Assert.Equal(6, RatSteps(match, "ai"));
        Assert.Equal(rubiesBefore + 3, Player(match, "ai").Rubies);

        AdvanceOneRound(match);

        Assert.Equal(ExpectedOrdinaryRatSteps(match, "ai"), RatSteps(match, "ai"));
        Assert.NotEqual(6, RatSteps(match, "ai"));
    }

    [Fact]
    public void RatInfestationDoublesRatsForOneRoundAndThenResets()
    {
        var match = StartRoundTwoWithPointGap(RatCard("rat-infestation"), new NoOpEvent("round-three"));

        Assert.Equal(18, RatSteps(match, "ai"));

        AdvanceOneRound(match);

        Assert.Equal(ExpectedOrdinaryRatSteps(match, "ai"), RatSteps(match, "ai"));
        Assert.NotEqual(18, RatSteps(match, "ai"));
    }

    [Fact]
    public void RatsAreYourFriendsFreezesPointOffersButRecalculatesActualRatsAfterPoints()
    {
        var match = StartRoundTwoWithPointGap(RatCard("rats-are-your-friends"), leaderId: "ai");
        var humanPoints = match.GetLegalActions("human").Single(action =>
            action.Id.EndsWith(":rat-points", StringComparison.Ordinal));
        var aiPointsBefore = match.GetLegalActions("ai").Single(action =>
            action.Id.EndsWith(":rat-points", StringComparison.Ordinal));

        Assert.Equal("Gain 9 victory points", humanPoints.Label);
        Assert.Equal("Gain 0 victory points", aiPointsBefore.Label);
        match.Execute("human", humanPoints);

        var aiPointsAfter = match.GetLegalActions("ai").Single(action =>
            action.Id.EndsWith(":rat-points", StringComparison.Ordinal));
        Assert.Equal(aiPointsBefore.Id, aiPointsAfter.Id);
        Assert.Equal("Gain 0 victory points", aiPointsAfter.Label);
        match.Execute("ai", aiPointsAfter);

        Assert.Equal(6, RatSteps(match, "human"));
        Assert.Equal(0, RatSteps(match, "ai"));
    }

    [Fact]
    public void RatsAreYourFriendsHonorsUnlocksAndSharedStock()
    {
        var shop = new[]
        {
            new ShopChipDefinition(TokenColor.Green, 4, 14, 1, 1),
            new ShopChipDefinition(TokenColor.Yellow, 4, 18, 1, 2)
        };
        var match = MatchSession.Create(new ZeroRandom(), RulesWithShop(shop, RatCard("rats-are-your-friends")));
        var staleAiGreen = match.GetLegalActions("ai").Single(action => action.Color == TokenColor.Green);

        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.Color == TokenColor.Yellow);
        ExecuteColor(match, "human", TokenColor.Green, 4);

        Assert.DoesNotContain(match.GetLegalActions("ai"), action => action.Color == TokenColor.Green);
        Assert.Contains(match.GetLegalActions("ai"), action => action.Id.EndsWith(":rat-points", StringComparison.Ordinal));
        Assert.Throws<InvalidOperationException>(() => match.Execute("ai", staleAiGreen));
        Assert.Equal(0, match.GetSnapshot("human").ShopOffers.Single().Remaining);
    }

    [Fact]
    public void WheelAndDealWithZeroStartingRubiesCannotTakeAFreeChip()
    {
        var match = MatchSession.Create(new ZeroRandom(), new MatchSettings(0), RulesWith(RatCard("wheel-and-deal")));

        Assert.All(new[] { "human", "ai" }, playerId =>
        {
            var action = Assert.Single(match.GetLegalActions(playerId));
            Assert.EndsWith(":keep-ruby", action.Id, StringComparison.Ordinal);
            Assert.Null(action.Color);
        });
    }

    [Fact]
    public void WheelAndDealChargesOnceHonorsUnlocksAndAddsTheChipToTheCurrentBag()
    {
        var shop = new[]
        {
            new ShopChipDefinition(TokenColor.Orange, 1, 3, 2, 1),
            new ShopChipDefinition(TokenColor.Yellow, 1, 8, 2, 2),
            new ShopChipDefinition(TokenColor.Purple, 1, 9, 2, 1),
            new ShopChipDefinition(TokenColor.Black, 1, 10, 2, 1)
        };
        var rules = RulesWithShop(shop, new GiveRubiesEvent(3), RatCard("wheel-and-deal"));
        var match = MatchSession.Create(new ZeroRandom(), rules);
        AdvanceOneRound(match);
        var before = Player(match, "human");

        Assert.Contains(match.GetLegalActions("human"), action => action.Color == TokenColor.Yellow);
        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.Color == TokenColor.Purple);
        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.Color == TokenColor.Black);
        ExecuteColor(match, "human", TokenColor.Yellow, 1);
        ExecuteSuffix(match, "ai", ":keep-ruby");

        var after = Player(match, "human");
        Assert.Equal(before.Rubies - 1, after.Rubies);
        Assert.Equal(before.InventoryCount + 1, after.InventoryCount);
        Assert.Equal(before.BagCount + 1, after.BagCount);
        Assert.Contains(match.GetSnapshot("human").OwnBag, chip => chip.Color == TokenColor.Yellow && chip.Value == 1);
        Assert.Equal(MatchPhase.Brewing, match.Phase);
        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.Kind == GameActionKind.Choose);
    }

    [Fact]
    public void WheelAndDealRemovesAnExhaustedSharedChipAndRejectsTheStaleChoice()
    {
        var shop = new[] { new ShopChipDefinition(TokenColor.Orange, 1, 3, 1, 1) };
        var match = MatchSession.Create(new ZeroRandom(), RulesWithShop(shop, RatCard("wheel-and-deal")));
        var staleAiExchange = match.GetLegalActions("ai").Single(action => action.Color == TokenColor.Orange);

        ExecuteColor(match, "human", TokenColor.Orange, 1);

        Assert.DoesNotContain(match.GetLegalActions("ai"), action => action.Color == TokenColor.Orange);
        Assert.Single(match.GetLegalActions("ai"));
        Assert.Throws<InvalidOperationException>(() => match.Execute("ai", staleAiExchange));
        Assert.Equal(0, match.GetSnapshot("human").ShopOffers.Single().Remaining);
    }

    private static MatchSession StartRoundTwoWithPointGap(IRoundEventRule roundTwoCard,
        IRoundEventRule? roundThreeCard = null, string leaderId = "human")
    {
        var events = new List<IRoundEventRule> { new GivePointsEvent(leaderId, 20), roundTwoCard };
        if (roundThreeCard != null) events.Add(roundThreeCard);
        var match = MatchSession.Create(new ZeroRandom(), RulesWith(events.ToArray()));
        AdvanceOneRound(match);
        Assert.Equal(2, match.Round);
        return match;
    }

    private static RuleSet RulesWith(params IRoundEventRule[] events)
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        return new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips, events);
    }

    private static RuleSet RulesWithShop(IEnumerable<ShopChipDefinition> shop, params IRoundEventRule[] events) =>
        new RuleSet(BoardTrack.Standard(), SetOneIngredients.Create(), shop, events);

    private static IRoundEventRule PreparationCard(string id) =>
        SetOneFortunes.CreatePreparationBatch().Single(card => string.Equals(card.Id, id, StringComparison.Ordinal));

    private static IRoundEventRule RatCard(string id) =>
        SetOneFortunes.CreateRatBatch().Single(card => string.Equals(card.Id, id, StringComparison.Ordinal));

    private static PlayerView Player(MatchSession match, string playerId) =>
        match.GetSnapshot(playerId).Players.Single(player => string.Equals(player.Id, playerId, StringComparison.Ordinal));

    private static int RatSteps(MatchSession match, string playerId)
    {
        var player = Player(match, playerId);
        return player.RatPosition - player.DropletPosition;
    }

    private static int ExpectedOrdinaryRatSteps(MatchSession match, string playerId)
    {
        var players = match.GetSnapshot(playerId).Players;
        var trailing = players.Single(player => player.Id == playerId).VictoryPoints;
        var leading = players.Max(player => player.VictoryPoints);
        var boundaries = new HashSet<int>
        { 1, 4, 7, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48 };
        var total = 0;
        for (var score = trailing; score < leading; score++)
        {
            var modulo = score % 50;
            if (modulo < 0) modulo += 50;
            if (boundaries.Contains(modulo)) total++;
        }
        return total;
    }

    private static void AdvanceOneRound(MatchSession match)
    {
        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);
        while (match.Phase != MatchPhase.RoundComplete)
        {
            var acted = false;
            foreach (var playerId in new[] { "human", "ai" })
            {
                var action = match.GetLegalActions(playerId).FirstOrDefault(candidate =>
                    candidate.Kind == GameActionKind.FinishShopping ||
                    candidate.Kind == GameActionKind.FinishRubySpending);
                if (action == null) continue;
                match.Execute(playerId, action);
                acted = true;
                if (match.Phase == MatchPhase.RoundComplete) break;
            }
            Assert.True(acted, "The round reached a phase with no legal way to continue.");
        }
        ExecuteKind(match, "human", GameActionKind.NextRound);
    }

    private static void ExecuteColor(MatchSession match, string playerId, TokenColor color, int value)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Color == color && candidate.Value == value);
        match.Execute(playerId, action);
    }

    private static void ExecuteSuffix(MatchSession match, string playerId, string suffix)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Id.EndsWith(suffix, StringComparison.Ordinal));
        match.Execute(playerId, action);
    }

    private static void ExecuteKind(MatchSession match, string playerId, GameActionKind kind)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Kind == kind);
        match.Execute(playerId, action);
    }

    private sealed class GivePointsEvent : RoundEventRule
    {
        private readonly string _playerId;
        private readonly int _points;

        internal GivePointsEvent(string playerId, int points) : base("give-points", "Give points", "Test score setup.")
        {
            _playerId = playerId;
            _points = points;
        }

        public override void OnRevealed(RoundEventContext context) => context.GainPoints(_playerId, _points);
    }

    private sealed class GiveRubiesEvent : RoundEventRule
    {
        private readonly int _rubies;

        internal GiveRubiesEvent(int rubies) : base("give-rubies", "Give rubies", "Test ruby setup.") => _rubies = rubies;

        public override void OnRevealed(RoundEventContext context)
        {
            foreach (var playerId in context.PlayerIds) context.GainRubies(playerId, _rubies);
        }
    }

    private sealed class NoOpEvent : RoundEventRule
    {
        internal NoOpEvent(string id) : base(id, id, "Test round without a fortune effect.") { }
    }

    private sealed class ZeroRandom : IRandomSource
    {
        public int NextInt(int exclusiveMax) => 0;
    }
}
