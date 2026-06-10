using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quackies.Core.Cauldrons
{
    public sealed class CauldronTrack
    {
        private readonly List<CauldronSpace> _spaces;

        public CauldronTrack(IEnumerable<CauldronSpace> spaces)
        {
            if (spaces == null)
            {
                throw new ArgumentNullException(nameof(spaces));
            }

            _spaces = new List<CauldronSpace>();

            foreach (var space in spaces)
            {
                if (space == null)
                {
                    throw new ArgumentException("Cauldron track cannot contain null spaces.", nameof(spaces));
                }

                _spaces.Add(space);
            }

            _spaces.Sort((left, right) => left.Position.CompareTo(right.Position));

            if (_spaces.Count == 0)
            {
                throw new ArgumentException("Cauldron track must contain at least one space.", nameof(spaces));
            }

            for (var i = 0; i < _spaces.Count; i++)
            {
                if (i > 0 && _spaces[i].Position == _spaces[i - 1].Position)
                {
                    throw new ArgumentException("Cauldron track cannot contain duplicate positions.", nameof(spaces));
                }
            }
        }

        public IReadOnlyList<CauldronSpace> Spaces
        {
            get { return new ReadOnlyCollection<CauldronSpace>(_spaces); }
        }

        public CauldronSpace GetRewardSpace(int position)
        {
            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative.");
            }

            CauldronSpace? bestSpace = null;

            foreach (var space in _spaces)
            {
                if (space.Position > position)
                {
                    break;
                }

                bestSpace = space;
            }

            return bestSpace ?? _spaces[0];
        }
    }
}
