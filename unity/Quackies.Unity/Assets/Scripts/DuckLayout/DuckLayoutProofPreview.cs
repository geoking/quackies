using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.DuckLayout
{
    /// <summary>
    /// Interaction for the saved M3 fixture scene. It changes only fixed visual samples;
    /// it deliberately has no MatchSession, action issue, reward calculation, or game state.
    /// </summary>
    public sealed class DuckLayoutProofPreview : MonoBehaviour
    {
        [Serializable]
        public sealed class TokenPresentation
        {
            public string title;
            public int space;
            public Sprite sprite;
        }

        [SerializeField] private GameObject adventureRoot;
        [SerializeField] private GameObject dreamRoot;
        [SerializeField] private Button showAdventureButton;
        [SerializeField] private Button showDreamButton;
        [SerializeField] private Button emptyWellsButton;
        [SerializeField] private Button occupiedWellsButton;
        [SerializeField] private TMP_Text previewStatus;
        [SerializeField] private DuckLayoutSpaceView[] spaces;
        [SerializeField] private DuckLayoutOfferView[] offers;
        [SerializeField] private TokenPresentation[] tokenPresentations;
        [SerializeField] private GameObject duckRestOverlay;
        [SerializeField] private GameObject zzzOverlay;
        [SerializeField] private GameObject featherTrailOverlay;
        [SerializeField] private GameObject inspectionShade;
        [SerializeField] private TMP_Text inspectionTitle;
        [SerializeField] private TMP_Text inspectionDetail;
        [SerializeField] private TMP_Text inspectionSleepValue;
        [SerializeField] private TMP_Text inspectionTwigValue;
        [SerializeField] private TMP_Text inspectionFeatherValue;
        [SerializeField] private Image inspectionArt;
        [SerializeField] private Image inspectionSleepIcon;
        [SerializeField] private Image inspectionTwigIcon;
        [SerializeField] private Image inspectionFeatherIcon;
        [SerializeField] private Button closeInspectionButton;

        private bool showingOccupied;

        public DuckLayoutSpaceView[] Spaces => spaces;
        public DuckLayoutOfferView[] Offers => offers;
        public TokenPresentation[] TokenPresentations => tokenPresentations;

        public void Configure(GameObject adventure, GameObject dream, Button adventureButton, Button dreamButton,
            Button emptyButton, Button occupiedButton, TMP_Text status, DuckLayoutSpaceView[] boardSpaces,
            DuckLayoutOfferView[] shopOffers, TokenPresentation[] tokens, GameObject duckRest, GameObject zzz,
            GameObject featherTrail, GameObject shade, TMP_Text title, TMP_Text detail, TMP_Text sleepValue, TMP_Text twigValue,
            TMP_Text featherValue, Image art,
            Image sleepIcon, Image twigIcon, Image featherIcon, Button close)
        {
            adventureRoot = adventure;
            dreamRoot = dream;
            showAdventureButton = adventureButton;
            showDreamButton = dreamButton;
            emptyWellsButton = emptyButton;
            occupiedWellsButton = occupiedButton;
            previewStatus = status;
            spaces = boardSpaces;
            offers = shopOffers;
            tokenPresentations = tokens;
            duckRestOverlay = duckRest;
            zzzOverlay = zzz;
            featherTrailOverlay = featherTrail;
            inspectionShade = shade;
            inspectionTitle = title;
            inspectionDetail = detail;
            inspectionSleepValue = sleepValue;
            inspectionTwigValue = twigValue;
            inspectionFeatherValue = featherValue;
            inspectionArt = art;
            inspectionSleepIcon = sleepIcon;
            inspectionTwigIcon = twigIcon;
            inspectionFeatherIcon = featherIcon;
            closeInspectionButton = close;
            ApplyAdventure(true);
            ApplyOccupied(false);
            if (inspectionShade != null) inspectionShade.SetActive(false);
        }

        private void Awake()
        {
            ApplyAdventure(true);
            ApplyOccupied(false);
            if (inspectionShade != null) inspectionShade.SetActive(false);
        }

        private void OnEnable()
        {
            Listen(showAdventureButton, ShowAdventure);
            Listen(showDreamButton, ShowDream);
            Listen(emptyWellsButton, ShowEmptyWells);
            Listen(occupiedWellsButton, ShowOccupiedWells);
            Listen(closeInspectionButton, CloseInspection);
            BindInspectableCards();
        }

        private void OnDisable()
        {
            Unlisten(showAdventureButton, ShowAdventure);
            Unlisten(showDreamButton, ShowDream);
            Unlisten(emptyWellsButton, ShowEmptyWells);
            Unlisten(occupiedWellsButton, ShowOccupiedWells);
            Unlisten(closeInspectionButton, CloseInspection);
            // Per-card listeners are reset on the next OnEnable, avoiding stale closures after a rebuilt saved scene.
        }

        public void ShowAdventure() => ApplyAdventure(true);
        public void ShowDream() => ApplyAdventure(false);
        public void ShowEmptyWells() => ApplyOccupied(false);
        public void ShowOccupiedWells() => ApplyOccupied(true);
        public void CloseInspection() { if (inspectionShade != null) inspectionShade.SetActive(false); }

        private void ApplyAdventure(bool adventure)
        {
            if (adventureRoot != null) adventureRoot.SetActive(adventure);
            if (dreamRoot != null) dreamRoot.SetActive(!adventure);
            if (previewStatus != null) previewStatus.text = adventure
                ? (showingOccupied ? "Encounter-fit sample • 16 variants, duck rest, zzz and Feather trail." : "Empty-well sample • 50 fixed rest spaces and visible rewards.")
                : "Dream Concept B • fixed Night 4, Level 2, purchase limit 2 sample.";
        }

        private void ApplyOccupied(bool occupied)
        {
            showingOccupied = occupied;
            if (spaces != null)
            {
                foreach (var space in spaces) if (space != null) space.HideToken();
            }
            if (occupied && tokenPresentations != null && spaces != null)
            {
                foreach (var token in tokenPresentations)
                {
                    var target = FindSpace(token.space);
                    if (target != null) target.ShowToken(token.sprite);
                }
            }
            if (duckRestOverlay != null) duckRestOverlay.SetActive(occupied);
            if (zzzOverlay != null) zzzOverlay.SetActive(occupied);
            if (featherTrailOverlay != null) featherTrailOverlay.SetActive(occupied);
            if (adventureRoot != null && adventureRoot.activeSelf) ApplyAdventure(true);
        }

        private void BindInspectableCards()
        {
            if (spaces != null)
            {
                foreach (var space in spaces)
                {
                    if (space == null || space.InspectButton == null) continue;
                    var captured = space;
                    space.InspectButton.onClick.RemoveAllListeners();
                    space.InspectButton.onClick.AddListener(() => InspectSpace(captured));
                }
            }
            if (offers != null)
            {
                foreach (var offer in offers)
                {
                    if (offer == null || offer.InspectButton == null) continue;
                    var captured = offer;
                    offer.InspectButton.onClick.RemoveAllListeners();
                    offer.InspectButton.onClick.AddListener(() => InspectOffer(captured));
                }
            }
        }

        private void InspectSpace(DuckLayoutSpaceView space)
        {
            if (inspectionShade == null) return;
            inspectionShade.SetActive(true);
            if (inspectionTitle != null) inspectionTitle.text = space.IsHaven ? space.HavenName : "Rest space " + space.Space;
            if (inspectionDetail != null)
            {
                inspectionDetail.text = space.IsHaven ? "Shelter reward preview. This fixed layout does not resolve a game action."
                    : "Rest reward preview. This fixed layout does not resolve a game action.";
            }
            SetRewardPreview(space.Sleep, space.Twigs, space.Feathers);
            var token = showingOccupied ? FindToken(space.Space) : null;
            SetInspectionArt(token == null ? space.WellSprite : token.sprite, space.Space == 50 && token == null);
        }

        private void InspectOffer(DuckLayoutOfferView offer)
        {
            if (inspectionShade == null) return;
            inspectionShade.SetActive(true);
            if (inspectionTitle != null) inspectionTitle.text = offer.Family;
            if (inspectionDetail != null)
                inspectionDetail.text = offer.Detail + "\n\nPrice: " + offer.SleepPrice + " Sleep\n\nFixed Dream catalogue sample; the card is inspectable only.";
            SetValue(inspectionSleepValue, offer.SleepPrice.ToString(), true);
            SetValue(inspectionTwigValue, string.Empty, false);
            SetValue(inspectionFeatherValue, string.Empty, false);
            if (inspectionSleepIcon != null) inspectionSleepIcon.gameObject.SetActive(true);
            if (inspectionTwigIcon != null) inspectionTwigIcon.gameObject.SetActive(false);
            if (inspectionFeatherIcon != null) inspectionFeatherIcon.gameObject.SetActive(false);
            SetInspectionArt(offer.PreviewSprite, false);
        }

        private void SetRewardPreview(int sleep, int twigs, int feathers)
        {
            SetValue(inspectionSleepValue, sleep.ToString(), true);
            SetValue(inspectionTwigValue, twigs.ToString(), twigs > 0);
            SetValue(inspectionFeatherValue, feathers.ToString(), feathers > 0);
            if (inspectionSleepIcon != null) inspectionSleepIcon.gameObject.SetActive(true);
            if (inspectionTwigIcon != null) inspectionTwigIcon.gameObject.SetActive(twigs > 0);
            if (inspectionFeatherIcon != null) inspectionFeatherIcon.gameObject.SetActive(feathers > 0);
        }

        private static void SetValue(TMP_Text value, string text, bool visible)
        {
            if (value == null) return;
            value.text = text;
            value.gameObject.SetActive(visible);
        }

        private void SetInspectionArt(Sprite sprite, bool endpointFallback)
        {
            if (inspectionArt == null) return;
            inspectionArt.sprite = sprite ?? (endpointFallback && inspectionFeatherIcon != null ? inspectionFeatherIcon.sprite : null);
            inspectionArt.color = inspectionArt.sprite == null ? Color.clear : Color.white;
            inspectionArt.useSpriteMesh = inspectionArt.sprite != null;
            inspectionArt.gameObject.SetActive(inspectionArt.sprite != null);
        }

        private TokenPresentation FindToken(int space)
        {
            if (tokenPresentations == null) return null;
            foreach (var token in tokenPresentations)
                if (token != null && token.space == space) return token;
            return null;
        }

        private DuckLayoutSpaceView FindSpace(int number)
        {
            foreach (var space in spaces) if (space != null && space.Space == number) return space;
            return null;
        }

        private static void Listen(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null) return;
            button.onClick.RemoveListener(action);
            button.onClick.AddListener(action);
        }

        private static void Unlisten(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button != null) button.onClick.RemoveListener(action);
        }
    }
}
