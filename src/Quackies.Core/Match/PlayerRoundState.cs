using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Tokens;

namespace Quackies.Core.Match
{
    /// <summary>Session-owned state. A chip remains in Inventory while in Bag, Pot or a temporary selection.</summary>
    internal sealed class PlayerRoundState
    {
        internal PlayerRoundState(string id, string name, int startingRubies = 1)
        {
            Id = id; Name = name;
            Inventory.AddRange(Enumerable.Range(0, 4).Select(_ => new Token(TokenColor.White, 1)));
            Inventory.AddRange(Enumerable.Range(0, 2).Select(_ => new Token(TokenColor.White, 2)));
            Inventory.Add(new Token(TokenColor.White, 3));
            Inventory.Add(new Token(TokenColor.Orange, 1));
            Inventory.Add(new Token(TokenColor.Green, 1));
            Rubies = startingRubies;
        }
        internal string Id { get; }
        internal string Name { get; }
        internal readonly List<Token> Inventory = new List<Token>();
        internal readonly List<Token> Bag = new List<Token>();
        internal readonly List<PlacedChip> Pot = new List<PlacedChip>();
        internal readonly Queue<PendingChoice> Choices = new Queue<PendingChoice>();
        internal readonly HashSet<TokenColor> PurchasedColors = new HashSet<TokenColor>();
        internal int Points, Coins, Droplet, RatPosition, WhiteTotal, PurchaseCount;
        internal int Rubies;
        internal bool FlaskFull = true;
        internal bool Stopped, Exploded, ShoppingDone, RubiesDone;
        internal bool MayUseFlask;
        internal bool RewardResolved;
        internal TrackSpaceView? ScoringSpaceAtStop;
        internal int ExplosionThreshold = 7;
        internal int TemporaryRatCount;
        internal int Position => Pot.Count == 0 ? Math.Max(Droplet, RatPosition) : Pot[Pot.Count - 1].Position;

        internal void ResetRound()
        {
            Pot.Clear(); Bag.Clear(); Bag.AddRange(Inventory); Choices.Clear(); PurchasedColors.Clear();
            Coins = WhiteTotal = PurchaseCount = TemporaryRatCount = 0;
            Stopped = Exploded = ShoppingDone = RubiesDone = MayUseFlask = RewardResolved = false;
            ScoringSpaceAtStop = null;
            ExplosionThreshold = 7; RatPosition = Droplet;
        }
        internal int Count(TokenColor color) => Pot.Count(c => c.Token.Color == color);
        internal int CountInLast(TokenColor color, int count) => Pot.Skip(Math.Max(0, Pot.Count - count)).Count(c => c.Token.Color == color);
    }
    internal sealed class PlacedChip
    {
        internal PlacedChip(Token token, int position) { Token = token; Position = position; }
        internal Token Token { get; }
        internal int Position { get; set; }
    }
    internal sealed class PendingChoice
    {
        internal PendingChoice(long sequence, string title, IEnumerable<ChoiceOption> options)
        { Sequence = sequence; Title = title; Options = options.ToList(); }
        internal long Sequence { get; }
        internal string Title { get; }
        internal IReadOnlyList<ChoiceOption> Options { get; }
    }
    internal sealed class ChoiceOption
    {
        internal ChoiceOption(string id, string label, Action apply, TokenColor? color = null, int value = 0,
            Func<bool>? isAvailable = null)
        { Id = id; Label = label; Apply = apply; Color = color; Value = value; IsAvailable = isAvailable ?? (() => true); }
        internal string Id { get; }
        internal string Label { get; }
        internal Action Apply { get; }
        internal Func<bool> IsAvailable { get; }
        internal TokenColor? Color { get; }
        internal int Value { get; }
    }
}
