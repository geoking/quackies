using Quackies.Core.AI;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Quackies.Core.Rules.Fortunes;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class NormalPolicyTests
{
    [Fact]
    public void SavesFlaskWhenTheNextDrawIsAlreadySafe()
    {
        var match = Create();
        Draw(match, 1);
        Assert.Equal(GameActionKind.Draw, Choose(match).Kind);
    }

    [Fact]
    public void StopsInsteadOfSpendingFlaskWithLessThanHalfTheBagEarly()
    {
        var match = Create();
        Draw(match, 5); // Four white 1s and a white 2; four of nine chips remain.
        Assert.Equal(GameActionKind.Stop, Choose(match).Kind);
    }

    [Fact]
    public void UsesFlaskToContinueSafelyWhenMostOfTheBagRemains()
    {
        var match = Create(new SequenceRandom(6, 4, 4)); // White 3, 2, 2.
        Draw(match, 3);
        Assert.Equal(GameActionKind.UseFlask, Choose(match).Kind);
    }

    [Fact]
    public void ValuesUsingTheFlaskMoreInLaterRounds()
    {
        var match = Create();
        while (match.Round < 4)
        {
            var actions = match.GetLegalActions("human");
            if (actions.Count > 0) match.Execute("human", Passive(actions));
            actions = match.GetLegalActions("ai");
            if (actions.Count > 0) match.Execute("ai", Passive(actions));
        }
        Draw(match, 5);
        Assert.Equal(GameActionKind.UseFlask, Choose(match).Kind);
    }

    [Fact]
    public void DoesNotWasteFlaskWhenItCannotMakeTheNextDrawSafe()
    {
        var match = Create(new SequenceRandom(6, 4, 0, 0)); // White 3, 2, 1, 1; a 2 remains.
        Draw(match, 4);
        Assert.Equal(GameActionKind.Stop, Choose(match).Kind);
    }

    [Fact]
    public void NeverExplodesAndFinishesAcrossEveryImplementedFortune()
    {
        var cards = ImplementedFortunes();
        foreach (var card in cards)
        for (var seed = 0; seed < 8; seed++)
        {
            var match = MatchSession.Create(new SeededRandomSource(seed), RuleSet.SetOne(new[] { card }));
            var policy = new NormalPolicy();
            for (var step = 0; step < 2000 && match.Phase != MatchPhase.Finished; step++)
            {
                var acted = false;
                foreach (var id in new[] { "human", "ai" })
                {
                    var legal = match.GetLegalActions(id);
                    if (legal.Count == 0) continue;
                    match.Execute(id, policy.Choose(match.GetSnapshot(id), legal));
                    Assert.All(match.GetSnapshot(id).Players, p =>
                        Assert.False(p.Exploded, $"Normal exploded: {card.Id}, seed {seed}, round {match.Round}."));
                    acted = true;
                }
                Assert.True(acted, $"Match stalled: {card.Id}, seed {seed}.");
            }
            Assert.Equal(MatchPhase.Finished, match.Phase);
            Assert.Equal(9, match.Round);
        }
    }

    [Fact]
    public void FinishesMixedFortuneMatchesWithoutExplosionsOrRepeatedCards()
    {
        for (var seed = 0; seed < 32; seed++)
        {
            var match = MatchSession.Create(new SeededRandomSource(seed), RuleSet.SetOne(ImplementedFortunes()));
            var policy = new NormalPolicy();
            var titles = new Dictionary<int, string>();
            for (var step = 0; step < 2000 && match.Phase != MatchPhase.Finished; step++)
            {
                var view = match.GetSnapshot("human");
                titles[view.Round] = view.EventTitle;
                var acted = false;
                foreach (var id in new[] { "human", "ai" })
                {
                    var legal = match.GetLegalActions(id);
                    if (legal.Count == 0) continue;
                    match.Execute(id, policy.Choose(match.GetSnapshot(id), legal));
                    Assert.All(match.GetSnapshot(id).Players, player =>
                        Assert.False(player.Exploded, $"Normal exploded in mixed deck: seed {seed}, round {match.Round}."));
                    acted = true;
                }
                Assert.True(acted, $"Mixed deck stalled: seed {seed}, round {match.Round}.");
            }
            Assert.Equal(MatchPhase.Finished, match.Phase);
            Assert.Equal(9, match.Round);
            Assert.Equal(9, titles.Count);
            Assert.All(titles.Values, title => Assert.False(string.IsNullOrWhiteSpace(title)));
            Assert.Equal(9, titles.Values.Distinct().Count());
        }
    }

    private static IEnumerable<IRoundEventRule> ImplementedFortunes() =>
        SetOneFortunes.CreatePreparationBatch()
            .Concat(SetOneFortunes.CreateOngoingBatch())
            .Concat(SetOneFortunes.CreateRatBatch())
            .Concat(SetOneFortunes.CreateInteractiveBatch())
            .Concat(SetOneFortunes.CreateFinalBatch());

    private static MatchSession Create(IRandomSource? random = null) =>
        MatchSession.Create(random ?? new SequenceRandom(), RuleSet.SetOne(Array.Empty<IRoundEventRule>()));

    private static GameAction Choose(MatchSession match) =>
        new NormalPolicy().Choose(match.GetSnapshot("human"), match.GetLegalActions("human"));

    private static void Draw(MatchSession match, int count)
    {
        for (var i = 0; i < count; i++)
            match.Execute("human", match.GetLegalActions("human").Single(a => a.Kind == GameActionKind.Draw));
    }

    private static GameAction Passive(IReadOnlyList<GameAction> actions) => actions.FirstOrDefault(a =>
        a.Kind == GameActionKind.Stop || a.Kind == GameActionKind.FinishShopping ||
        a.Kind == GameActionKind.FinishRubySpending || a.Kind == GameActionKind.NextRound) ?? actions[0];

    private sealed class SequenceRandom : IRandomSource
    {
        private readonly Queue<int> values;
        public SequenceRandom(params int[] sequence) { values = new Queue<int>(sequence); }
        public int NextInt(int exclusiveMax) => values.Count == 0 ? 0 : values.Dequeue();
    }
}
