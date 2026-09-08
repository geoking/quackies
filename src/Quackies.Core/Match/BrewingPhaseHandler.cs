using System;
using System.Collections.Generic;
using Quackies.Core.Rules;
using Quackies.Core.Tokens;

namespace Quackies.Core.Match
{
    internal sealed class BrewingPhaseHandler
    {
        private readonly MatchSession _session;

        internal BrewingPhaseHandler(MatchSession session) { _session = session; }

        internal IEnumerable<GameAction> GetLegalActions(PlayerRoundState player)
        {
            if (player.Stopped || player.Exploded) yield break;
            if (player.Bag.Count > 0 && player.Position < _session.Rules.Track.LastChipPosition)
                yield return new GameAction("draw", GameActionKind.Draw, "Draw a chip");
            yield return new GameAction("stop", GameActionKind.Stop, "Stop brewing");
            if (player.FlaskFull && player.MayUseFlask && player.Pot.Count > 0 &&
                player.Pot[player.Pot.Count - 1].Token.Color == TokenColor.White)
                yield return new GameAction("use-flask", GameActionKind.UseFlask, "Use flask");
        }

        internal void Execute(PlayerRoundState player, GameAction action)
        {
            switch (action.Kind)
            {
                case GameActionKind.Draw:
                    player.MayUseFlask = false;
                    var index = _session.Random.NextInt(player.Bag.Count);
                    var chip = player.Bag[index];
                    player.Bag.RemoveAt(index);
                    PlaceChip(player, chip, resolveIngredient: true, mayExplode: true, ChipPlacementSource.BagDraw);
                    break;
                case GameActionKind.Stop:
                    player.MayUseFlask = false;
                    player.Stopped = true;
                    _session.AddLog($"{player.Name} stopped at physical position {player.Position}.");
                    _session.NotifyPlayerStopped(player);
                    break;
                case GameActionKind.UseFlask:
                    UseFlask(player);
                    break;
                default:
                    throw new InvalidOperationException($"{action.Kind} is not a brewing action.");
            }
        }

        internal void PlaceChip(PlayerRoundState player, Token chip, bool resolveIngredient, bool mayExplode,
            ChipPlacementSource source = ChipPlacementSource.IngredientSelection)
        {
            var position = Math.Min(player.Position + chip.Value, _session.Rules.Track.LastChipPosition);
            player.Pot.Add(new PlacedChip(chip, position));
            player.MayUseFlask = chip.Color == TokenColor.White;
            if (chip.Color == TokenColor.White) player.WhiteTotal += chip.Value;
            _session.AddLog($"{player.Name} placed {chip} at physical position {position}.");

            if (mayExplode && player.WhiteTotal > player.ExplosionThreshold)
            {
                player.Exploded = true;
                player.Stopped = true;
                player.MayUseFlask = false;
                _session.AddLog($"{player.Name}'s pot exploded with a white total of {player.WhiteTotal}.");
                _session.NotifyPlayerStopped(player);
            }
            if (resolveIngredient) _session.Rules.Ingredients[chip.Color].OnPlaced(new IngredientContext(_session, player), chip);
            _session.NotifyChipPlaced(player, chip, source);
            if (player.Position >= _session.Rules.Track.LastChipPosition && !player.Stopped)
            {
                player.Stopped = true;
                _session.NotifyPlayerStopped(player);
            }
        }

        private static void UseFlask(PlayerRoundState player)
        {
            var last = player.Pot[player.Pot.Count - 1];
            player.Pot.RemoveAt(player.Pot.Count - 1);
            player.Bag.Add(last.Token);
            player.WhiteTotal -= last.Token.Value;
            player.FlaskFull = false;
            player.MayUseFlask = false;
        }
    }
}
