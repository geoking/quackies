using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Tokens;

namespace Quackies.Core.Match
{
    /// <summary>Session-owned snapshot and one-time decision state for restarting one player's brewing round.</summary>
    internal sealed class BrewingRestartState
    {
        private readonly IReadOnlyList<Token> _bag;
        private readonly IReadOnlyList<PlacedChip> _pot;
        private readonly int _whiteTotal;
        private readonly bool _flaskFull;
        private readonly bool _stopped;
        private readonly bool _exploded;
        private readonly bool _mayUseFlask;
        private readonly int _droplet;
        private readonly int _ratPosition;
        private readonly int _temporaryRatCount;
        private readonly int _explosionThreshold;

        internal BrewingRestartState(PlayerRoundState player)
        {
            _bag = player.Bag.ToList();
            _pot = player.Pot.Select(Clone).ToList();
            _whiteTotal = player.WhiteTotal;
            _flaskFull = player.FlaskFull;
            _stopped = player.Stopped;
            _exploded = player.Exploded;
            _mayUseFlask = player.MayUseFlask;
            _droplet = player.Droplet;
            _ratPosition = player.RatPosition;
            _temporaryRatCount = player.TemporaryRatCount;
            _explosionThreshold = player.ExplosionThreshold;
        }

        internal int PlacementCount { get; private set; }
        internal bool Pending { get; private set; }
        internal bool Used { get; private set; }
        internal string ChoiceTitle { get; private set; } = string.Empty;
        internal void RecordPlacement(int requiredPlacements, string title)
        {
            if (requiredPlacements < 1) throw new ArgumentOutOfRangeException(nameof(requiredPlacements));
            PlacementCount++;
            if (Used || Pending || PlacementCount != requiredPlacements) return;
            Pending = true;
            ChoiceTitle = title ?? throw new ArgumentNullException(nameof(title));
        }

        internal void MarkOffered()
        {
            if (!Pending || Used) throw new InvalidOperationException("The brewing restart is not awaiting an offer.");
            Pending = false;
            Used = true;
        }

        internal void Restore(PlayerRoundState player)
        {
            player.Bag.Clear();
            player.Bag.AddRange(_bag);
            player.Pot.Clear();
            player.Pot.AddRange(_pot.Select(Clone));
            player.WhiteTotal = _whiteTotal;
            player.FlaskFull = _flaskFull;
            player.Stopped = _stopped;
            player.Exploded = _exploded;
            player.MayUseFlask = _mayUseFlask;
            player.Droplet = _droplet;
            player.RatPosition = _ratPosition;
            player.TemporaryRatCount = _temporaryRatCount;
            player.ExplosionThreshold = _explosionThreshold;
            PlacementCount = 0;
            Pending = false;
            player.Choices.Clear();
        }

        private static PlacedChip Clone(PlacedChip chip) =>
            new PlacedChip(chip.Token, chip.Position, chip.IngredientEffectEnabled);
    }
}
