using System.Collections.Generic;

namespace Quackies.Core.Cauldrons
{
    public static class DefaultCauldronTrackFactory
    {
        public static CauldronTrack CreatePrototypeTrack()
        {
            var spaces = new List<CauldronSpace>();

            // TODO: Replace this prototype table with the verified Quackies track data.
            // Until then, unverified spaces use simple placeholder values so game logic can
            // depend on CauldronTrack rather than hard-coded reward rules.
            for (var position = 0; position <= 33; position++)
            {
                spaces.Add(CreatePlaceholderSpace(position));
            }

            Replace(spaces, new CauldronSpace(position: 15, buyingPower: 15, victoryPoints: 3, hasRuby: true));
            Replace(spaces, new CauldronSpace(position: 19, buyingPower: 19, victoryPoints: 5, hasRuby: false));
            Replace(spaces, new CauldronSpace(position: 23, buyingPower: 23, victoryPoints: 7, hasRuby: false));
            Replace(spaces, new CauldronSpace(position: 33, buyingPower: 35, victoryPoints: 15, hasRuby: false));

            return new CauldronTrack(spaces);
        }

        private static CauldronSpace CreatePlaceholderSpace(int position)
        {
            return new CauldronSpace(
                position,
                buyingPower: position,
                victoryPoints: 0,
                hasRuby: false);
        }

        private static void Replace(IList<CauldronSpace> spaces, CauldronSpace replacement)
        {
            for (var i = 0; i < spaces.Count; i++)
            {
                if (spaces[i].Position == replacement.Position)
                {
                    spaces[i] = replacement;
                    return;
                }
            }

            spaces.Add(replacement);
        }
    }
}
