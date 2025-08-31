using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBootstrap : MonoBehaviour
{
    [SerializeField] string[] additiveScenes = { "Board_Player", "ScoreTrack" /*, "Board_CPU_1"*/ };

    IEnumerator Start()
    {
        foreach (var s in additiveScenes)
        {
            if (!SceneManager.GetSceneByName(s).isLoaded)
            {
                yield return SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive);
                yield return null; // one frame so Awake/Start on Score register
                FindFirstObjectByType<ViewButtonsAuto>()?.Rebuild();
            }
        }

        // Pick initial view once cameras have registered
        yield return null; // wait one frame for registration
        ViewSwitcher.Instance.Show("Player"); // assuming you register with key "Player"
    }
}