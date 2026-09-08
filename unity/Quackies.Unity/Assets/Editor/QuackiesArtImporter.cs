using System;
using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Tokens;
using Quackies.Unity.Art;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Quackies.Unity.Editor
{
    /// <summary>
    /// Reproducible import recipe for the supplied base-game artwork. Source image bytes
    /// are never changed: the base pot and flask faces are non-destructive sprite rectangles.
    /// Existing slices are retained so unrelated references to the raw art remain valid.
    /// </summary>
    public static class QuackiesArtImporter
    {
        public const string CatalogPath = "Assets/Art/QuackiesArtCatalog.asset";
        private const string RawPath = "Assets/Art/raw/";
        private const float PotSourceWidth = 1593f;
        private const float PotCropHeight = 1175f;

        // Centers measured against cauldron/blue/cauldron.png at its original 1593x1439
        // resolution. Coordinates use the image convention (x right, y down from the top).
        // Both supplied blue/red pots share this spiral. The crop removes the optional
        // test-tube row while retaining the complete standard pot and spoon reward.
        // Repeated printed coin values each have their own physical coordinate.
        private static readonly Vector2[] PotCentersFromTop =
        {
            new Vector2(809, 583), // 0: starting droplet
            new Vector2(687, 628), new Vector2(593, 549), new Vector2(659, 441),
            new Vector2(791, 425), new Vector2(917, 463), new Vector2(992, 559),
            new Vector2(958, 678), new Vector2(854, 756), new Vector2(721, 772),
            new Vector2(589, 741), new Vector2(477, 665), new Vector2(429, 549),
            new Vector2(455, 432), new Vector2(551, 335), new Vector2(676, 291),
            new Vector2(812, 291), new Vector2(938, 319), new Vector2(1049, 378),
            new Vector2(1124, 481), new Vector2(1148, 602), new Vector2(1110, 720),
            new Vector2(1018, 807), new Vector2(901, 879), new Vector2(766, 901),
            new Vector2(626, 894), new Vector2(493, 849), new Vector2(384, 769),
            new Vector2(308, 661), new Vector2(276, 536), new Vector2(302, 418),
            new Vector2(360, 300), new Vector2(465, 220), new Vector2(598, 171),
            new Vector2(734, 156), new Vector2(871, 161), new Vector2(1000, 195),
            new Vector2(1115, 262), new Vector2(1213, 343), new Vector2(1273, 446),
            new Vector2(1299, 563), new Vector2(1286, 685), new Vector2(1238, 802),
            new Vector2(1150, 898), new Vector2(1037, 970), new Vector2(912, 1022),
            new Vector2(771, 1047), new Vector2(632, 1038), new Vector2(492, 1004),
            new Vector2(371, 932), new Vector2(269, 840), new Vector2(198, 726),
            new Vector2(166, 598), new Vector2(180, 409) // 53: spoon, 35 coins / 15 VP
        };

        // Source atlas order: rows from top to bottom, columns from left to right.
        private static readonly string[] FortuneTitles =
        {
            "A Second Chance", "Just in Time", "Well Stirred", "Lucky Devil", "It's Shining Extra Bright",
            "Seasoned Perfectly", "Less Is More", "The Pot Is Full", "Choose Wisely", "Living in Luxury",
            "An Opportunistic Moment", "Roll the Die", "Pumpkin Patch Party", "You Only Get to Choose One", "Strong Ingredient",
            "Magic Potion", "A Good Start", "Rat Infestation", "Rats Are Your Friends", "Charity",
            "The Pot Is Filling Up", "Wheel and Deal", "Beginner's Bonus", "Schadenfreude"
        };

        /// <summary>
        /// Cookbook: call before building the scene; assign the returned catalog to its
        /// presenter; fit the cauldron Image and marker parent together; use GetPotAnchor
        /// for chip anchors. Only selected base-game textures are configured/reimported.
        /// Re-running reuses the catalog, sprite identities, and source files.
        /// </summary>
        public static QuackiesArtCatalog BuildCatalog()
        {
            var tokens = BuildTokens();
            var books = BuildBooks();
            var fortunes = BuildFortunes();
            var human = BuildPlayerArt("blue");
            var opponent = BuildPlayerArt("red");
            var board = ImportFull("board.png");
            var marker = ImportFull("round_counter.png");
            var back = ImportFull("back_of_card.jpg");
            var atlas = ImportFull("cards.jpg", 4096);
            var die = ImportFull("dice.png");
            var anchors = PotCentersFromTop.Select(point =>
                new Vector2(point.x / PotSourceWidth, 1f - point.y / PotCropHeight)).ToArray();

            var catalog = AssetDatabase.LoadAssetAtPath<QuackiesArtCatalog>(CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<QuackiesArtCatalog>();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }
            catalog.Configure(tokens, books, fortunes, human, opponent, board, marker, back, atlas, die, anchors);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            return catalog;
        }

        private static QuackiesArtCatalog.TokenArt[] BuildTokens()
        {
            var entries = new List<QuackiesArtCatalog.TokenArt>();
            foreach (TokenColor color in Enum.GetValues(typeof(TokenColor)))
            {
                var values = color == TokenColor.White ? new[] { 1, 2, 3 }
                    : color == TokenColor.Orange || color == TokenColor.Black || color == TokenColor.Purple
                        ? new[] { 1 } : new[] { 1, 2, 4 };
                foreach (var value in values)
                    entries.Add(new QuackiesArtCatalog.TokenArt
                    {
                        Color = color,
                        Value = value,
                        Sprite = ImportFull($"tokens/{color.ToString().ToLowerInvariant()}/{value}.png")
                    });
            }
            return entries.ToArray();
        }

        private static QuackiesArtCatalog.BookArt[] BuildBooks()
        {
            var entries = new List<QuackiesArtCatalog.BookArt>();
            foreach (TokenColor color in Enum.GetValues(typeof(TokenColor)))
            {
                if (color == TokenColor.White) continue;
                var file = color == TokenColor.Black ? "2_player.png" : "1.png";
                entries.Add(new QuackiesArtCatalog.BookArt
                {
                    Color = color,
                    Sprite = ImportFull($"books/{color.ToString().ToLowerInvariant()}/{file}")
                });
            }
            return entries.ToArray();
        }

        private static QuackiesArtCatalog.PlayerArt BuildPlayerArt(string color)
        {
            var folder = $"cauldron/{color}/";
            var potImporter = GetTextureImporter(folder + "cauldron.png");
            potImporter.GetSourceTextureWidthAndHeight(out var width, out var height);
            if (width != PotSourceWidth || height < PotCropHeight)
                throw new InvalidOperationException($"The {color} cauldron has changed dimensions; remeasure its board anchors.");
            var pot = ImportSlices(folder + "cauldron.png", new[]
            {
                new Slice("Quackies_BasePot", new Rect(0, height - PotCropHeight, width, PotCropHeight))
            })[0];

            // The supplied JPG holds empty (left) and full (right) flasks side by side.
            var flaskImporter = GetTextureImporter(folder + "flask.jpg");
            flaskImporter.GetSourceTextureWidthAndHeight(out var flaskWidth, out var flaskHeight);
            var flaskFaces = ImportSlices(folder + "flask.jpg", new[]
            {
                new Slice("Quackies_EmptyFlask", new Rect(0, 0, flaskWidth * 0.5f, flaskHeight)),
                new Slice("Quackies_FullFlask", new Rect(flaskWidth * 0.5f, 0, flaskWidth * 0.5f, flaskHeight))
            });
            return new QuackiesArtCatalog.PlayerArt
            {
                Cauldron = pot,
                Droplet = ImportFull(folder + "droplet.png"),
                Rat = ImportFull(folder + "rat.png"),
                EmptyFlask = flaskFaces[0],
                FullFlask = flaskFaces[1]
            };
        }

        private static QuackiesArtCatalog.FortuneArt[] BuildFortunes()
        {
            var importer = GetTextureImporter("cards.jpg");
            importer.GetSourceTextureWidthAndHeight(out var width, out var height);
            var cellWidth = width / 5f;
            var cellHeight = height / 5f;
            var slices = new Slice[FortuneTitles.Length];
            for (var index = 0; index < slices.Length; index++)
            {
                var rowFromTop = index / 5;
                var column = index % 5;
                // Remove the black gutters, retaining each complete card face.
                var rect = new Rect(column * cellWidth + 36, height - (rowFromTop + 1) * cellHeight + 39,
                    cellWidth - 78, cellHeight - 79);
                slices[index] = new Slice($"Quackies_Fortune_{index + 1:00}", rect);
            }
            var sprites = ImportSlices("cards.jpg", slices, 4096);
            return FortuneTitles.Select((title, index) => new QuackiesArtCatalog.FortuneArt
                { Title = title, Sprite = sprites[index] }).ToArray();
        }

        private readonly struct Slice
        {
            public readonly string Name;
            public readonly Rect Rect;
            public Slice(string name, Rect rect) { Name = name; Rect = rect; }
        }

        private static Sprite ImportFull(string relativePath, int maxSize = 2048)
        {
            var importer = GetTextureImporter(relativePath);
            importer.GetSourceTextureWidthAndHeight(out var width, out var height);
            return ImportSlices(relativePath, new[]
                { new Slice("Quackies_FullImage", new Rect(0, 0, width, height)) }, maxSize)[0];
        }

        private static TextureImporter GetTextureImporter(string relativePath)
        {
            var path = RawPath + relativePath;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new InvalidOperationException($"Required Quackies artwork is missing or not imported: {path}");
            return importer;
        }

        private static Sprite[] ImportSlices(string relativePath, Slice[] requested, int maxSize = 2048)
        {
            var importer = GetTextureImporter(relativePath);
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            var changed = importer.textureType != TextureImporterType.Sprite
                || importer.spriteImportMode != SpriteImportMode.Multiple
                || importer.mipmapEnabled || !importer.alphaIsTransparency
                || importer.wrapMode != TextureWrapMode.Clamp || importer.filterMode != FilterMode.Bilinear
                || importer.maxTextureSize < maxSize || settings.spriteMeshType != SpriteMeshType.FullRect;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = Mathf.Max(importer.maxTextureSize, maxSize);
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();
            var rects = provider.GetSpriteRects().ToList();
            foreach (var slice in requested)
            {
                var rect = rects.FirstOrDefault(existing => existing.name == slice.Name);
                if (rect == null)
                {
                    rect = new SpriteRect { name = slice.Name, spriteID = GUID.Generate() };
                    rects.Add(rect);
                    changed = true;
                }
                if (rect.rect != slice.Rect || rect.pivot != new Vector2(0.5f, 0.5f)
                    || rect.alignment != SpriteAlignment.Center) changed = true;
                rect.rect = slice.Rect;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.alignment = SpriteAlignment.Center;
            }
            if (changed)
            {
                provider.SetSpriteRects(rects.ToArray());
                var names = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();
                if (names != null)
                    names.SetNameFileIdPairs(rects.Select(rect => new SpriteNameFileIdPair(rect.name, rect.spriteID)));
                provider.Apply();
                importer.SaveAndReimport();
            }
            var sprites = AssetDatabase.LoadAllAssetsAtPath(RawPath + relativePath).OfType<Sprite>().ToArray();
            return requested.Select(slice => sprites.FirstOrDefault(sprite => sprite.name == slice.Name)
                ?? throw new InvalidOperationException($"Could not import {slice.Name} from {relativePath}.")).ToArray();
        }
    }
}
