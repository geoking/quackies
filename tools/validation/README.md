# Unity full-match validation

`QuackiesUnityFullMatchProbe.cs` is an Editor-only validation harness. It is
compiled ephemerally and does not enter the Unity player build. The public entry
point is `FullMatchUiDriver.Start()`.

## Preconditions

1. Open `unity/Quackies.Unity` in Unity 6000.6.0f1.
2. Confirm the Editor is stopped and no live validation probe is running. Read
   `Temp/QuackiesValidation/full-match-report.txt` and check the Unity process
   state before resuming. Do not restart an active probe because an old report
   looks stale.
3. In Edit mode run **Quackies → Build and Play Initial Scene**. This rebuilds
   and saves the scene, then starts Play mode. The driver refuses to start
   outside Play mode.

The probe dispatches `IPointerClickHandler` callbacks through Unity's
`EventSystem` and drives the human side with Core `NormalPolicy`; the AI side
uses the live match presenter. This proves callback wiring, legal actions,
Core-to-UI state, labels, references, dice, settings restart and the complete
nine-round path. It does not prove native mouse clicks, physical touch,
device behavior or an iOS export.

## Run-script request

The exact command schema is available without invoking an Editor action:

```sh
/Users/george/.unity/bin/unity command --query run_script --json
```

The `run_script` request must use these fields:

```json
{
  "file": "../../tools/validation/QuackiesUnityFullMatchProbe.cs",
  "entry": "FullMatchUiDriver.Start",
  "mode": "ephemeral",
  "args": [],
  "timeout_ms": 30000,
  "dry_run": false
}
```

`file` is relative to the Unity project root (the parent of `Assets/`), so the
two parent segments reach the repository's `tools/validation/` directory.
`entry` is the static method above. `ephemeral` is the default and is required
for execution; `dry_run: true` only compiles and does not start the driver.
The command schema also permits `references`, `defines`, and `pdb`; none are
needed here. The schema's `timeout_ms` bounds the dispatcher wait, while the
driver itself continues asynchronously through the stages below.

## Resume protocol and stages

The driver writes durable files under
`unity/Quackies.Unity/Temp/QuackiesValidation/`:

- `full-match-report.txt` contains the current `state`, round, phase, checks,
  pointer actions and observed fortune titles.
- `resume-active-fortune` acknowledges the active-fortune capture.
- `resume-final-score` acknowledges the final-results capture.

The normal sequence is `starting`, settings open/toggle/apply restart, surface
checks (fortune reference, scoreboard and opponent pot), then
`capture-active-fortune-ready`. At that state, use `capture_game_view` with
`source=screen` at the current aspect so overlay UI is included. After saving
the capture, create the empty `resume-active-fortune` marker and let the probe
continue. At the final scoreboard it writes `capture-final-score-ready`; take
the corresponding current-aspect screen capture, create
`resume-final-score`, and allow the driver to finish.

The terminal report state is `complete` or `failed`. A complete report includes
nine distinct fortune titles, a final score, at least one rendered dice result,
readable Core choice labels, and successful settings/reference/scoreboard/pot
callbacks. A failed report includes the exception and should be preserved for
diagnosis. Do not delete or overwrite an active report while the probe is live.
