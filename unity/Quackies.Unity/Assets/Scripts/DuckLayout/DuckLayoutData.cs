using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quackies.Unity.DuckLayout
{
    /// <summary>JSON contract for the fixed M3 board geometry. Coordinates are normalised, top-left origin.</summary>
    [Serializable]
    public sealed class DuckLayoutBoardData
    {
        public int version;
        public float boardWidth = 1536f;
        public float boardHeight = 1024f;
        public float wellWidth = 76f;
        public float wellHeight = 62f;
        public float rewardHeight = 24f;
        public DuckLayoutNestAnchor nest;
        public DuckLayoutBoardRow[] rows;

        public static bool TryParse(string json, out DuckLayoutBoardData data, out string issue)
        {
            data = JsonUtility.FromJson<DuckLayoutBoardData>(json);
            if (data == null) { issue = "The JSON could not be read."; return false; }
            if (data.version != 1) { issue = "Expected board-layout version 1."; return false; }
            if (data.boardWidth <= 0 || data.boardHeight <= 0) { issue = "Board dimensions must be positive."; return false; }
            if (data.wellWidth <= 0 || data.wellHeight <= 0 || data.rewardHeight <= 0) { issue = "Well and reward dimensions must be positive."; return false; }
            if (data.rows == null || data.rows.Length != 50) { issue = "Expected exactly 50 board rows."; return false; }
            var spaces = new HashSet<int>();
            foreach (var row in data.rows)
            {
                if (row == null || string.IsNullOrWhiteSpace(row.id)) { issue = "Every board row needs an id."; return false; }
                if (!spaces.Add(row.space) || row.space < 1 || row.space > 50) { issue = "Board spaces must be unique IDs 1–50."; return false; }
                if (row.x < 0f || row.x > 1f || row.y < 0f || row.y > 1f) { issue = row.id + " has an out-of-range normalised position."; return false; }
                if (row.sleep < 0 || row.twigs < 0 || row.feathers < 0) { issue = row.id + " has a negative reward."; return false; }
                if (row.feathers > 0 && !row.haven) { issue = row.id + " awards Feathers but is not a haven."; return false; }
            }
            issue = null;
            return true;
        }
    }

    [Serializable]
    public sealed class DuckLayoutNestAnchor { public float x; public float y; }

    [Serializable]
    public sealed class DuckLayoutBoardRow
    {
        public string id;
        public int space;
        public float x;
        public float y;
        public string biome;
        public int sleep;
        public int twigs;
        public int feathers;
        public bool haven;
        public string havenName;
        public float havenX;
        public float havenY;

        public Vector2 Position(float boardWidth, float boardHeight) => new Vector2(x * boardWidth, y * boardHeight);
        public Vector2 HavenPosition(float boardWidth, float boardHeight)
        {
            return havenX > 0f || havenY > 0f
                ? new Vector2(havenX * boardWidth, havenY * boardHeight)
                : Position(boardWidth, boardHeight) + new Vector2(0f, -44f);
        }
    }

    [Serializable]
    public sealed class DuckLayoutOfferFixture
    {
        public string id;
        public string family;
        public int movement;
        public int reedsQuantity;
        public int sleepPrice;
        public string detail;
        public string sourceAsset;
        public int cropIndex;
        public string displayName;
    }

    /// <summary>Fixed, approved M2 values used only to prove the Dream layout. No gameplay consumes these.</summary>
    public static class DuckLayoutFixtures
    {
        public static readonly int[] HavenSpaces = { 7, 13, 21, 27, 32, 38, 44, 50 };

        public static readonly DuckLayoutOfferFixture[] Offers =
        {
            Offer("seeds", "Seeds", 1, 0, 3, "everyday-encounters.png", 0),
            Offer("tailwind_2", "Tailwind", 2, 0, 5, "tailwind-variants.png", 0),
            Offer("tailwind_4", "Tailwind", 4, 0, 10, "tailwind-variants.png", 1),
            Offer("tailwind_6", "Tailwind", 6, 0, 15, "tailwind-variants.png", 2),
            Offer("signpost", "Signpost", 2, 0, 7, "everyday-encounters.png", 1),
            Offer("splash", "Refreshing splash", 1, 0, 4, "everyday-encounters.png", 2),
            Offer("reeds_1", "Nesting reeds", 1, 1, 6, "reeds-quantities.png", 0),
            Offer("reeds_2", "Nesting reeds", 1, 2, 11, "reeds-quantities.png", 1),
            Offer("reeds_3", "Nesting reeds", 1, 3, 16, "reeds-quantities.png", 2),
            Offer("companion", "Companion duck", 0, 0, 7, "everyday-encounters.png", 3),
            Offer("wildflowers", "Wildflowers", 1, 0, 5, "everyday-encounters.png", 4)
        };

        public static readonly DuckLayoutTokenFixture[] Tokens =
        {
            Token("Log", "white-obstacles.png", 0, 2), Token("Mud", "white-obstacles.png", 1, 5),
            Token("Pebbles", "white-obstacles.png", 2, 8), Token("Brambles", "white-obstacles.png", 3, 11),
            Token("Goose", "white-obstacles.png", 4, 14), Token("Seeds", "everyday-encounters.png", 0, 3),
            Token("Tailwind 2", "tailwind-variants.png", 0, 6), Token("Tailwind 4", "tailwind-variants.png", 1, 10),
            Token("Tailwind 6", "tailwind-variants.png", 2, 15), Token("Signpost", "everyday-encounters.png", 1, 18),
            Token("Refreshing splash", "everyday-encounters.png", 2, 22), Token("Reeds ×1", "reeds-quantities.png", 0, 26),
            Token("Reeds ×2", "reeds-quantities.png", 1, 30), Token("Reeds ×3", "reeds-quantities.png", 2, 34),
            Token("Companion", "everyday-encounters.png", 3, 38), Token("Wildflowers", "everyday-encounters.png", 4, 42)
        };

        private static DuckLayoutOfferFixture Offer(string id, string family, int movement, int reedsQuantity, int sleepPrice,
            string sourceAsset, int cropIndex)
        {
            var detail = reedsQuantity > 0
                ? "Reeds ×" + reedsQuantity
                : movement > 0 ? "→ " + movement : "Companion";
            return new DuckLayoutOfferFixture
            {
                id = id, family = family, movement = movement, reedsQuantity = reedsQuantity, sleepPrice = sleepPrice,
                detail = detail, sourceAsset = sourceAsset, cropIndex = cropIndex, displayName = family + " " + detail
            };
        }

        private static DuckLayoutTokenFixture Token(string title, string asset, int column, int space)
        {
            return new DuckLayoutTokenFixture { title = title, sourceAsset = asset, cropIndex = column, space = space };
        }
    }

    [Serializable]
    public sealed class DuckLayoutTokenFixture
    {
        public string title;
        public string sourceAsset;
        public int cropIndex;
        public int space;
    }

    /// <summary>
    /// Optional editor input for extracting encounter sprites without changing any source pixels.
    /// x/y use the supplied PNG's top-left origin, matching the board-layout convention.
    /// </summary>
    [Serializable]
    public sealed class DuckLayoutCropManifest
    {
        public DuckLayoutCropEntry[] entries;
    }

    [Serializable]
    public sealed class DuckLayoutCropEntry
    {
        public string asset;
        public int index;
        public float x;
        public float y;
        public float width;
        public float height;
        public DuckLayoutOutlinePoint[] outline;
    }

    [Serializable]
    public sealed class DuckLayoutOutlinePoint
    {
        public float x;
        public float y;
    }
}
