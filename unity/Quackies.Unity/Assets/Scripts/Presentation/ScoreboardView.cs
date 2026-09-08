using Quackies.Core.Match;
using Quackies.Unity.Art;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>Renders public scores on the supplied board artwork; it never changes a match.</summary>
    public sealed class ScoreboardView : MonoBehaviour
    {
        // These are measured from the supplied 1600 x 1148 board, in source-image
        // coordinates (top-left origin), then normalized for its fitted Image.
        private static readonly Vector2[] ScoreAnchors =
        {
            new Vector2(.200f, .750f), // 0 VP: inside the seal book, clear of the numbered score track.
            new Vector2(.047f, .923f), new Vector2(.046f, .827f), new Vector2(.083f, .757f),
            new Vector2(.046f, .671f), new Vector2(.078f, .602f), new Vector2(.046f, .536f),
            new Vector2(.082f, .460f), new Vector2(.046f, .382f), new Vector2(.081f, .315f),
            new Vector2(.046f, .240f), new Vector2(.078f, .160f), new Vector2(.047f, .082f),
            new Vector2(.111f, .082f), new Vector2(.176f, .082f), new Vector2(.241f, .082f),
            new Vector2(.302f, .082f), new Vector2(.362f, .082f), new Vector2(.426f, .082f),
            new Vector2(.491f, .082f), new Vector2(.554f, .082f), new Vector2(.616f, .082f),
            new Vector2(.679f, .082f), new Vector2(.742f, .082f), new Vector2(.805f, .082f),
            new Vector2(.876f, .082f), new Vector2(.934f, .082f), new Vector2(.905f, .160f),
            new Vector2(.936f, .241f), new Vector2(.899f, .311f), new Vector2(.938f, .388f),
            new Vector2(.900f, .459f), new Vector2(.934f, .530f), new Vector2(.906f, .605f),
            new Vector2(.934f, .678f), new Vector2(.898f, .753f), new Vector2(.934f, .828f),
            new Vector2(.934f, .922f), new Vector2(.872f, .922f), new Vector2(.805f, .922f),
            new Vector2(.743f, .922f), new Vector2(.679f, .922f), new Vector2(.616f, .922f),
            new Vector2(.553f, .922f), new Vector2(.491f, .922f), new Vector2(.425f, .922f),
            new Vector2(.363f, .922f), new Vector2(.301f, .922f), new Vector2(.239f, .922f),
            new Vector2(.175f, .922f), new Vector2(.113f, .922f)
        };

        private static readonly Vector2[] RoundAnchors =
        {
            new Vector2(.250f, .254f), new Vector2(.319f, .417f), new Vector2(.388f, .252f),
            new Vector2(.455f, .417f), new Vector2(.526f, .252f), new Vector2(.593f, .417f),
            new Vector2(.663f, .252f), new Vector2(.723f, .417f), new Vector2(.800f, .252f)
        };

        [SerializeField] private GameObject shade;
        [SerializeField] private Image board;
        [SerializeField] private RectTransform markerLayer;
        [SerializeField] private Image humanCounter;
        [SerializeField] private Image opponentCounter;
        [SerializeField] private Image roundCounter;
        [SerializeField] private TMP_Text humanScore;
        [SerializeField] private TMP_Text opponentScore;
        [SerializeField] private TMP_Text roundLabel;
        [SerializeField] private Button backButton;
        [SerializeField] private QuackiesArtCatalog catalog;
        private bool bound;

        public void Configure(GameObject dimmer, QuackiesArtCatalog art, Image boardImage, RectTransform markers,
            Image humanMarker, Image opponentMarker, Image roundMarker, TMP_Text humanLabel,
            TMP_Text opponentLabel, TMP_Text roundText, Button back)
        {
            shade = dimmer;
            catalog = art;
            board = boardImage;
            markerLayer = markers;
            humanCounter = humanMarker;
            opponentCounter = opponentMarker;
            roundCounter = roundMarker;
            humanScore = humanLabel;
            opponentScore = opponentLabel;
            roundLabel = roundText;
            backButton = back;
            board.sprite = art.RoundBoard;
            humanCounter.sprite = art.HumanCounter;
            opponentCounter.sprite = art.OpponentCounter;
            roundCounter.sprite = art.RoundMarker;
        }

        private void Awake()
        {
            if (bound || backButton == null) return;
            bound = true;
            backButton.onClick.AddListener(Close);
        }

        public void Show() => shade.SetActive(true);
        public void Close() => shade.SetActive(false);

        public void Render(MatchView view, PlayerView human, PlayerView opponent)
        {
            if (view == null || human == null || opponent == null) return;
            humanScore.text = "YOU\n" + human.VictoryPoints + " VP";
            opponentScore.text = "CPU\n" + opponent.VictoryPoints + " VP";
            roundLabel.text = "ROUND " + view.Round + " / 9";
            var tied = human.VictoryPoints == opponent.VictoryPoints;
            Place(humanCounter.rectTransform, ScoreAnchor(human.VictoryPoints), 46f, tied ? new Vector2(-16f, 0f) : Vector2.zero);
            Place(opponentCounter.rectTransform, ScoreAnchor(opponent.VictoryPoints), 46f, tied ? new Vector2(16f, 0f) : Vector2.zero);
            Place(roundCounter.rectTransform, RoundAnchors[Mathf.Clamp(view.Round - 1, 0, RoundAnchors.Length - 1)], 45f, Vector2.zero);
        }

        private static Vector2 ScoreAnchor(int score)
        {
            if (score <= 0) return ScoreAnchors[0];
            // The printed track ends at 50. Continue around it for a final score over 50.
            return ScoreAnchors[((score - 1) % 50) + 1];
        }

        private void Place(RectTransform marker, Vector2 sourceAnchor, float diameter, Vector2 offset)
        {
            var anchor = FittedAnchor(sourceAnchor);
            marker.SetParent(marker.parent, false);
            marker.anchorMin = marker.anchorMax = anchor;
            marker.pivot = new Vector2(.5f, .5f);
            marker.anchoredPosition = offset;
            marker.sizeDelta = new Vector2(diameter, diameter);
        }

        private Vector2 FittedAnchor(Vector2 sourceAnchor)
        {
            // RectTransform anchors use a bottom-left origin; the measured source art does not.
            var normalized = new Vector2(sourceAnchor.x, 1f - sourceAnchor.y);
            if (board == null || markerLayer == null || board.sprite == null || !board.preserveAspect)
                return normalized;
            var container = markerLayer.rect;
            var sprite = board.sprite.rect;
            var scale = Mathf.Min(container.width / sprite.width, container.height / sprite.height);
            var fittedSize = sprite.size * scale;
            var fitted = new Rect(container.center - fittedSize * .5f, fittedSize);
            var local = fitted.min + Vector2.Scale(normalized, fitted.size);
            return new Vector2(Mathf.InverseLerp(container.xMin, container.xMax, local.x),
                Mathf.InverseLerp(container.yMin, container.yMax, local.y));
        }
    }
}
