using System;
using System.Collections.Generic;
using Quackies.Core.Bags;
using Quackies.Core.Cauldrons;
using Quackies.Core.Players;
using Quackies.Core.Randomness;
using Quackies.Core.Tokens;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class DrawLoopTests
{
    [Fact]
    public void PlayerStateOwnsCauldronState()
    {
        var player = new PlayerState(DefaultBagFactory.CreateStartingBag());

        Assert.NotNull(player.Cauldron);
        Assert.IsType<CauldronState>(player.Cauldron);
    }

    [Fact]
    public void DrawingRemovesExactlyOneTokenFromBag()
    {
        var bag = new Bag(new[]
        {
            new Token(TokenColor.White, 1),
            new Token(TokenColor.Orange, 1)
        });

        var drawn = bag.DrawToken(new FixedRandomSource(0));

        Assert.Equal("White 1", drawn.ToString());
        Assert.Equal(1, bag.Count);
    }

    [Fact]
    public void CauldronStateTracksCurrentPosition()
    {
        var cauldron = new CauldronState();

        cauldron.PlaceToken(new Token(TokenColor.Orange, 2));
        cauldron.PlaceToken(new Token(TokenColor.Green, 3));

        Assert.Equal(5, cauldron.CurrentPosition);
    }

    [Fact]
    public void CauldronStateTracksPlacedTokens()
    {
        var cauldron = new CauldronState();

        cauldron.PlaceToken(new Token(TokenColor.Orange, 1));
        cauldron.PlaceToken(new Token(TokenColor.Green, 2));

        Assert.Equal(2, cauldron.PlacedTokens.Count);
        Assert.Equal(1, cauldron.PlacedTokens[0].DrawIndex);
        Assert.Equal(2, cauldron.PlacedTokens[1].DrawIndex);
        Assert.Equal(3, cauldron.LastPlacedToken?.Position);
    }

    [Fact]
    public void DrawingAdvancesCauldronPositionByTokenValue()
    {
        var player = new PlayerState(new Bag(new[]
        {
            new Token(TokenColor.White, 2)
        }));

        var result = player.DrawToken(new FixedRandomSource(0));

        Assert.Equal(2, result.NewCauldronPosition);
        Assert.Equal(2, player.Cauldron.CurrentPosition);
    }

    [Fact]
    public void WhiteChipsIncreaseWhiteChipTotal()
    {
        var player = new PlayerState(new Bag(new[]
        {
            new Token(TokenColor.White, 2)
        }));

        var result = player.DrawToken(new FixedRandomSource(0));

        Assert.Equal(2, result.WhiteChipTotal);
        Assert.Equal(2, player.Cauldron.WhiteChipTotal);
    }

    [Fact]
    public void NonWhiteChipsDoNotIncreaseWhiteChipTotal()
    {
        var player = new PlayerState(new Bag(new[]
        {
            new Token(TokenColor.Orange, 1)
        }));

        var result = player.DrawToken(new FixedRandomSource(0));

        Assert.Equal(0, result.WhiteChipTotal);
        Assert.Equal(0, player.Cauldron.WhiteChipTotal);
    }

    [Fact]
    public void WhiteChipTotalIncreasesOnlyForWhiteTokens()
    {
        var player = new PlayerState(new Bag(new[]
        {
            new Token(TokenColor.White, 2),
            new Token(TokenColor.Orange, 1)
        }));
        var random = new FixedRandomSource(0, 0);

        player.DrawToken(random);
        player.DrawToken(random);

        Assert.Equal(2, player.Cauldron.WhiteChipTotal);
    }

    [Fact]
    public void WhiteChipTotalGreaterThanSevenCausesExplosion()
    {
        var player = new PlayerState(new Bag(new[]
        {
            new Token(TokenColor.White, 3),
            new Token(TokenColor.White, 3),
            new Token(TokenColor.White, 2)
        }));
        var random = new FixedRandomSource(0, 0, 0);

        player.DrawToken(random);
        player.DrawToken(random);
        var result = player.DrawToken(random);

        Assert.Equal(8, result.WhiteChipTotal);
        Assert.True(result.HasExploded);
        Assert.True(player.Cauldron.HasExploded);
    }

    [Fact]
    public void DrawingFromEmptyBagFailsSafely()
    {
        var bag = new Bag(Array.Empty<Token>());

        var ex = Assert.Throws<InvalidOperationException>(() => bag.DrawToken(new FixedRandomSource(0)));
        Assert.Contains("empty bag", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SeededRandomnessIsRepeatable()
    {
        var firstDraws = DrawAllTokens(DefaultBagFactory.CreateStartingBag(), new SeededRandomSource(12345));
        var secondDraws = DrawAllTokens(DefaultBagFactory.CreateStartingBag(), new SeededRandomSource(12345));

        Assert.Equal(firstDraws, secondDraws);
    }

    private static IReadOnlyList<string> DrawAllTokens(Bag bag, IRandomSource random)
    {
        var draws = new List<string>();

        while (!bag.IsEmpty)
        {
            draws.Add(bag.DrawToken(random).ToString());
        }

        return draws;
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
