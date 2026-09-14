# Duck migration status

Updated 14 September 2026. **M2 and M3 are complete and approved by the user.**
The accepted visual checkpoint is `2c7cd6a`. **M4 is active through C3, then
stops for a progress report.** No duck Core session is bound to Unity.

## Closed milestones

M2 approved the ten-Day game, complete encounter/event catalogues, rewards and
prices, independent Days 1–9 actions, hidden final-Day decisions, final Twig/Sleep
ranking and local save/Continue requirement. These are specifications, not a
claim that the new game is implemented or balanced through complete matches.

M3 approved the fixed-data board/Dream layout and its visual refinements:

- 43 scorable spaces split 14 wetlands / 14 meadow / 15 wasteland; havens at
  4, 10, 16, 21, 26, 32, 36 and 43. JSON/CSV match the accepted Unity rows.
- Distinguishable biome tiles, scattered twig artwork, right-middle Twig
  numbers and bottom moon/Sleep information, with more prominent haven Feathers.
- Centred 64-pixel chip/duck frames; corrected route/shelter/scenery clearances;
  smaller oasis Feathers in front of the pool and winnings below.
- Rounded outlined typography throughout, stronger outlines on the 86 reward
  labels, slightly larger Twig numerals and a clear rest-where-you-land footer.

[M3 closeout](m3-closeout/README.md) links the accepted sources and recorded
1133 × 744 / 2732 × 2048 audits, 43 centre interactions, 672 chip placements,
19 rendered-mesh comparisons, 150 fitted labels and final zero-error compilation
and Console checks. The proof uses fixed sample data; it is not the completed
duck game. Old studies and rejected versions remain historical evidence.

The source painting remains 1536 × 1024. The detailed 3072 × 2048 master is
explicitly deferred; higher render dimensions do not create new painted detail.

## Active: M4 Core/CLI through C3

Evolve the existing engine and CLI, preserving the original tested profile and
snapshot/legal-actions/execute boundary. C1–C3 are authorized; C4/C5 remain later:

1. C1: exact duck data, token identity, saveable state and profile foundation.
2. C2: Adventure, encounters, exact private previews and decision timing.
3. C3: a complete Day → Night → next-Day CLI cycle and all event fixtures.
4. C4: all ten Days, Dawn/Goose/nest transitions, final conversion and winners.
5. C5: Normal AI, local autosave/Continue and deterministic resume checks.

The [C1 brief](IMPLEMENTATION_PLAN.md#c1--the-first-work-after-m4-approval)
defines the first deliverable and its tests. The approved shared 0–3 starting
Feather setting must be checked against the 43-space endpoint before all options
are exposed. The [endpoint review](m3-closeout/endpoint-review.md) distinguishes
current default-zero evidence from the optional settings. C1 now has a
[constructive setting-3 overflow](m4/starting-feather-review.md), and the user
has been asked to decide the endpoint policy. No adjustment is approved yet.
Do not silently cap/convert Feathers or remove approved options; surface a
specific rule decision to the user if analysis requires one.

Current implementation: shared typed command boundary, resumable randomness,
exact catalogue and saveable state foundation are checked. All 174 tests pass
at the state/isolated-Night boundary. Adventure and Dream/Dawn integration are
active; [M4 evidence](m4/README.md) records checkpoints and outstanding work.

M5 connects the accepted Unity board/Dream views to committed Core actions/state,
finishes runtime nest/award/event presentation, and verifies a complete human/AI
match. M6 covers actual balance evidence and iOS export. Device installation,
networking, shorter games, alternate rules and additional content remain deferred.

The [overall plan](PLAN.md), [implementation plan](IMPLEMENTATION_PLAN.md),
[rule recap](RULES_AT_A_GLANCE.md), [encounters](ENCOUNTER_RULES.md),
[board/shop](v1/BOARD_AND_SHOP.md) and [events](v1/WORLD_EVENTS.md) are the current
contract. The original 129-test baseline is historical evidence in
[BASELINE.md](BASELINE.md). The ProjectSettings preload-removal draft remains
unchanged and outside this documentation checkpoint.
