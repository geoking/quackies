using UnityEngine;

public class AppBootstrap : MonoBehaviour
{
    void Awake()
    {
        // Persist the whole root object (keeps all manager components alive)
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
    }
}