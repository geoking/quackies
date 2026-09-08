using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Core.Tokens;

namespace Quackies.Core.AI
{
    /// <summary>
    /// A modest, deterministic opponent. It reasons from known bag composition,
    /// never a shuffled order or the session's random generator. New rule sets can
    /// supply a different policy without changing the match or Unity presenter.
    /// </summary>
    public sealed class BalancedPolicy : IPlayerPolicy
    {
        public GameAction Choose(MatchView observation, IReadOnlyList<GameAction> legalActions)
        {
            if (observation == null) throw new ArgumentNullException(nameof(observation));
            if (legalActions == null || legalActions.Count == 0)
                throw new ArgumentException("A policy needs at least one legal action.", nameof(legalActions));

            var player = observation.Players.Single(p => p.Id == observation.ViewerId);
            var flask = legalActions.FirstOrDefault(a => a.Kind == GameActionKind.UseFlask);
            if (flask != null && player.WhiteTotal >= player.ExplosionThreshold - 2) return flask;

            var draw = legalActions.FirstOrDefault(a => a.Kind == GameActionKind.Draw);
            var stop = legalActions.FirstOrDefault(a => a.Kind == GameActionKind.Stop);
            if (draw != null && stop != null)
            {
                var dangerous = observation.OwnBag.Count(t => t.Color == TokenColor.White &&
                    player.WhiteTotal + t.Value > player.ExplosionThreshold);
                var risk = observation.OwnBag.Count == 0 ? 1 : dangerous / (double)observation.OwnBag.Count;
                // Willing to risk a little more while the pot still has little value.
                var tolerance = player.ScoringSpace.Points < 3 ? 0.40 : 0.26;
                return risk <= tolerance ? draw : stop;
            }

            var purchases = legalActions.Where(a => a.Kind == GameActionKind.BuyIngredient).ToArray();
            if (purchases.Length > 0)
                // Larger chips improve a draw without adding several weak chips
                // to the bag. Spending efficiently is a secondary preference.
                return purchases.OrderByDescending(a => IngredientValue(a.Color, a.Value))
                    .ThenBy(a => a.Cost).First();

            var refill = legalActions.FirstOrDefault(a => a.Kind == GameActionKind.RefillFlask);
            if (refill != null && !player.FlaskFull && observation.Round < 9) return refill;
            var improve = legalActions.FirstOrDefault(a => a.Kind == GameActionKind.MoveDroplet);
            if (improve != null && observation.Round < 9) return improve;
            var convert = legalActions.FirstOrDefault(a => a.Kind == GameActionKind.ConvertRubies);
            if (convert != null) return convert;

            var choices = legalActions.Where(a => a.Kind == GameActionKind.Choose).ToArray();
            if (choices.Length > 0)
                return choices.OrderByDescending(a => ChoiceValue(a, player, observation.Round)).First();

            return legalActions[0];
        }

        private static double ChoiceValue(GameAction action, PlayerView player, int round)
        {
            // An exploded pot earns either this round's points or spending
            // money. Investing early can repay over several later rounds.
            if (action.Id.EndsWith(":take-points", StringComparison.Ordinal))
                return round >= 7 || player.ScoringSpace.Coins < 4 ? 100 : 1;
            if (action.Id.EndsWith(":take-coins", StringComparison.Ordinal))
                return round < 7 && player.ScoringSpace.Coins >= 4 ? 100 : 1;
            if (!action.Color.HasValue) return 0;
            if (action.Color == TokenColor.White)
                return player.WhiteTotal + action.Value > player.ExplosionThreshold ? -100 : -action.Value;
            return IngredientValue(action.Color, action.Value);
        }

        private static double IngredientValue(TokenColor? color, int value)
        {
            var bonus = color == TokenColor.Blue ? 3.0 : color == TokenColor.Yellow ? 2.8 :
                color == TokenColor.Purple ? 2.5 : color == TokenColor.Black ? 2.2 :
                color == TokenColor.Red ? 1.5 : color == TokenColor.Green ? 1.2 : 0;
            return value * 1.6 + bonus;
        }
    }
}
