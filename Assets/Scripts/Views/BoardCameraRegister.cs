using UnityEngine;

public class BoardCameraRegister : MonoBehaviour
{
    [SerializeField] string key = "Player";
    [SerializeField] Camera cam;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        // If AppShell isn’t loaded yet, we'll try again on Start
        if (ViewSwitcher.Instance != null) ViewSwitcher.Instance.Register(key, cam);
    }

    void Start()
    {
        if (ViewSwitcher.Instance != null) ViewSwitcher.Instance.Register(key, cam);
    }
}