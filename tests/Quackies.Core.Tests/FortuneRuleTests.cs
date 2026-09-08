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

public sealed class FortuneRuleTests
{
    [Fact]
    public void BeginnersBonusGiftIsImmediatelyInInventoryAndDrawableBag()
    {
        var match = CreateMatch("beginners-bonus");
        var view = match.GetSnapshot("human");
        var player = view.Players.Single(candidate => candidate.Id == "human");

        Assert.Equal(MatchPhase.Brewing, view.Phase);
        Assert.Equal(10, player.InventoryCount);
        Assert.Equal(10, player.BagCount);
        Assert.Equal(2, view.OwnBag.Count(chip => chip.Color == TokenColor.Green && chip.Value == 1));
        Assert.Equal(11, view.ShopOffers.Single(offer => offer.Color == TokenColor.Green && offer.Value == 1).Remaining);
    }

    [Fact]
    public void SupplyChoiceDisappearsAfterOtherPlayerTakesLastChipButFallbackRemains()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var stockOne = new[] { new ShopChipDefinition(TokenColor.Black, 1, 10, 1, 1) };
        var card = Card("you-only-get-to-choose-one");
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, stockOne, new[] { card });
        var match = MatchSession.Create(new ZeroRandom(), rules);
        var staleAiChipChoice = match.GetLegalActions("ai").Single(action => action.Color == TokenColor.Black);

        ExecuteByColor(match, "human", TokenColor.Black, 1);

        var currentAiActions = match.GetLegalActions("ai");
        Assert.DoesNotContain(currentAiActions, action => action.Color == TokenColor.Black);
        Assert.Contains(currentAiActions, action => action.Id.EndsWith(":three-rubies", StringComparison.Ordinal));
        Assert.Throws<InvalidOperationException>(() => match.Execute("ai", staleAiChipChoice));
        Assert.Equal(0, match.GetSnapshot("ai").ShopOffers.Single().Remaining);
    }

    [Fact]
    public void RemovingColoredStartingChipUpdatesBagInventoryAndSupply()
    {
        var match = MatchSession.Create(new ZeroRandom(), RulesWith(new RemoveGreenEvent()));

        ExecuteBySuffix(match, "human", ":remove-green");

        var view = match.GetSnapshot("human");
        var human = view.Players.Single(player => player.Id == "human");
        Assert.Equal(8, human.InventoryCount);
        Assert.Equal(8, human.BagCount);
        Assert.DoesNotContain(view.OwnBag, chip => chip.Color == TokenColor.Green);
        Assert.Equal(14, view.ShopOffers.Single(offer => offer.Color == TokenColor.Green && offer.Value == 1).Remaining);
    }

    [Fact]
    public void CharityAwardsEveryPlayerTiedForFewestRubies()
    {
        var match = CreateMatch("charity");

        Assert.All(match.GetSnapshot("human").Players, player => Assert.Equal(2, player.Rubies));
    }

    [Fact]
    public void BeginnersBonusAwardsEveryPlayerTiedForFewestPoints()
    {
        var match = CreateMatch("beginners-bonus");
        var view = match.GetSnapshot("human");

        Assert.All(view.Players, player => Assert.Equal(10, player.InventoryCount));
        Assert.Equal(11, view.ShopOffers.Single(offer => offer.Color == TokenColor.Green && offer.Value == 1).Remaining);
    }

    [Fact]
    public void ChooseWiselyOffersPurpleOnceItIsUnlockedInRoundThree()
    {
        var events = new IRoundEventRule[] { new NoOpEvent("first"), new NoOpEvent("second"), Card("choose-wisely") };
        var match = MatchSession.Create(new ZeroRandom(), RulesWith(events));
        FinishRoundAndAdvance(match);
        FinishRoundAndAdvance(match);

        var view = match.GetSnapshot("human");
        Assert.Equal(3, view.Round);
        Assert.Equal(MatchPhase.Preparation, view.Phase);
        Assert.Contains(match.GetLegalActions("human"), action => action.Color == TokenColor.Purple && action.Value == 1);
    }

    [Fact]
    public void LegalActionCollectionsAreReadOnlySnapshotsOfCurrentCapabilities()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values,
            new[] { new ShopChipDefinition(TokenColor.Black, 1, 10, 1, 1) },
            new[] { Card("you-only-get-to-choose-one") });
        var match = MatchSession.Create(new ZeroRandom(), rules);
        var firstSnapshot = match.GetLegalActions("ai");

        Assert.Throws<NotSupportedException>(() => ((IList<GameAction>)firstSnapshot).Add(firstSnapshot[0]));
        ExecuteByColor(match, "human", TokenColor.Black, 1);

        Assert.Contains(firstSnapshot, action => action.Color == TokenColor.Black);
        Assert.DoesNotContain(match.GetLegalActions("ai"), action => action.Color == TokenColor.Black);
    }

    private static MatchSession CreateMatch(string cardId) =>
        MatchSession.Create(new ZeroRandom(), RulesWith(Card(cardId)));

    private static RuleSet RulesWith(params IRoundEventRule[] events)
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        return new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips, events);
    }

    private static IRoundEventRule Card(string id) =>
        SetOneFortunes.CreatePreparationBatch().Single(card => string.Equals(card.Id, id, StringComparison.Ordinal));

    private static void ExecuteByColor(MatchSession match, string playerId, TokenColor color, int value)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Color == color && candidate.Value == value);
        match.Execute(playerId, action);
    }

    private static void ExecuteBySuffix(MatchSession match, string playerId, string suffix)
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
                var actions = match.GetLegalActions(playerId);
                var action = actions.FirstOrDefault(candidate => candidate.Kind == GameActionKind.FinishShopping ||
                    candidate.Kind == GameActionKind.FinishRubySpending);
                if (action == null) continue;
                match.Execute(playerId, action);
                progressed = true;
                if (match.Phase == MatchPhase.RoundComplete) break;
            }
            Assert.True(progressed);
        }
        ExecuteKind(match, "human", GameActionKind.NextRound);
    }

    private static void ExecuteKind(MatchSession match, string playerId, GameActionKind kind)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Kind == kind);
        match.Execute(playerId, action);
    }

    private sealed class RemoveGreenEvent : RoundEventRule
    {
        internal RemoveGreenEvent() : base("remove-green-test", "Remove green", "Test-only supply restoration.") { }
        public override void OnRevealed(RoundEventContext context)
        {
            context.OfferChoice("human", Title, new RoundEventChoice("remove-green", "Remove green",
                (round, playerId) => round.RemoveFromBag(playerId, TokenColor.Green, 1), TokenColor.Green, 1));
        }
    }

    private sealed class NoOpEvent : RoundEventRule
    {
        internal NoOpEvent(string id) : base(id, id, id) { }
    }

    private sealed class ZeroRandom : IRandomSource
    {
        public int NextInt(int exclusiveMax) => 0;
    }
}
