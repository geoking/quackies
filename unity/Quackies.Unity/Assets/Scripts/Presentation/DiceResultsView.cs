using System.Collections.Generic;
using System.Linq;
using Quackies.Core.Match;
using Quackies.Unity.Art;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>
    /// Presents completed Core die observations. It neither rolls a die nor changes
    /// a reward; Continue only dismisses the presentation overlay.
    /// </summary>
    public sealed class DiceResultsView : MonoBehaviour
    {
        [SerializeField] private GameObject shade;
        [SerializeField] private Image dieFace;
        [SerializeField] private TMP_Text heading;
        [SerializeField] private TMP_Text earnedBy;
        [SerializeField] private TMP_Text results;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button shadeButton;
        [SerializeField] private QuackiesArtCatalog catalog;
        private bool bound;

        public void Configure(GameObject dimmer, QuackiesArtCatalog art, Image face, TMP_Text title,
            TMP_Text earned, TMP_Text outcomeList, Button continueAction, Button shadeClose)
        {
            shade = dimmer;
            catalog = art;
            dieFace = face;
            heading = title;
            earnedBy = earned;
            results = outcomeList;
            continueButton = continueAction;
            shadeButton = shadeClose;
        }

        private void Awake()
        {
            if (bound) return;
            bound = true;
            continueButton.onClick.AddListener(Close);
            shadeButton.onClick.AddListener(Close);
        }

        /// <summary>Displays every result that appeared in the most recent Core update.</summary>
        public void ShowNew(MatchView view, IReadOnlyList<DieRollView> newRolls)
        {
            if (newRolls == null || newRolls.Count == 0) return;
            Render(view, newRolls, NoRoundBonusWasAwarded(view));
            shade.SetActive(true);
        }

        /// <summary>Lets the player reopen the last resolved die batch without changing the session.</summary>
        public void ShowReview(MatchView view)
        {
            var currentRound = view.DieRolls.Where(roll => roll.Round == view.Round).ToArray();
            if (currentRound.Length == 0)
            {
                RenderEmpty(view);
                shade.SetActive(true);
                return;
            }

            // A fortune die and a round bonus can happen in the same round. Review the
            // most recent current-round reason, rather than silently opening an old round.
            var latest = currentRound[currentRound.Length - 1];
            var batch = currentRound.Where(roll => roll.Reason == latest.Reason).ToArray();
            Render(view, batch, NoRoundBonusWasAwarded(view));
            shade.SetActive(true);
        }

        public void Close() => shade.SetActive(false);

        private void Render(MatchView view, IReadOnlyList<DieRollView> rolls, bool noRoundBonusAwarded)
        {
            var first = rolls[0];
            heading.text = "ROUND " + first.Round + " — "
                + (first.Reason == DieRollReason.Fortune ? "FORTUNE DIE RESULTS" : "BONUS DIE RESULTS");
            earnedBy.text = EarnedBy(view, rolls, first.Reason);
            results.text = string.Join("\n", rolls.Select(roll => PlayerName(view, roll.PlayerId) + ": "
                + roll.Description + (roll.RewardApplied ? "" : " (unavailable)")));
            if (noRoundBonusAwarded)
                results.text += "\n\nNo round bonus die was awarded: both pots exploded.";
            var latest = rolls[rolls.Count - 1];
            dieFace.sprite = catalog.GetDieFace(latest.Face);
            dieFace.enabled = dieFace.sprite != null;
        }

        private void RenderEmpty(MatchView view)
        {
            heading.text = "ROUND " + view.Round + " — NO DIE RESULTS";
            dieFace.enabled = false;
            if (view.Players.All(player => player.Exploded))
            {
                earnedBy.text = "No bonus die was awarded.";
                results.text = "Both pots exploded, so no player was eligible for the round bonus die.";
                return;
            }
            earnedBy.text = "No die has been rolled yet.";
            results.text = "Completed bonus and fortune die results will appear here.";
        }

        private static bool NoRoundBonusWasAwarded(MatchView view)
        {
            return view.Players.All(player => player.Exploded)
                && !view.DieRolls.Any(roll => roll.Round == view.Round && roll.Reason == DieRollReason.RoundBonus);
        }

        private static string EarnedBy(MatchView view, IReadOnlyList<DieRollView> rolls, DieRollReason reason)
        {
            var players = rolls.Select(roll => PlayerName(view, roll.PlayerId)).Distinct().ToArray();
            var names = string.Join(" and ", players);
            var rollsSuffix = rolls.Count == 1 ? "a die roll" : rolls.Count + " die rolls";
            if (reason == DieRollReason.Fortune)
                return names + " earned " + rollsSuffix + " from the fortune card.";
            if (players.Length > 1)
                return names + " earned " + rollsSuffix + " after tying for the leading scoring space.";
            return names + " earned " + rollsSuffix + " for the leading scoring space.";
        }

        private static string PlayerName(MatchView view, string playerId)
        {
            var player = view.Players.FirstOrDefault(candidate => candidate.Id == playerId);
            return player == null ? playerId : player.Name;
        }
    }
}
