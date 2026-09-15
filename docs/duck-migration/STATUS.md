# Duck migration status

Updated 15 September 2026. **M2 and M3 are complete and approved by the user.**
The accepted visual checkpoint is 2c7cd6a. **M4 C1–C5 is complete and closed.** No
duck Core session is bound to Unity.

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

## M4 Core/CLI complete

Evolve the existing engine and CLI, preserving the original tested profile and
snapshot/legal-actions/execute boundary. C1–C5 implementation is complete:

1. C1: complete exact duck data, token identity, saveable state and profile foundation.
2. C2: complete Adventure, encounters, exact private previews and decision timing.
3. C3: complete the bounded Day → Night → next-Day CLI cycle and event fixtures.
4. C4: complete all ten Days, Dawn/Goose/nest transitions, final conversion and winners.
5. C5: complete Normal AI, local autosave/Continue and deterministic resume checks.

The [C1 brief](IMPLEMENTATION_PLAN.md#c1--the-first-work-after-m4-approval)
defines the completed foundation. Every duck starts at nest 0 with zero
Feathers. Permanent Feathers come only from safe haven rewards and Dawn Delivery
thresholds; Most Rested is a temporary +1. The default pre-Day-10 bound is at
most 42, so no effective-start cap is needed. The setting-3 witness in
[starting-feather-review.md](m4/starting-feather-review.md) is historical
evidence for an excluded configuration; its proof JSON/script remain unchanged.

Current implementation: C1 foundations and C2/C3 checkpoints are complete.
C4 calendar source/tests are 9eba093 / 48f1dca; C5 persistence is 71d731e /
70be133; Normal policy is c723812 / 7e5ca37; CLI integration is 6bad090 /
129b2ba. Resume integration covers seeds 0/42/137, 427 after-action restores,
72 purchases, 11 private previews and 6 pending Day 10 commits. The isolated
committed-tree Release build passed with zero warnings/errors and 286/286 tests;
see [M4 completion audit](m4/COMPLETION_AUDIT.md).

M4.5 is authorized and active: review AI quality, balance and playability using
actual Core legal games before Unity work. Its authority is the detailed
[M4.5 plan](m4-5/PLAN.md); [the working record](m4-5/README.md) tracks evidence.
AI corrections pass 328 tests. The [review report](m4-5/REPORT.md) preserves
3,600 fresh approved-price matches and 1,800 isolated proposed-price matches.
Exact price approval and human playability review remain pending; current Core
prices are unchanged.
M4 remains complete as Core/CLI implementation, without a claim of balanced
play or convincing AI. M5 then connects the accepted Unity board/Dream views to
committed Core actions/state and finishes runtime nest/award/event presentation.
M6 confirms the balance in connected Unity and handles readability, performance
and iOS export. Device installation, networking, shorter games, alternate rules
and additional content remain deferred.

The [overall plan](PLAN.md), [implementation plan](IMPLEMENTATION_PLAN.md),
[rule recap](RULES_AT_A_GLANCE.md), [encounters](ENCOUNTER_RULES.md),
[board/shop](v1/BOARD_AND_SHOP.md) and [events](v1/WORLD_EVENTS.md) are the current
contract. The original 129-test baseline is historical evidence in
[BASELINE.md](BASELINE.md). The ProjectSettings preload-removal draft remains
unchanged and outside this documentation checkpoint.
