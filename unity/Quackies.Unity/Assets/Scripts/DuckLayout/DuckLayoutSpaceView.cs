using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.DuckLayout
{
    /// <summary>Serialized visual binding for one fixed board space; it contains no reward or rules logic.</summary>
    public sealed class DuckLayoutSpaceView : MonoBehaviour
    {
        [SerializeField] private string stableId;
        [SerializeField] private int space;
        [SerializeField] private bool haven;
        [SerializeField] private bool useBoardArt;
        [SerializeField] private string havenName;
        [SerializeField] private int sleep;
        [SerializeField] private int twigs;
        [SerializeField] private int feathers;
        [SerializeField] private RectTransform wellRect;
        [SerializeField] private RectTransform rewardRect;
        [SerializeField] private Button inspectButton;
        [SerializeField] private Image wellImage;
        [SerializeField] private Sprite featherRewardSprite;
        [SerializeField] private Image tokenImage;
        [SerializeField] private TMP_Text[] rewardNumbers;
        [SerializeField] private RectTransform[] rewardArtAreas;

        public string StableId => stableId;
        public int Space => space;
        public bool IsHaven => haven;
        public bool UsesBoardArt => useBoardArt;
        public string HavenName => havenName;
        public int Sleep => sleep;
        public int Twigs => twigs;
        public int Feathers => feathers;
        public RectTransform WellRect => wellRect;
        public RectTransform RewardRect => rewardRect;
        public RectTransform TokenRect => tokenImage == null ? null : tokenImage.rectTransform;
        public Button InspectButton => inspectButton;
        public Sprite WellSprite => wellImage == null ? null : wellImage.sprite;
        public Sprite FeatherRewardSprite => featherRewardSprite;
        public Sprite EncounterSprite => tokenImage == null ? null : tokenImage.sprite;
        public TMP_Text[] RewardNumbers => rewardNumbers ?? System.Array.Empty<TMP_Text>();
        public RectTransform[] RewardArtAreas => rewardArtAreas ?? System.Array.Empty<RectTransform>();

        public void ConfigurePaintedRewards(TMP_Text[] numbers, RectTransform[] protectedArt)
        {
            rewardNumbers = numbers;
            rewardArtAreas = protectedArt;
        }

        public void Configure(string id, int number, bool isHaven, bool usesBoardArt, string shelterName, int sleepReward, int twigReward,
            int featherReward, RectTransform well, RectTransform reward, Button button, Image wellArtwork, Sprite featherArtwork, Image token)
        {
            stableId = id;
            space = number;
            haven = isHaven;
            useBoardArt = usesBoardArt;
            havenName = shelterName ?? string.Empty;
            sleep = sleepReward;
            twigs = twigReward;
            feathers = featherReward;
            wellRect = well;
            rewardRect = reward;
            inspectButton = button;
            wellImage = wellArtwork;
            featherRewardSprite = featherArtwork;
            tokenImage = token;
        }

        public void ShowToken(Sprite sprite)
        {
            if (tokenImage != null)
            {
                tokenImage.sprite = sprite;
                tokenImage.color = sprite == null ? new Color(.25f, .12f, .38f, .95f) : Color.white;
                tokenImage.useSpriteMesh = sprite != null;
                tokenImage.gameObject.SetActive(true);
            }
        }

        public void HideToken()
        {
            if (tokenImage != null) tokenImage.gameObject.SetActive(false);
        }
    }
}
