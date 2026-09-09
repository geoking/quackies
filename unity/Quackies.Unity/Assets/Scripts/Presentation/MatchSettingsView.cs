using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>
    /// Displays the current immutable match setup alongside the saved setup for the
    /// next match. Changing a control is deliberately non-destructive until Apply
    /// and restart is selected.
    /// </summary>
    public sealed class MatchSettingsView : MonoBehaviour
    {
        [SerializeField] private GameObject shade;
        [SerializeField] private TMP_Text currentMatchLabel;
        [SerializeField] private TMP_Text nextMatchLabel;
        [SerializeField] private TMP_Text rubyToggleLabel;
        [SerializeField] private Button rubyToggleButton;
        [SerializeField] private Button applyAndRestartButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button shadeButton;

        private int currentStartingRubies;
        private int pendingStartingRubies;
        private Action<int> changed;
        private Action applyAndRestart;
        private bool bound;

        public void Configure(GameObject dimmer, TMP_Text currentLabel, TMP_Text nextLabel,
            TMP_Text toggleLabel, Button toggle, Button apply, Button back, Button shadeClose)
        {
            shade = dimmer;
            currentMatchLabel = currentLabel;
            nextMatchLabel = nextLabel;
            rubyToggleLabel = toggleLabel;
            rubyToggleButton = toggle;
            applyAndRestartButton = apply;
            backButton = back;
            shadeButton = shadeClose;
        }

        private void Awake()
        {
            if (bound) return;
            bound = true;
            rubyToggleButton.onClick.AddListener(ToggleStartingRuby);
            applyAndRestartButton.onClick.AddListener(() => applyAndRestart?.Invoke());
            backButton.onClick.AddListener(Close);
            shadeButton.onClick.AddListener(Close);
        }

        /// <summary>Opens the sheet without changing the live session.</summary>
        public void Show(int currentRubies, int nextRubies, Action<int> onChanged, Action onApplyAndRestart)
        {
            currentStartingRubies = currentRubies;
            pendingStartingRubies = nextRubies;
            changed = onChanged;
            applyAndRestart = onApplyAndRestart;
            Render();
            shade.SetActive(true);
        }

        public bool IsOpen => shade != null && shade.activeSelf;

        public void Close() => shade.SetActive(false);

        private void ToggleStartingRuby()
        {
            pendingStartingRubies = pendingStartingRubies == 0 ? 1 : 0;
            changed?.Invoke(pendingStartingRubies);
            Render();
        }

        private void Render()
        {
            currentMatchLabel.text = "CURRENT MATCH\nStarted with " + RubyCount(currentStartingRubies) + ".";
            nextMatchLabel.text = "NEXT NEW MATCH\nWill start with " + RubyCount(pendingStartingRubies) + ".";
            rubyToggleLabel.text = "Start with 1 ruby: " + (pendingStartingRubies == 1 ? "ON" : "OFF");
            rubyToggleButton.image.color = pendingStartingRubies == 1 ? TableTheme.Gold : TableTheme.Raised;
        }

        private static string RubyCount(int rubies) => rubies == 1 ? "1 ruby" : "no rubies";
    }
}
