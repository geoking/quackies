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

    [Fact]
    public void StrongIngredientPlacesAWhiteChipWithoutExplodingAboveTheThreshold()
    {
        var match = CreateMatch("strong-ingredient", new FixedRandom(0, 0, 0, 0, 0, 2, 0));
        ExecuteKind(match, "ai", GameActionKind.Stop);
        for (var draw = 0; draw < 5; draw++) ExecuteKind(match, "human", GameActionKind.Draw);
        Assert.Equal(7, Player(match, "human").WhiteTotal);
        ExecuteKind(match, "human", GameActionKind.Stop);

        ExecuteColor(match, "human", TokenColor.White, 2);
        ExecuteSuffix(match, "ai", ":fortune-none");

        var human = Player(match, "human");
        Assert.Equal(9, human.WhiteTotal);
        Assert.False(human.Exploded);
        Assert.Equal(6, human.PlacedChips.Count);
    }

    [Fact]
    public void StrongIngredientChoicesResolveSequentiallyInRoundStartOrder()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var strong = SetOneFortunes.CreateFinalBatch().Single(candidate => candidate.Id == "strong-ingredient");
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips,
            new IRoundEventRule[] { new NoOpEvent(), strong });
        var match = MatchSession.Create(new ZeroRandom(), rules);
        FinishRoundAndAdvance(match);

        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);

        Assert.Equal(2, match.Round);
        Assert.Contains(match.GetLegalActions("ai"), action => action.Id.EndsWith(":fortune-none", StringComparison.Ordinal));
        Assert.Empty(match.GetLegalActions("human"));
        ExecuteSuffix(match, "ai", ":fortune-none");
        Assert.Contains(match.GetLegalActions("human"), action => action.Id.EndsWith(":fortune-none", StringComparison.Ordinal));
    }

    [Fact]
    public void StrongIngredientSuppressesThePlacedChipsImmediateIngredientAbility()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var strong = SetOneFortunes.CreateFinalBatch().Single(candidate => candidate.Id == "strong-ingredient");
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips,
            new IRoundEventRule[] { new GiveHumanBlue(), strong });
        var random = new FixedRandom(0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 0, 0, 0, 0);
        var match = MatchSession.Create(random, rules);
        FinishRoundAndAdvance(match);
        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);
        ExecuteSuffix(match, "ai", ":fortune-none");

        ExecuteColor(match, "human", TokenColor.Blue, 1);

        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.ChoiceTitle.Contains("Crow skull", StringComparison.Ordinal));
        Assert.Single(Player(match, "human").PlacedChips);
    }

    [Fact]
    public void StrongIngredientChipContributesToDeferredColorEvaluation()
    {
        var match = CreateMatch("strong-ingredient", new FixedRandom(0, 8, 0, 0, 0, 0));
        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);
        ExecuteColor(match, "human", TokenColor.Green, 1);
        ExecuteSuffix(match, "ai", ":fortune-none");

        Assert.Equal(2, Player(match, "human").Rubies);
    }

    [Fact]
    public void StrongIngredientDoesNotStackAChipOnAnAlreadyFullPot()
    {
        var spaces = new[]
        {
            new TrackSpaceView(0, 0, 0, false),
            new TrackSpaceView(1, 1, 0, false),
            new TrackSpaceView(2, 35, 15, false)
        };
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var strong = SetOneFortunes.CreateFinalBatch().Single(candidate => candidate.Id == "strong-ingredient");
        var rules = new RuleSet(new BoardTrack(spaces), baseline.Ingredients.Values, baseline.ShopChips, new[] { strong });
        var match = MatchSession.Create(new ZeroRandom(), rules);

        ExecuteKind(match, "ai", GameActionKind.Stop);
        ExecuteKind(match, "human", GameActionKind.Draw);

        Assert.Empty(match.GetLegalActions("human"));
        Assert.Contains(match.GetLegalActions("ai"), action => action.Id.EndsWith(":fortune-none", StringComparison.Ordinal));
        ExecuteSuffix(match, "ai", ":fortune-none");
        var human = Player(match, "human");
        Assert.Single(human.PlacedChips);
        Assert.Equal(2, human.ScoringSpace.Position);
    }

    [Fact]
    public void StrongIngredientOffersNoFortuneSelectionWhenBothPotsExploded()
    {
        var match = CreateMatch("strong-ingredient", new ZeroRandom());
        for (var draw = 0; draw < 6; draw++) ExecuteKind(match, "human", GameActionKind.Draw);
        for (var draw = 0; draw < 6; draw++) ExecuteKind(match, "ai", GameActionKind.Draw);

        Assert.Equal(MatchPhase.Evaluation, match.Phase);
        Assert.All(match.GetSnapshot("human").Players, player => Assert.True(player.Exploded));
        Assert.All(new[] { "human", "ai" }, playerId =>
            Assert.DoesNotContain(match.GetLegalActions(playerId), action => action.ChoiceTitle == "Strong Ingredient"));
    }

    [Fact]
    public void SequentialFortuneSelectionSkipsPlayersWhoseBagsAreEmpty()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var rules = new RuleSet(baseline.Track, baseline.Ingredients.Values, baseline.ShopChips,
            new IRoundEventRule[] { new EmptyBagSelectionEvent() });
        var match = MatchSession.Create(new ZeroRandom(), rules);

        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "ai", GameActionKind.Stop);

        Assert.NotEqual(MatchPhase.Brewing, match.Phase);
        Assert.All(new[] { "human", "ai" }, playerId =>
            Assert.DoesNotContain(match.GetLegalActions(playerId), action => action.ChoiceTitle == "Empty bag selection"));
    }

    [Fact]
    public void BlueDrawThatFillsThePotDoesNotOfferAnotherPlacement()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var rules = new RuleSet(ShortTrack(lastChipPosition: 1), baseline.Ingredients.Values, baseline.ShopChips,
            new IRoundEventRule[] { new GiveHumanBlue(1) });
        var match = MatchSession.Create(new FixedRandom(0, 9, 0), rules);
        ExecuteKind(match, "ai", GameActionKind.Stop);

        ExecuteKind(match, "human", GameActionKind.Draw);

        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.ChoiceTitle.Contains("Crow skull", StringComparison.Ordinal));
        Assert.Single(Player(match, "human").PlacedChips);
        Assert.Equal(2, Player(match, "human").ScoringSpace.Position);
    }

    [Fact]
    public void BlueSelectedByBlueThatFillsThePotDoesNotNestAnotherSelection()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var rules = new RuleSet(ShortTrack(lastChipPosition: 2), baseline.Ingredients.Values, baseline.ShopChips,
            new IRoundEventRule[] { new GiveHumanBlue(2) });
        var match = MatchSession.Create(new FixedRandom(0, 9, 9, 0), rules);
        ExecuteKind(match, "ai", GameActionKind.Stop);
        ExecuteKind(match, "human", GameActionKind.Draw);

        ExecuteColor(match, "human", TokenColor.Blue, 1);

        Assert.DoesNotContain(match.GetLegalActions("human"), action => action.ChoiceTitle.Contains("Crow skull", StringComparison.Ordinal));
        Assert.Equal(2, Player(match, "human").PlacedChips.Count);
        Assert.Equal(3, Player(match, "human").ScoringSpace.Position);
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

    private static void ExecuteColor(MatchSession match, string playerId, TokenColor color, int value)
    {
        var action = match.GetLegalActions(playerId).First(candidate => candidate.Color == color && candidate.Value == value);
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

    private static BoardTrack ShortTrack(int lastChipPosition)
    {
        var spaces = Enumerable.Range(0, lastChipPosition + 2)
            .Select(position => new TrackSpaceView(position, position == lastChipPosition + 1 ? 35 : position,
                position == lastChipPosition + 1 ? 15 : 0, false));
        return new BoardTrack(spaces);
    }

    private sealed class GiveHumanBlue : RoundEventRule
    {
        private readonly int _count;
        internal GiveHumanBlue(int count = 1) : base("give-human-blue", "Give blue", "Test setup.") => _count = count;
        public override void OnRevealed(RoundEventContext context)
        {
            for (var index = 0; index < _count; index++) context.TryGiveChip("human", TokenColor.Blue, 1);
        }
    }

    private sealed class NoOpEvent : RoundEventRule
    {
        internal NoOpEvent() : base("no-op", "No event", "Test setup.") { }
    }

    private sealed class EmptyBagSelectionEvent : RoundEventRule
    {
        internal EmptyBagSelectionEvent() : base("empty-bag-selection", "Empty bag selection", "Test setup.") { }
        public override void OnRevealed(RoundEventContext context)
        {
            foreach (var playerId in context.PlayerIds)
            {
                for (var count = 0; count < 4; count++) context.RemoveFromBag(playerId, TokenColor.White, 1);
                for (var count = 0; count < 2; count++) context.RemoveFromBag(playerId, TokenColor.White, 2);
                context.RemoveFromBag(playerId, TokenColor.White, 3);
                context.RemoveFromBag(playerId, TokenColor.Orange, 1);
                context.RemoveFromBag(playerId, TokenColor.Green, 1);
            }
        }
        public override void OnPlayerStopped(RoundEventContext context, string playerId)
        {
            if (context.AllPlayersFinishedBrewing && context.TryUseOnce(Id))
                context.OfferSequentialFortuneBagSelections(context.PlayerIdsInStartOrder, 5, Title);
        }
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
