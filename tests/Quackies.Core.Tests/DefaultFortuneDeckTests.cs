using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Quackies.Core.Rules.Fortunes;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class DefaultFortuneDeckTests
{
    [Fact]
    public void StandardRuleSetContainsAllTwentyFourDistinctFortunesInAtlasOrder()
    {
        var expected = new[]
        {
            "a-second-chance", "just-in-time", "well-stirred", "lucky-devil", "its-shining-extra-bright",
            "seasoned-perfectly", "less-is-more", "the-pot-is-full", "choose-wisely", "living-in-luxury",
            "an-opportunistic-moment", "roll-the-die", "pumpkin-patch-party", "you-only-get-to-choose-one",
            "strong-ingredient", "magic-potion", "a-good-start", "rat-infestation", "rats-are-your-friends",
            "charity", "the-pot-is-filling-up", "wheel-and-deal", "beginners-bonus", "schadenfreude"
        };

        Assert.Equal(expected, SetOneFortunes.CreateAll().Select(card => card.Id));
        Assert.Equal(expected, RuleSet.SetOne().RoundEvents.Select(card => card.Id));
        Assert.Equal(24, expected.Distinct(StringComparer.Ordinal).Count());
        Assert.Empty(RuleSet.SetOne(Array.Empty<IRoundEventRule>()).RoundEvents);
    }

    [Fact]
    public void StandardMatchDrawsNineFortunesWithoutReplacement()
    {
        var match = MatchSession.Create(new SeededRandomSource(20260909));
        var eventsByRound = new Dictionary<int, string>();

        for (var step = 0; step < 1000 && match.Phase != MatchPhase.Finished; step++)
        {
            var view = match.GetSnapshot("human");
            eventsByRound[view.Round] = view.EventId;
            var progressed = false;
            foreach (var playerId in new[] { "human", "ai" })
            {
                var action = ChooseProgressAction(match.GetLegalActions(playerId));
                if (action == null) continue;
                match.Execute(playerId, action);
                progressed = true;
                break;
            }
            Assert.True(progressed, $"Standard match stalled in {match.Phase} during round {match.Round}.");
        }

        Assert.Equal(MatchPhase.Finished, match.Phase);
        Assert.Equal(Enumerable.Range(1, 9), eventsByRound.Keys.OrderBy(round => round));
        Assert.All(eventsByRound.Values, eventId => Assert.False(string.IsNullOrWhiteSpace(eventId)));
        Assert.Equal(9, eventsByRound.Values.Distinct(StringComparer.Ordinal).Count());
    }

    private static GameAction? ChooseProgressAction(IReadOnlyList<GameAction> actions) =>
        actions.FirstOrDefault(action => action.Kind == GameActionKind.Choose) ??
        actions.FirstOrDefault(action => action.Kind == GameActionKind.Stop) ??
        actions.FirstOrDefault(action => action.Kind == GameActionKind.FinishShopping) ??
        actions.FirstOrDefault(action => action.Kind == GameActionKind.FinishRubySpending) ??
        actions.FirstOrDefault(action => action.Kind == GameActionKind.NextRound);
}
