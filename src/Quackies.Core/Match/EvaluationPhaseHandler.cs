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
                for (var roll = 0; roll < _session.BonusDieRolls; roll++)
                    RollDie(player, DieRollReason.RoundBonus, addRewardChipToCurrentBag: false);
        }

        internal void RollDie(PlayerRoundState player, DieRollReason reason, bool addRewardChipToCurrentBag)
        {
            var face = _session.Random.NextInt(6);
            var rewardApplied = true;
            string description;
            switch (face)
            {
                case 0:
                case 1:
                    description = "1 victory point";
                    _session.AddLog(player.Id, $"{player.Name} rolled the die: {description}.");
                    _session.GainPoints(player, 1);
                    break;
                case 2:
                    description = "2 victory points";
                    _session.AddLog(player.Id, $"{player.Name} rolled the die: {description}.");
                    _session.GainPoints(player, 2);
                    break;
                case 3:
                    description = "1 ruby";
                    _session.AddLog(player.Id, $"{player.Name} rolled the die: {description}.");
                    _session.GainRubies(player, 1);
                    break;
                case 4:
                    description = "Orange 1 chip";
                    rewardApplied = _session.TryGiveSupplyChip(player, TokenColor.Orange, 1, addRewardChipToCurrentBag);
                    _session.AddLog(player.Id, rewardApplied
                        ? $"{player.Name} rolled the die: Orange 1 chip."
                        : $"{player.Name} rolled the die: Orange 1 chip, but the supply was empty.");
                    break;
                case 5:
                    description = "Advance droplet 1 space";
                    var previousDroplet = player.Droplet;
                    _session.AddLog(player.Id, $"{player.Name} rolled the die: advance droplet 1 space.");
                    _session.AdvanceDroplet(player, 1);
                    rewardApplied = player.Droplet > previousDroplet;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(face));
            }
            _session.RecordDieRoll(player, face, reason, rewardApplied, description);
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
                if (player.ScoringSpaceAtStop!.HasRuby) _session.GainRubies(player, 1);
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
                    _session.AddLog(player.Id, $"{player.Name} evaluated physical scoring space {scoring.Position}: {scoring.Points} point(s), {scoring.Coins} coin(s).");
                    continue;
                }

                if (_session.Round == 9)
                {
                    // Explosion still permits only one reward. With no future
                    // shopping, settle the better point outcome automatically.
                    var convertedCoins = scoring.Coins / 5;
                    var finalPoints = Math.Max(scoring.Points, convertedCoins);
                    player.Points += finalPoints;
                    player.Coins = 0;
                    player.RewardResolved = true;
                    _session.AddLog(player.Id, $"{player.Name}'s exploded final pot earned {finalPoints} victory point(s): the better of {scoring.Points} printed point(s) or {convertedCoins} point(s) from {scoring.Coins} coins at five per point.");
                    continue;
                }

                _session.Offer(player, "Exploded pot reward",
                    new ChoiceOption("take-points", $"Take {scoring.Points} victory point(s)", () =>
                    {
                        player.Points += scoring.Points;
                        player.Coins = 0;
                        player.RewardResolved = true;
                        _session.AddLog(player.Id, $"{player.Name} chose {scoring.Points} point(s) after exploding.");
                    }),
                    new ChoiceOption("take-coins", $"Take {scoring.Coins} coin(s) for shopping", () =>
                    {
                        player.Coins = scoring.Coins;
                        player.RewardResolved = true;
                        _session.AddLog(player.Id, $"{player.Name} chose {scoring.Coins} shopping coin(s) after exploding.");
                    }));
            }
        }
    }
}
