using UnityEngine;

public class BoardAutoFrame : MonoBehaviour
{
    public Camera cam;
    public float padding = 0.5f;

    [ContextMenu("Frame Now")]
    public void FrameNow()
    {
        if (!cam) cam = GetComponentInChildren<Camera>();
        if (!cam || !cam.orthographic) return;

        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        var bounds = new Bounds(renderers[0].bounds.center, Vector3.zero);
        foreach (var r in renderers) bounds.Encapsulate(r.bounds);

        // center camera on content
        cam.transform.position = new Vector3(bounds.center.x, bounds.center.y, cam.transform.position.z);

        // choose orthographic size that fits height and width
        float halfH = bounds.extents.y + padding;
        float halfW = bounds.extents.x + padding;
        float aspect = (float)Screen.width / Screen.height; // iPad ~ 4:3
        cam.orthographicSize = Mathf.Max(halfH, halfW / aspect);
    }

    void Start() => FrameNow();
#if UNITY_EDITOR
    void OnValidate() { if (!Application.isPlaying) FrameNow(); }
#endif
}