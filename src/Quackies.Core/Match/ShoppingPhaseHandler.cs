using System;
using System.Collections.Generic;
using System.Linq;

namespace Quackies.Core.Match
{
    internal sealed class ShoppingPhaseHandler
    {
        private readonly MatchSession _session;
        internal ShoppingPhaseHandler(MatchSession session) { _session = session; }

        internal IEnumerable<GameAction> GetLegalActions(PlayerRoundState player)
        {
            if (player.ShoppingDone) yield break;
            var currentShopper = _session.PlayersInStartOrder().First(candidate => !candidate.ShoppingDone);
            if (currentShopper != player) yield break;
            if (player.PurchaseCount < 2)
            {
                foreach (var chip in _session.Rules.ShopChips.Where(chip => chip.AvailableFromRound <= _session.Round &&
                    chip.Price <= player.Coins && _session.Remaining(chip) > 0 && !player.PurchasedColors.Contains(chip.Color)))
                    yield return new GameAction($"buy:{chip.Color.ToString().ToLowerInvariant()}:{chip.Value}",
                        GameActionKind.BuyIngredient, $"Buy {chip.Color} {chip.Value}", color: chip.Color, value: chip.Value, cost: chip.Price);
            }
            yield return new GameAction("finish-shopping", GameActionKind.FinishShopping, "Finish shopping");
        }

        internal void Execute(PlayerRoundState player, GameAction action)
        {
            if (action.Kind == GameActionKind.FinishShopping)
            {
                player.ShoppingDone = true;
                player.Coins = 0;
                _session.AddLog(player.Id, $"{player.Name} finished shopping.");
                _session.FinishShoppingIfReady();
                return;
            }
            if (action.Kind != GameActionKind.BuyIngredient || !action.Color.HasValue)
                throw new InvalidOperationException($"{action.Kind} is not a shopping action.");

            var definition = _session.Rules.ShopChips.Single(chip => chip.Color == action.Color.Value && chip.Value == action.Value);
            player.Coins -= definition.Price;
            player.Inventory.Add(new Tokens.Token(definition.Color, definition.Value));
            player.PurchasedColors.Add(definition.Color);
            player.PurchaseCount++;
            _session.TakeFromSupply(definition);
            _session.AddLog(player.Id, $"{player.Name} bought {definition.Color} {definition.Value} for {definition.Price} coin(s).");
        }
    }
}
