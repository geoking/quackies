using Quackies.Core.AI;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class BalancedPolicyTests
{
    [Fact]
    public void CompleteMatchesAreLegalAndReproducibleAcrossSeeds()
    {
        for (var seed = 0; seed < 64; seed++)
            Assert.Equal(PlayMatch(seed), PlayMatch(seed));
    }

    [Fact]
    public void EarlyExplosionChoosesMoneyForFutureRounds()
    {
        var match = MatchSession.Create(new ZeroRandom(), WithoutFortunes());
        match.Execute("ai", match.GetLegalActions("ai").Single(a => a.Kind == GameActionKind.Stop));
        for (var draw = 0; draw < 6; draw++)
            match.Execute("human", match.GetLegalActions("human").Single(a => a.Kind == GameActionKind.Draw));

        var action = new BalancedPolicy().Choose(match.GetSnapshot("human"), match.GetLegalActions("human"));

        Assert.EndsWith(":take-coins", action.Id);
    }

    private static string PlayMatch(int seed)
    {
        var match = MatchSession.Create(new SeededRandomSource(seed), WithoutFortunes());
        var policy = new BalancedPolicy();
        for (var step = 0; step < 2000; step++)
        {
            var acted = false;
            foreach (var playerId in new[] { "human", "ai" })
            {
                var view = match.GetSnapshot(playerId);
                if (view.Phase == MatchPhase.Finished)
                {
                    Assert.Equal(9, view.Round);
                    Assert.NotEmpty(view.WinnerIds);
                    Assert.All(view.Players, player => Assert.True(player.VictoryPoints >= 0 && player.Rubies >= 0));
                    return string.Join(";", view.Players.Select(p => $"{p.Id}:{p.VictoryPoints}:{p.Position}:{p.InventoryCount}"));
                }

                var legal = match.GetLegalActions(playerId);
                if (legal.Count == 0) continue;
                var action = policy.Choose(view, legal);
                Assert.Contains(action, legal);
                match.Execute(playerId, action);
                acted = true;
            }
            Assert.True(acted, $"Seed {seed} stalled in {match.Phase} during round {match.Round}.");
        }
        throw new InvalidOperationException($"Seed {seed} exceeded the complete-match action bound.");
    }

    private static RuleSet WithoutFortunes() => RuleSet.SetOne(Array.Empty<IRoundEventRule>());

    private sealed class ZeroRandom : IRandomSource
    {
        public int NextInt(int exclusiveMax) => 0;
    }
}
