using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }

        Instance = this;
        // We’re already under AppShellRoot which is DontDestroyOnLoad
        // so we don't need another DontDestroyOnLoad here.
    }

    // Put global state here later (GameState, save/load, etc.)
}