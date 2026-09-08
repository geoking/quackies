using UnityEngine;

namespace Quackies.Unity.Presentation
{
    /// <summary>Keeps the authored 1133×744 tabletop centred inside the safe area.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class FittedViewport : MonoBehaviour
    {
        public const float Width = 1133f;
        public const float Height = 744f;
        [SerializeField] private RectTransform safeParent;
        private RectTransform rect;
        private bool isFitting;
        private Vector2 lastParentSize;

        public void Configure(RectTransform parent)
        {
            safeParent = parent;
            rect = (RectTransform)transform;
            Fit();
        }

        private void OnEnable() { rect = (RectTransform)transform; Fit(); }
        private void OnRectTransformDimensionsChange() => Fit();

        private void LateUpdate()
        {
            if (safeParent == null) return;
            if (safeParent.rect.size != lastParentSize) Fit();
        }

        private void Fit()
        {
            if (isFitting || rect == null || safeParent == null) return;
            var parentSize = safeParent.rect.size;
            if (parentSize.x <= 0 || parentSize.y <= 0) return;
            isFitting = true;
            var scale = Mathf.Min(parentSize.x / Width, parentSize.y / Height);
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(Width, Height);
            rect.localScale = new Vector3(scale, scale, 1f);
            lastParentSize = parentSize;
            isFitting = false;
        }
    }
}
