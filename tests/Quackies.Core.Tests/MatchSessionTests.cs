using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Quackies.Core.Rules.Ingredients;
using Quackies.Core.Tokens;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class MatchSessionTests
{
    [Fact]
    public void MatchStartsWithAuthoritativeTwoPlayerSetup()
    {
        var match = MatchSession.Create(new FixedRandomSource());
        var view = match.GetSnapshot("human");

        Assert.Equal(1, view.Round);
        Assert.Equal(MatchPhase.Brewing, view.Phase);
        Assert.Equal(9, view.OwnBag.Count);
        Assert.Equal(7, view.OwnBag.Count(chip => chip.Color == TokenColor.White));
        Assert.Contains(view.OwnBag, chip => chip.Color == TokenColor.Orange && chip.Value == 1);
        Assert.Contains(view.OwnBag, chip => chip.Color == TokenColor.Green && chip.Value == 1);
        Assert.All(view.Players, player =>
        {
            Assert.Equal(1, player.Rubies);
            Assert.True(player.FlaskFull);
            Assert.Equal(7, player.ExplosionThreshold);
        });
    }

    [Fact]
    public void BoardUsesPhysicalPositionsThroughSpoonReward()
    {
        var track = BoardTrack.Standard();

        Assert.Equal(54, track.Spaces.Count);
        Assert.Equal(52, track.LastChipPosition);
        Assert.Equal(33, track.At(52).Coins);
        Assert.Equal(35, track.ScoringSpace(52).Coins);
        Assert.Equal(15, track.ScoringSpace(52).Points);
    }

    [Fact]
    public void DefaultCatalogHasVerifiedStocksPricesAndUnlocks()
    {
        var rules = RuleSet.SetOne();

        AssertOffer(rules, TokenColor.Orange, 1, price: 3, stock: 18, round: 1);
        AssertOffer(rules, TokenColor.Green, 4, price: 14, stock: 13, round: 1);
        AssertOffer(rules, TokenColor.Blue, 4, price: 19, stock: 10, round: 1);
        AssertOffer(rules, TokenColor.Red, 2, price: 10, stock: 8, round: 1);
        AssertOffer(rules, TokenColor.Yellow, 2, price: 12, stock: 6, round: 2);
        AssertOffer(rules, TokenColor.Purple, 1, price: 9, stock: 15, round: 3);
        AssertOffer(rules, TokenColor.Black, 1, price: 10, stock: 18, round: 1);
    }

    [Fact]
    public void WhiteTotalAboveSevenExplodesAndRubyStillApplies()
    {
        var match = MatchSession.Create(new FixedRandomSource(0, 0, 0, 0, 0, 0, 0));
        Execute(match, "ai", "stop");
        for (var draw = 0; draw < 6; draw++) Execute(match, "human", "draw");

        var view = match.GetSnapshot("human");
        var human = view.Players.Single(player => player.Id == "human");
        Assert.Equal(MatchPhase.Evaluation, view.Phase);
        Assert.True(human.Exploded);
        Assert.Equal(8, human.WhiteTotal);
        Assert.Equal(2, human.Rubies); // Start ruby plus physical scoring-space ruby at index 9.
        Assert.Contains(match.GetLegalActions("human"), action => action.Id.EndsWith(":take-points", StringComparison.Ordinal));
        Assert.Contains(match.GetLegalActions("human"), action => action.Id.EndsWith(":take-coins", StringComparison.Ordinal));
    }

    [Fact]
    public void EvaluationFreezesScoringSpaceBeforeDieMovesDroplet()
    {
        var match = MatchSession.Create(new FixedRandomSource(5, 5));

        Execute(match, "human", "stop");
        Execute(match, "ai", "stop");

        var view = match.GetSnapshot("human");
        var human = view.Players.Single(player => player.Id == "human");
        Assert.Equal(MatchPhase.Shopping, view.Phase);
        Assert.Equal(1, human.DropletPosition);
        Assert.Equal(1, human.Coins);
        Assert.Equal(1, human.ScoringSpace.Position);
    }

    [Fact]
    public void ShoppingUsesStartPlayerOrderAndTwoDifferentColors()
    {
        var cheapShop = new[]
        {
            new ShopChipDefinition(TokenColor.Green, 1, 1, 3, 1),
            new ShopChipDefinition(TokenColor.Green, 2, 1, 3, 1),
            new ShopChipDefinition(TokenColor.Orange, 1, 1, 3, 1),
            new ShopChipDefinition(TokenColor.Blue, 1, 1, 3, 1)
        };
        var rules = new RuleSet(BoardTrack.Standard(), SetOneIngredients.Create(), cheapShop);
        var match = MatchSession.Create(new FixedRandomSource(0, 0), rules);
        Execute(match, "ai", "stop");
        Execute(match, "human", "draw");
        Execute(match, "human", "stop");

        Assert.Empty(match.GetLegalActions("ai"));
        Execute(match, "human", "buy:green:1");
        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.Color == TokenColor.Green);
        Execute(match, "human", "buy:orange:1");
        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.Kind == GameActionKind.BuyIngredient);
        Execute(match, "human", "finish-shopping");
        Assert.NotEmpty(match.GetLegalActions("ai"));
    }

    [Fact]
    public void ChoiceFromEarlierDecisionCannotApplyToLaterDecision()
    {
        var rules = RuleSet.SetOne(new[] { new ChainedChoiceEvent() });
        var match = MatchSession.Create(new FixedRandomSource(0), rules);
        var stale = match.GetLegalActions("human").Single();

        match.Execute("human", stale);

        Assert.Throws<InvalidOperationException>(() => match.Execute("human", stale));
        Assert.Single(match.GetLegalActions("human"));
    }

    [Fact]
    public void RoundNineHoldsDrawUntilOtherActivePlayerCommits()
    {
        var match = MatchSession.Create(new FixedRandomSource());
        AdvanceToRound(match, 9);
        var before = match.GetSnapshot("human").Players.Single(player => player.Id == "human");

        Execute(match, "human", "draw");

        var waiting = match.GetSnapshot("human");
        Assert.True(waiting.AwaitingSimultaneousDecision);
        Assert.Equal(before.Position, waiting.Players.Single(player => player.Id == "human").Position);
        Assert.Empty(match.GetLegalActions("human"));

        Execute(match, "ai", "stop");
        var revealed = match.GetSnapshot("human");
        Assert.False(revealed.AwaitingSimultaneousDecision);
        Assert.Single(revealed.Players.Single(player => player.Id == "human").PlacedChips);
    }

    [Fact]
    public void RoundSixAddsOneWhiteOneOnlyOnce()
    {
        var match = MatchSession.Create(new FixedRandomSource());

        AdvanceToRound(match, 6);

        var bag = match.GetSnapshot("human").OwnBag;
        Assert.Equal(10, bag.Count);
        Assert.Equal(5, bag.Count(chip => chip.Color == TokenColor.White && chip.Value == 1));
    }

    private static void AdvanceToRound(MatchSession match, int targetRound)
    {
        while (match.GetSnapshot("human").Round < targetRound)
        {
            Execute(match, "human", "stop");
            Execute(match, "ai", "stop");
            FinishCurrentPhase(match);
            Execute(match, "human", "next-round");
        }
    }

    private static void FinishCurrentPhase(MatchSession match)
    {
        while (match.GetSnapshot("human").Phase != MatchPhase.RoundComplete)
        {
            var acted = false;
            foreach (var playerId in new[] { "human", "ai" })
            {
                var actions = match.GetLegalActions(playerId);
                if (actions.Count == 0) continue;
                var action = actions.FirstOrDefault(candidate => candidate.Kind == GameActionKind.FinishShopping ||
                    candidate.Kind == GameActionKind.FinishRubySpending) ?? actions[0];
                match.Execute(playerId, action);
                acted = true;
                if (match.GetSnapshot("human").Phase == MatchPhase.RoundComplete) break;
            }
            Assert.True(acted, "The match reached a phase with no legal way to continue.");
        }
    }

    private static void Execute(MatchSession match, string playerId, string actionId)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => string.Equals(candidate.Id, actionId, StringComparison.Ordinal));
        match.Execute(playerId, action);
    }

    private static void AssertOffer(RuleSet rules, TokenColor color, int value, int price, int stock, int round)
    {
        var offer = rules.ShopChips.Single(chip => chip.Color == color && chip.Value == value);
        Assert.Equal(price, offer.Price);
        Assert.Equal(stock, offer.Stock);
        Assert.Equal(round, offer.AvailableFromRound);
    }

    private sealed class ChainedChoiceEvent : RoundEventRule
    {
        internal ChainedChoiceEvent() : base("test-chain", "Test chain", "Exercises stale choice rejection.") { }

        public override void OnRevealed(RoundEventContext context)
        {
            context.OfferChoice("human", "First", new RoundEventChoice("continue", "Continue", (next, playerId) =>
                next.OfferChoice(playerId, "Second", new RoundEventChoice("continue", "Continue again", (_, _) => { }))));
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
