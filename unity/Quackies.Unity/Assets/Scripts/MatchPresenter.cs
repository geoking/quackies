using System.Collections;
using System.Linq;
using Quackies.Core.AI;
using Quackies.Core.Match;
using Quackies.Core.Randomness;
using Quackies.Core.Tokens;
using Quackies.Unity.Art;
using Quackies.Unity.Presentation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity
{
    /// <summary>
    /// Unity adapter for the MatchSession public command/query API. It contains no
    /// rules: every input is a Core-issued GameAction and every view is a MatchView.
    /// </summary>
    public class MatchPresenter : MonoBehaviour
    {
        public const string HumanId = "human";
        public const string AiId = "ai";

        [Header("Authored by InitialSceneBuilder")]
        [SerializeField] private QuackiesArtCatalog catalog;
        [SerializeField] private PotView humanPot;
        [SerializeField] private PotView opponentPot;
        [SerializeField] private ActionPanelView actions;
        [SerializeField] private Transform primaryActions;
        [SerializeField] private ReferenceModal modal;
        [SerializeField] private TMP_Text roundText;
        [SerializeField] private TMP_Text phaseText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text logText;
        [SerializeField] private TMP_Text bagContentsText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button eventButton;
        [SerializeField] private Image eventArtwork;
        [SerializeField] private TMP_Text eventTitle;

        [SerializeField] private int startingSeed = 26031982;
        [SerializeField] private float opponentStepSeconds = .45f;

        private readonly BalancedPolicy opponentPolicy = new BalancedPolicy();
        private MatchSession session;
        private bool opponentIsActing;

        public void Configure(QuackiesArtCatalog art, PotView human, PotView opponent,
            ActionPanelView actionPanel, Transform footer, ReferenceModal referenceModal,
            TMP_Text roundLabel, TMP_Text phaseLabel, TMP_Text statusLabel, TMP_Text gameLog,
            TMP_Text bagLabel, Button restart, Button activeEvent, Image activeEventArtwork, TMP_Text activeEventTitle)
        {
            catalog = art;
            humanPot = human;
            opponentPot = opponent;
            actions = actionPanel;
            primaryActions = footer;
            modal = referenceModal;
            roundText = roundLabel;
            phaseText = phaseLabel;
            statusText = statusLabel;
            logText = gameLog;
            bagContentsText = bagLabel;
            restartButton = restart;
            eventButton = activeEvent;
            eventArtwork = activeEventArtwork;
            eventTitle = activeEventTitle;
        }

        private void Awake()
        {
            // Old prototype scenes may still contain the compatibility component.
            if (!HasBindings()) return;
            restartButton.onClick.AddListener(Restart);
            eventButton.onClick.AddListener(ShowActiveEvent);
            NewMatch();
        }

        public void Restart()
        {
            if (!HasBindings()) return;
            startingSeed++;
            StopAllCoroutines();
            opponentIsActing = false;
            NewMatch();
        }

        public void ShowActiveEvent()
        {
            if (session == null || catalog == null) return;
            var view = session.GetSnapshot(HumanId);
            modal.Show(view.EventTitle, view.EventDescription, catalog.GetFortuneCardSprite(view.EventTitle));
        }

        public void ShowIngredientBook(TokenColor color)
        {
            if (catalog == null) return;
            modal.Show(color + " ingredient book", "Reference for this ingredient's active rules.", catalog.GetBookSprite(color));
        }

        public void CloseModal() => modal.Close();

        private void NewMatch()
        {
            session = MatchSession.Create(new SeededRandomSource(startingSeed));
            Refresh();
            StartCoroutine(AdvanceOpponent());
        }

        private bool HasBindings()
        {
            return catalog != null && humanPot != null && opponentPot != null && actions != null
                && primaryActions != null && modal != null && roundText != null && phaseText != null
                && statusText != null && logText != null && restartButton != null && eventButton != null
                && eventArtwork != null && eventTitle != null && bagContentsText != null;
        }

        private void ExecuteHumanAction(GameAction action)
        {
            if (action == null || session == null) return;
            session.Execute(HumanId, action);
            Refresh();
            // A human and AI may both have legal decisions. The guarded coroutine
            // continues its paced sequence without making the human input inert.
            StartCoroutine(AdvanceOpponent());
        }

        private IEnumerator AdvanceOpponent()
        {
            if (opponentIsActing || session == null) yield break;
            opponentIsActing = true;
            Refresh();
            // The cap prevents a defective policy or future rule extension from locking the frame.
            for (var step = 0; step < 48; step++)
            {
                var legal = session.GetLegalActions(AiId);
                var automated = legal == null ? null : legal.Where(action => action.Kind != GameActionKind.NextRound).ToArray();
                if (automated == null || automated.Length == 0) break;
                var observation = session.GetSnapshot(AiId);
                session.Execute(AiId, opponentPolicy.Choose(observation, automated));
                Refresh();
                yield return new WaitForSeconds(opponentStepSeconds);
            }
            opponentIsActing = false;
            Refresh();
        }

        private void Refresh()
        {
            if (session == null) return;
            var view = session.GetSnapshot(HumanId);
            var human = view.Players.FirstOrDefault(player => player.Id == HumanId);
            var opponent = view.Players.FirstOrDefault(player => player.Id == AiId);
            if (human != null) humanPot.Render(human);
            if (opponent != null) opponentPot.Render(opponent);

            roundText.text = "ROUND " + view.Round + " / 9";
            phaseText.text = FriendlyPhase(view.Phase);
            statusText.text = Status(view, human);
            logText.text = view.RecentLog.Count == 0 ? string.Empty : view.RecentLog[view.RecentLog.Count - 1];
            bagContentsText.text = BagSummary(view, human);
            var art = catalog.GetFortuneCardSprite(view.EventTitle);
            eventArtwork.sprite = art != null ? art : catalog.CardBack;
            eventArtwork.preserveAspect = true;
            eventTitle.text = string.IsNullOrEmpty(view.EventTitle) ? "Active fortune" : view.EventTitle;
            var legal = session.GetLegalActions(HumanId);
            actions.Render(legal, ExecuteHumanAction, false);
            actions.RenderPrimary(primaryActions, legal, ExecuteHumanAction, false);
        }

        private static string FriendlyPhase(MatchPhase phase)
        {
            switch (phase)
            {
                case MatchPhase.Preparation: return "PREPARING";
                case MatchPhase.Brewing: return "BREWING";
                case MatchPhase.Evaluation: return "EVALUATION";
                case MatchPhase.Shopping: return "SHOPPING";
                case MatchPhase.RubySpending: return "RUBIES";
                case MatchPhase.RoundComplete: return "ROUND COMPLETE";
                case MatchPhase.Finished: return "FINAL SCORE";
                default: return phase.ToString();
            }
        }

        private static string Status(MatchView view, PlayerView human)
        {
            if (view.Phase == MatchPhase.Finished)
            {
                var winners = view.WinnerIds.Count == 0 ? "No winner" : string.Join(" & ", view.WinnerIds);
                return "Match complete · " + winners;
            }
            if (human != null && human.Exploded) return "Your pot exploded. Resolve the available choice.";
            return "Choose an available action. Ingredient effects can add more choices.";
        }

        private static string BagSummary(MatchView view, PlayerView human)
        {
            var chips = view.OwnBag.GroupBy(token => token.Color)
                .OrderBy(group => group.Key)
                .Select(group => group.Key + " " + string.Join("/", group.GroupBy(token => token.Value)
                    .OrderBy(values => values.Key).Select(values => values.Key + "×" + values.Count()).ToArray()));
            return "OWN BAG  " + string.Join("  ·  ", chips.ToArray()) + "     INVENTORY " + human.InventoryCount;
        }
    }
}
