using System;
using System.Collections.Generic;
using Quackies.Core.Match;

namespace Quackies.Core.Rules
{
    /// <summary>Standard front-side board. Indices are physical spaces, never the printed coin values.</summary>
    public sealed class BoardTrack
    {
        private readonly IReadOnlyList<TrackSpaceView> _spaces;
        public BoardTrack(IEnumerable<TrackSpaceView> spaces)
        {
            _spaces = MatchView.Freeze(spaces);
            if (_spaces.Count < 2) throw new ArgumentException("A board needs a droplet space and a scoring space.", nameof(spaces));
            for (var i = 0; i < _spaces.Count; i++)
                if (_spaces[i].Position != i) throw new ArgumentException("Track positions must be contiguous from zero.", nameof(spaces));
        }
        public IReadOnlyList<TrackSpaceView> Spaces => _spaces;
        public int LastChipPosition => _spaces.Count - 2;
        public TrackSpaceView ScoringSpace(int lastChipPosition) => _spaces[Math.Min(lastChipPosition + 1, _spaces.Count - 1)];
        public TrackSpaceView At(int physicalPosition) => _spaces[physicalPosition];

        public static BoardTrack Standard()
        {
            // Transcribed from Assets/Art/raw/cauldron/blue/cauldron.png and checked against the publisher rules.
            var coins = new[] {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,15,16,16,17,17,18,18,19,19,20,20,21,21,22,22,23,23,24,24,25,25,26,26,27,27,28,28,29,29,30,30,31,31,32,32,33,33,35};
            var points = new[] {0,0,0,0,0,0,1,1,1,1,2,2,2,2,3,3,3,3,4,4,4,4,5,5,5,5,6,6,6,7,7,7,8,8,8,9,9,9,10,10,10,11,11,11,12,12,12,12,13,13,13,14,14,15};
            var rubies = new HashSet<int> {5,9,13,16,20,24,28,30,34,36,40,42,46,50,52};
            var spaces = new List<TrackSpaceView>();
            for (var i = 0; i < coins.Length; i++) spaces.Add(new TrackSpaceView(i, coins[i], points[i], rubies.Contains(i)));
            return new BoardTrack(spaces);
        }
    }
}
