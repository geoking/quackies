using System;
using Quackies.Core.Bags;
using Quackies.Core.Cauldrons;
using Quackies.Core.Players;
using Quackies.Core.Rewards;
using Quackies.Core.Tokens;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class RewardResolutionTests
{
    [Fact]
    public void RewardResolverReturnsKnownTrackSpaceResult()
    {
        var cauldron = new CauldronState();
        cauldron.PlaceToken(new Token(TokenColor.Orange, 3));
        var track = new CauldronTrack(new[]
        {
            new CauldronSpace(0, 0, 0, false),
            new CauldronSpace(3, 8, 2, true)
        });

        var reward = CauldronRewardResolver.Resolve(cauldron, track);

        Assert.Equal(3, reward.Position);
        Assert.Equal(8, reward.BuyingPower);
        Assert.Equal(2, reward.VictoryPoints);
        Assert.True(reward.HasRuby);
        Assert.False(reward.PlayerExploded);
        Assert.True(reward.MayTakeBothVictoryPointsAndBuyingPower);
    }

    [Fact]
    public void DefaultTrackPositionFifteenGivesKnownPrototypeReward()
    {
        var reward = ResolveDefaultTrackAtPosition(15);

        Assert.Equal(15, reward.Position);
        Assert.Equal(15, reward.BuyingPower);
        Assert.Equal(3, reward.VictoryPoints);
        Assert.True(reward.HasRuby);
    }

    [Fact]
    public void DefaultTrackPositionNineteenGivesKnownPrototypeReward()
    {
        var reward = ResolveDefaultTrackAtPosition(19);

        Assert.Equal(19, reward.Position);
        Assert.Equal(19, reward.BuyingPower);
        Assert.Equal(5, reward.VictoryPoints);
    }

    [Fact]
    public void DefaultTrackPositionThirtyThreeOrBeyondGivesKnownPrototypeReward()
    {
        var rewardAtThirtyThree = ResolveDefaultTrackAtPosition(33);
        var rewardBeyondThirtyThree = ResolveDefaultTrackAtPosition(34);

        Assert.Equal(35, rewardAtThirtyThree.BuyingPower);
        Assert.Equal(15, rewardAtThirtyThree.VictoryPoints);
        Assert.Equal(35, rewardBeyondThirtyThree.BuyingPower);
        Assert.Equal(15, rewardBeyondThirtyThree.VictoryPoints);
    }

    [Fact]
    public void NonExplodedPlayerReceivesBothVictoryPointsAndBuyingPower()
    {
        var player = CreatePlayerAtPosition(15, exploded: false);
        var reward = CauldronRewardResolver.Resolve(player.Cauldron, DefaultCauldronTrackFactory.CreatePrototypeTrack());

        var appliedReward = player.ApplyEndRoundReward(reward);

        Assert.Equal(3, appliedReward.AppliedVictoryPoints);
        Assert.Equal(15, appliedReward.AppliedBuyingPower);
        Assert.Equal(1, appliedReward.AppliedRubies);
        Assert.Equal(3, player.VictoryPoints);
        Assert.Equal(15, player.BuyingPowerAvailableThisRound);
        Assert.Equal(1, player.Rubies);
    }

    [Fact]
    public void ExplodedPlayerMustChooseBetweenVictoryPointsAndBuyingPower()
    {
        var player = CreatePlayerAtPosition(8, exploded: true);
        var track = new CauldronTrack(new[]
        {
            new CauldronSpace(0, 0, 0, false),
            new CauldronSpace(8, 8, 4, false)
        });
        var reward = CauldronRewardResolver.Resolve(player.Cauldron, track);

        Assert.True(reward.MustChooseVictoryPointsOrBuyingPower);
        Assert.Throws<InvalidOperationException>(() => player.ApplyEndRoundReward(reward));

        var appliedReward = player.ApplyEndRoundReward(reward, ExplodedRewardChoice.TakeVictoryPoints);

        Assert.Equal(4, appliedReward.AppliedVictoryPoints);
        Assert.Equal(0, appliedReward.AppliedBuyingPower);
        Assert.Equal(4, player.VictoryPoints);
        Assert.Equal(0, player.BuyingPowerAvailableThisRound);
    }

    [Fact]
    public void ExplodedPlayerCanChooseBuyingPowerInsteadOfVictoryPoints()
    {
        var player = CreatePlayerAtPosition(8, exploded: true);
        var track = new CauldronTrack(new[]
        {
            new CauldronSpace(0, 0, 0, false),
            new CauldronSpace(8, 8, 4, false)
        });
        var reward = CauldronRewardResolver.Resolve(player.Cauldron, track);

        var appliedReward = player.ApplyEndRoundReward(reward, ExplodedRewardChoice.TakeBuyingPower);

        Assert.Equal(0, appliedReward.AppliedVictoryPoints);
        Assert.Equal(8, appliedReward.AppliedBuyingPower);
        Assert.Equal(0, player.VictoryPoints);
        Assert.Equal(8, player.BuyingPowerAvailableThisRound);
    }

    [Fact]
    public void RubyIsAwardedFromRewardSpaceEvenIfPlayerExploded()
    {
        var player = CreatePlayerAtPosition(8, exploded: true);
        var track = new CauldronTrack(new[]
        {
            new CauldronSpace(0, 0, 0, false),
            new CauldronSpace(8, 8, 4, true)
        });
        var reward = CauldronRewardResolver.Resolve(player.Cauldron, track);

        var appliedReward = player.ApplyEndRoundReward(reward, ExplodedRewardChoice.TakeBuyingPower);

        Assert.True(reward.HasRuby);
        Assert.Equal(1, appliedReward.AppliedRubies);
        Assert.Equal(1, player.Rubies);
    }

    private static EndRoundRewardResult ResolveDefaultTrackAtPosition(int position)
    {
        var cauldron = new CauldronState();
        cauldron.PlaceToken(new Token(TokenColor.Orange, position));

        return CauldronRewardResolver.Resolve(cauldron, DefaultCauldronTrackFactory.CreatePrototypeTrack());
    }

    private static PlayerState CreatePlayerAtPosition(int position, bool exploded)
    {
        var player = new PlayerState(new Bag(new[] { new Token(TokenColor.Orange, 1) }));

        if (exploded)
        {
            player.Cauldron.PlaceToken(new Token(TokenColor.White, position));
        }
        else
        {
            player.Cauldron.PlaceToken(new Token(TokenColor.Orange, position));
        }

        return player;
    }
}
