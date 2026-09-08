using System;
using System.Collections.Generic;
using Quackies.Core.Bags;
using Quackies.Core.Cauldrons;
using Quackies.Core.Randomness;
using Quackies.Core.Rewards;
using Quackies.Core.Rounds;
using Quackies.Core.Tokens;

namespace Quackies.Core.Players
{
    public sealed class PlayerState
    {
        public PlayerState(Bag bag)
            : this("Player 1", bag, new CauldronState())
        {
        }

        public PlayerState(string playerId, Bag bag)
            : this(playerId, bag, new CauldronState())
        {
        }

        public PlayerState(string playerId, Bag bag, CauldronState cauldron)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                throw new ArgumentException("Player id is required.", nameof(playerId));
            }

            if (bag == null)
            {
                throw new ArgumentNullException(nameof(bag));
            }

            if (cauldron == null)
            {
                throw new ArgumentNullException(nameof(cauldron));
            }

            PlayerId = playerId;
            Bag = bag;
            Cauldron = cauldron;
        }

        public string PlayerId { get; }

        public Bag Bag { get; private set; }

        public CauldronState Cauldron { get; private set; }

        public int VictoryPoints { get; private set; }

        public int Rubies { get; private set; }

        public int BuyingPowerAvailableThisRound { get; private set; }

        public DrawResult DrawToken(IRandomSource random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (Cauldron.HasExploded)
            {
                throw new InvalidOperationException("Cannot draw another token after the player has exploded.");
            }

            if (Cauldron.IsStopped)
            {
                throw new InvalidOperationException("Cannot draw another token after the player has stopped.");
            }

            var token = Bag.DrawToken(random);
            var placedToken = Cauldron.PlaceToken(token);
            var events = new List<DrawEvent>
            {
                new DrawEvent(DrawEventType.TokenDrawn, $"Drew {token}.")
            };

            if (token.Color == TokenColor.White)
            {
                events.Add(new DrawEvent(DrawEventType.WhiteChipTotalChanged, $"White chip total is now {Cauldron.WhiteChipTotal}."));
            }

            if (Cauldron.HasExploded)
            {
                events.Add(new DrawEvent(DrawEventType.Explosion, "The cauldron exploded."));
            }

            return new DrawResult(
                token,
                placedToken,
                Cauldron.CurrentPosition,
                Cauldron.WhiteChipTotal,
                Cauldron.HasExploded,
                Bag.Count,
                events);
        }

        public void StopRound()
        {
            Cauldron.Stop();
        }

        public AppliedEndRoundRewardResult ApplyEndRoundReward(
            EndRoundRewardResult reward,
            ExplodedRewardChoice? explodedRewardChoice = null)
        {
            if (reward == null)
            {
                throw new ArgumentNullException(nameof(reward));
            }

            var appliedVictoryPoints = 0;
            var appliedBuyingPower = 0;
            var appliedRubies = reward.HasRuby ? 1 : 0;

            if (reward.PlayerExploded)
            {
                if (!explodedRewardChoice.HasValue)
                {
                    throw new InvalidOperationException("Exploded players must choose victory points or buying power.");
                }

                if (explodedRewardChoice.Value == ExplodedRewardChoice.TakeVictoryPoints)
                {
                    appliedVictoryPoints = reward.VictoryPoints;
                }
                else
                {
                    appliedBuyingPower = reward.BuyingPower;
                }
            }
            else
            {
                appliedVictoryPoints = reward.VictoryPoints;
                appliedBuyingPower = reward.BuyingPower;
            }

            VictoryPoints += appliedVictoryPoints;
            BuyingPowerAvailableThisRound += appliedBuyingPower;
            Rubies += appliedRubies;

            return new AppliedEndRoundRewardResult(
                appliedVictoryPoints,
                appliedBuyingPower,
                appliedRubies,
                explodedRewardChoice);
        }

        public void StartNextRound(Bag bag)
        {
            if (bag == null)
            {
                throw new ArgumentNullException(nameof(bag));
            }

            Bag = bag;
            Cauldron = new CauldronState();
            BuyingPowerAvailableThisRound = 0;
        }
    }
}
