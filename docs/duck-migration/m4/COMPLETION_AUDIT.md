# M4 completion audit

M4 C1–C5 is complete and closed. The isolated committed-tree Release build at
cc4c11a completed with zero warnings/errors and 286/286 tests. M5 Unity
integration has not started and no duck Core DLL was synced into Unity.

| Requirement | Evidence |
| --- | --- |
| C1 profile, 43-space data, identity and saveable state | DuckFoundationTests, DuckDefinitionTests, DuckPersistenceTests; source checkpoints dae9174 / a286e25 and persistence 71d731e / 70be133 |
| C2 Adventure, encounters, endpoint and decision timing | DuckAdventureTests, DuckDayCycleTests; source/test checkpoints 3c2e73c / bebf161, with the then-untracked Dream dependency closed by C3 |
| C3 Night, Dream, Dawn and CLI day cycle | DuckNightResolutionTests, DuckDayCycleTests, DuckCliTests; source/tests 64f4c58 / dc19f33, CLI/tests 2ac42de / 754f544 |
| Ten-Day calendar, Goose, final results and winner flow | DuckCalendarTests; source/tests 9eba093 / 48f1dca |
| Normal policy | DuckNormalPolicyTests; source/tests c723812 / 7e5ca37; latest worn-out-opponent estimate correction 5ff6996 / cc4c11a |
| Calendar-facing presentation state | DuckMatchView authoritative NestLevel remains level 3 on Day 10 while purchase limit is 0; source/tests cb59c9b / 1b58957, CLI eef9a0a / e01aceb |
| CLI full-game and persistence commands | DuckCliTests, DuckCliPersistenceTests; source/tests 6bad090 / 129b2ba |
| Exact resume state and action-boundary restoration | DuckResumeIntegrationTests; seeds 0/42/137, 427 after-action JSON restores, 72 purchases, 11 private previews and 6 pending Day 10 commits; integration checkpoint cf295d7 |
| Adventure regressions | Endpoint overshoot and final-Day wear-out preserving another committed draw; checkpoint 673a177 |
| Classic reference and component boundaries | All original regressions pass in the full 286-test suite. Core targets netstandard2.1 and capture/restore owns no JSON, filesystem or Unity dependency; CLI owns storage. |

The CLI contract is human versus Normal by default. --two-player is a
developer mode; --demo-game runs a full Normal-versus-Normal match;
--demo-day runs the earlier Day 1 → Night 1 → Day 2 demonstration; and
--inspect prints the catalogue. Interactive mode autosaves by default;
--save selects a path, --continue resumes, --new-game starts fresh, r restarts
and q quits. Host persistence uses atomic JSON writes with a previous-action
.bak recovery path and a user-facing recovery message. Core persistence remains
serializer-neutral through the DuckSaves API.

The [final build and test log](m4-final-validation.txt), [full-game output](cli-full-game.txt)
and [final save](full-game-save.json) are durable evidence. The seed-42 full game
exited 0 with AI 29 Twigs versus Human 27; both had frozen Sleep 12 and four
Dream Twigs. The [policy review](POLICY_REVIEW.md) records decision checks and
limitations. Earlier C3 outputs remain historical evidence in
[the short day cycle](cli-day-cycle.txt) and [catalogue inspection](cli-inspection.txt).

This audit does not claim complete-game balance, M5 Unity binding, device
validation, iOS export, or public release. M6 still owns seeded balance review,
readability/performance work and export validation. The implemented Dawn bands
remain 0–2 → 0, 3–6 → 1, 7–10 → 2 and 11+ → 3; this is a recorded product
choice, not a balance conclusion.
