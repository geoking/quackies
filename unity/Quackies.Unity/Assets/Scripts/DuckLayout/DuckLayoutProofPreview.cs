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
        [SerializeField] private Button closeInspectionButton;

        private bool showingOccupied;

        public DuckLayoutSpaceView[] Spaces => spaces;
        public DuckLayoutOfferView[] Offers => offers;
        public TokenPresentation[] TokenPresentations => tokenPresentations;

        public void Configure(GameObject adventure, GameObject dream, Button adventureButton, Button dreamButton,
            Button emptyButton, Button occupiedButton, TMP_Text status, DuckLayoutSpaceView[] boardSpaces,
            DuckLayoutOfferView[] shopOffers, TokenPresentation[] tokens, GameObject duckRest, GameObject zzz,
            GameObject featherTrail, GameObject shade, TMP_Text title, TMP_Text detail, Button close)
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
                var feather = space.Feathers > 0 ? " • " + space.Feathers + " Feather" + (space.Feathers == 1 ? string.Empty : "s") : string.Empty;
                inspectionDetail.text = "Space " + space.Space + "\nMoon Sleep " + space.Sleep + " • Twigs " + space.Twigs + feather
                    + "\n\nPreview only: inspect this fixed rest reward and encounter fit. It does not resolve a game action.";
            }
        }

        private void InspectOffer(DuckLayoutOfferView offer)
        {
            if (inspectionShade == null) return;
            inspectionShade.SetActive(true);
            if (inspectionTitle != null) inspectionTitle.text = offer.Family;
            if (inspectionDetail != null)
                inspectionDetail.text = offer.Detail + "\n\nPrice: " + offer.SleepPrice + " Sleep\n\nFixed Dream catalogue sample; the card is inspectable only.";
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
