using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Rules;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class IssuedActionTests
{
    [Fact]
    public void ADrawCannotBeReplayedEvenWhileAnotherDrawIsLegal()
    {
        var match = Create();
        var draw = Draw(match, "human");
        match.Execute("human", draw);
        Assert.Contains(match.GetLegalActions("human"), a => a.Kind == GameActionKind.Draw);
        Assert.Throws<InvalidOperationException>(() => match.Execute("human", draw));
        Assert.Single(match.GetSnapshot("human").Players.Single(p => p.Id == "human").PlacedChips);
    }

    [Fact]
    public void AnIssuedActionCannotMoveAnotherPlayerOrAnotherSession()
    {
        var match = Create();
        var other = Create();
        var draw = Draw(match, "human");
        Assert.Throws<InvalidOperationException>(() => match.Execute("ai", draw));
        Assert.Throws<InvalidOperationException>(() => other.Execute("human", draw));
        Assert.All(match.GetSnapshot("human").Players, p => Assert.Empty(p.PlacedChips));
        Assert.All(other.GetSnapshot("human").Players, p => Assert.Empty(p.PlacedChips));
        match.Execute("human", draw); // Rejections must not consume its authorization.
    }

    [Fact]
    public void IndependentOpponentActionDoesNotConsumeMyStillLegalAction()
    {
        var match = Create();
        var humanDraw = Draw(match, "human");
        var aiDraw = Draw(match, "ai");
        match.Execute("ai", aiDraw);
        match.Execute("human", humanDraw);
        Assert.All(match.GetSnapshot("human").Players, p => Assert.Single(p.PlacedChips));
    }

    [Fact]
    public void OldStopCannotBeUsedAfterADrawOrInANewRound()
    {
        var match = Create();
        var oldStop = match.GetLegalActions("human").Single(a => a.Kind == GameActionKind.Stop);
        match.Execute("human", Draw(match, "human"));
        Assert.Throws<InvalidOperationException>(() => match.Execute("human", oldStop));
        foreach (var id in new[] { "human", "ai" })
            match.Execute(id, match.GetLegalActions(id).Single(a => a.Kind == GameActionKind.Stop));
        while (match.Phase != MatchPhase.RoundComplete)
        {
            foreach (var id in new[] { "human", "ai" })
            {
                var finish = match.GetLegalActions(id).FirstOrDefault(a =>
                    a.Kind == GameActionKind.FinishShopping || a.Kind == GameActionKind.FinishRubySpending);
                if (finish != null) match.Execute(id, finish);
            }
        }
        match.Execute("ai", match.GetLegalActions("ai").Single());
        Assert.Equal(2, match.Round);
        Assert.Throws<InvalidOperationException>(() => match.Execute("human", oldStop));
    }

    private static MatchSession Create() => MatchSession.Create(new ZeroRandom(), RuleSet.SetOne(Array.Empty<IRoundEventRule>()));
    private static GameAction Draw(MatchSession match, string player) => match.GetLegalActions(player).Single(a => a.Kind == GameActionKind.Draw);
    private sealed class ZeroRandom : IRandomSource { public int NextInt(int exclusiveMax) => 0; }
}
