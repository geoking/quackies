using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Tokens;
using Quackies.Unity;
using Quackies.Unity.Art;
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
    /// <summary>Deterministically authors the only initial-playable scene from code.</summary>
    public static class InitialSceneBuilder
    {
        public const string ScenePath = "Assets/Scenes/QuackiesInitialScene.unity";

        [MenuItem("Quackies/Build and Play Initial Scene")]
        public static void BuildAndPlayInitialScene()
        {
            if (!TryBuildInitialScene()) return;
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Quackies/Build Initial Scene")]
        public static void BuildInitialScene()
        {
            TryBuildInitialScene();
        }

        private static bool TryBuildInitialScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("Exit Play mode before rebuilding the Quackies initial scene.");
                return false;
            }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;
            var catalog = QuackiesArtImporter.BuildCatalog();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            BuildEventSystem();
            BuildBackgroundCamera();
            var canvas = BuildCanvas();
            var safe = BuildSafeArea(canvas.transform);
            var viewport = BuildViewport(safe);
            var presenter = BuildTable(viewport, catalog);
            ConfigureBuildSettings();
            EditorSceneManager.SaveScene(scene, ScenePath);
            Selection.activeGameObject = presenter.gameObject;
            AssetDatabase.SaveAssets();
            return true;
        }

        private static Canvas BuildCanvas()
        {
            var root = new GameObject("Quackies Table", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(Image));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(FittedViewport.Width, FittedViewport.Height);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = .5f;
            var background = root.GetComponent<Image>();
            background.color = TableTheme.Background;
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

        private static RectTransform BuildViewport(RectTransform safe)
        {
            var viewport = TableUi.Rect("1133 x 744 Tabletop", safe);
            viewport.gameObject.AddComponent<FittedViewport>().Configure(safe);
            var background = viewport.gameObject.AddComponent<Image>();
            background.color = TableTheme.Background;
            background.raycastTarget = true;
            return viewport;
        }

        private static MatchPresenter BuildTable(RectTransform root, QuackiesArtCatalog catalog)
        {
            var header = Panel("Header", root, 20, 16, 1093, 78, TableTheme.Panel);
            var title = TableUi.Text("Title", header, "QUACKIES", 34, TableTheme.Gold, true);
            TableUi.Place(title.rectTransform, 20, 10, 300, 42);
            var subtitle = TableUi.Text("Subtitle", header, "THE QUACKS OF QUEDLINBURG", 14, TableTheme.Muted, true);
            TableUi.Place(subtitle.rectTransform, 23, 48, 380, 22);
            var round = TableUi.Text("Round", header, "ROUND 1 / 9", 22, TableTheme.Ink, true);
            round.alignment = TextAlignmentOptions.Center;
            TableUi.Place(round.rectTransform, 420, 18, 170, 40);
            var phase = TableUi.Text("Phase", header, "BREWING", 18, TableTheme.Teal, true);
            phase.alignment = TextAlignmentOptions.Center;
            TableUi.Place(phase.rectTransform, 600, 22, 180, 34);

            var presenterObject = new GameObject("Match Presenter", typeof(RectTransform), typeof(MatchPresenter));
            presenterObject.transform.SetParent(root, false);
            var presenter = presenterObject.GetComponent<MatchPresenter>();
            var restart = TableUi.Button("Restart", header, "Restart", TableTheme.Raised, TableTheme.Ink);
            TableUi.Place(restart.GetComponent<RectTransform>(), 967, 14, 106, 48);

            var human = BuildPot("Your Pot", root, catalog, true, 20, 112, 655, 510, out var bagContents);
            var opponent = BuildPot("Rival Pot", root, catalog, false, 702, 112, 411, 242, out _);
            var eventButton = BuildEventCard(root, catalog, out var eventArtwork, out var eventTitle);
            var actionPanel = BuildActions(root, out var footer);
            var status = TableUi.Text("Status", root, "Choose an available action.", 16, TableTheme.Ink, false);
            status.alignment = TextAlignmentOptions.Center;
            TableUi.Place(status.rectTransform, 702, 590, 411, 24);
            var log = TableUi.Text("Game Log", root, "", 13, TableTheme.Muted, false);
            log.alignment = TextAlignmentOptions.TopLeft;
            log.enableAutoSizing = true;
            log.fontSizeMin = 9;
            log.fontSizeMax = 13;
            TableUi.Place(log.rectTransform, 702, 616, 411, 64);
            BuildIngredientShelf(root, catalog, presenter);
            var modal = BuildModal(root);
            presenter.Configure(catalog, human, opponent, actionPanel, footer, modal, round, phase, status, log,
                bagContents, restart, eventButton, eventArtwork, eventTitle);
            return presenter;
        }

        private static PotView BuildPot(string name, RectTransform root, QuackiesArtCatalog catalog, bool human,
            float x, float y, float width, float height, out TMP_Text bagContents)
        {
            var panel = Panel(name, root, x, y, width, height, TableTheme.Panel);
            var summary = TableUi.Text("Summary", panel, name, human ? 17 : 14, TableTheme.Ink, true);
            summary.alignment = TextAlignmentOptions.TopLeft;
            TableUi.Place(summary.rectTransform, 16, 12, width - 32, human ? 42 : 40);
            bagContents = null;
            if (human)
            {
                bagContents = TableUi.Text("Own Bag", panel, "OWN BAG", 11, TableTheme.Muted, false);
                TableUi.Place(bagContents.rectTransform, 16, 56, width - 32, 30);
            }
            var image = TableUi.Image("Cauldron", panel, Color.white, human ? catalog.HumanCauldron : catalog.OpponentCauldron);
            image.preserveAspect = true;
            TableUi.Place(image.rectTransform, human ? 10 : 76, human ? 94 : 47, human ? width - 20 : width - 152,
                human ? height - 104 : height - 57);
            // Preserve the placed rectangle while aligning the Image's preserve-aspect
            // calculation with the centred marker layer and catalog fitted rectangle.
            var imageRect = image.rectTransform;
            imageRect.pivot = new Vector2(.5f, .5f);
            imageRect.anchoredPosition += new Vector2(imageRect.sizeDelta.x * .5f, -imageRect.sizeDelta.y * .5f);
            var markers = TableUi.Rect("Physical Track Markers", image.transform);
            TableUi.Fill(markers);
            var view = panel.gameObject.AddComponent<PotView>();
            view.Configure(catalog, human, image, markers, summary);
            return view;
        }

        private static Button BuildEventCard(RectTransform root, QuackiesArtCatalog catalog, out Image artwork, out TMP_Text title)
        {
            var panel = Panel("Fortune", root, 702, 370, 150, 212, TableTheme.Raised);
            var button = TableUi.Button("Active Fortune", panel, "Active\nfortune card", Color.white, TableTheme.Background);
            artwork = button.GetComponent<Image>();
            artwork.sprite = catalog.CardBack;
            artwork.preserveAspect = true;
            title = button.GetComponentInChildren<TMP_Text>();
            TableUi.Place(button.GetComponent<RectTransform>(), 12, 12, 126, 188);
            return button;
        }

        private static ActionPanelView BuildActions(RectTransform root, out Transform footer)
        {
            var panel = Panel("Action Scroll", root, 864, 370, 249, 212, TableTheme.Panel);
            var heading = TableUi.Text("Heading", panel, "Available actions", 15, TableTheme.Gold, true);
            heading.alignment = TextAlignmentOptions.Center;
            heading.enableAutoSizing = true;
            heading.fontSizeMin = 10;
            heading.fontSizeMax = 15;
            TableUi.Place(heading.rectTransform, 10, 7, 229, 42);
            var scroll = panel.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            var viewport = TableUi.Image("Viewport", panel, Color.clear);
            viewport.gameObject.AddComponent<RectMask2D>();
            // Two complete 60-point rows are clearer than a clipped third row;
            // further Core-issued choices remain available through scrolling.
            TableUi.Place(viewport.rectTransform, 10, 54, 229, 120);
            var content = TableUi.Rect("Content", viewport.transform);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(.5f, 1);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(0, 60);
            scroll.viewport = viewport.rectTransform;
            scroll.content = content;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 28;
            var result = panel.gameObject.AddComponent<ActionPanelView>();
            result.Configure(content, heading);

            var footerRect = TableUi.Rect("Primary Actions", root);
            TableUi.Place(footerRect, 0, 690, 1133, 54);
            footer = footerRect;
            return result;
        }

        private static void BuildIngredientShelf(RectTransform root, QuackiesArtCatalog catalog, MatchPresenter presenter)
        {
            var shelf = Panel("Ingredient Reference", root, 20, 632, 655, 48, TableTheme.Raised);
            var label = TableUi.Text("Label", shelf, "INGREDIENTS", 11, TableTheme.Muted, true);
            TableUi.Place(label.rectTransform, 8, 15, 112, 18);
            var colors = new[] { TokenColor.Green, TokenColor.Blue, TokenColor.Red, TokenColor.Yellow, TokenColor.Purple, TokenColor.Black };
            for (var index = 0; index < colors.Length; index++)
            {
                var color = colors[index];
                var button = TableUi.Button("Book_" + color, shelf, color.ToString(), TableTheme.Panel, TableTheme.Ink);
                TableUi.Place(button.GetComponent<RectTransform>(), 125 + index * 87, 2, 81, 44);
                button.GetComponentInChildren<TMP_Text>().fontSize = 11;
                button.gameObject.AddComponent<IngredientBookButton>().Configure(presenter, color, button);
            }
        }

        private static ReferenceModal BuildModal(RectTransform root)
        {
            var shade = TableUi.Image("Reference Modal", root, new Color(0.02f, .04f, .04f, .92f));
            TableUi.Fill(shade.rectTransform);
            shade.raycastTarget = true;
            var closeOnShade = shade.gameObject.AddComponent<Button>();
            closeOnShade.targetGraphic = shade;
            var card = Panel("Modal Card", shade.rectTransform, 250, 55, 633, 590, TableTheme.Panel);
            var heading = TableUi.Text("Heading", card, "", 24, TableTheme.Gold, true);
            heading.alignment = TextAlignmentOptions.Center;
            TableUi.Place(heading.rectTransform, 30, 22, 573, 36);
            var art = TableUi.Image("Artwork", card, Color.white);
            art.preserveAspect = true;
            TableUi.Place(art.rectTransform, 144, 70, 345, 396);
            var detail = TableUi.Text("Detail", card, "", 16, TableTheme.Ink, false);
            detail.alignment = TextAlignmentOptions.Center;
            TableUi.Place(detail.rectTransform, 40, 478, 553, 56);
            var close = TableUi.Button("Close", card, "Close", TableTheme.Gold, TableTheme.Background);
            TableUi.Place(close.GetComponent<RectTransform>(), 240, 534, 153, 44);
            var modal = card.gameObject.AddComponent<ReferenceModal>();
            modal.Configure(shade.gameObject, art, heading, detail, close, closeOnShade);
            shade.gameObject.SetActive(false);
            return modal;
        }

        private static RectTransform Panel(string name, Transform parent, float x, float y, float width, float height, Color color)
        {
            var panel = TableUi.Image(name, parent, color).rectTransform;
            TableUi.Place(panel, x, y, width, height);
            return panel;
        }

        private static void BuildEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            new GameObject("Event System", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }

        private static void BuildBackgroundCamera()
        {
            var cameraObject = new GameObject("Tabletop Background Camera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = TableTheme.Background;
            camera.cullingMask = 0;
            camera.orthographic = true;
        }

        private static void ConfigureBuildSettings()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPadOnly;
            PlayerSettings.runInBackground = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            var scenes = EditorBuildSettings.scenes.Where(scene => scene.path != ScenePath).ToList();
            scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        [MenuItem("Quackies/Build and Play Initial Scene", true)]
        private static bool CanBuildAndPlayInitialScene() => !EditorApplication.isPlayingOrWillChangePlaymode;

        [MenuItem("Quackies/Build Initial Scene", true)]
        private static bool CanBuildInitialScene() => !EditorApplication.isPlayingOrWillChangePlaymode;
    }
}
