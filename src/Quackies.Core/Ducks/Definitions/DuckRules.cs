using System.Collections.Generic;
using System.Linq;

namespace Quackies.Core.Ducks.Definitions
{
    /// <summary>Authoritative, Unity-independent Duck rules catalogues.</summary>
    public static class DuckRules
    {
        public static DuckRuleDefinitions V1 { get; } = CreateV1();

        private static DuckRuleDefinitions CreateV1()
        {
            var encounters = new[]
            {
                Helpful("seeds", "Seeds", DuckEncounterType.Seeds, 1),
                Helpful("tailwind_2", "Tailwind 2", DuckEncounterType.Tailwind, 2),
                Helpful("tailwind_4", "Tailwind 4", DuckEncounterType.Tailwind, 4),
                Helpful("tailwind_6", "Tailwind 6", DuckEncounterType.Tailwind, 6),
                Helpful("signpost", "Signpost", DuckEncounterType.Signpost, 2),
                Helpful("splash", "Refreshing splash", DuckEncounterType.Splash, 1),
                Helpful("reeds_1", "Nesting reeds 1", DuckEncounterType.Reeds, 1, 1),
                Helpful("reeds_2", "Nesting reeds 2", DuckEncounterType.Reeds, 1, 2),
                Helpful("reeds_3", "Nesting reeds 3", DuckEncounterType.Reeds, 1, 3),
                new DuckEncounterDefinition("companion", "Companion duck", DuckEncounterType.Companion, null, 0, 0),
                Helpful("wildflowers", "Wildflowers", DuckEncounterType.Wildflowers, 1),
                Obstacle("fallen_log", "Fallen log", DuckEncounterType.FallenLog),
                Obstacle("mud_puddle", "Mud puddle", DuckEncounterType.MudPuddle),
                Obstacle("loose_pebbles", "Loose pebbles", DuckEncounterType.LoosePebbles),
                Obstacle("brambles", "Brambles", DuckEncounterType.Brambles),
                Obstacle("grumpy_goose", "Grumpy Goose", DuckEncounterType.GrumpyGoose)
            };

            var byId = encounters.ToDictionary(item => item.DefinitionId);
            var shopOffers = new[]
            {
                Offer("seeds", 3),
                Offer("tailwind_2", 5),
                Offer("tailwind_4", 10),
                Offer("tailwind_6", 15),
                Offer("signpost", 7),
                Offer("splash", 4),
                Offer("reeds_1", 6),
                Offer("reeds_2", 11),
                Offer("reeds_3", 16),
                Offer("companion", 7),
                Offer("wildflowers", 5)
            };

            var openingBagIds = new[]
            {
                "fallen_log", "fallen_log",
                "mud_puddle", "mud_puddle",
                "loose_pebbles", "loose_pebbles",
                "brambles", "brambles",
                "seeds", "seeds",
                "tailwind_2",
                "signpost",
                "splash"
            };

            return new DuckRuleDefinitions(
                CreateBoard(),
                encounters,
                shopOffers,
                CreateWorldEvents(),
                openingBagIds.Select(id => byId[id]));

            DuckShopOffer Offer(string id, int price) => new DuckShopOffer(id, byId[id], price);
        }

        private static DuckEncounterDefinition Helpful(
            string id,
            string name,
            DuckEncounterType type,
            int movement,
            int twigYield = 0)
        {
            return new DuckEncounterDefinition(id, name, type, movement, twigYield, 0);
        }

        private static DuckEncounterDefinition Obstacle(string id, string name, DuckEncounterType type)
        {
            return new DuckEncounterDefinition(id, name, type, 1, 0, 1);
        }

        private static IEnumerable<DuckBoardSpace> CreateBoard()
        {
            return new[]
            {
                Space(1, DuckBiome.Wetlands, 3, 1),
                Space(2, DuckBiome.Wetlands, 3, 1),
                Space(3, DuckBiome.Wetlands, 5, 1),
                Haven(4, DuckBiome.Wetlands, 6, 1, 1, "Reed hammock"),
                Space(5, DuckBiome.Wetlands, 5, 2),
                Space(6, DuckBiome.Wetlands, 6, 2),
                Space(7, DuckBiome.Wetlands, 6, 2),
                Space(8, DuckBiome.Wetlands, 6, 2),
                Space(9, DuckBiome.Wetlands, 7, 3),
                Haven(10, DuckBiome.Wetlands, 10, 3, 1, "Willow nest"),
                Space(11, DuckBiome.Wetlands, 8, 3),
                Space(12, DuckBiome.Wetlands, 8, 3),
                Space(13, DuckBiome.Wetlands, 9, 3),
                Space(14, DuckBiome.Wetlands, 9, 3),
                Space(15, DuckBiome.Meadow, 10, 4),
                Haven(16, DuckBiome.Meadow, 13, 4, 1, "Clover hollow"),
                Space(17, DuckBiome.Meadow, 11, 4),
                Space(18, DuckBiome.Meadow, 11, 4),
                Space(19, DuckBiome.Meadow, 11, 4),
                Space(20, DuckBiome.Meadow, 12, 5),
                Haven(21, DuckBiome.Meadow, 15, 5, 1, "Orchard shelter"),
                Space(22, DuckBiome.Meadow, 13, 5),
                Space(23, DuckBiome.Meadow, 13, 5),
                Space(24, DuckBiome.Meadow, 13, 5),
                Space(25, DuckBiome.Meadow, 14, 5),
                Haven(26, DuckBiome.Meadow, 16, 5, 1, "Hayloft hideaway"),
                Space(27, DuckBiome.Meadow, 14, 5),
                Space(28, DuckBiome.Meadow, 14, 6),
                Space(29, DuckBiome.Wasteland, 11, 6),
                Space(30, DuckBiome.Wasteland, 11, 7),
                Space(31, DuckBiome.Wasteland, 11, 7),
                Haven(32, DuckBiome.Wasteland, 18, 7, 2, "Shaded rock nook"),
                Space(33, DuckBiome.Wasteland, 10, 7),
                Space(34, DuckBiome.Wasteland, 11, 8),
                Space(35, DuckBiome.Wasteland, 12, 8),
                Haven(36, DuckBiome.Wasteland, 20, 8, 2, "Spring-fed refuge"),
                Space(37, DuckBiome.Wasteland, 12, 8),
                Space(38, DuckBiome.Wasteland, 12, 8),
                Space(39, DuckBiome.Wasteland, 11, 8),
                Space(40, DuckBiome.Wasteland, 12, 8),
                Space(41, DuckBiome.Wasteland, 13, 8),
                Space(42, DuckBiome.Wasteland, 13, 8),
                Haven(43, DuckBiome.Wasteland, 21, 9, 2, "Oasis sanctuary")
            };
        }

        private static DuckBoardSpace Space(int space, DuckBiome biome, int sleep, int twigs)
        {
            return new DuckBoardSpace(space, biome, sleep, twigs, 0, null);
        }

        private static DuckBoardSpace Haven(
            int space,
            DuckBiome biome,
            int sleep,
            int twigs,
            int feathers,
            string name)
        {
            return new DuckBoardSpace(space, biome, sleep, twigs, feathers, name);
        }

        private static IEnumerable<DuckWorldEventDefinition> CreateWorldEvents()
        {
            return new[]
            {
                Event("rain_softened_seeds", DuckWorldEventType.RainSoftenedSeeds, "Rain-Softened Seeds"),
                Event("sunlit_signboards", DuckWorldEventType.SunlitSignboards, "Sunlit Signboards"),
                Event("a_friendly_guide", DuckWorldEventType.FriendlyGuide, "A Friendly Guide"),
                Event("a_pocket_of_driftwood", DuckWorldEventType.PocketOfDriftwood, "A Pocket of Driftwood"),
                Event("all_tucked_in", DuckWorldEventType.AllTuckedIn, "All Tucked In"),
                Event("home_before_dark", DuckWorldEventType.HomeBeforeDark, "Home Before Dark"),
                Event("shared_supper", DuckWorldEventType.SharedSupper, "Shared Supper"),
                Event("still_air", DuckWorldEventType.StillAir, "Still Air"),
                Event("thick_morning_mist", DuckWorldEventType.ThickMorningMist, "Thick Morning Mist"),
                Event("restless_night", DuckWorldEventType.RestlessNight, "Restless Night")
            };
        }

        private static DuckWorldEventDefinition Event(string id, DuckWorldEventType type, string name)
        {
            return new DuckWorldEventDefinition(id, type, name);
        }
    }
}
