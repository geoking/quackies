using Quackies.Core.Match;
using Quackies.Unity.Art;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>Renders one public cauldron observation. It owns no game state.</summary>
    public sealed class PotView : MonoBehaviour
    {
        [SerializeField] private Image cauldronImage;
        [SerializeField] private RectTransform markers;
        [SerializeField] private TMP_Text potSummary;
        [SerializeField] private bool isHuman;

        [SerializeField] private QuackiesArtCatalog catalog;

        public void Configure(QuackiesArtCatalog art, bool human, Image cauldron, RectTransform markerLayer, TMP_Text summary)
        {
            catalog = art;
            isHuman = human;
            cauldronImage = cauldron;
            markers = markerLayer;
            potSummary = summary;
            cauldronImage.sprite = human ? art.HumanCauldron : art.OpponentCauldron;
            cauldronImage.preserveAspect = true;
        }

        public void Render(PlayerView player)
        {
            if (catalog == null || player == null) return;
            cauldronImage.sprite = isHuman ? catalog.HumanCauldron : catalog.OpponentCauldron;
            TableUi.Clear(markers);

            AddMarker(isHuman ? catalog.HumanDroplet : catalog.OpponentDroplet, player.DropletPosition, 0.72f, "Droplet");
            if (player.RatPosition > player.DropletPosition)
                AddMarker(isHuman ? catalog.HumanRat : catalog.OpponentRat, player.RatPosition, 0.68f, "Rat");
            foreach (var chip in player.PlacedChips)
                AddMarker(catalog.GetTokenSprite(chip.Color, chip.Value), chip.Position, 1f, chip.Color + " " + chip.Value);

            var risk = player.Exploded ? "EXPLODED" : player.WhiteTotal + " / " + player.ExplosionThreshold + " white";
            potSummary.text = string.Format("{0}  ·  {1} VP\n{2}  ·  {3} coins  ·  {4} rubies",
                player.Name, player.VictoryPoints, risk, player.Coins, player.Rubies);
        }

        private void AddMarker(Sprite sprite, int physicalPosition, float scale, string name)
        {
            if (sprite == null || physicalPosition < 0 || physicalPosition > 53) return;
            var image = TableUi.Image(name, markers, Color.white, sprite);
            image.preserveAspect = true;
            var rect = image.rectTransform;
            // Translate the artwork-normalized physical anchor through the same
            // preserve-aspect fitted rectangle as the cauldron Image. Anchoring the
            // marker then avoids mixing the image's top-left layout pivot with the
            // marker layer's centred local coordinates.
            var fitted = catalog.GetFittedPotRect(markers.rect, cauldronImage.sprite);
            var local = fitted.min + Vector2.Scale(catalog.GetPotAnchor(physicalPosition), fitted.size);
            var normalized = new Vector2(
                Mathf.InverseLerp(markers.rect.xMin, markers.rect.xMax, local.x),
                Mathf.InverseLerp(markers.rect.yMin, markers.rect.yMax, local.y));
            rect.anchorMin = rect.anchorMax = normalized;
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            var diameter = Mathf.Max(20f, markers.rect.width * catalog.PotChipWidth * scale);
            rect.sizeDelta = new Vector2(diameter, diameter);
        }
    }
}
