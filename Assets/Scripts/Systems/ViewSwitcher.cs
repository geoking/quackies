using System.Collections.Generic;
using UnityEngine;

public class ViewSwitcher : MonoBehaviour
{
    public static ViewSwitcher Instance { get; private set; }
    readonly Dictionary<string, Camera> _cams = new();
    public IReadOnlyCollection<string> Keys => _cams.Keys;


    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }

        Instance = this;
    }

    public void Register(string key, Camera cam)
    {
        _cams[key] = cam;
        cam.enabled = false; // default off; we'll turn one on
    }

    public void Show(string key)
    {
        foreach (var kv in _cams)
        {
            kv.Value.enabled = false;
        }

        if (_cams.TryGetValue(key, out var cam))
        {
            cam.enabled = true;
        }
    }
}