using System;
using System.Collections.Generic;
using Quackies.Core.Game;
using Quackies.Core.Randomness;
using Quackies.Core.Rewards;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class GameFacadeTests
{
    [Fact]
    public void CreateSinglePlayerCreatesValidGame()
    {
        var game = QuackiesGame.CreateSinglePlayer(new FixedRandomSource(0));
        var snapshot = game.GetSnapshot();

        Assert.Equal(GamePhase.Drawing, snapshot.Phase);
        Assert.Equal("Player 1", snapshot.CurrentPlayer.PlayerId);
        Assert.Equal(8, snapshot.CurrentPlayer.RemainingBagCount);
        Assert.Equal(0, snapshot.CurrentPlayer.Cauldron.CurrentPosition);
        Assert.NotNull(snapshot.CurrentRewardSpace);
    }

    [Fact]
    public void DrawTokenChangesCauldronState()
    {
        var game = QuackiesGame.CreateSinglePlayer(new FixedRandomSource(0));

        var result = game.DrawToken();
        var snapshot = game.GetSnapshot();

        Assert.Equal(1, result.NewCauldronPosition);
        Assert.Equal(1, snapshot.CurrentPlayer.Cauldron.CurrentPosition);
        Assert.Single(snapshot.CurrentPlayer.Cauldron.PlacedTokens);
    }

    [Fact]
    public void StopRoundResolvesRewards()
    {
        var game = QuackiesGame.CreateSinglePlayer(new FixedRandomSource(0));
        game.DrawToken();

        var snapshot = game.StopRound();

        Assert.Equal(GamePhase.RoundEnded, snapshot.Phase);
        Assert.NotNull(snapshot.PendingReward);
        Assert.NotNull(snapshot.AppliedReward);
        Assert.Equal(snapshot.PendingReward!.BuyingPower, snapshot.AppliedReward!.AppliedBuyingPower);
        Assert.Equal(snapshot.PendingReward.VictoryPoints, snapshot.AppliedReward.AppliedVictoryPoints);
    }

    [Fact]
    public void ExplodedPlayerEntersAwaitingExplosionRewardChoice()
    {
        var game = QuackiesGame.CreateSinglePlayer(new FixedRandomSource(0, 0, 0, 0, 0, 0));

        while (game.GetSnapshot().Phase == GamePhase.Drawing)
        {
            game.DrawToken();
        }

        var snapshot = game.GetSnapshot();

        Assert.Equal(GamePhase.AwaitingExplosionRewardChoice, snapshot.Phase);
        Assert.True(snapshot.CurrentPlayer.Cauldron.HasExploded);
        Assert.NotNull(snapshot.PendingReward);
        Assert.True(snapshot.PendingReward!.PlayerExploded);
        Assert.Null(snapshot.AppliedReward);
    }

    [Fact]
    public void ApplyExplodedRewardChoiceEndsRoundAndAppliesChoice()
    {
        var game = QuackiesGame.CreateSinglePlayer(new FixedRandomSource(0, 0, 0, 0, 0, 0));

        while (game.GetSnapshot().Phase == GamePhase.Drawing)
        {
            game.DrawToken();
        }

        var awaitingChoice = game.GetSnapshot();
        var ended = game.ApplyExplodedRewardChoice(ExplodedRewardChoice.TakeBuyingPower);

        Assert.Equal(GamePhase.RoundEnded, ended.Phase);
        Assert.NotNull(awaitingChoice.PendingReward);
        Assert.NotNull(ended.AppliedReward);
        Assert.Equal(awaitingChoice.PendingReward!.BuyingPower, ended.AppliedReward!.AppliedBuyingPower);
        Assert.Equal(0, ended.AppliedReward.AppliedVictoryPoints);
    }

    [Fact]
    public void StartNextRoundResetsRoundStateAndKeepsPersistentRewards()
    {
        var game = QuackiesGame.CreateSinglePlayer(new FixedRandomSource(0));
        game.DrawToken();
        game.StopRound();

        var nextRound = game.StartNextRound();

        Assert.Equal(GamePhase.Drawing, nextRound.Phase);
        Assert.Equal(0, nextRound.CurrentPlayer.Cauldron.CurrentPosition);
        Assert.Empty(nextRound.CurrentPlayer.Cauldron.PlacedTokens);
        Assert.Equal(8, nextRound.CurrentPlayer.RemainingBagCount);
        Assert.Equal(0, nextRound.CurrentPlayer.BuyingPowerAvailableThisRound);
    }

    private sealed class FixedRandomSource : IRandomSource
    {
        private readonly Queue<int> _indexes;

        public FixedRandomSource(params int[] indexes)
        {
            _indexes = new Queue<int>(indexes);
        }

        public int NextInt(int exclusiveMax)
        {
            if (exclusiveMax <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exclusiveMax));
            }

            if (_indexes.Count == 0)
            {
                return 0;
            }

            var index = _indexes.Dequeue();

            if (index < 0 || index >= exclusiveMax)
            {
                throw new InvalidOperationException("Fixed random index was outside the requested range.");
            }

            return index;
        }
    }
}
