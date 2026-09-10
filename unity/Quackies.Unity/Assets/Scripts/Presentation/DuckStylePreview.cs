using TMPro;
using Quackies.Core.Rules;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>
    /// Gives the M1 style scene a small, rules-neutral interaction. It only cycles
    /// through fixed example encounter placements; a MatchSession is deliberately
    /// not created in this scene.
    /// </summary>
    public sealed class DuckStylePreview : MonoBehaviour
    {
        [SerializeField] private Button exploreButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private GameObject[] seedMarkers;
        [SerializeField] private RectTransform[] boardCells;
        [SerializeField] private RectTransform restingMarker;
        [SerializeField] private TMP_Text restingLabel;

        private readonly int[] placementStages =
        {
            3, 5, 7, 9
        };

        private int stage;

        public void Configure(Button explore, Button reset, TMP_Text status, GameObject[] markers,
            RectTransform[] cells, RectTransform restMarker, TMP_Text restLabel)
        {
            exploreButton = explore;
            resetButton = reset;
            statusLabel = status;
            seedMarkers = markers;
            boardCells = cells;
            restingMarker = restMarker;
            restingLabel = restLabel;
            stage = 0;
            ApplyStage();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying) return;
            if (exploreButton != null) exploreButton.onClick.AddListener(ExplorePreview);
            if (resetButton != null) resetButton.onClick.AddListener(ResetPreview);
        }

        private void Start()
        {
            if (Application.isPlaying) ApplyStage();
        }

        private void OnDisable()
        {
            if (exploreButton != null) exploreButton.onClick.RemoveListener(ExplorePreview);
            if (resetButton != null) resetButton.onClick.RemoveListener(ResetPreview);
        }

        /// <summary>Cycles a fixed visual example and does not submit a game action.</summary>
        public void ExplorePreview()
        {
            stage = (stage + 1) % placementStages.Length;
            ApplyStage();
        }

        public void ResetPreview()
        {
            stage = 0;
            ApplyStage();
        }

        private void ApplyStage()
        {
            if (seedMarkers == null) return;
            var visibleCount = placementStages[stage];
            for (var index = 0; index < seedMarkers.Length; index++)
            {
                if (seedMarkers[index] != null) seedMarkers[index].SetActive(index < visibleCount);
            }

            if (statusLabel != null)
            {
                statusLabel.text = "Today’s trail • " + visibleCount + " seed encounters previewed.";
            }

            var scoring = BoardTrack.Standard().ScoringSpace(visibleCount);
            if (restingMarker != null && boardCells != null && scoring.Position < boardCells.Length)
            {
                restingMarker.SetParent(boardCells[scoring.Position], false);
                restingMarker.anchorMin = restingMarker.anchorMax = new Vector2(.5f, .5f);
                restingMarker.pivot = new Vector2(.5f, .5f);
                restingMarker.anchoredPosition = new Vector2(0, -2);
            }
            if (restingLabel != null)
            {
                restingLabel.text = "Rest at space " + scoring.Position + "\n"
                    + scoring.Coins + " Pond pennies • " + scoring.Points + " Twigs";
            }
        }
    }
}
