using Quackies.Core.Match;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>Full-screen inspection surface for a public opponent pot observation.</summary>
    public sealed class PotInspectionModal : MonoBehaviour
    {
        [SerializeField] private GameObject shade;
        [SerializeField] private PotView pot;
        [SerializeField] private TMP_Text heading;
        [SerializeField] private Button backButton;
        private bool bound;

        public void Configure(GameObject dimmer, PotView inspectedPot, TMP_Text title, Button back)
        {
            shade = dimmer;
            pot = inspectedPot;
            heading = title;
            backButton = back;
        }

        private void Awake()
        {
            if (bound || backButton == null) return;
            bound = true;
            backButton.onClick.AddListener(Close);
        }

        public void Show(PlayerView player)
        {
            if (player == null) return;
            heading.text = player.Name + " — FULL POT";
            Render(player);
            shade.SetActive(true);
        }

        /// <summary>Updates an already-open inspection without forcing the modal open.</summary>
        public void Render(PlayerView player)
        {
            if (player != null) pot.Render(player);
        }

        public bool IsOpen => shade != null && shade.activeSelf;

        public void Close() => shade.SetActive(false);
    }
}
