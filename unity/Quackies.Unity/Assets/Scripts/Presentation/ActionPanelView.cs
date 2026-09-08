using System;
using System.Collections.Generic;
using Quackies.Core.Match;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>Turns legal Core commands into touch targets without interpreting game rules.</summary>
    public sealed class ActionPanelView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private TMP_Text heading;
        [SerializeField] private float rowHeight = 50f;

        public void Configure(RectTransform listContent, TMP_Text title)
        {
            content = listContent;
            heading = title;
        }

        public void Render(IReadOnlyList<GameAction> actions, Action<GameAction> onAction, bool busy)
        {
            TableUi.Clear(content);
            if (actions == null || actions.Count == 0)
            {
                heading.text = busy ? "Opponent is brewing…" : "Waiting for the next phase";
                content.sizeDelta = new Vector2(content.sizeDelta.x, rowHeight);
                return;
            }

            heading.text = string.IsNullOrEmpty(actions[0].ChoiceTitle) ? "Available actions" : actions[0].ChoiceTitle;
            var y = 0f;
            foreach (var action in actions)
            {
                if (IsPrimary(action.Kind)) continue;
                var captured = action;
                var button = TableUi.Button("Action_" + action.Id, content, ActionLabel(action),
                    action.Kind == GameActionKind.Choose ? TableTheme.Raised : TableTheme.Panel, TableTheme.Ink);
                var label = button.GetComponentInChildren<TMP_Text>();
                label.fontSize = 13;
                label.overflowMode = TextOverflowModes.Overflow;
                label.textWrappingMode = TextWrappingModes.Normal;
                TableUi.Place(button.GetComponent<RectTransform>(), 0, y, content.rect.width, rowHeight - 6);
                button.interactable = !busy;
                button.onClick.AddListener(() => onAction(captured));
                y += rowHeight;
            }
            content.sizeDelta = new Vector2(content.sizeDelta.x, Mathf.Max(rowHeight, y));
        }

        public void RenderPrimary(Transform parent, IReadOnlyList<GameAction> actions, Action<GameAction> onAction, bool busy)
        {
            TableUi.Clear(parent);
            var primaries = new List<GameAction>();
            if (actions != null)
                foreach (var action in actions)
                    if (IsPrimary(action.Kind)) primaries.Add(action);
            if (primaries.Count == 0) return;
            var width = 180f;
            var total = primaries.Count * width + (primaries.Count - 1) * 12f;
            var start = (1133f - total) * .5f;
            for (var i = 0; i < primaries.Count; i++)
            {
                var action = primaries[i];
                var button = TableUi.Button("Primary_" + action.Id, parent, ActionLabel(action),
                    action.Kind == GameActionKind.Stop ? TableTheme.Red : TableTheme.Gold, TableTheme.Background);
                TableUi.Place(button.GetComponent<RectTransform>(), start + i * (width + 12), 0, width, 54);
                button.interactable = !busy;
                button.onClick.AddListener(() => onAction(action));
            }
        }

        private static bool IsPrimary(GameActionKind kind)
        {
            return kind == GameActionKind.Draw || kind == GameActionKind.Stop || kind == GameActionKind.UseFlask;
        }

        private static string ActionLabel(GameAction action)
        {
            if (action.Cost > 0) return action.Label + "  ·  " + action.Cost + " coins";
            return action.Label;
        }
    }
}
