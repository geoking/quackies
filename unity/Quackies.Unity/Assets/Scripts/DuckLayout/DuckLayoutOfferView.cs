using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.DuckLayout
{
    /// <summary>Serialized Dream-offer card binding for the fixed M3 sample catalogue.</summary>
    public sealed class DuckLayoutOfferView : MonoBehaviour
    {
        [SerializeField] private string offerId;
        [SerializeField] private string family;
        [SerializeField] private string displayName;
        [SerializeField] private int sleepPrice;
        [SerializeField] private string detail;
        [SerializeField] private Button inspectButton;
        [SerializeField] private Image artImage;

        public string OfferId => offerId;
        public string Family => family;
        public string DisplayName => displayName;
        public int SleepPrice => sleepPrice;
        public string Detail => detail;
        public Button InspectButton => inspectButton;
        public Sprite PreviewSprite => artImage == null ? null : artImage.sprite;

        public void Configure(DuckLayoutOfferFixture fixture, Button button, Image art)
        {
            offerId = fixture.id;
            family = fixture.family;
            displayName = fixture.displayName;
            sleepPrice = fixture.sleepPrice;
            detail = fixture.detail;
            inspectButton = button;
            artImage = art;
        }
    }
}
