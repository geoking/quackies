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

public sealed class MatchHistoryTests
{
    [Fact]
    public void HistoryAttributesDrawFlaskStopPurchaseDieAndIngredientEffects()
    {
        var baseline = RuleSet.SetOne(Array.Empty<IRoundEventRule>());
        var shop = new[] { new ShopChipDefinition(TokenColor.Orange, 1, 1, 2, 1) };
        var rules = new RuleSet(baseline.Track, SetOneIngredients.Create(), shop);
        var match = MatchSession.Create(new FixedRandom(0, 7, 0), rules);

        ExecuteKind(match, "human", GameActionKind.Draw);
        ExecuteKind(match, "human", GameActionKind.UseFlask);
        ExecuteKind(match, "human", GameActionKind.Draw);
        ExecuteKind(match, "ai", GameActionKind.Stop);
        ExecuteKind(match, "human", GameActionKind.Stop);
        ExecuteKind(match, "human", GameActionKind.BuyIngredient);

        var history = match.GetSnapshot("human").History;
        AssertEntry(history, 1, "human", "Human drew and placed White 1");
        AssertEntry(history, 1, "human", "Human used their flask");
        AssertEntry(history, 1, "human", "Human drew and placed Green 1");
        AssertEntry(history, 1, "ai", "AI stopped at physical position");
        AssertEntry(history, 1, "human", "Human stopped at physical position");
        AssertEntry(history, 1, "human", "Human rolled the die:");
        AssertEntry(history, 1, "human", "Human gained 1 ruby/rubies.");
        AssertEntry(history, 1, "human", "Human bought Orange 1 for 1 coin(s).");
    }

    [Fact]
    public void HistoryIsAFullDetachedImmutableRecordBeyondEightyEntries()
    {
        var match = MatchSession.Create(new ZeroRandom());
        var openingSnapshot = match.GetSnapshot("human");

        PlayCompleteMatchByStoppingImmediately(match);

        var completed = match.GetSnapshot("human");
        Assert.True(completed.History.Count > 80, $"Expected more than 80 entries, but found {completed.History.Count}.");
        Assert.Single(openingSnapshot.History);
        Assert.Equal("Round 1 begins. Human is start player.", completed.History[0].Message);
        Assert.Equal(Enumerable.Range(1, 9), completed.History.Select(entry => entry.Round).Distinct());
        Assert.All(completed.History, entry =>
        {
            Assert.InRange(entry.Round, 1, 9);
            Assert.True(string.IsNullOrEmpty(entry.ActorId) || entry.ActorId == "human" || entry.ActorId == "ai",
                $"Unexpected actor '{entry.ActorId}'.");
            Assert.False(string.IsNullOrWhiteSpace(entry.Message));
        });
        Assert.Contains(completed.History, entry => entry.ActorId.Length == 0 && entry.Message == "The match is complete.");
        Assert.Equal(completed.History.TakeLast(16).Select(entry => entry.Message), completed.RecentLog);
        Assert.Throws<NotSupportedException>(() =>
            ((IList<MatchLogEntry>)completed.History).Add(completed.History[0]));
    }

    private static void PlayCompleteMatchByStoppingImmediately(MatchSession match)
    {
        while (match.GetSnapshot("human").Phase != MatchPhase.Finished)
        {
            var progressed = false;
            foreach (var playerId in new[] { "human", "ai" })
            {
                var actions = match.GetLegalActions(playerId);
                var action = actions.FirstOrDefault(candidate =>
                    candidate.Kind == GameActionKind.Stop ||
                    candidate.Kind == GameActionKind.FinishShopping ||
                    candidate.Kind == GameActionKind.FinishRubySpending ||
                    candidate.Kind == GameActionKind.NextRound);
                if (action == null) continue;
                match.Execute(playerId, action);
                progressed = true;
                break;
            }
            Assert.True(progressed, "The match reached a phase with no legal way to continue.");
        }
    }

    private static void ExecuteKind(MatchSession match, string playerId, GameActionKind kind)
    {
        var action = match.GetLegalActions(playerId).Single(candidate => candidate.Kind == kind);
        match.Execute(playerId, action);
    }

    private static void AssertEntry(IEnumerable<MatchLogEntry> history, int round, string actorId, string messageFragment)
    {
        Assert.Contains(history, entry => entry.Round == round &&
            string.Equals(entry.ActorId, actorId, StringComparison.Ordinal) &&
            entry.Message.Contains(messageFragment, StringComparison.Ordinal));
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
