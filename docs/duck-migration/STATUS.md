# Duck migration status

**M1 complete — awaiting the user's style feedback.** The user accepted M0 and
authorized M1 on 10 September 2026. Branch: `codex/duck-game-milestone-0`.

## Completed work

- Original playmat, happy duck and seed assets generated, inspected and published
  in `ff2c41b`, with prompts and import metadata in [ASSET_MANIFEST.md](ASSET_MANIFEST.md).
- Separate `DuckStyleTestScene.unity` implemented with a complete 54-position
  winding trail, duck start, representative seed encounters, resting preview,
  nest-score and resource placeholders, and Explore/Reset preview controls,
  published in `42e51b3`.
- Final Unity compile completed without errors; the captured console was empty.
  The native 1133 × 744 view contains all 54 cells, five rounded bends and no TMP
  text overflow. Repeat construction produced no duplicate cells.
- Explore changed three seeds/rest space 4 to five seeds/rest space 6; Reset
  restored the initial example. The duck stayed at space 0 and nest score at
  Twigs 12. The final scene uses code-native ellipses and 14pt space indices.
- [Native screenshot and validation](evidence/m1/README.md) are saved for review.
  The preview was left in Play mode at its reset state.

This is an isolated visual preview. It does not create a MatchSession, implement
M2 terminology support, or change game rules. The original playable scene, Core,
CLI, raw art and shipping scene list remain unchanged. Preserve the pre-existing
ProjectSettings preload removal and keep it out of commits.

## Review gate

Show the final native GameView capture and pause for the user's reaction before
M2. The user is judging the
happy cartoon direction, the full winding trail, and duck/seed readability at
actual use sizes. Further artwork and a shelter experiment remain later work.

## Earlier milestone

M0 completed with a clean solution build, 129 passing tests and a CLI smoke.
Its repository map and evidence are in [BASELINE.md](BASELINE.md); plan/art brief
checkpoint `bc55369`, baseline/review checkpoint `824fa5d`. The user accepted that
milestone. The current gradual roadmap remains [PLAN.md](PLAN.md).

## Resume without repeated discovery

Wait for the user's reaction; do not begin M2 automatically. When resuming,
inspect live Git and Editor state, then read this file and the evidence. Retain
one Editor owner and small worker briefs. Do not repeat unchanged Core tests for
this visual-only milestone. Lead owns Git and pushes coherent checkpoints; no
main merge or release is authorized.
