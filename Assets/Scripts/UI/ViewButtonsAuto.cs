using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class ViewButtonsAuto : MonoBehaviour
{
    [Tooltip("Assign a simple UI Button prefab (with Text/TMP).")]
    public GameObject buttonPrefab;

    void Start()
    {
        // Wait one frame so BoardSpawner has time to register cameras
        StartCoroutine(BuildNextFrame());
    }

    IEnumerator BuildNextFrame()
    {
        yield return null; // one frame delay
        Rebuild();
    }

    public void Rebuild()
    {
        // Clear existing buttons
        foreach (Transform child in transform) Destroy(child.gameObject);

        // If ViewSwitcher is alive, use its registered keys; otherwise a fallback list
        var keys = (ViewSwitcher.Instance != null && ViewSwitcher.Instance.Keys != null && ViewSwitcher.Instance.Keys.Count > 0)
            ? ViewSwitcher.Instance.Keys.ToList()
            : new System.Collections.Generic.List<string> { "Player", "Score", "CPU1" };

        foreach (var key in keys)
        {
            var b = Instantiate(buttonPrefab, transform);
            b.name = $"Btn_{key}";

            // Ensure the button has our click handler
            var vsb = b.GetComponent<ViewSwitchButton>();
            if (vsb == null) vsb = b.AddComponent<ViewSwitchButton>();
            vsb.targetKey = key;

            // Set the label (TMP first, then legacy Text)
            var tmp = b.GetComponentInChildren<TMPro.TMP_Text>();
            if (tmp) tmp.text = key;
            else
            {
                var legacy = b.GetComponentInChildren<Text>();
                if (legacy) legacy.text = key;
            }

            // Wire the click
            var btn = b.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(vsb.OnClick);
        }
    }
}