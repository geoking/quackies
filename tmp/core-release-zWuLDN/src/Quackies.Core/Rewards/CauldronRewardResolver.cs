using System;
using Quackies.Core.Cauldrons;

namespace Quackies.Core.Rewards
{
    public static class CauldronRewardResolver
    {
        public static EndRoundRewardResult Resolve(CauldronState cauldron, CauldronTrack track)
        {
            if (cauldron == null)
            {
                throw new ArgumentNullException(nameof(cauldron));
            }

            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            var rewardSpace = track.GetRewardSpace(cauldron.CurrentPosition);

            return new EndRoundRewardResult(rewardSpace, cauldron.HasExploded);
        }
    }
}
