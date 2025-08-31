using UnityEngine;

public class ViewSwitchButton : MonoBehaviour
{
    [Tooltip("Exactly match the key you registered with the ViewSwitcher: e.g., Player, Score, CPU1")]
    public string targetKey = "Player";

    // Hook this up to the Button's OnClick in the Inspector
    public void OnClick()
    {
        if (ViewSwitcher.Instance != null)
            ViewSwitcher.Instance.Show(targetKey);
        else
            Debug.LogWarning("ViewSwitcher not ready yet.");
    }
}