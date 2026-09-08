using System;
using System.Linq;
using Quackies.Core.Rules;
using Quackies.Core.Tokens;

namespace Quackies.Core.Match
{
    internal sealed class EvaluationPhaseHandler
    {
        private readonly MatchSession _session;
        private bool _eventCompletionApplied;
        internal EvaluationPhaseHandler(MatchSession session) { _session = session; }

        internal void Begin()
        {
            _eventCompletionApplied = false;
            _session.NotifyEvaluationStarted();
            foreach (var player in _session.Players)
                player.ScoringSpaceAtStop = _session.Rules.Track.ScoringSpace(player.Position);
            ApplyBonusDie();
            ApplyIngredientEvaluation();
            ApplyScoringRubies();
            ResolvePointsAndCoins();
            FinishIfReady();
        }

        internal void FinishIfReady()
        {
            if (_session.Players.Any(player => player.Choices.Count > 0 || !player.RewardResolved)) return;
            if (!_eventCompletionApplied)
            {
                _eventCompletionApplied = true;
                _session.NotifyEvaluationComplete();
                if (_session.Players.Any(player => player.Choices.Count > 0)) return;
            }
            _session.EnterShopping();
        }

        private void ApplyBonusDie()
        {
            var eligible = _session.Players.Where(player => !player.Exploded).ToArray();
            if (eligible.Length == 0) return;
            var furthestScoringPosition = eligible.Max(player => player.ScoringSpaceAtStop!.Position);
            foreach (var player in eligible.Where(candidate => candidate.ScoringSpaceAtStop!.Position == furthestScoringPosition))
                ApplyDieFace(player, _session.Random.NextInt(6));
        }

        private void ApplyDieFace(PlayerRoundState player, int face)
        {
            switch (face)
            {
                case 0:
                case 1: player.Points += 1; break;
                case 2: player.Points += 2; break;
                case 3: player.Rubies += 1; break;
                case 4: _session.TryGiveSupplyChip(player, TokenColor.Orange, 1); break;
                case 5: _session.AdvanceDroplet(player, 1); break;
                default: throw new ArgumentOutOfRangeException(nameof(face));
            }
        }

        private void ApplyIngredientEvaluation()
        {
            // The printed evaluation banners resolve black, green, then purple.
            var order = new[] { TokenColor.Black, TokenColor.Green, TokenColor.Purple };
            foreach (var color in order)
                foreach (var player in _session.PlayersInStartOrder())
                    _session.Rules.Ingredients[color].OnEvaluation(new IngredientContext(_session, player));
        }

        private void ApplyScoringRubies()
        {
            foreach (var player in _session.Players)
                if (player.ScoringSpaceAtStop!.HasRuby) player.Rubies++;
        }

        private void ResolvePointsAndCoins()
        {
            foreach (var player in _session.Players)
            {
                var scoring = player.ScoringSpaceAtStop!;
                if (!player.Exploded)
                {
                    player.Points += scoring.Points;
                    player.Coins = scoring.Coins;
                    player.RewardResolved = true;
                    continue;
                }

                _session.Offer(player, "Exploded pot reward",
                    new ChoiceOption("take-points", $"Take {scoring.Points} victory point(s)", () =>
                    {
                        player.Points += scoring.Points;
                        player.Coins = 0;
                        player.RewardResolved = true;
                    }),
                    new ChoiceOption("take-coins", $"Take {scoring.Coins} coin(s) for shopping", () =>
                    {
                        player.Coins = scoring.Coins;
                        player.RewardResolved = true;
                    }));
            }
        }
    }
}
