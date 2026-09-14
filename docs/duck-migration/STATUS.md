# Duck migration status

Updated 14 September 2026. **M2 and M3 are complete and approved by the user.**
The accepted visual checkpoint is `2c7cd6a`. **M4 C1–C3 are complete for this
bounded slice; C4/C5 are unstarted pending the progress report.** No duck Core
session is bound to Unity.

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
snapshot/legal-actions/execute boundary. C1–C3 are complete; C4/C5 remain later:

1. C1: complete exact duck data, token identity, saveable state and profile foundation.
2. C2: complete Adventure, encounters, exact private previews and decision timing.
3. C3: complete the bounded Day → Night → next-Day CLI cycle and event fixtures.
4. C4: all ten Days, Dawn/Goose/nest transitions, final conversion and winners.
5. C5: Normal AI, local autosave/Continue and deterministic resume checks.

The [C1 brief](IMPLEMENTATION_PLAN.md#c1--the-first-work-after-m4-approval)
defines the completed foundation. Every duck starts at nest 0 with zero
Feathers. Permanent Feathers come only from safe haven rewards and Dawn Delivery
thresholds; Most Rested is a temporary +1. The default pre-Day-10 bound is at
most 42, so no effective-start cap is needed. The setting-3 witness in
[starting-feather-review.md](m4/starting-feather-review.md) is historical
evidence for an excluded configuration; its proof JSON/script remain unchanged.

Current implementation: C1 foundations are checked; C2 source/test checkpoints
are 3c2e73c / bebf161; C3 Night/Dream/Dawn/CLI integration is complete for the
bounded slice. Full-suite validation is 241/241 including real Day 1 → Night 1
→ Day 2 CLI, zero-start and Dawn checks. Normal AI and local save/Continue
remain later C5 work.

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
