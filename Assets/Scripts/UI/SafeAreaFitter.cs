using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    RectTransform _rt;
    Rect _last;

    void Awake() { _rt = GetComponent<RectTransform>(); Apply(); }

    void Update() { if (Screen.safeArea != _last) { Apply(); } }

    void Apply()
    {
        _last = Screen.safeArea;
        var anchorMin = _last.position;
        var anchorMax = _last.position + _last.size;
        anchorMin.x /= Screen.width; anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width; anchorMax.y /= Screen.height;
        _rt.anchorMin = anchorMin; _rt.anchorMax = anchorMax;
    }
}