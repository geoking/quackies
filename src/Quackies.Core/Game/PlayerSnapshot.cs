using Quackies.Core.Players;

namespace Quackies.Core.Game
{
    public sealed class PlayerSnapshot
    {
        public PlayerSnapshot(
            string playerId,
            int remainingBagCount,
            int victoryPoints,
            int rubies,
            int buyingPowerAvailableThisRound,
            CauldronSnapshot cauldron)
        {
            PlayerId = playerId;
            RemainingBagCount = remainingBagCount;
            VictoryPoints = victoryPoints;
            Rubies = rubies;
            BuyingPowerAvailableThisRound = buyingPowerAvailableThisRound;
            Cauldron = cauldron;
        }

        public string PlayerId { get; }

        public int RemainingBagCount { get; }

        public int VictoryPoints { get; }

        public int Rubies { get; }

        public int BuyingPowerAvailableThisRound { get; }

        public CauldronSnapshot Cauldron { get; }

        internal static PlayerSnapshot FromPlayer(PlayerState player)
        {
            return new PlayerSnapshot(
                player.PlayerId,
                player.Bag.Count,
                player.VictoryPoints,
                player.Rubies,
                player.BuyingPowerAvailableThisRound,
                CauldronSnapshot.FromCauldron(player.Cauldron));
        }
    }
}
