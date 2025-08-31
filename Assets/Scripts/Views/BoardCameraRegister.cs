using UnityEngine;

public class BoardCameraRegister : MonoBehaviour
{
    [SerializeField] string key = "Player";
    [SerializeField] Camera cam;

    public void Initialize(string newKey, Camera overrideCam = null)
    {
        key = newKey;
        if (overrideCam != null) cam = overrideCam;
        Register();
    }

    void Awake()
    {
        if (!cam) cam = GetComponentInChildren<Camera>();
        Register();
    }

    void Register()
    {
        if (ViewSwitcher.Instance != null && cam != null)
        {
            ViewSwitcher.Instance.Register(key, cam);
        }
    }
}