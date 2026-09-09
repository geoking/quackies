using System;
using System.Collections.Generic;
using Quackies.Core.Match;
using Quackies.Unity.Art;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>Keeps the three brewing controls in fixed, serialized slots and submits only Core-issued actions.</summary>
    public sealed class PrimaryActionsView : MonoBehaviour
    {
        [SerializeField] private Button drawButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private Button flaskButton;
        [SerializeField] private Image flaskArtwork;
        [SerializeField] private TMP_Text drawLabel;
        [SerializeField] private TMP_Text stopLabel;
        [SerializeField] private TMP_Text flaskLabel;
        [SerializeField] private QuackiesArtCatalog catalog;

        private GameAction drawAction;
        private GameAction stopAction;
        private GameAction flaskAction;
        private Action<GameAction> submit;
        private bool bound;

        public void Configure(QuackiesArtCatalog art, Button draw, Button stop, Button flask, Image flaskIcon,
            TMP_Text drawText, TMP_Text stopText, TMP_Text flaskText)
        {
            catalog = art;
            drawButton = draw;
            stopButton = stop;
            flaskButton = flask;
            flaskArtwork = flaskIcon;
            drawLabel = drawText;
            stopLabel = stopText;
            flaskLabel = flaskText;
        }

        private void Awake()
        {
            if (bound) return;
            bound = true;
            drawButton.onClick.AddListener(() => Submit(drawAction));
            stopButton.onClick.AddListener(() => Submit(stopAction));
            flaskButton.onClick.AddListener(() => Submit(flaskAction));
        }

        /// <summary>
        /// Refreshes the actions stored in the fixed slots. Flask fullness is an observation
        /// used only to select the supplied full or empty artwork; eligibility still comes
        /// exclusively from the Core-issued action list.
        /// </summary>
        public void Render(IReadOnlyList<GameAction> actions, Action<GameAction> onAction, bool busy, bool flaskFull)
        {
            submit = onAction;
            drawAction = Find(actions, GameActionKind.Draw);
            stopAction = Find(actions, GameActionKind.Stop);
            flaskAction = Find(actions, GameActionKind.UseFlask);
            Set(drawButton, drawAction, busy);
            Set(stopButton, stopAction, busy);
            Set(flaskButton, flaskAction, busy);
            Label(drawLabel, "Draw a chip", drawAction, busy);
            Label(stopLabel, "Stop brewing", stopAction, busy);
            Label(flaskLabel, "Use flask", flaskAction, busy);
            flaskArtwork.sprite = flaskFull ? catalog.HumanFlask : catalog.HumanEmptyFlask;
            flaskArtwork.color = flaskAction != null && !busy ? Color.white : new Color(1f, 1f, 1f, .38f);
            flaskArtwork.enabled = flaskArtwork.sprite != null;
        }

        private void Submit(GameAction action)
        {
            if (action != null) submit?.Invoke(action);
        }

        private static GameAction Find(IReadOnlyList<GameAction> actions, GameActionKind kind)
        {
            if (actions == null) return null;
            foreach (var action in actions)
                if (action.Kind == kind) return action;
            return null;
        }

        private static void Set(Button button, GameAction action, bool busy)
        {
            button.interactable = action != null && !busy;
        }

        private static void Label(TMP_Text label, string available, GameAction action, bool busy)
        {
            label.text = action != null && !busy ? available : available + " unavailable";
        }
    }
}
