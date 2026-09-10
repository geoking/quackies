using Quackies.Unity.Art;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>Reusable closeable artwork modal for the active fortune card and ingredient books.</summary>
    public sealed class ReferenceModal : MonoBehaviour
    {
        [SerializeField] private GameObject shade;
        [SerializeField] private Image artwork;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text detail;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button shadeButton;
        private bool listenersBound;

        public void Configure(GameObject dimmer, Image image, TMP_Text heading, TMP_Text body,
            Button close, Button shadeClose)
        {
            shade = dimmer;
            artwork = image;
            title = heading;
            detail = body;
            closeButton = close;
            shadeButton = shadeClose;
        }

        private void Awake()
        {
            if (listenersBound) return;
            listenersBound = true;
            if (closeButton != null) closeButton.onClick.AddListener(Close);
            if (shadeButton != null) shadeButton.onClick.AddListener(Close);
        }

        public void Show(string heading, string body, Sprite sprite)
        {
            title.text = heading;
            detail.text = body;
            artwork.sprite = sprite;
            artwork.enabled = sprite != null;
            shade.SetActive(true);
        }

        public void Close()
        {
            if (shade != null) shade.SetActive(false);
        }
    }
}
