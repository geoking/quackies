# Duck migration status

**M0 complete — paused for the user's reaction.** M1 has not started.
Updated 10 September 2026. Branch: `codex/duck-game-milestone-0`.

## What changed

- Created and published a new branch from merged playable baseline `74e40cf`.
- Reconciled the design chat through Revision 4 into [PLAN.md](PLAN.md): agreed
  duck vocabulary, winding trail, daily route versus persistent nest, unchanged
  rules initially and a separately approved shelter experiment later.
- Wrote the [three-asset brief](ASSET_BRIEF.md) for M1: playmat, happy duck, seeds.
- Recorded the actual Core/API/CLI/Unity boundaries and compatibility risks in
  [BASELINE.md](BASELINE.md), with current build/test/tool evidence.
- Kept the existing worker settings and checkpoint workflow, adding the requested
  pause for feedback at every milestone.

No gameplay or artwork changed. The existing Unity settings preload removal is
still present and excluded from commits. No asset generation, scene rebuilding,
rule changes, main merge or release was performed.

## M0 acceptance

| Requirement | Evidence |
| --- | --- |
| Recover agreed plan and amendments | PLAN.md source/provenance and shared terminology |
| Inspect real architecture, clients, effects, AI and persistence | BASELINE.md Core/CLI/Unity map and compatibility findings |
| New branch from verified current state | Baseline hash, origin comparison and workspace record |
| Preserve unrelated work | Recorded settings patch/hash; file remains uncommitted |
| Current baseline checks | Build: 0 warnings/errors; tests: 129 passed; CLI smoke exits cleanly |
| Verify Unity project | Connected 6000.6.0f1, correct checkout and clean saved initial scene, no captured errors |
| Verify art capability without generating a batch | Available imagegen tool/skill recorded; no invocation or billed fallback |
| Repository-specific plan, asset brief and worker assignments | PLAN.md, ASSET_BRIEF.md and retained .codex role configuration |
| Regular visible progress | Plan/art checkpoint `bc55369`; baseline/evidence checkpoint follows in branch history |
| Pause and request reaction | This is the review boundary; do not begin M1 until the user responds |

## Smallest next task, after feedback

M1: generate the three style assets, then have one Unity owner create a separate
`DuckStyleTestScene.unity` with a full 54-position winding placeholder trail,
duck at its start, representative seed encounters ahead, a resting preview and
resource/button samples. Capture at the actual 1133 × 744 GameView target and
ask how the happy cartoon direction feels before doing terminology or gameplay
integration. Use placeholders for Twigs, Pond pennies, Feathers and nest art.

The proposed 6 × 9 trail is a readability experiment. Release title, shelter
density/reward values and polished nest growth are still undecided. The first
playable duck version must retain current mechanics.

## Resume without repeated discovery

Read this file, PLAN.md and BASELINE.md. Inspect live Git/Editor state before
mutations. The current code baseline and tests are already recorded; repeat
checks only for new changes or a specific concern. Lead owns Git; workers never
stage/commit and only one owner may mutate the connected Unity Editor.
