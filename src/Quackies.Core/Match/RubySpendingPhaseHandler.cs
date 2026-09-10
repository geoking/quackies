using System;
using System.Collections.Generic;

namespace Quackies.Core.Match
{
    internal sealed class RubySpendingPhaseHandler
    {
        private readonly MatchSession _session;
        internal RubySpendingPhaseHandler(MatchSession session) { _session = session; }

        internal IEnumerable<GameAction> GetLegalActions(PlayerRoundState player)
        {
            if (player.RubiesDone) yield break;
            if (player.Rubies >= 2)
            {
                if (_session.Round == 9)
                    yield return new GameAction("convert-rubies", GameActionKind.ConvertRubies, "Convert 2 rubies to 1 point");
                else
                {
                    yield return new GameAction("move-droplet", GameActionKind.MoveDroplet, "Spend 2 rubies to move droplet");
                    if (!player.FlaskFull)
                        yield return new GameAction("refill-flask", GameActionKind.RefillFlask, "Spend 2 rubies to refill flask");
                }
            }
            yield return new GameAction("finish-ruby-spending", GameActionKind.FinishRubySpending, "Finish ruby spending");
        }

        internal void Execute(PlayerRoundState player, GameAction action)
        {
            switch (action.Kind)
            {
                case GameActionKind.MoveDroplet:
                    player.Rubies -= 2;
                    _session.AdvanceDroplet(player, 1);
                    _session.AddLog(player.Id, $"{player.Name} spent 2 rubies to move their droplet.");
                    break;
                case GameActionKind.RefillFlask:
                    player.Rubies -= 2;
                    player.FlaskFull = true;
                    _session.AddLog(player.Id, $"{player.Name} spent 2 rubies to refill their flask.");
                    break;
                case GameActionKind.ConvertRubies:
                    player.Rubies -= 2;
                    player.Points++;
                    _session.AddLog(player.Id, $"{player.Name} converted 2 rubies to 1 victory point.");
                    break;
                case GameActionKind.FinishRubySpending:
                    player.RubiesDone = true;
                    _session.AddLog(player.Id, $"{player.Name} finished ruby spending.");
                    _session.FinishRubySpendingIfReady();
                    break;
                default:
                    throw new InvalidOperationException($"{action.Kind} is not a ruby-spending action.");
            }
        }
    }
}
