using System.Collections.Generic;
using Quackies.Unity.Presentation;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Quackies.Unity.Editor
{
    /// <summary>Authors the isolated M1 art and scale review scene without changing game scenes or build settings.</summary>
    public static class DuckStyleTestSceneBuilder
    {
        public const string ScenePath = "Assets/Scenes/DuckStyleTestScene.unity";
        private const string PlaymatPath = "Assets/Art/DuckTheme/Backgrounds/duck_playmat_v1.png";
        private const string DuckPath = "Assets/Art/DuckTheme/Characters/duck_happy_v1.png";
        private const string SeedPath = "Assets/Art/DuckTheme/Encounters/encounter_seeds_v1.png";
        private const int Columns = 9;
        private const int Rows = 6;
        private const int SpaceCount = Columns * Rows;

        private static readonly Color Ink = new Color(.08f, .25f, .27f);
        private static readonly Color Cream = new Color(1f, .96f, .82f, .94f);
        private static readonly Color Mint = new Color(.73f, .91f, .78f, .91f);
        private static readonly Color Teal = new Color(.08f, .48f, .50f, .96f);
        private static readonly Color DeepTeal = new Color(.04f, .29f, .31f, .96f);
        private static readonly Color Orange = new Color(1f, .53f, .20f, 1f);
        private static readonly Color Track = new Color(.48f, .79f, .69f, .86f);
        private static readonly Color Reward = new Color(.98f, .79f, .31f, 1f);

        [MenuItem("Quackies/Build Duck Style Test")]
        public static void BuildDuckStyleTest()
        {
            TryBuildDuckStyleTest();
        }

        [MenuItem("Quackies/Build and Play Duck Style Test")]
        public static void BuildAndPlayDuckStyleTest()
        {
            if (!TryBuildDuckStyleTest()) return;
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            EditorApplication.isPlaying = true;
        }

        /// <summary>Rebuilds a clean scene so repeated authoring cannot duplicate board objects.</summary>
        public static bool TryBuildDuckStyleTest()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("Exit Play mode before rebuilding the Duck style test.");
                return false;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;
            var playmat = LoadSprite(PlaymatPath, false);
            var duck = LoadSprite(DuckPath, true);
            var seeds = LoadSprite(SeedPath, true);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            BuildEventSystem();
            BuildBackgroundCamera();
            var canvas = BuildCanvas(playmat);
            var safeArea = BuildSafeArea(canvas.transform);
            var root = BuildViewport(safeArea);
            BuildHeader(root);
            var board = BuildBoard(root, duck, seeds, out var seedMarkers, out var boardCells, out var restingMarker);
            var restingLabel = BuildSidePanel(root, duck, seeds);
            var preview = BuildControls(root, seedMarkers, boardCells, restingMarker, restingLabel);
            EditorSceneManager.SaveScene(scene, ScenePath);
            Selection.activeGameObject = preview.gameObject;
            AssetDatabase.SaveAssets();
            Debug.Log("Built Duck style test with " + board + " indexed cells (0–53).");
            return true;
        }

        private static Canvas BuildCanvas(Sprite playmat)
        {
            var root = new GameObject("Duck Style Test", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(FittedViewport.Width, FittedViewport.Height);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = .5f;
            var background = root.AddComponent<Image>();
            background.color = playmat == null ? new Color(.61f, .84f, .76f) : Color.white;
            background.sprite = playmat;
            background.preserveAspect = false;
            background.raycastTarget = false;
            return canvas;
        }

        private static RectTransform BuildSafeArea(Transform parent)
        {
            var safe = TableUi.Rect("Safe Area", parent);
            TableUi.Fill(safe);
            safe.gameObject.AddComponent<Quackies.Unity.Presentation.SafeArea>();
            return safe;
        }

        private static RectTransform BuildViewport(RectTransform safeArea)
        {
            var viewport = TableUi.Rect("1133 x 744 Style Preview", safeArea);
            viewport.gameObject.AddComponent<FittedViewport>().Configure(safeArea);
            var overlay = viewport.gameObject.AddComponent<Image>();
            overlay.color = new Color(.88f, 1f, .91f, .12f);
            overlay.raycastTarget = false;
            return viewport;
        }

        private static void BuildHeader(RectTransform root)
        {
            var header = Panel("Style Preview Header", root, 18, 14, 1097, 58, Cream);
            var title = Text("Title", header, "QUACKIES", 31, DeepTeal, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(title.rectTransform, 20, 5, 270, 45);
            var subtitle = Text("Subtitle", header, "A cheerful wetlands table", 15, Ink, false, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(subtitle.rectTransform, 292, 11, 290, 32);
            var badge = Panel("M1 Badge", header, 848, 9, 222, 40, Teal);
            var badgeText = Text("Label", badge, "STYLE PREVIEW", 14, Color.white, true, TextAlignmentOptions.Center);
            TableUi.Fill(badgeText.rectTransform, 4);
        }

        private static int BuildBoard(RectTransform root, Sprite duck, Sprite seeds, out GameObject[] seedMarkers,
            out RectTransform[] boardCells, out RectTransform restingMarker)
        {
            var board = Panel("Complete Wetland Trail — 54 spaces", root, 18, 88, 778, 568, new Color(.93f, 1f, .91f, .82f));
            var title = Text("Board label", board, "TODAY'S TRAIL", 16, DeepTeal, true,
                TextAlignmentOptions.MidlineLeft);
            TableUi.Place(title.rectTransform, 20, 12, 720, 24);
            var note = Text("Board note", board, "Explore cycles a few seed placements; this preview does not start a match.",
                12, Ink, false, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(note.rectTransform, 20, 35, 730, 18);

            for (var row = 0; row < Rows; row++)
            {
                var guide = Ellipse("Rounded row guide " + row, board, Track, new Color(.04f, .29f, .31f, .28f), 1.25f);
                TableUi.Place(guide.rectTransform, 43, 78 + row * 77, 676, 49);
            }

            for (var turn = 0; turn < Rows - 1; turn++)
            {
                var upperRow = Rows - 2 - turn;
                var x = turn % 2 == 0 ? 680 : 31;
                var bend = Ellipse("Rounded bend " + turn, board, Track, new Color(.04f, .29f, .31f, .28f), 1.25f);
                TableUi.Place(bend.rectTransform, x, 100 + upperRow * 77, 50, 82);
            }

            var markers = new List<GameObject>();
            var cells = new RectTransform[SpaceCount];
            for (var index = 0; index < SpaceCount; index++)
            {
                var rowFromBottom = index / Columns;
                var displayRow = Rows - 1 - rowFromBottom;
                var directionColumn = index % Columns;
                var displayColumn = rowFromBottom % 2 == 0 ? directionColumn : Columns - 1 - directionColumn;
                var x = 55 + displayColumn * 79f;
                var y = 72 + displayRow * 77f;
                var cell = Ellipse("Space " + index.ToString("00"), board,
                    index == 0 ? Orange : Cream, new Color(.04f, .29f, .31f, .48f), 1.5f);
                TableUi.Place(cell.rectTransform, x, y, 60, 60);
                cells[index] = cell.rectTransform;

                var indexLabel = Text("Index", cell.transform, index.ToString(), 14, Ink, true, TextAlignmentOptions.Top);
                TableUi.Place(indexLabel.rectTransform, 8, 8, 44, 18);

                if (index == 0)
                {
                    var duckMarker = Image("Permanent duck start", cell.transform, duck == null ? Cream : Color.white, duck);
                    TableUi.Place(duckMarker.rectTransform, 11, 12, 38, 38);
                    duckMarker.preserveAspect = true;
                    var start = Text("Start", board, "Duck start", 11, DeepTeal, true, TextAlignmentOptions.MidlineLeft);
                    TableUi.Place(start.rectTransform, 18, 540, 170, 17);
                }
            }

            // These nine markers are deliberately contiguous example placements 1–9.
            // DuckStylePreview only changes their visibility; it never submits a game action.
            var previewIndices = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (var index in previewIndices)
            {
                var marker = BuildSeedMarker(board.transform.Find("Space " + index.ToString("00")), seeds);
                markers.Add(marker.gameObject);
            }
            seedMarkers = markers.ToArray();
            boardCells = cells;
            restingMarker = Ellipse("Resting preview marker", cells[4], Teal, Color.white, 2f).rectTransform;
            restingMarker.SetAsLastSibling();
            restingMarker.anchorMin = restingMarker.anchorMax = new Vector2(.5f, .5f);
            restingMarker.pivot = new Vector2(.5f, .5f);
            restingMarker.sizeDelta = new Vector2(26, 26);
            restingMarker.anchoredPosition = new Vector2(0, -2);
            return SpaceCount;
        }

        private static Image BuildSeedMarker(Transform cell, Sprite seeds)
        {
            var marker = Image("Representative 1-seed encounter", cell, seeds == null ? Orange : Color.white, seeds);
            TableUi.Place(marker.rectTransform, 18, 18, 26, 26);
            marker.preserveAspect = true;
            var value = Text("Value", marker.transform, "1", 13, Ink, true, TextAlignmentOptions.Center);
            TableUi.Fill(value.rectTransform, 1);
            return marker;
        }

        private static TMP_Text BuildSidePanel(RectTransform root, Sprite duck, Sprite seeds)
        {
            var side = Panel("Preview Details", root, 814, 88, 301, 568, new Color(1f, .98f, .85f, .92f));
            var heading = Text("Heading", side, "YOUR DUCK", 18, DeepTeal, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(heading.rectTransform, 20, 16, 260, 28);

            var sample = Panel("Large duck and seed sample", side, 20, 55, 261, 116, Mint);
            var duckIcon = Image("Large duck icon", sample, duck == null ? Cream : Color.white, duck);
            TableUi.Place(duckIcon.rectTransform, 15, 13, 68, 68);
            duckIcon.preserveAspect = true;
            var duckLabel = Text("Large duck label", sample, "DUCK", 10, Ink, true, TextAlignmentOptions.Center);
            TableUi.Place(duckLabel.rectTransform, 12, 84, 74, 18);
            var sampleIcon = Image("Large seed icon", sample, seeds == null ? Orange : Color.white, seeds);
            TableUi.Place(sampleIcon.rectTransform, 95, 13, 68, 68);
            sampleIcon.preserveAspect = true;
            var seedLabel = Text("Large seed value", sample, "SEED  1", 10, Ink, true, TextAlignmentOptions.Center);
            TableUi.Place(seedLabel.rectTransform, 91, 84, 78, 18);
            var scaleLabel = Text("Scale label", sample, "Scale\ncheck", 15, DeepTeal, true, TextAlignmentOptions.Center);
            TableUi.Place(scaleLabel.rectTransform, 178, 31, 68, 52);

            var rest = Panel("Resting preview", side, 20, 187, 261, 88, Reward);
            var restTitle = Text("Resting title", rest, "RESTING PREVIEW", 13, DeepTeal, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(restTitle.rectTransform, 15, 9, 220, 18);
            var restText = Text("Resting text", rest, "Rest at space 4\n4 Pond pennies • 0 Twigs", 13, Ink, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(restText.rectTransform, 15, 31, 225, 45);

            var nest = Panel("Scored nest", side, 20, 292, 261, 79, new Color(.63f, .84f, .62f, .97f));
            var nestTitle = Text("Nest title", nest, "YOUR NEST", 13, DeepTeal, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(nestTitle.rectTransform, 15, 9, 225, 18);
            var twigs = Text("Twigs", nest, "Twigs  12", 22, Ink, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(twigs.rectTransform, 15, 31, 220, 36);

            var pennies = Panel("Pond pennies", side, 20, 388, 126, 88, Cream);
            var penniesLabel = Text("Pond pennies label", pennies, "POND PENNIES", 11, DeepTeal, true, TextAlignmentOptions.Center);
            TableUi.Place(penniesLabel.rectTransform, 6, 9, 114, 18);
            var penniesText = Text("Pond pennies value", pennies, "3\nSpend today", 15, Ink, true, TextAlignmentOptions.Center);
            TableUi.Place(penniesText.rectTransform, 5, 28, 116, 50);
            var feathers = Panel("Feathers", side, 155, 388, 126, 88, Cream);
            var feathersLabel = Text("Feathers label", feathers, "FEATHERS", 11, DeepTeal, true, TextAlignmentOptions.Center);
            TableUi.Place(feathersLabel.rectTransform, 6, 9, 114, 18);
            var feathersText = Text("Feathers value", feathers, "2\nplaceholder", 15, Ink, true, TextAlignmentOptions.Center);
            TableUi.Place(feathersText.rectTransform, 5, 28, 116, 50);
            return restText;
        }

        private static DuckStylePreview BuildControls(RectTransform root, GameObject[] seedMarkers, RectTransform[] boardCells,
            RectTransform restingMarker, TMP_Text restingLabel)
        {
            var footer = Panel("Preview controls", root, 18, 672, 1097, 58, Cream);
            var status = Text("Preview status", footer, "Today’s trail • 3 seed encounters previewed.",
                14, Ink, false, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(status.rectTransform, 18, 11, 570, 35);
            var reset = TableUi.Button("Reset preview", footer, "Reset preview", Mint, Ink);
            TableUi.Place(reset.GetComponent<RectTransform>(), 657, 7, 170, 44);
            var explore = TableUi.Button("Explore preview", footer, "Explore", Teal, Color.white);
            TableUi.Place(explore.GetComponent<RectTransform>(), 841, 7, 238, 44);
            var preview = footer.gameObject.AddComponent<DuckStylePreview>();
            preview.Configure(explore, reset, status, seedMarkers, boardCells, restingMarker, restingLabel);
            return preview;
        }

        private static TMP_Text Text(string name, Transform parent, string value, float size, Color color, bool bold,
            TextAlignmentOptions alignment)
        {
            var text = TableUi.Text(name, parent, value, size, color, bold);
            text.alignment = alignment;
            return text;
        }

        private static Image Image(string name, Transform parent, Color color, Sprite sprite = null)
        {
            return TableUi.Image(name, parent, color, sprite);
        }

        private static DuckStyleEllipse Ellipse(string name, Transform parent, Color fill, Color border, float borderThickness)
        {
            var ellipse = TableUi.Rect(name, parent).gameObject.AddComponent<DuckStyleEllipse>();
            ellipse.raycastTarget = false;
            ellipse.Configure(fill, border, borderThickness);
            return ellipse;
        }

        private static RectTransform Panel(string name, Transform parent, float x, float y, float width, float height, Color color)
        {
            var panel = Image(name, parent, color, BuiltinPanel()).rectTransform;
            panel.GetComponent<UnityEngine.UI.Image>().type = UnityEngine.UI.Image.Type.Sliced;
            TableUi.Place(panel, x, y, width, height);
            return panel;
        }

        private static Sprite LoadSprite(string path, bool transparent)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return null;
            var changed = importer.textureType != TextureImporterType.Sprite || importer.spriteImportMode != SpriteImportMode.Single
                || importer.alphaIsTransparency != transparent || importer.maxTextureSize != 2048;
            if (changed)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = transparent;
                importer.maxTextureSize = 2048;
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Sprite BuiltinPanel()
        {
            return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        }

        private static void BuildEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            new GameObject("Event System", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }

        private static void BuildBackgroundCamera()
        {
            var camera = new GameObject("Style Preview Background Camera", typeof(Camera)).GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.61f, .84f, .76f);
            camera.cullingMask = 0;
            camera.orthographic = true;
        }

        [MenuItem("Quackies/Build Duck Style Test", true)]
        [MenuItem("Quackies/Build and Play Duck Style Test", true)]
        private static bool CanBuildDuckStyleTest() => !EditorApplication.isPlayingOrWillChangePlaymode;
    }
}
