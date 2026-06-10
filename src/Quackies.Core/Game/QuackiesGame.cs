using System;
using Quackies.Core.Bags;
using Quackies.Core.Cauldrons;
using Quackies.Core.Players;
using Quackies.Core.Randomness;
using Quackies.Core.Rewards;
using Quackies.Core.Rounds;

namespace Quackies.Core.Game
{
    public sealed class QuackiesGame
    {
        private readonly IRandomSource _random;
        private EndRoundRewardResult? _pendingReward;
        private AppliedEndRoundRewardResult? _lastAppliedReward;
        private string? _roundEndReason;

        private QuackiesGame(PlayerState currentPlayer, IRandomSource random, CauldronTrack cauldronTrack)
        {
            CurrentPlayer = currentPlayer ?? throw new ArgumentNullException(nameof(currentPlayer));
            _random = random ?? throw new ArgumentNullException(nameof(random));
            CauldronTrack = cauldronTrack ?? throw new ArgumentNullException(nameof(cauldronTrack));
            Phase = GamePhase.Drawing;
        }

        public GamePhase Phase { get; private set; }

        public PlayerState CurrentPlayer { get; }

        public CauldronTrack CauldronTrack { get; }

        public static QuackiesGame CreateSinglePlayer(IRandomSource random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return new QuackiesGame(
                new PlayerState(DefaultBagFactory.CreateStartingBag()),
                random,
                DefaultCauldronTrackFactory.CreatePrototypeTrack());
        }

        public DrawResult DrawToken()
        {
            EnsurePhase(GamePhase.Drawing, "draw a token");
            ClearRoundResolution();

            var result = CurrentPlayer.DrawToken(_random);

            if (result.HasExploded)
            {
                ResolveRound("Round ended because the cauldron exploded.");
            }
            else if (CurrentPlayer.Bag.IsEmpty)
            {
                CurrentPlayer.StopRound();
                ResolveRound("Round ended because the bag is empty.");
            }

            return result;
        }

        public GameSnapshot StopRound()
        {
            EnsurePhase(GamePhase.Drawing, "stop the round");

            CurrentPlayer.StopRound();
            ResolveRound("Round stopped.");

            return GetSnapshot();
        }

        public GameSnapshot ApplyExplodedRewardChoice(ExplodedRewardChoice choice)
        {
            EnsurePhase(GamePhase.AwaitingExplosionRewardChoice, "apply an exploded reward choice");

            if (_pendingReward == null)
            {
                throw new InvalidOperationException("There is no pending exploded reward to apply.");
            }

            _lastAppliedReward = CurrentPlayer.ApplyEndRoundReward(_pendingReward, choice);
            Phase = GamePhase.RoundEnded;

            return GetSnapshot();
        }

        public GameSnapshot StartNextRound()
        {
            EnsurePhase(GamePhase.RoundEnded, "start the next round");

            CurrentPlayer.StartNextRound(DefaultBagFactory.CreateStartingBag());
            ClearRoundResolution();
            Phase = GamePhase.Drawing;

            return GetSnapshot();
        }

        public GameSnapshot GetSnapshot()
        {
            var rewardSpace = CauldronTrack.GetRewardSpace(CurrentPlayer.Cauldron.CurrentPosition);

            return new GameSnapshot(
                Phase,
                PlayerSnapshot.FromPlayer(CurrentPlayer),
                RewardSpaceSnapshot.FromRewardSpace(rewardSpace),
                _pendingReward == null ? null : EndRoundRewardSnapshot.FromReward(_pendingReward),
                _lastAppliedReward == null ? null : AppliedEndRoundRewardSnapshot.FromAppliedReward(_lastAppliedReward),
                _roundEndReason);
        }

        private void ResolveRound(string reason)
        {
            _pendingReward = CauldronRewardResolver.Resolve(CurrentPlayer.Cauldron, CauldronTrack);
            _roundEndReason = reason;

            if (_pendingReward.PlayerExploded)
            {
                Phase = GamePhase.AwaitingExplosionRewardChoice;
                return;
            }

            _lastAppliedReward = CurrentPlayer.ApplyEndRoundReward(_pendingReward);
            Phase = GamePhase.RoundEnded;
        }

        private void ClearRoundResolution()
        {
            _pendingReward = null;
            _lastAppliedReward = null;
            _roundEndReason = null;
        }

        private void EnsurePhase(GamePhase requiredPhase, string action)
        {
            if (Phase != requiredPhase)
            {
                throw new InvalidOperationException($"Cannot {action} while the game phase is {Phase}.");
            }
        }
    }
}
