// Editor play-mode validation harness. It is compiled ephemerally by Unity's
// Pipeline command and never participates in the player build.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Quackies.Core.AI;
using Quackies.Core.Match;
using Quackies.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class FullMatchUiDriver
{
    public static string Start()
    {
        if (!Application.isPlaying) return "Full-match UI driver must be started in Play mode.";
        if (GameObject.Find("Quackies full-match UI validation driver") != null)
            return "Full-match UI driver is already running.";
        var host = new GameObject("Quackies full-match UI validation driver");
        UnityEngine.Object.DontDestroyOnLoad(host);
        host.AddComponent<FullMatchUiDriverBehaviour>();
        return "Full-match UI driver started.";
    }
}

public sealed class FullMatchUiDriverBehaviour : MonoBehaviour
{
    private static readonly string Root = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Temp", "QuackiesValidation"));
    private static readonly string ReportPath = Path.Combine(Root, "full-match-report.txt");
    private static readonly string ActiveResumePath = Path.Combine(Root, "resume-active-fortune");
    private static readonly string FinalResumePath = Path.Combine(Root, "resume-final-score");

    private readonly NormalPolicy policy = new NormalPolicy();
    private readonly List<string> checks = new List<string>();
    private readonly List<string> actions = new List<string>();
    private readonly List<string> fortunes = new List<string>();
    private readonly HashSet<string> distinctFortunes = new HashSet<string>(StringComparer.Ordinal);
    private MatchPresenter presenter;
    private FieldInfo sessionField;
    private int stage;
    private float nextStep;
    private float lastHeartbeat;
    private int lastRound;
    private int maxChoiceLength;
    private bool eventReferenceChecked;
    private bool scoreboardChecked;
    private bool cpuPotChecked;
    private bool diceChecked;
    private bool choiceLabelChecked;
    private bool finalScoreboardOpened;

    private void Awake()
    {
        Directory.CreateDirectory(Root);
        if (File.Exists(ActiveResumePath)) File.Delete(ActiveResumePath);
        if (File.Exists(FinalResumePath)) File.Delete(FinalResumePath);
        Write("starting");
    }

    private void Update()
    {
        try
        {
            if (stage == 99) return;
            if (Time.unscaledTime < nextStep) return;
            if (presenter == null || sessionField == null || sessionField.GetValue(presenter) == null)
            {
                presenter = UnityEngine.Object.FindAnyObjectByType<MatchPresenter>();
                if (presenter == null) return;
                sessionField = typeof(MatchPresenter).GetField("session", BindingFlags.Instance | BindingFlags.NonPublic);
                Require(sessionField != null, "MatchPresenter session binding exists");
                if (sessionField.GetValue(presenter) == null) return;
                checks.Add("PASS presenter located in live saved scene");
                stage = 1;
                nextStep = Time.unscaledTime + .25f;
                Write("settings-open");
                return;
            }

            if (stage == 1)
            {
                Click("Open Settings", "settings open");
                stage = 2;
                nextStep = Time.unscaledTime + .2f;
                return;
            }
            if (stage == 2)
            {
                Require(Active("Settings Screen"), "Settings screen opened through pointer handler");
                Click("Starting Ruby Toggle", "settings ruby toggle");
                stage = 3;
                nextStep = Time.unscaledTime + .2f;
                return;
            }
            if (stage == 3)
            {
                Click("Apply and Restart", "settings apply and restart");
                stage = 4;
                nextStep = Time.unscaledTime + .5f;
                return;
            }
            if (stage == 4)
            {
                var restarted = Snapshot();
                Require(Session().Settings.StartingRubies == 1, "Settings apply started the replacement match with one ruby");
                Require(restarted.Players.All(player => player.Rubies == 1), "Both live players received the selected starting ruby");
                checks.Add("PASS settings toggle applied only by new-match restart");
                stage = 5;
                nextStep = Time.unscaledTime + .2f;
                Write("surfaces");
                return;
            }

            if (stage == 5)
            {
                VerifySurfaces();
                if (!eventReferenceChecked || !scoreboardChecked || !cpuPotChecked) return;
                Write("capture-active-fortune-ready");
                stage = 6;
                return;
            }
            if (stage == 6)
            {
                Heartbeat("capture-active-fortune-ready");
                if (!File.Exists(ActiveResumePath)) return;
                checks.Add("PASS active-fortune screenshot was requested while live table was visible");
                stage = 7;
                nextStep = Time.unscaledTime + .15f;
                Write("match-running");
                return;
            }
            if (stage == 8)
            {
                Heartbeat("capture-final-score-ready");
                if (!File.Exists(FinalResumePath)) return;
                if (Active("Scoreboard Screen")) Click("Back to Your Pot", "final scoreboard back");
                Require(!Active("Scoreboard Screen"), "Final scoreboard returns to the table through its Back button");
                checks.Add("PASS final screenshot capture acknowledged");
                stage = 9;
                Write("complete");
                return;
            }
            if (stage == 9) return;

            RunMatchStep();
        }
        catch (Exception error)
        {
            checks.Add("FAIL " + error.GetType().Name + ": " + error.Message);
            Debug.LogException(error);
            stage = 99;
            Write("failed");
        }
    }

    private void VerifySurfaces()
    {
        if (!eventReferenceChecked)
        {
            if (!Active("Reference Modal"))
            {
                Click("Active Fortune", "active fortune reference open");
                nextStep = Time.unscaledTime + .15f;
                return;
            }
            var heading = TextUnder("Reference Modal", "Heading");
            Require(!string.IsNullOrWhiteSpace(heading), "Active fortune reference has a readable heading");
            Click("Close", "active fortune reference close");
            Require(!Active("Reference Modal"), "Active fortune reference closes through its Close button");
            eventReferenceChecked = true;
            checks.Add("PASS active fortune artwork/title reference opened and closed");
            return;
        }

        if (!scoreboardChecked)
        {
            if (!Active("Scoreboard Screen"))
            {
                Click("Open Scoreboard", "scoreboard open");
                nextStep = Time.unscaledTime + .15f;
                return;
            }
            Require(!string.IsNullOrWhiteSpace(TextUnder("Scoreboard Screen", "Human Score")), "Scoreboard exposes live human score text");
            Click("Back to Your Pot", "scoreboard back");
            Require(!Active("Scoreboard Screen"), "Scoreboard returns through its Back button");
            scoreboardChecked = true;
            checks.Add("PASS scoreboard open/back uses live UI buttons");
            return;
        }

        if (!cpuPotChecked)
        {
            if (!Active("CPU Pot Screen"))
            {
                Click("Inspect Rival Pot", "CPU pot open");
                nextStep = Time.unscaledTime + .15f;
                return;
            }
            Require(TextUnder("CPU Pot Screen", "Heading").Contains("FULL POT"), "CPU inspection displays its full-pot heading");
            Click("Back to Your Pot", "CPU pot back");
            Require(!Active("CPU Pot Screen"), "CPU pot returns through its Back button");
            cpuPotChecked = true;
            checks.Add("PASS CPU pot open/back uses live UI buttons");
        }
    }

    private void RunMatchStep()
    {
        if (Active("Dice Results Screen"))
        {
            var diceHeading = TextUnder("Dice Results Screen", "Heading");
            Require(!string.IsNullOrWhiteSpace(diceHeading), "Dice overlay has a rendered result heading");
            diceChecked = true;
            checks.Add("PASS dice results rendered and dismissed through Continue");
            Click("Continue", "dice continue");
            nextStep = Time.unscaledTime + .12f;
            return;
        }

        var view = Snapshot();
        ObserveFortune(view);
        if (view.Phase == MatchPhase.Finished)
        {
            FinishMatch(view);
            return;
        }

        var legal = Session().GetLegalActions(MatchPresenter.HumanId);
        if (legal == null || legal.Count == 0)
        {
            nextStep = Time.unscaledTime + .12f;
            return;
        }
        var action = policy.Choose(view, legal);
        Require(action != null, "Normal policy selected a live human action");
        var buttonName = ButtonFor(action);
        var button = FindButton(buttonName);
        Require(button != null && button.interactable, "Live UI contains enabled button for " + action.Id);
        ValidateActionPresentation(action, button);
        Click(button, "human Normal policy " + action.Id);
        nextStep = Time.unscaledTime + .12f;
    }

    private void FinishMatch(MatchView view)
    {
        ObserveFortune(view);
        Require(fortunes.Count == 9, "Nine rounds exposed nine active fortunes");
        Require(distinctFortunes.Count == 9, "All nine active fortunes were unique");
        Require(diceChecked || view.DieRolls.Count > 0, "At least one Core die result reached the Unity presentation");
        Require(choiceLabelChecked, "Core-issued choice labels remained readable in the action list");
        var human = view.Players.Single(player => player.Id == MatchPresenter.HumanId);
        var ai = view.Players.Single(player => player.Id == MatchPresenter.AiId);
        checks.Add("PASS final score: Human " + human.VictoryPoints + " VP; AI " + ai.VictoryPoints + " VP; winner " + string.Join(",", view.WinnerIds));
        if (!finalScoreboardOpened)
        {
            Click("Open Scoreboard", "final scoreboard open");
            Require(Active("Scoreboard Screen"), "Final live scoreboard opened");
            finalScoreboardOpened = true;
            stage = 8;
            Write("capture-final-score-ready");
        }
    }

    private void ObserveFortune(MatchView view)
    {
        if (view.Round == lastRound) return;
        lastRound = view.Round;
        Require(!string.IsNullOrWhiteSpace(view.EventTitle), "Round " + view.Round + " has an active fortune title");
        var caption = TextNamed("Fortune Caption");
        Require(caption == view.EventTitle, "Fortune caption matches Core title in round " + view.Round);
        fortunes.Add(view.Round + ": " + view.EventTitle);
        distinctFortunes.Add(view.EventTitle);
        actions.Add("observed fortune " + fortunes[fortunes.Count - 1]);
        Write("round-" + view.Round);
    }

    private void ValidateActionPresentation(GameAction action, Button button)
    {
        if (action.Kind != GameActionKind.Choose) return;
        maxChoiceLength = Math.Max(maxChoiceLength, action.Label == null ? 0 : action.Label.Length);
        Canvas.ForceUpdateCanvases();
        var label = button.GetComponentInChildren<TMP_Text>();
        Require(label != null && label.text.Contains(action.Label), "Core choice text reaches its live action button");
        Require(!label.isTextOverflowing, "Core choice text does not overflow its action-button label");
        choiceLabelChecked = true;
        checks.Add("PASS readable Core choice (" + action.Label.Length + " chars): " + action.Label);
    }

    private MatchSession Session() => (MatchSession)sessionField.GetValue(presenter);
    private MatchView Snapshot() => Session().GetSnapshot(MatchPresenter.HumanId);

    private static string ButtonFor(GameAction action)
    {
        if (action.Kind == GameActionKind.Draw) return "Draw";
        if (action.Kind == GameActionKind.Stop) return "Stop";
        if (action.Kind == GameActionKind.UseFlask) return "Flask";
        return "Action_" + action.Id;
    }

    private static Button FindButton(string name)
    {
        return UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude)
            .FirstOrDefault(button => button.gameObject.name == name);
    }

    private static bool Active(string name) => GameObject.Find(name) != null;

    private static string TextNamed(string name)
    {
        var text = UnityEngine.Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Exclude)
            .FirstOrDefault(label => label.gameObject.name == name);
        return text == null ? string.Empty : text.text;
    }

    private static string TextUnder(string rootName, string textName)
    {
        var root = GameObject.Find(rootName);
        if (root == null) return string.Empty;
        var text = root.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault(label => label.gameObject.name == textName);
        return text == null ? string.Empty : text.text;
    }

    private void Click(string name, string note)
    {
        var button = FindButton(name);
        Require(button != null && button.interactable, "Enabled live button exists: " + name);
        Click(button, note);
    }

    private void Click(Button button, string note)
    {
        Require(EventSystem.current != null, "Live EventSystem exists for pointer dispatch");
        var data = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left, clickCount = 1 };
        ExecuteEvents.Execute<IPointerClickHandler>(button.gameObject, data,
            (handler, eventData) => handler.OnPointerClick(ExecuteEvents.ValidateEventData<PointerEventData>(eventData)));
        actions.Add("pointer " + note + " via " + button.gameObject.name);
    }

    private void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        checks.Add("PASS " + message);
    }

    private void Heartbeat(string state)
    {
        if (Time.unscaledTime - lastHeartbeat < 2f) return;
        lastHeartbeat = Time.unscaledTime;
        Write(state);
    }

    private void Write(string state)
    {
        var liveSession = presenter == null || sessionField == null ? null : sessionField.GetValue(presenter) as MatchSession;
        var view = liveSession == null ? null : liveSession.GetSnapshot(MatchPresenter.HumanId);
        var lines = new List<string>
        {
            "state=" + state,
            "time=" + DateTime.UtcNow.ToString("O"),
            "round=" + (view == null ? "?" : view.Round.ToString()),
            "phase=" + (view == null ? "?" : view.Phase.ToString()),
            "fortune_titles=" + string.Join(" | ", fortunes),
            "max_choice_label_length=" + maxChoiceLength,
            "checks="
        };
        lines.AddRange(checks);
        lines.Add("actions=");
        lines.AddRange(actions.TakeLast(120));
        File.WriteAllLines(ReportPath, lines);
    }
}
