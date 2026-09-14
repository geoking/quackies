using System.Text.Json;
using Quackies.Core.Ducks.Definitions;
using Xunit;

namespace Quackies.Core.Tests;

public sealed class DuckDefinitionTests
{
    [Fact]
    public void V1_board_matches_every_approved_json_reward_row()
    {
        using var document = ReadReferenceJson("board.json");
        var root = document.RootElement;
        var rules = DuckRules.V1;

        Assert.Equal(0, DuckRuleDefinitions.NestPosition);
        Assert.False(root.GetProperty("nest").GetProperty("scorable").GetBoolean());
        Assert.Equal(DuckRuleDefinitions.NestPosition, root.GetProperty("nest").GetProperty("index").GetInt32());

        var rows = root.GetProperty("rows").EnumerateArray().ToArray();
        Assert.Equal(43, rows.Length);
        Assert.Equal(43, rules.BoardSpaces.Count);

        foreach (var row in rows)
        {
            var spaceNumber = row.GetProperty("space").GetInt32();
            var actual = rules.BoardSpaceAt(spaceNumber);
            var expectedHavenName = row.GetProperty("haven_name").GetString();

            Assert.Equal("space_" + spaceNumber.ToString("00"), actual.DefinitionId);
            Assert.Equal(spaceNumber, actual.Space);
            Assert.Equal(ParseBiome(row.GetProperty("biome").GetString()), actual.Biome);
            Assert.Equal(row.GetProperty("sleep").GetInt32(), actual.Sleep);
            Assert.Equal(row.GetProperty("twigs").GetInt32(), actual.Twigs);
            Assert.Equal(row.GetProperty("feathers").GetInt32(), actual.Feathers);
            Assert.Equal(row.GetProperty("haven").GetBoolean(), actual.IsHaven);
            Assert.Equal(string.IsNullOrEmpty(expectedHavenName) ? null : expectedHavenName, actual.HavenName);
            Assert.Same(actual, rules.BoardSpace(actual.DefinitionId));
        }

        Assert.Equal(new[] { 4, 10, 16, 21, 26, 32, 36, 43 },
            rules.BoardSpaces.Where(space => space.IsHaven).Select(space => space.Space));
    }

    [Fact]
    public void V1_shop_matches_every_approved_json_offer_and_uses_type_for_variant_limits()
    {
        using var document = ReadReferenceJson("shop.json");
        var offers = document.RootElement.GetProperty("offers").EnumerateArray().ToArray();
        var rules = DuckRules.V1;

        Assert.Equal(11, offers.Length);
        Assert.Equal(11, rules.ShopOffers.Count);

        foreach (var expected in offers)
        {
            var id = expected.GetProperty("id").GetString()!;
            var actual = rules.ShopOffer(id);
            var expectedMovement = NullableInt(expected.GetProperty("intrinsic_movement"));
            var expectedTwigYield = NullableInt(expected.GetProperty("bundle_quantity")) ?? 0;

            Assert.Equal(id, actual.DefinitionId);
            Assert.Equal(id, actual.EncounterDefinitionId);
            Assert.Equal(expected.GetProperty("sleep_price").GetInt32(), actual.SleepPrice);
            Assert.Equal(expectedMovement, actual.Encounter.BaseMovement);
            Assert.Equal(expectedTwigYield, actual.Encounter.TwigYield);
            Assert.Equal(actual.Encounter.EncounterType, actual.ShopType);
            Assert.Same(actual.Encounter, rules.Encounter(actual.EncounterDefinitionId));
        }

        Assert.All(rules.ShopOffers.Where(offer => offer.DefinitionId.StartsWith("tailwind_")),
            offer => Assert.Equal(DuckEncounterType.Tailwind, offer.ShopType));
        Assert.All(rules.ShopOffers.Where(offer => offer.DefinitionId.StartsWith("reeds_")),
            offer => Assert.Equal(DuckEncounterType.Reeds, offer.ShopType));
        Assert.Equal(7, rules.ShopOffers.Select(offer => offer.ShopType).Distinct().Count());
    }

    [Fact]
    public void V1_encounters_have_exact_canonical_identity_and_separate_quantities()
    {
        var actual = DuckRules.V1.EncounterDefinitions
            .Select(item => (item.DefinitionId, item.EncounterType, item.BaseMovement, item.TwigYield, item.ExhaustionValue))
            .ToArray();

        var expected = new (string, DuckEncounterType, int?, int, int)[]
        {
            ("seeds", DuckEncounterType.Seeds, 1, 0, 0),
            ("tailwind_2", DuckEncounterType.Tailwind, 2, 0, 0),
            ("tailwind_4", DuckEncounterType.Tailwind, 4, 0, 0),
            ("tailwind_6", DuckEncounterType.Tailwind, 6, 0, 0),
            ("signpost", DuckEncounterType.Signpost, 2, 0, 0),
            ("splash", DuckEncounterType.Splash, 1, 0, 0),
            ("reeds_1", DuckEncounterType.Reeds, 1, 1, 0),
            ("reeds_2", DuckEncounterType.Reeds, 1, 2, 0),
            ("reeds_3", DuckEncounterType.Reeds, 1, 3, 0),
            ("companion", DuckEncounterType.Companion, null, 0, 0),
            ("wildflowers", DuckEncounterType.Wildflowers, 1, 0, 0),
            ("fallen_log", DuckEncounterType.FallenLog, 1, 0, 1),
            ("mud_puddle", DuckEncounterType.MudPuddle, 1, 0, 1),
            ("loose_pebbles", DuckEncounterType.LoosePebbles, 1, 0, 1),
            ("brambles", DuckEncounterType.Brambles, 1, 0, 1),
            ("grumpy_goose", DuckEncounterType.GrumpyGoose, 1, 0, 1)
        };

        Assert.Equal(expected, actual);
        Assert.Equal(11, DuckRules.V1.EncounterDefinitions.Count(item => item.IsHelpful));
        Assert.Equal(5, DuckRules.V1.EncounterDefinitions.Count(item => item.IsObstacle));
        Assert.Equal(7, DuckRules.V1.EncounterDefinitions.Where(item => item.IsHelpful)
            .Select(item => item.EncounterType).Distinct().Count());
        Assert.Equal(5, DuckRules.V1.EncounterDefinitions.Where(item => item.IsObstacle)
            .Select(item => item.EncounterType).Distinct().Count());
    }

    [Fact]
    public void V1_opening_bag_is_the_approved_thirteen_chip_composition()
    {
        var counts = DuckRules.V1.OpeningBag
            .GroupBy(item => item.DefinitionId)
            .ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(13, DuckRules.V1.OpeningBag.Count);
        Assert.Equal(new Dictionary<string, int>
        {
            ["fallen_log"] = 2,
            ["mud_puddle"] = 2,
            ["loose_pebbles"] = 2,
            ["brambles"] = 2,
            ["seeds"] = 2,
            ["tailwind_2"] = 1,
            ["signpost"] = 1,
            ["splash"] = 1
        }, counts);
        Assert.All(DuckRules.V1.OpeningBag,
            encounter => Assert.Same(encounter, DuckRules.V1.Encounter(encounter.DefinitionId)));
        Assert.DoesNotContain(DuckRules.V1.OpeningBag,
            encounter => encounter.EncounterType == DuckEncounterType.GrumpyGoose);
    }

    [Fact]
    public void V1_world_event_deck_has_the_ten_approved_stable_identities()
    {
        var expected = new[]
        {
            ("rain_softened_seeds", DuckWorldEventType.RainSoftenedSeeds, "Rain-Softened Seeds"),
            ("sunlit_signboards", DuckWorldEventType.SunlitSignboards, "Sunlit Signboards"),
            ("a_friendly_guide", DuckWorldEventType.FriendlyGuide, "A Friendly Guide"),
            ("a_pocket_of_driftwood", DuckWorldEventType.PocketOfDriftwood, "A Pocket of Driftwood"),
            ("all_tucked_in", DuckWorldEventType.AllTuckedIn, "All Tucked In"),
            ("home_before_dark", DuckWorldEventType.HomeBeforeDark, "Home Before Dark"),
            ("shared_supper", DuckWorldEventType.SharedSupper, "Shared Supper"),
            ("still_air", DuckWorldEventType.StillAir, "Still Air"),
            ("thick_morning_mist", DuckWorldEventType.ThickMorningMist, "Thick Morning Mist"),
            ("restless_night", DuckWorldEventType.RestlessNight, "Restless Night")
        };
        var actual = DuckRules.V1.WorldEvents
            .Select(item => (item.DefinitionId, item.EventType, item.Name))
            .ToArray();

        Assert.Equal(expected, actual);
        Assert.All(DuckRules.V1.WorldEvents,
            worldEvent => Assert.Same(worldEvent, DuckRules.V1.WorldEvent(worldEvent.DefinitionId)));
    }

    [Fact]
    public void V1_catalogue_collections_and_definitions_are_immutable()
    {
        Assert.Throws<NotSupportedException>(() => ((IList<DuckBoardSpace>)DuckRules.V1.BoardSpaces).Clear());
        Assert.Throws<NotSupportedException>(() => ((IList<DuckEncounterDefinition>)DuckRules.V1.EncounterDefinitions).Clear());
        Assert.Throws<NotSupportedException>(() => ((IList<DuckShopOffer>)DuckRules.V1.ShopOffers).Clear());
        Assert.Throws<NotSupportedException>(() => ((IList<DuckWorldEventDefinition>)DuckRules.V1.WorldEvents).Clear());
        Assert.Throws<NotSupportedException>(() => ((IList<DuckEncounterDefinition>)DuckRules.V1.OpeningBag).Clear());

        Assert.All(new[]
        {
            typeof(DuckBoardSpace),
            typeof(DuckEncounterDefinition),
            typeof(DuckShopOffer),
            typeof(DuckWorldEventDefinition)
        }, type => Assert.All(type.GetProperties(), property => Assert.False(property.CanWrite)));
    }

    [Fact]
    public void Definitions_reject_invalid_identity_and_rule_ranges()
    {
        Assert.Throws<ArgumentException>(() =>
            new DuckEncounterDefinition(" companion", "Companion", DuckEncounterType.Companion, null, 0, 0));
        Assert.Throws<ArgumentException>(() =>
            new DuckEncounterDefinition("companion", "Companion", DuckEncounterType.Companion, 2, 0, 0));
        Assert.Throws<ArgumentException>(() =>
            new DuckEncounterDefinition("seeds", "Seeds", DuckEncounterType.Seeds, null, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new DuckEncounterDefinition("seeds", "Seeds", DuckEncounterType.Seeds, 0, 0, 0));
        Assert.Throws<ArgumentException>(() =>
            new DuckEncounterDefinition("seeds", "Seeds", DuckEncounterType.Seeds, 1, 1, 0));
        Assert.Throws<ArgumentException>(() =>
            new DuckBoardSpace(1, DuckBiome.Wetlands, 3, 1, 1, null));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new DuckBoardSpace(0, DuckBiome.Wetlands, 0, 0, 0, null));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new DuckShopOffer("seeds", DuckRules.V1.Encounter("seeds"), 0));
    }

    private static JsonDocument ReadReferenceJson(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            var path = Path.Combine(directory.FullName, "docs", "duck-migration", "v1", fileName);
            if (File.Exists(path)) return JsonDocument.Parse(File.ReadAllText(path));
            directory = directory.Parent;
        }

        throw new FileNotFoundException("Could not locate the duck v1 reference data.", fileName);
    }

    private static DuckBiome ParseBiome(string? biome)
    {
        return biome switch
        {
            "wetlands" => DuckBiome.Wetlands,
            "meadow" => DuckBiome.Meadow,
            "wasteland" => DuckBiome.Wasteland,
            _ => throw new InvalidDataException("Unknown biome in reference data: " + biome)
        };
    }

    private static int? NullableInt(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Null ? null : element.GetInt32();
    }
}
