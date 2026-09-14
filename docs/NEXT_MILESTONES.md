# Future Quackies milestones

Updated 14 September 2026. The active target is the original ten-Day duck game,
with approved 50-space rewards, prices and ten World Events. The authoritative
[plan](duck-migration/PLAN.md), [implementation plan](duck-migration/IMPLEMENTATION_PLAN.md)
and [status](duck-migration/STATUS.md) replace older 53-space/Penny art notes.
**M2 is complete:** all defaults and local save/resume are approved. Days 1–9
use visible independent draws; only Day 10 uses simultaneous decisions. Final
Twig ties use retained Night 10 Sleep, then a draw. M3 waits for the user's command.

## Next work, after the user's milestone command

1. **M3:** Prove the selected board's 50-space token/reward fit and full Dream view at
   iPad mini size in an isolated layout scene.
2. **M4:** Refactor Core with CLI support at each checkpoint: one complete Day/Night,
   then ten Days, all encounters/events, Normal AI and local autosave/resume.
3. **M5:** Bind the committed Core build to the measured Unity presentation and verify
   complete play, settings, restart and local match restoration.
4. **M6:** Gather balance evidence, polish readability/input and verify iOS export.

Stop for feedback between milestones. Regular source/test/Unity checkpoint
commits remain separate and are pushed; no automatic merge or public release.

## Deferred work

Shorter matches, expanded events/token rules, additional AI levels, network play,
a separate AI-history pane and physical-device installation remain later scope.
Optional Quacks test-tube rules belong to the retained reference-game backlog;
they are not an implicit requirement of the new duck profile.

The original playable baseline's 129 Core tests, nine-round Unity run and iOS
export are historical evidence in [PROGRESS.md](PROGRESS.md) and
[the baseline report](../tools/validation/evidence/2026-09-10/README.md).
Those results do not validate the not-yet-implemented duck game.
