using UnityEngine;

namespace Quackies.Unity.Presentation
{
    /// <summary>Insets the UI when the device reports a changed safe area or orientation.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        private Rect lastArea;
        private Vector2Int lastScreen;

        private void OnEnable() => Apply();
        private void Update()
        {
            if (lastArea != Screen.safeArea || lastScreen.x != Screen.width || lastScreen.y != Screen.height)
                Apply();
        }

        private void Apply()
        {
            if (Screen.width == 0 || Screen.height == 0) return;
            lastArea = Screen.safeArea;
            lastScreen = new Vector2Int(Screen.width, Screen.height);
            var rect = (RectTransform)transform;
            rect.anchorMin = lastArea.min / new Vector2(Screen.width, Screen.height);
            rect.anchorMax = lastArea.max / new Vector2(Screen.width, Screen.height);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
