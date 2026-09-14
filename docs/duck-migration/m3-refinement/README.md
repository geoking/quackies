> Historical 40-space review evidence. The user subsequently selected 43
> spaces; [the current M3 review](../m3-43/README.md) supersedes the route-count
> question below. Historical captures and measurements are preserved.

# M3 layout refinement: provisional 40-space fixture and route choice

Status: **reopened for a focused visual decision; no 40-space or 45-space
choice is final.** The previous revised-M3 evidence remains historical and is
rejected for the current spacing review. M2 remains approved. M4 is unstarted
and waits for the user's command.

## Current review question

The 90 × 66 wells and previous revised 50-space route were rejected as too
small or too tight for the intended board reading. The current
`Assets/Art/DuckLayout/board-layout.json` is a **provisional 40-space visual
fixture** using a 20% larger well treatment, approximately **108 × 79.2 design
units**. The [final candidate validation](validation.md) passes actual
1133 × 744 and 2732 × 2048 audits, including 40/40 center raycasts and
PointerClick inspections. The 45-space alternatives are decision options only
and have not all been generated. This document does not select 40 or 45, change canonical reward
rows, or change endpoint values. The candidate's visual reward labels are a
fixture-only remapping; canonical v1 board data remains 50 spaces.

The refinement must keep every tile center on a painted path, including the
curves through the wasteland. No tiles may sit on either bridge: leave the
painted approaches and deck gaps between spaces **13/14** and **26/27** clear.
Future duck animation needs explicit bridge waypoints for the approach/deck;
do not assume straight center interpolation. The provisional
40-space fixture proposes haven IDs **3, 10, 15, 20, 24, 29, 33 and 40**;
check shelter-entry alignment, including the bottom entries at **20/33**.
Tiles within a biome should share the biome tile colour, with a green leafy
nest border and a large integrated Feather 1 or Feather 2 treatment. Shelter
art and the path must remain visible.

The 3072 × 2048 board master remains deferred production work. Keep the
resolution-independent UI, high-resolution authoring plan and 4096 import cap.
Dream Concept-B likeness and fun typography remain M5 follow-ups. No Core,
CLI, rules or Unity gameplay work is part of this refinement.

## Evidence and decision record

- [x] Runtime audits pass at 1133 × 744 and 2732 × 2048; the 108 × 79.2
      treatment is reviewed against the 90 × 66 baseline.
- [ ] Compare the current 40 candidate with the user-question alternatives;
      the 45 alternatives are not assumed to exist until generated.
- [x] Confirm every candidate center follows painted path artwork, including
      wasteland curves.
- [x] Confirm bridges contain no tile wells or token placements, with clear
      approach/deck gaps at 13/14 and 26/27.
- [x] Confirm proposed haven entries 3/10/15/20/24/29/33/40 align with shelter
      entrances, including bottom entries 20/33, with no shelter covered.
- [x] Confirm same-biome tile colour, green leafy nest border and readable
      integrated Feather 1/2 treatment.
- [ ] Record the selected route count and any root-owned data migration only
      after user review. Until then, 40 versus 45 is unresolved and canonical
      v1 remains 50.

The prior [M3 revision README](../m3-revision/README.md) and
[validation record](../m3-revision/validation.md) remain historical/rejected
evidence for the 90 × 66/50-space presentation. They must not be treated as
approval of this refinement. Candidate compilation, Console, rebuild, texture,
runtime and pointer checks are recorded as passing in [validation.md](validation.md).
The route-count decision and M3 approval remain open.

## Read-only impact inventory if any route-count change is approved

No canonical v1 files are changed by this refinement. The visual fixture may
carry remapped sample labels for review, but those are not rules data. Root must
update the selected source of truth and regenerate dependent evidence together
only after an approved count choice.

- `docs/duck-migration/v1/board.json`, `board.csv` and
  `BOARD_AND_SHOP.md`: replace the 50-row reward table, havens and any endpoint
  row mapping; preserve shop prices and policies unless separately approved.
- `docs/duck-migration/v1/validation.json`: update board-row count, haven
  metadata and endpoint assertions.
- `docs/duck-migration/v1/balance-audit.py` and `balance-audit.json`: update
  `range(1, 51)`, `trail_spaces`, endpoint/maximum-start assertions,
  `HAVENS`, and any report values derived from the route length. Re-run the
  audit; do not weaken its checks.
- `docs/duck-migration/m3-revision/route-audit.py` is historical and must remain
  unchanged. Copy or adapt its checks into new refinement evidence, updating
  route counts, well counts, route indices and captured audit metadata only for
  the selected route count.
- `unity/Quackies.Unity/Assets/Art/DuckLayout/board-layout.json` and the
  DuckLayout editor/data validators: update row count, endpoint and route
  expectations only after the selected route-count decision is approved.
- Current migration docs that state “50 spaces” or “space 50” (including
  `PLAN.md`, `IMPLEMENTATION_PLAN.md`, `STATUS.md`, `HANDOFF.md`,
  `RULES_AT_A_GLANCE.md`, `ENCOUNTER_RULES.md` and `ENGINE_EVOLUTION.md`)
  require a coordinated wording/data review. Historical `docs/duck-migration/m3/`
  evidence remains unchanged.

The existing 50-space values, including endpoint 21 Sleep / 9 Twigs / 2
Feathers, remain current until the user selects a route count and root records
the resulting data migration.
