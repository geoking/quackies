using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Quackies.Core.Tokens;

namespace Quackies.Core.Match
{
    public enum MatchPhase { Preparation, Brewing, Evaluation, Shopping, RubySpending, RoundComplete, Finished }

    /// <summary>Detached immutable observation. Only OwnBag contains private information, for ViewerId.</summary>
    public sealed class MatchView
    {
        internal MatchView(int round, MatchPhase phase, string viewerId, string eventId, string eventTitle, string eventDescription,
            IEnumerable<PlayerView> players, IEnumerable<Token> ownBag, IEnumerable<Token> startingBag, IEnumerable<ShopOffer> shop,
            IEnumerable<MatchLogEntry> history, IEnumerable<string> winnerIds, bool awaitingSimultaneousDecision)
        {
            Round = round; Phase = phase; ViewerId = viewerId; EventId = eventId; EventTitle = eventTitle; EventDescription = eventDescription;
            Players = Freeze(players); OwnBag = Freeze(ownBag); StartingBag = Freeze(startingBag); ShopOffers = Freeze(shop);
            History = Freeze(history); RecentLog = Freeze(History.TakeLast(16).Select(entry => entry.Message)); WinnerIds = Freeze(winnerIds);
            AwaitingSimultaneousDecision = awaitingSimultaneousDecision;
        }
        public int Round { get; }
        public MatchPhase Phase { get; }
        public string ViewerId { get; }
        public string EventId { get; }
        public string EventTitle { get; }
        public string EventDescription { get; }
        public IReadOnlyList<PlayerView> Players { get; }
        public IReadOnlyList<Token> OwnBag { get; }
        public IReadOnlyList<Token> StartingBag { get; }
        public IReadOnlyList<ShopOffer> ShopOffers { get; }
        public IReadOnlyList<MatchLogEntry> History { get; }
        public IReadOnlyList<string> RecentLog { get; }
        public IReadOnlyList<string> WinnerIds { get; }
        public bool AwaitingSimultaneousDecision { get; }
        internal static IReadOnlyList<T> Freeze<T>(IEnumerable<T> values) => new ReadOnlyCollection<T>(values.ToList());
    }

    public sealed class PlayerView
    {
        internal PlayerView(string id, string name, int victoryPoints, int rubies, int coins, int whiteTotal, int explosionThreshold,
            int position, int dropletPosition, int ratPosition, int bagCount, int inventoryCount, bool flaskFull, bool stopped, bool exploded,
            IEnumerable<PlacedChipView> placed, TrackSpaceView scoringSpace)
        {
            Id = id; Name = name; VictoryPoints = victoryPoints; Rubies = rubies; Coins = coins; WhiteTotal = whiteTotal;
            ExplosionThreshold = explosionThreshold; Position = position; DropletPosition = dropletPosition; RatPosition = ratPosition;
            BagCount = bagCount; InventoryCount = inventoryCount; FlaskFull = flaskFull; Stopped = stopped; Exploded = exploded;
            PlacedChips = MatchView.Freeze(placed); ScoringSpace = scoringSpace;
        }
        public string Id { get; }
        public string Name { get; }
        public int VictoryPoints { get; }
        public int Rubies { get; }
        public int Coins { get; }
        public int WhiteTotal { get; }
        public int ExplosionThreshold { get; }
        public int Position { get; }
        public int DropletPosition { get; }
        public int RatPosition { get; }
        public int BagCount { get; }
        public int InventoryCount { get; }
        public bool FlaskFull { get; }
        public bool Stopped { get; }
        public bool Exploded { get; }
        public IReadOnlyList<PlacedChipView> PlacedChips { get; }
        public TrackSpaceView ScoringSpace { get; }
    }

    public sealed class PlacedChipView
    {
        internal PlacedChipView(TokenColor color, int value, int position) { Color = color; Value = value; Position = position; }
        public TokenColor Color { get; }
        public int Value { get; }
        public int Position { get; }
    }
    public sealed class TrackSpaceView
    {
        public TrackSpaceView(int position, int coins, int points, bool hasRuby) { Position = position; Coins = coins; Points = points; HasRuby = hasRuby; }
        public int Position { get; }
        public int Coins { get; }
        public int Points { get; }
        public bool HasRuby { get; }
    }
    public sealed class ShopOffer
    {
        internal ShopOffer(TokenColor color, int value, int cost, int remaining, string effect)
        { Color = color; Value = value; Cost = cost; Remaining = remaining; Effect = effect; }
        public TokenColor Color { get; }
        public int Value { get; }
        public int Cost { get; }
        public int Remaining { get; }
        public string Effect { get; }
    }
}
