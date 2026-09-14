using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Quackies.Unity.DuckLayout;
using Quackies.Unity.Presentation;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Quackies.Unity.Editor
{
    /// <summary>Deterministically authors the isolated M3 layout-proof scene from fixed, non-gameplay fixtures.</summary>
    public static class DuckLayoutSceneBuilder
    {
        public const string ScenePath = "Assets/Scenes/DuckLayoutProof.unity";
        public const string ArtDirectory = "Assets/Art/DuckLayout";
        private const string BoardLayoutPath = ArtDirectory + "/board-layout.json";
        private const string CropManifestPath = ArtDirectory + "/encounter-crops.json";
        private const float ViewWidth = FittedViewport.Width;
        private const float ViewHeight = FittedViewport.Height;
        private const float BoardY = 57f;
        private const float BoardBottomInset = 5f;
        private const float BoardHorizontalInset = 13f;
        private const int HighResolutionTextureSize = 4096;

        private static readonly Color Navy = new Color(.07f, .10f, .24f);
        private static readonly Color Lavender = new Color(.58f, .52f, .77f);
        private static readonly Color Cream = new Color(1f, .95f, .82f);
        private static readonly Color Ink = new Color(.11f, .08f, .22f);
        private static readonly Color Paper = new Color(.96f, .91f, .84f, .96f);
        private static readonly Color WetlandTint = new Color(.78f, .97f, .86f, .94f);
        private static readonly Color MeadowTint = new Color(.98f, .88f, .50f, .94f);
        private static readonly Color WastelandTint = new Color(.93f, .65f, .40f, .94f);

        [MenuItem("Quackies/Build Duck Layout Proof")]
        public static void BuildDuckLayoutProof() => TryBuildDuckLayoutProof();

        [MenuItem("Quackies/Build and Play Duck Layout Proof")]
        public static void BuildAndPlayDuckLayoutProof()
        {
            if (!TryBuildDuckLayoutProof()) return;
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Quackies/Write Duck Layout Audit")]
        public static void WriteDuckLayoutAudit()
        {
            if (!File.Exists(Path.Combine(Application.dataPath, "Scenes/DuckLayoutProof.unity")))
            {
                Debug.LogError("Build Duck Layout Proof before writing its audit.");
                return;
            }
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var report = AuditOpenScene();
            var reportPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/DuckLayoutAudit.md"));
            File.WriteAllText(reportPath, report);
            Debug.Log(report + "\nDuck layout audit written to " + reportPath);
        }

        /// <summary>Rebuilds a clean saved scene. It does not change build settings or any playable scene.</summary>
        public static bool TryBuildDuckLayoutProof()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("Exit Play mode before rebuilding the Duck layout proof.");
                return false;
            }
            if (!TryLoadInputs(out var data, out var art, out var issue))
            {
                Debug.LogError("Duck layout proof was not built: " + issue);
                return false;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            BuildEventSystem();
            BuildBackgroundCamera();
            var canvas = BuildCanvas();
            var safe = BuildSafeArea(canvas.transform);
            var viewport = BuildViewport(safe);
            var proof = new GameObject("Duck Layout Proof Controller", typeof(RectTransform), typeof(DuckLayoutProofPreview))
                .GetComponent<DuckLayoutProofPreview>();
            proof.transform.SetParent(viewport, false);

            var adventure = BuildAdventure(viewport, data, art, out var spaces, out var tokens,
                out var dreamButton, out var emptyButton, out var occupiedButton, out var status,
                out var duckRest, out var zzz, out var featherTrail);
            var dream = BuildDream(viewport, art, out var offers, out var dreamAdventureButton);
            var inspection = BuildInspection(viewport, art, out var inspectionTitle, out var inspectionDetail, out var inspectionSleepValue,
                out var inspectionTwigValue, out var inspectionFeatherValue,
                out var inspectionArt, out var inspectionSleep, out var inspectionTwig, out var inspectionFeather, out var closeInspection);
            dream.gameObject.SetActive(false);
            proof.Configure(adventure.gameObject, dream.gameObject, dreamAdventureButton, dreamButton, emptyButton, occupiedButton,
                status, spaces, offers, tokens, duckRest, zzz, featherTrail, inspection, inspectionTitle, inspectionDetail,
                inspectionSleepValue, inspectionTwigValue, inspectionFeatherValue, inspectionArt, inspectionSleep, inspectionTwig,
                inspectionFeather, closeInspection);
            proof.transform.SetAsLastSibling();
            inspection.transform.SetAsLastSibling();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = proof.gameObject;
            Debug.Log("Built Duck Layout Proof: " + spaces.Length + " stable wells, " + offers.Length
                + " Dream offers, " + tokens.Length + " encounter-fit samples.\n" + AuditOpenScene());
            return true;
        }

        private static bool TryLoadInputs(out DuckLayoutBoardData data, out DuckLayoutArt art, out string issue)
        {
            data = null;
            art = null;
            if (!File.Exists(ToAbsolutePath(BoardLayoutPath))) { issue = "Missing required " + BoardLayoutPath + "."; return false; }
            if (!DuckLayoutBoardData.TryParse(File.ReadAllText(ToAbsolutePath(BoardLayoutPath)), out data, out issue)) return false;
            var configuredHavens = data.rows.Where(row => row.haven).Select(row => row.space).OrderBy(space => space).ToArray();
            if (configuredHavens.Length != 8 || !configuredHavens.Contains(50))
            {
                issue = "Expected exactly eight havens, including the endpoint at space 50.";
                return false;
            }
            var oasis = data.rows.Single(row => row.space == 50);
            if (!oasis.haven || oasis.feathers != 2)
            {
                issue = "Endpoint 50 must be the oasis haven with its two Feather reward.";
                return false;
            }
            var required = new[]
            {
                "board.png", "well-grass.png", "well-wasteland.png", "well-haven-grass.png", "well-haven-wasteland.png",
                "endpoint-feathers.png", "feathers-two.png", "feather.png", "sleep.png", "twig.png",
                "dream-concept.png"
            };
            foreach (var file in required)
            {
                var assetPath = ArtDirectory + "/" + file;
                if (!File.Exists(ToAbsolutePath(assetPath))) { issue = "Missing required art: " + assetPath + "."; return false; }
            }
            art = new DuckLayoutArt
            {
                board = LoadSprite(ArtDirectory + "/board.png"),
                grassWell = LoadSprite(ArtDirectory + "/well-grass.png"),
                wastelandWell = LoadSprite(ArtDirectory + "/well-wasteland.png"),
                grassHavenWell = LoadSprite(ArtDirectory + "/well-haven-grass.png"),
                wastelandHavenWell = LoadSprite(ArtDirectory + "/well-haven-wasteland.png"),
                endpointFeathers = LoadSprite(ArtDirectory + "/endpoint-feathers.png"),
                feathers = LoadSprite(ArtDirectory + "/feathers-two.png"),
                feather = LoadSprite(ArtDirectory + "/feather.png"),
                sleep = LoadSprite(ArtDirectory + "/sleep.png"),
                twig = LoadSprite(ArtDirectory + "/twig.png")
            };
            art.LoadCrops(LoadCropManifest());
            art.duck = art.RequiredCrop("ducks.png", 0);
            art.zzz = art.RequiredCrop("zzz.png", 0);
            art.dreamNest = art.RequiredCrop("dream-concept.png", 0);
            if (art.board == null || art.grassWell == null || art.wastelandWell == null || art.grassHavenWell == null
                || art.wastelandHavenWell == null || art.endpointFeathers == null || art.feathers == null || art.feather == null
                || art.sleep == null || art.twig == null || art.duck == null || art.zzz == null || art.dreamNest == null)
            {
                issue = "A required image or alpha sprite crop could not be imported. Provide all 16 encounter crops plus ducks.png#0 and zzz.png#0.";
                return false;
            }
            if (!art.HasAllRequiredCrops())
            {
                issue = "The crop manifest must provide distinct crops for all 16 encounter fixtures, ducks.png#0, zzz.png#0, and dream-concept.png#0.";
                return false;
            }
            issue = null;
            return true;
        }

        private static Canvas BuildCanvas()
        {
            var root = new GameObject("Duck Layout Proof", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                typeof(GraphicRaycaster), typeof(Image));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ViewWidth, ViewHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = .5f;
            var image = root.GetComponent<Image>();
            image.color = Navy;
            image.raycastTarget = false;
            return canvas;
        }

        private static RectTransform BuildSafeArea(Transform parent)
        {
            var safe = TableUi.Rect("Safe Area", parent);
            TableUi.Fill(safe);
            safe.gameObject.AddComponent<Quackies.Unity.Presentation.SafeArea>();
            return safe;
        }

        private static RectTransform BuildViewport(RectTransform safe)
        {
            var viewport = TableUi.Rect("1133 x 744 Duck Layout Proof", safe);
            viewport.gameObject.AddComponent<FittedViewport>().Configure(safe);
            var paper = viewport.gameObject.AddComponent<Image>();
            paper.color = Navy;
            paper.raycastTarget = false;
            return viewport;
        }

        private static RectTransform BuildAdventure(RectTransform root, DuckLayoutBoardData data, DuckLayoutArt art,
            out DuckLayoutSpaceView[] spaces, out DuckLayoutProofPreview.TokenPresentation[] tokenPresentations,
            out Button dreamButton, out Button emptyButton, out Button occupiedButton,
            out TMP_Text status, out GameObject duckRest, out GameObject zzz, out GameObject featherTrail)
        {
            var adventure = TableUi.Rect("Adventure Layout", root);
            TableUi.Fill(adventure);
            var mat = Image("Adventure Paper Mat", adventure, Paper);
            TableUi.Fill(mat.rectTransform);
            var header = Panel("Compact Day Header", adventure, 13, 8, 1107, 40, Navy);
            var day = Label("Day", header, "DAY 4", 18, Cream, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(day.rectTransform, 16, 2, 88, 34);
            var eventLabel = Label("World event", header, "WORLD EVENT  •  Still Air", 15, Cream, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(eventLabel.rectTransform, 114, 4, 393, 30);
            emptyButton = TableUi.Button("Show empty wells", header, "Empty wells", Cream, Ink);
            TableUi.Place(emptyButton.GetComponent<RectTransform>(), 514, 5, 98, 30);
            ConfigureCompactButton(emptyButton);
            occupiedButton = TableUi.Button("Show occupied encounter sample", header, "Encounter fit", Lavender, Cream);
            TableUi.Place(occupiedButton.GetComponent<RectTransform>(), 619, 5, 117, 30);
            ConfigureCompactButton(occupiedButton);
            dreamButton = TableUi.Button("Dream tab", header, "Dream", new Color(.33f, .27f, .54f), Cream);
            TableUi.Place(dreamButton.GetComponent<RectTransform>(), 743, 5, 88, 30);
            ConfigureCompactButton(dreamButton);
            var exhaustion = Label("Exhaustion", header, "EXHAUSTION  3 / 5", 14, Cream, true, TextAlignmentOptions.MidlineRight);
            TableUi.Place(exhaustion.rectTransform, 840, 4, 248, 30);
            status = null;

            var projection = BoardProjection.From(data);
            var board = TableUi.Rect("Approved Board Art", adventure);
            TableUi.Place(board, projection.x, projection.y, projection.width, projection.height);
            var boardImage = Image("Approved Board Art", board, Color.white, art.board);
            boardImage.preserveAspect = true;
            TableUi.Fill(boardImage.rectTransform);
            var overlays = TableUi.Rect("Measured Board Overlays", board);
            TableUi.Fill(overlays);

            var result = new List<DuckLayoutSpaceView>();
            foreach (var row in data.rows.OrderBy(row => row.space))
                result.Add(BuildSpace(overlays, projection, data, row, art));
            spaces = result.ToArray();
            featherTrail = BuildFeatherTrail(overlays, projection, data, art);
            duckRest = BuildDuckRest(overlays, projection, data, art);
            zzz = BuildZzz(overlays, projection, data, art);
            tokenPresentations = BuildTokenFixtures(art);
            BuildBoardLegend(board, projection, art);

            return adventure;
        }

        private static DuckLayoutSpaceView BuildSpace(RectTransform overlayRoot, BoardProjection projection, DuckLayoutBoardData data,
            DuckLayoutBoardRow row, DuckLayoutArt art)
        {
            var scaleX = projection.width / data.boardWidth;
            var scaleY = projection.height / data.boardHeight;
            var wellWidth = data.wellWidth * scaleX;
            var wellHeight = data.wellHeight * scaleY;
            var rewardHeight = data.rewardHeight * scaleY;
            var position = row.Position(projection.width, projection.height);
            var root = TableUi.Rect("Space " + row.id, overlayRoot);
            TableUi.Place(root, position.x - wellWidth * .5f, position.y - wellHeight * .5f, wellWidth, wellHeight + rewardHeight + 2f);
            var hit = Image("Inspect " + row.id, root, Color.clear);
            TableUi.Fill(hit.rectTransform);
            hit.raycastTarget = true;
            var button = hit.gameObject.AddComponent<Button>();
            button.targetGraphic = hit;
            button.navigation = new Navigation { mode = Navigation.Mode.None };

            RectTransform wellRect = null;
            Image wellArtwork = null;
            if (row.space != 50)
            {
                var wasteland = string.Equals(row.biome, "wasteland", StringComparison.OrdinalIgnoreCase);
                var wellSprite = row.haven
                    ? (wasteland ? art.wastelandHavenWell : art.grassHavenWell)
                    : (wasteland ? art.wastelandWell : art.grassWell);
                var tint = row.haven
                    ? Color.white
                    : string.Equals(row.biome, "wasteland", StringComparison.OrdinalIgnoreCase) ? WastelandTint
                    : string.Equals(row.biome, "meadow", StringComparison.OrdinalIgnoreCase) ? MeadowTint : WetlandTint;
                var well = Image("Rest Well", root, tint, wellSprite);
                well.preserveAspect = true;
                TableUi.Place(well.rectTransform, 0, 0, wellWidth, wellHeight);
                well.raycastTarget = false;
                wellRect = well.rectTransform;
                wellArtwork = well;
            }
            else
            {
                // The native oasis is the single endpoint without a well. Its medallion remains part of the oasis binding.
                var footprint = Label("Oasis endpoint", root, "", 1, Color.clear, false, TextAlignmentOptions.Center);
                TableUi.Place(footprint.rectTransform, 0, 0, wellWidth, wellHeight);
                wellRect = footprint.rectTransform;
                wellArtwork = Image("Oasis Feather Medallion", root, Color.white, art.endpointFeathers);
                wellArtwork.preserveAspect = true;
            }

            var rewardWidth = wellWidth * .86f;
            var reward = Panel("Reward Row", root, (wellWidth - rewardWidth) * .5f, wellHeight + 1f, rewardWidth, rewardHeight,
                new Color(.12f, .12f, .22f, .88f));
            if (row.space == 50 && wellArtwork != null)
            {
                // Keep the painted oasis pool unobscured: the cohesive endpoint seal belongs beside its reward strip.
                var sealSize = Mathf.Min(rewardHeight * .9f, wellWidth * .24f);
                var sealX = (wellWidth + rewardWidth) * .5f + 2f;
                var sealY = wellHeight + 1f + (rewardHeight - sealSize) * .5f;
                TableUi.Place(wellArtwork.rectTransform, sealX, sealY, sealSize, sealSize);
            }
            var rewardScale = rewardWidth / 49f;
            var sleepIcon = Image("Moon", reward, Color.white, art.sleep);
            sleepIcon.preserveAspect = true;
            TableUi.Place(sleepIcon.rectTransform, rewardScale, (rewardHeight - 10f * rewardScale) * .5f, 10f * rewardScale, 10f * rewardScale);
            var sleepNumber = Label("Sleep number", reward, row.sleep.ToString(), 10f * rewardScale, Cream,
                true, TextAlignmentOptions.MidlineLeft);
            ConfigureRewardNumber(sleepNumber);
            TableUi.Place(sleepNumber.rectTransform, 12f * rewardScale, 0, 14f * rewardScale, rewardHeight);
            var twigIcon = Image("Twig", reward, Color.white, art.twig);
            twigIcon.preserveAspect = true;
            TableUi.Place(twigIcon.rectTransform, 27f * rewardScale, (rewardHeight - 10f * rewardScale) * .5f, 10f * rewardScale, 10f * rewardScale);
            var twigNumber = Label("Twig number", reward, row.twigs.ToString(), 10f * rewardScale, Cream,
                true, TextAlignmentOptions.MidlineLeft);
            ConfigureRewardNumber(twigNumber);
            TableUi.Place(twigNumber.rectTransform, 38f * rewardScale, 0, 8f * rewardScale, rewardHeight);
            var token = Image("Encounter overlay", root, new Color(.25f, .12f, .38f, .95f));
            token.preserveAspect = true;
            var tokenSize = row.haven
                ? Mathf.Min(wellWidth * .65f, wellHeight * .9f)
                : Mathf.Min(wellWidth * .8f, wellHeight);
            var tokenX = row.haven ? wellWidth * .04f : (wellWidth - tokenSize) * .5f;
            var tokenY = row.haven ? wellHeight * .04f : wellHeight - tokenSize;
            TableUi.Place(token.rectTransform, tokenX, tokenY, tokenSize, tokenSize);
            token.gameObject.SetActive(false);
            var view = root.gameObject.AddComponent<DuckLayoutSpaceView>();
            view.Configure(row.id, row.space, row.haven, row.space == 50, row.havenName, row.sleep, row.twigs, row.feathers, wellRect,
                reward, button, wellArtwork, row.feathers > 1 ? art.feathers : art.feather, token);
            return view;
        }

        private static void BuildBoardLegend(RectTransform board, BoardProjection projection, DuckLayoutArt art)
        {
            var legend = Panel("Board reward legend", board, 22f, projection.height - 46f, projection.width - 44f, 30f,
                new Color(1f, .95f, .82f, .88f));
            var moon = Image("Legend moon", legend, Color.white, art.sleep);
            moon.preserveAspect = true;
            TableUi.Place(moon.rectTransform, 15, 7, 16, 16);
            var sleep = Label("Legend Sleep", legend, "Sleep", 14, Ink, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(sleep.rectTransform, 35, 2, 50, 26);
            var twig = Image("Legend twig", legend, Color.white, art.twig);
            twig.preserveAspect = true;
            TableUi.Place(twig.rectTransform, 94, 7, 16, 16);
            var twigs = Label("Legend Twigs", legend, "Twigs", 14, Ink, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(twigs.rectTransform, 114, 2, 52, 26);
            var feather = Image("Legend Feather", legend, Color.white, art.feather);
            feather.preserveAspect = true;
            TableUi.Place(feather.rectTransform, 178, 7, 16, 16);
            var trail = Label("Legend permanent trail", legend, "Permanent trail", 14, Ink, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(trail.rectTransform, 198, 2, 116, 26);
            var instruction = Label("Rest instruction", legend, "Rest where you land. Tap a space to inspect.", 14, Ink, true,
                TextAlignmentOptions.MidlineLeft);
            TableUi.Place(instruction.rectTransform, 338, 2, projection.width - 390f, 26);
        }

        private static GameObject BuildFeatherTrail(RectTransform overlayRoot, BoardProjection projection, DuckLayoutBoardData data, DuckLayoutArt art)
        {
            var trail = TableUi.Rect("Permanent Feather Trail • 3", overlayRoot);
            TableUi.Fill(trail);
            for (var index = 0; index < 3; index++)
            {
                var row = data.rows.Single(item => item.space == index + 1);
                var point = row.Position(projection.width, projection.height);
                var feather = Image("Permanent Feather on space " + (index + 1), trail, Color.white, art.feather);
                feather.preserveAspect = true;
                TableUi.Place(feather.rectTransform, point.x - 9f, point.y - 27f, 18, 18);
            }
            return trail.gameObject;
        }

        private static GameObject BuildDuckRest(RectTransform overlayRoot, BoardProjection projection, DuckLayoutBoardData data, DuckLayoutArt art)
        {
            var row = data.rows.First(item => item.space == 32);
            var point = row.Position(projection.width, projection.height);
            var duck = Image("Duck resting at space 32", overlayRoot, Color.white, art.duck);
            duck.preserveAspect = true;
            TableUi.Place(duck.rectTransform, point.x - 18, point.y - 23, 36, 36);
            return duck.gameObject;
        }

        private static GameObject BuildZzz(RectTransform overlayRoot, BoardProjection projection, DuckLayoutBoardData data, DuckLayoutArt art)
        {
            var row = data.rows.First(item => item.space == 4);
            var point = row.Position(projection.width, projection.height);
            var marker = Image("Temporary Most Rested zzz on space 4", overlayRoot, Color.white, art.zzz);
            marker.preserveAspect = true;
            TableUi.Place(marker.rectTransform, point.x - 15, point.y - 19, 30, 30);
            return marker.gameObject;
        }

        private static DuckLayoutProofPreview.TokenPresentation[] BuildTokenFixtures(DuckLayoutArt art)
        {
            return DuckLayoutFixtures.Tokens.Select(token => new DuckLayoutProofPreview.TokenPresentation
            {
                title = token.title,
                space = token.space,
                sprite = art.RequiredCrop(token.sourceAsset, token.cropIndex)
            }).ToArray();
        }

        private static RectTransform BuildDream(RectTransform root, DuckLayoutArt art, out DuckLayoutOfferView[] offers, out Button adventureButton)
        {
            var dream = TableUi.Rect("Dream Concept B", root);
            TableUi.Fill(dream);
            var mat = Image("Navy Lavender Cream Paper Mat", dream, Navy);
            TableUi.Fill(mat.rectTransform);
            var title = Label("Dream heading", dream, "DREAM CHOICES", 31, Cream, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(title.rectTransform, 28, 17, 390, 42);
            var bands = Label("Shared Night bands", dream, "1–3  LEVEL 1    •    4–6  LEVEL 2    •    7–10  LEVEL 3", 14,
                Cream, false, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(bands.rectTransform, 31, 58, 570, 24);
            var preview = Label("Layout preview", dream, "LAYOUT PREVIEW", 10, Cream, true, TextAlignmentOptions.MidlineRight);
            TableUi.Place(preview.rectTransform, 764, 25, 144, 20);
            adventureButton = TableUi.Button("View adventure", dream, "View adventure", Cream, Ink);
            TableUi.Place(adventureButton.GetComponent<RectTransform>(), 922, 20, 174, 42);

            var nest = Panel("Nest progression and resources", dream, 25, 101, 330, 616, Paper);
            var nestHeading = Label("Nest heading", nest, "YOUR NEST", 21, Navy, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(nestHeading.rectTransform, 19, 13, 135, 30);
            var current = Panel("Current Night", nest, 151, 12, 160, 34, Lavender);
            var currentText = Label("Current Night label", current, "NIGHT 4 • LEVEL 2 • 2 CHOICES", 11, Cream, true, TextAlignmentOptions.Center);
            TableUi.Fill(currentText.rectTransform, 5);
            var nestArt = Image("Nest illustration crop", nest, Color.white, art.dreamNest);
            nestArt.preserveAspect = true;
            TableUi.Place(nestArt.rectTransform, 14, 54, 302, 167);
            var progression = Label("Nest progression", nest, "LEVEL 1   1–3\nLEVEL 2   4–6\nLEVEL 3   7–10", 13,
                Ink, true, TextAlignmentOptions.Center);
            TableUi.Place(progression.rectTransform, 28, 225, 274, 52);
            var resources = Panel("Frozen and remaining Sleep", nest, 18, 291, 294, 121, new Color(.77f, .72f, .88f));
            var frozenMoon = Image("Frozen Sleep moon", resources, Color.white, art.sleep);
            frozenMoon.preserveAspect = true;
            TableUi.Place(frozenMoon.rectTransform, 15, 14, 34, 34);
            var frozenText = Label("Frozen Sleep", resources, "18", 30, Navy, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(frozenText.rectTransform, 54, 9, 52, 43);
            var frozenCaption = Label("Frozen explanation", resources, "EARNED SLEEP\nMost Rested", 11, Navy, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(frozenCaption.rectTransform, 108, 13, 166, 37);
            var remainingMoon = Image("Remaining Sleep moon", resources, Color.white, art.sleep);
            remainingMoon.preserveAspect = true;
            TableUi.Place(remainingMoon.rectTransform, 15, 67, 34, 34);
            var remainingText = Label("Remaining Sleep", resources, "18", 30, Navy, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(remainingText.rectTransform, 54, 62, 52, 43);
            var remainingCaption = Label("Remaining explanation", resources, "SLEEP LEFT\nDream choices", 11, Navy, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(remainingCaption.rectTransform, 108, 66, 166, 37);
            var twigs = Panel("Twig score", nest, 18, 429, 294, 111, Cream);
            var twigIcon = Image("Twig score icon", twigs, Color.white, art.twig);
            twigIcon.preserveAspect = true;
            TableUi.Place(twigIcon.rectTransform, 23, 25, 55, 55);
            var twigText = Label("Twig total", twigs, "42", 47, Navy, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(twigText.rectTransform, 89, 18, 98, 68);
            var twigCaption = Label("Twig caption", twigs, "TWIGS", 14, Ink, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(twigCaption.rectTransform, 188, 41, 74, 27);
            var note = Label("Adventure marker note", nest, "Permanent Feather trail and temporary zzz live on Adventure.", 12, Ink,
                false, TextAlignmentOptions.Center);
            TableUi.Place(note.rectTransform, 21, 555, 285, 40);

            var shop = Panel("All eleven Dream offers", dream, 375, 101, 733, 616, new Color(.18f, .15f, .35f));
            var result = new List<DuckLayoutOfferView>();
            for (var index = 0; index < DuckLayoutFixtures.Offers.Length; index++)
            {
                var col = index % 4;
                var row = index / 4;
                result.Add(BuildOffer(shop, art, DuckLayoutFixtures.Offers[index], 16 + col * 178, 16 + row * 196, 165, 181));
            }
            offers = result.ToArray();
            return dream;
        }

        private static DuckLayoutOfferView BuildOffer(RectTransform parent, DuckLayoutArt art, DuckLayoutOfferFixture fixture, float x, float y, float width, float height)
        {
            var card = Panel("Offer " + fixture.id, parent, x, y, width, height, Paper);
            var image = card.GetComponent<Image>();
            image.raycastTarget = true;
            var button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            var name = Label("Name", card, fixture.family, 15, Navy, true, TextAlignmentOptions.Center);
            TableUi.Place(name.rectTransform, 9, 8, width - 18, 28);
            var token = Image("Offer encounter art", card, Color.white, art.RequiredCrop(fixture.sourceAsset, fixture.cropIndex));
            token.preserveAspect = true;
            TableUi.Place(token.rectTransform, 46, 36, 72, 72);
            var detail = Label("Movement or Reeds quantity", card, fixture.detail, 17, Ink, true, TextAlignmentOptions.Center);
            TableUi.Place(detail.rectTransform, 10, 112, width - 20, 24);
            var price = Panel("Approved Sleep price", card, 11, height - 41, width - 22, 29, Lavender);
            var priceMoon = Image("Price moon", price, Color.white, art.sleep);
            priceMoon.preserveAspect = true;
            TableUi.Place(priceMoon.rectTransform, 35, 4, 21, 21);
            var priceText = Label("Price", price, fixture.sleepPrice.ToString(), 16, Cream, true, TextAlignmentOptions.MidlineLeft);
            TableUi.Place(priceText.rectTransform, 61, 1, 45, 27);
            var view = card.gameObject.AddComponent<DuckLayoutOfferView>();
            view.Configure(fixture, button, token);
            return view;
        }

        private static GameObject BuildInspection(RectTransform root, DuckLayoutArt art, out TMP_Text title, out TMP_Text detail,
            out TMP_Text sleepValue, out TMP_Text twigValue, out TMP_Text featherValue, out Image previewArt, out Image sleepIcon,
            out Image twigIcon, out Image featherIcon, out Button close)
        {
            var shade = Image("Inspection Shade", root, new Color(.02f, .02f, .08f, .86f));
            TableUi.Fill(shade.rectTransform);
            shade.raycastTarget = true;
            var card = Panel("Large Inspection Card", shade.rectTransform, 278, 132, 577, 480, Cream);
            title = Label("Inspection title", card, "", 29, Navy, true, TextAlignmentOptions.Center);
            TableUi.Place(title.rectTransform, 35, 23, 507, 48);
            previewArt = Image("Inspection artwork", card, Color.white);
            previewArt.preserveAspect = true;
            TableUi.Place(previewArt.rectTransform, 46, 91, 184, 150);
            var rewardPanel = Panel("Inspection rewards", card, 251, 96, 278, 92, new Color(.76f, .70f, .87f));
            sleepIcon = Image("Inspection moon", rewardPanel, Color.white, art.sleep);
            sleepIcon.preserveAspect = true;
            TableUi.Place(sleepIcon.rectTransform, 20, 14, 24, 24);
            twigIcon = Image("Inspection twig", rewardPanel, Color.white, art.twig);
            twigIcon.preserveAspect = true;
            TableUi.Place(twigIcon.rectTransform, 104, 14, 24, 24);
            featherIcon = Image("Inspection feather", rewardPanel, Color.white, art.feathers);
            featherIcon.preserveAspect = true;
            TableUi.Place(featherIcon.rectTransform, 193, 10, 34, 30);
            sleepValue = Label("Inspection Sleep value", rewardPanel, "", 18, Navy, true, TextAlignmentOptions.Center);
            sleepValue.textWrappingMode = TextWrappingModes.NoWrap;
            TableUi.Place(sleepValue.rectTransform, 15, 48, 34, 30);
            twigValue = Label("Inspection Twig value", rewardPanel, "", 18, Navy, true, TextAlignmentOptions.Center);
            twigValue.textWrappingMode = TextWrappingModes.NoWrap;
            TableUi.Place(twigValue.rectTransform, 99, 48, 34, 30);
            featherValue = Label("Inspection Feather value", rewardPanel, "", 18, Navy, true, TextAlignmentOptions.Center);
            featherValue.textWrappingMode = TextWrappingModes.NoWrap;
            TableUi.Place(featherValue.rectTransform, 192, 48, 40, 30);
            detail = Label("Inspection detail", card, "", 16, Ink, false, TextAlignmentOptions.Center);
            TableUi.Place(detail.rectTransform, 43, 268, 491, 82);
            close = TableUi.Button("Close inspection", card, "Close", Navy, Cream);
            TableUi.Place(close.GetComponent<RectTransform>(), 202, 401, 173, 48);
            return shade.gameObject;
        }

        private static RectTransform Panel(string name, Transform parent, float x, float y, float width, float height, Color color)
        {
            var panel = Image(name, parent, color);
            var rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            if (rounded != null)
            {
                panel.sprite = rounded;
                panel.type = UnityEngine.UI.Image.Type.Sliced;
            }
            TableUi.Place(panel.rectTransform, x, y, width, height);
            return panel.rectTransform;
        }

        private static Image Image(string name, Transform parent, Color color, Sprite sprite = null)
        {
            var image = TableUi.Image(name, parent, color, sprite);
            image.raycastTarget = false;
            image.useSpriteMesh = sprite != null;
            return image;
        }

        private static TMP_Text Label(string name, Transform parent, string text, float size, Color color, bool bold,
            TextAlignmentOptions alignment)
        {
            var label = TableUi.Text(name, parent, text, size, color, bold);
            label.alignment = alignment;
            label.enableAutoSizing = false;
            return label;
        }

        private static void ConfigureCompactButton(Button button)
        {
            var label = button.GetComponentInChildren<TMP_Text>();
            if (label == null) return;
            label.fontSize = 13f;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            TableUi.Fill(label.rectTransform, 3f);
        }

        private static void ConfigureRewardNumber(TMP_Text label)
        {
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.overflowMode = TextOverflowModes.Overflow;
        }

        private static void BuildEventSystem()
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.GetComponent<EventSystem>().sendNavigationEvents = false;
        }

        private static void BuildBackgroundCamera()
        {
            var camera = new GameObject("Layout Proof Camera", typeof(Camera));
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor = Navy;
        }

        private static Sprite LoadSprite(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null) ConfigureTextureImporter(importer, SpriteImportMode.Single, true);
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Sprite LoadOptionalSprite(string path) => File.Exists(ToAbsolutePath(path)) ? LoadSprite(path) : null;
        private static string ToAbsolutePath(string assetPath) => Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));

        private static DuckLayoutCropManifest LoadCropManifest()
        {
            if (!File.Exists(ToAbsolutePath(CropManifestPath))) return null;
            return JsonUtility.FromJson<DuckLayoutCropManifest>(File.ReadAllText(ToAbsolutePath(CropManifestPath)));
        }

        private static void ConfigureTextureImporter(TextureImporter importer, SpriteImportMode mode, bool saveAndReimport)
        {
            var changed = importer.textureType != TextureImporterType.Sprite || importer.spriteImportMode != mode
                || !importer.alphaIsTransparency || importer.textureCompression != TextureImporterCompression.Uncompressed
                || importer.mipmapEnabled || importer.filterMode != FilterMode.Bilinear || importer.wrapMode != TextureWrapMode.Clamp
                || importer.maxTextureSize != HighResolutionTextureSize || importer.npotScale != TextureImporterNPOTScale.None;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = mode;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.maxTextureSize = HighResolutionTextureSize;
            importer.npotScale = TextureImporterNPOTScale.None;
            if (saveAndReimport && changed) importer.SaveAndReimport();
        }

        private static string AuditOpenScene()
        {
            Canvas.ForceUpdateCanvases();
            var controller = UnityEngine.Object.FindObjectOfType<DuckLayoutProofPreview>();
            var issues = new List<string>();
            if (controller == null) issues.Add("Missing DuckLayoutProofPreview controller.");
            else
            {
                var spaces = controller.Spaces ?? Array.Empty<DuckLayoutSpaceView>();
                if (spaces.Length != 50) issues.Add("Expected 50 serialized space views, found " + spaces.Length + ".");
                var ids = new HashSet<string>();
                var numbers = new HashSet<int>();
                var havens = new HashSet<int>();
                foreach (var space in spaces)
                {
                    if (space == null) { issues.Add("A serialized space reference is null."); continue; }
                    if (!ids.Add(space.StableId)) issues.Add("Duplicate stable ID " + space.StableId + ".");
                    numbers.Add(space.Space);
                    if (space.IsHaven) havens.Add(space.Space);
                    var hasWell = space.transform.Find("Rest Well") != null;
                    if (space.Space == 50 && hasWell)
                        issues.Add("The native oasis endpoint must not have a well.");
                    if (space.Space < 50 && !hasWell)
                        issues.Add("Every nonendpoint space, including havens, needs a rest well.");
                }
                var serializedSpaces = spaces.Where(space => space != null).ToArray();
                foreach (var source in serializedSpaces)
                {
                    var sourceId = source.StableId + " (space " + source.Space + ")";
                    var well = source.WellRect == null ? default(Rect?) : WorldRect(source.WellRect);
                    var token = source.TokenRect == null ? default(Rect?) : WorldRect(source.TokenRect);
                    foreach (var target in serializedSpaces)
                    {
                        if (target.RewardRect == null) continue;
                        var reward = WorldRect(target.RewardRect);
                        var targetId = target.StableId + " (space " + target.Space + ")";
                        if (well.HasValue && Intersects(well.Value, reward))
                            issues.Add(sourceId + " well overlaps " + targetId + " reward row.");
                        if (token.HasValue && Intersects(token.Value, reward))
                            issues.Add(sourceId + " encounter token overlaps " + targetId + " reward row.");
                    }
                    if (source.IsHaven && source.Space != 50 && well.HasValue && token.HasValue
                        && Intersects(token.Value, HavenSealBounds(well.Value)))
                        issues.Add(sourceId + " encounter token overlaps its integrated Feather seal.");
                }
                if (!numbers.SetEquals(Enumerable.Range(1, 50))) issues.Add("Spaces are not exactly 1–50.");
                if (havens.Count != 8 || !havens.Contains(50))
                    issues.Add("Scene must serialize exactly eight havens, including endpoint 50.");
                var havenLinks = UnityEngine.Object.FindObjectsOfType<Image>(true).Count(image => image.name.StartsWith("Haven Link ", StringComparison.Ordinal));
                if (havenLinks != 0) issues.Add("Haven links must not cross the board artwork.");
                var floatingFeatherBadges = UnityEngine.Object.FindObjectsOfType<Image>(true)
                    .Count(image => image.name == "Feather badge backing" || image.name == "Feather badge");
                if (floatingFeatherBadges != 0) issues.Add("Haven Feather rewards must be integrated into their tile artwork.");
                var wells = UnityEngine.Object.FindObjectsOfType<Image>(true).Count(image => image.name == "Rest Well");
                var oasisBindings = UnityEngine.Object.FindObjectsOfType<TMP_Text>(true).Count(text => text.name == "Oasis endpoint");
                var oasisMedallions = UnityEngine.Object.FindObjectsOfType<Image>(true).Count(image => image.name == "Oasis Feather Medallion");
                var rewardRows = UnityEngine.Object.FindObjectsOfType<Image>(true).Count(image => image.name == "Reward Row");
                var endpoints = spaces.Count(space => space != null && space.UsesBoardArt);
                if (wells != 49 || endpoints != 1 || oasisBindings != 1 || oasisMedallions != 1 || rewardRows != 50)
                    issues.Add("Scene needs 49 wells, one native oasis medallion, and 50 reward rows.");
                var offers = controller.Offers ?? Array.Empty<DuckLayoutOfferView>();
                if (offers.Length != 11) issues.Add("Expected 11 serialized offers.");
                var expectedOfferIds = new HashSet<string>(DuckLayoutFixtures.Offers.Select(offer => offer.id));
                var actualOfferIds = new HashSet<string>(offers.Where(offer => offer != null).Select(offer => offer.OfferId));
                var offerNames = new HashSet<string>(offers.Where(offer => offer != null).Select(offer => offer.DisplayName));
                if (!actualOfferIds.SetEquals(expectedOfferIds) || offerNames.Count != 11)
                    issues.Add("Dream offers need the exact 11 unique offer IDs and display names.");
                var tokens = controller.TokenPresentations ?? Array.Empty<DuckLayoutProofPreview.TokenPresentation>();
                if (tokens.Length != 16)
                    issues.Add("Expected 16 token references.");
                else if (tokens.Any(token => token == null || token.sprite == null)
                    || tokens.Select(token => token.sprite).Distinct().Count() != 16)
                    issues.Add("All 16 token references need distinct non-null cropped sprites.");
            }
            foreach (var text in UnityEngine.Object.FindObjectsOfType<TMP_Text>(true))
                if (text.isTextOverflowing) issues.Add("TMP overflow: " + HierarchyPath(text.transform) + ".");
            var digest = HierarchyDigest(SceneManager.GetActiveScene());
            var body = new StringBuilder();
            body.AppendLine("# Duck Layout Audit");
            body.AppendLine();
            body.AppendLine("Scene: " + SceneManager.GetActiveScene().path);
            body.AppendLine("Hierarchy digest: `" + digest + "`");
            body.AppendLine("Status: " + (issues.Count == 0 ? "PASS" : "FAIL"));
            if (issues.Count > 0)
            {
                body.AppendLine();
                body.AppendLine("Issues:");
                foreach (var issue in issues.Distinct()) body.AppendLine("- " + issue);
            }
            return body.ToString();
        }

        private static Rect WorldRect(RectTransform transform)
        {
            var corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            return Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
        }

        private static bool Intersects(Rect a, Rect b) => a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin;

        private static Rect HavenSealBounds(Rect well)
        {
            const float sealCenterX = .85f;
            const float sealCenterY = .80f;
            const float sealRadiusByWellWidth = .11f;
            var radius = well.width * sealRadiusByWellWidth;
            var centerX = well.xMin + well.width * sealCenterX;
            var centerY = well.yMax - well.height * sealCenterY;
            return Rect.MinMaxRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);
        }

        private static string HierarchyDigest(Scene scene)
        {
            var rows = new List<string>();
            foreach (var root in scene.GetRootGameObjects().OrderBy(item => item.name)) AppendHierarchy(root.transform, "", rows);
            using (var sha = System.Security.Cryptography.SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", rows)))).Replace("-", "").ToLowerInvariant();
        }

        private static void AppendHierarchy(Transform current, string parent, List<string> rows)
        {
            var path = parent + "/" + current.name + "[" + current.GetSiblingIndex() + "]";
            rows.Add(path + ":" + string.Join(",", current.GetComponents<Component>().Where(item => item != null).Select(item => item.GetType().FullName).OrderBy(item => item)));
            for (var index = 0; index < current.childCount; index++) AppendHierarchy(current.GetChild(index), path, rows);
        }

        private static string HierarchyPath(Transform current)
        {
            var parts = new List<string>();
            while (current != null) { parts.Add(current.name); current = current.parent; }
            parts.Reverse();
            return string.Join("/", parts);
        }

        private sealed class DuckLayoutArt
        {
            public Sprite board;
            public Sprite grassWell;
            public Sprite wastelandWell;
            public Sprite grassHavenWell;
            public Sprite wastelandHavenWell;
            public Sprite endpointFeathers;
            public Sprite feathers;
            public Sprite feather;
            public Sprite sleep;
            public Sprite twig;
            public Sprite duck;
            public Sprite zzz;
            public Sprite dreamNest;
            private readonly Dictionary<string, Sprite> crops = new Dictionary<string, Sprite>();

            public void LoadCrops(DuckLayoutCropManifest manifest)
            {
                if (manifest == null || manifest.entries == null || manifest.entries.Length == 0) return;
                foreach (var group in manifest.entries.Where(entry => entry != null && !string.IsNullOrWhiteSpace(entry.asset))
                             .GroupBy(entry => entry.asset))
                {
                    var path = ArtDirectory + "/" + group.Key;
                    if (!File.Exists(ToAbsolutePath(path)))
                    {
                        Debug.LogWarning("Duck layout crop manifest references missing " + path + ".");
                        continue;
                    }
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (texture == null || importer == null) continue;
                    importer.GetSourceTextureWidthAndHeight(out var sourceWidth, out var sourceHeight);
                    ConfigureTextureImporter(importer, SpriteImportMode.Multiple, false);
                    var settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    settings.spriteMeshType = SpriteMeshType.Tight;
                    importer.SetTextureSettings(settings);
                    var factories = new SpriteDataProviderFactories();
                    factories.Init();
                    var dataProvider = factories.GetSpriteEditorDataProviderFromObject(importer);
                    if (dataProvider == null)
                    {
                        Debug.LogWarning("No Sprite Editor data provider is available for " + path + ".");
                        continue;
                    }
                    dataProvider.InitSpriteEditorDataProvider();
                    var existing = dataProvider.GetSpriteRects().ToDictionary(rect => rect.name, rect => rect);
                    var spriteRects = new List<SpriteRect>();
                    var outlines = new Dictionary<GUID, List<Vector2[]>>();
                    foreach (var crop in group.OrderBy(item => item.index))
                    {
                        if (crop.width <= 0 || crop.height <= 0 || crop.x < 0 || crop.y < 0
                            || crop.x + crop.width > sourceWidth || crop.y + crop.height > sourceHeight)
                        {
                            Debug.LogWarning("Ignoring invalid crop " + group.Key + " #" + crop.index + ".");
                            continue;
                        }
                        var cropName = CropName(group.Key, crop.index);
                        if (!existing.TryGetValue(cropName, out var spriteRect))
                            spriteRect = new SpriteRect { name = cropName, spriteID = StableGuid(group.Key, crop.index) };
                        spriteRect.rect = new Rect(crop.x, sourceHeight - crop.y - crop.height, crop.width, crop.height);
                        spriteRect.alignment = SpriteAlignment.Center;
                        spriteRect.pivot = new Vector2(.5f, .5f);
                        spriteRects.Add(spriteRect);
                        if (crop.outline != null && crop.outline.Length >= 3 && crop.outline.All(point => point != null))
                        {
                            // Manifest points are relative to the crop's top-left; the Sprite Editor expects centred, bottom-origin points.
                            outlines[spriteRect.spriteID] = new List<Vector2[]>
                            {
                                crop.outline.Select(point => new Vector2(point.x - crop.width * .5f, crop.height * .5f - point.y)).ToArray()
                            };
                        }
                    }
                    if (spriteRects.Count == 0) continue;
                    dataProvider.SetSpriteRects(spriteRects.ToArray());
                    var nameIds = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
                    if (nameIds != null)
                        nameIds.SetNameFileIdPairs(spriteRects.Select(rect => new SpriteNameFileIdPair(rect.name, rect.spriteID)));
                    var outlineProvider = dataProvider.GetDataProvider<ISpriteOutlineDataProvider>();
                    if (outlineProvider != null)
                    {
                        foreach (var entry in outlines) outlineProvider.SetOutlines(entry.Key, entry.Value);
                    }
                    dataProvider.Apply();
                    importer.SaveAndReimport();
                    foreach (var sprite in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>())
                    {
                        var entry = group.FirstOrDefault(item => CropName(group.Key, item.index) == sprite.name);
                        if (entry != null) crops[CropKey(group.Key, entry.index)] = sprite;
                    }
                }
            }

            public Sprite RequiredCrop(string sourceAsset, int cropIndex)
            {
                if (crops.TryGetValue(CropKey(sourceAsset, cropIndex), out var crop)) return crop;
                return null;
            }

            public bool HasAllRequiredCrops()
            {
                var required = DuckLayoutFixtures.Tokens.Select(token => RequiredCrop(token.sourceAsset, token.cropIndex)).ToArray();
                var support = new[] { RequiredCrop("ducks.png", 0), RequiredCrop("zzz.png", 0), RequiredCrop("dream-concept.png", 0) };
                return required.Length == 16 && required.All(sprite => sprite != null) && required.Distinct().Count() == 16
                    && support.All(sprite => sprite != null);
            }

            private static string CropKey(string asset, int index) => asset + "#" + index;
            private static string CropName(string asset, int index) => "ducklayout-" + Path.GetFileNameWithoutExtension(asset) + "-" + index.ToString("00");
            private static GUID StableGuid(string asset, int index)
            {
                using (var hash = System.Security.Cryptography.MD5.Create())
                {
                    var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes("quackies-duck-layout/" + asset + "#" + index));
                    return new GUID(BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant());
                }
            }
        }

        private readonly struct BoardProjection
        {
            public readonly float x;
            public readonly float y;
            public readonly float width;
            public readonly float height;

            private BoardProjection(float positionX, float positionY, float displayWidth, float displayHeight)
            {
                x = positionX;
                y = positionY;
                width = displayWidth;
                height = displayHeight;
            }

            public static BoardProjection From(DuckLayoutBoardData data)
            {
                var availableWidth = ViewWidth - BoardHorizontalInset * 2f;
                var availableHeight = ViewHeight - BoardY - BoardBottomInset;
                var scale = Mathf.Min(availableWidth / data.boardWidth, availableHeight / data.boardHeight);
                var width = data.boardWidth * scale;
                var height = data.boardHeight * scale;
                return new BoardProjection((ViewWidth - width) * .5f, BoardY, width, height);
            }
        }
    }
}
