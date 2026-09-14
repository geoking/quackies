# Quackies overall plan

Updated 14 September 2026. **M2 and M3 are complete and approved by the user.**
M3 closes with the 43-space board and reward typography at `2c7cd6a`.
**M4 is active. The user authorized C1–C3, then a progress report before C4.**
Historical decisions and checks remain in [PROGRESS.md](../PROGRESS.md).

## What we are building

A ten-Day tabletop game for iPad mini landscape, initially one human versus
Normal AI. Ducks adventure across wetlands, meadow and wasteland by Day, then
enter a full-screen Dream/nest view at Night. Each duck owns a bag, board progress
and nest; World Events affect the shared world. Most persistent Twigs, including
final Dream Twigs, wins. Equal Twigs use retained final-Night Sleep, then a draw.

The approved v1 includes the complete encounter catalogue, ten shuffled World
Events, all 43 rewards, 11 shop offers, rival-board inspection, restart and local
save/Continue. Nest purchase capacity follows the calendar. Feather trails give
one permanent starting step per Feather; Dawn Delivery is capped at three per
Day. Most Rested grants a temporary next-Day step on Nights 1–9. Days 1–9 allow
independent decisions with public completed actions; only Day 10 uses hidden
simultaneous decisions. The final Night converts retained Sleep to Dream Twigs.

## Authoritative specifications

Keep each detailed rule in its own reference rather than duplicating full lists:

| Reference | Authority |
| --- | --- |
| [Rules at a glance](RULES_AT_A_GLANCE.md) | Complete approved game loop and rule recap |
| [Encounter timing](ENCOUNTER_RULES.md) | Token powers, movement, protection and ordered Night resolution |
| [Board and shop](v1/BOARD_AND_SHOP.md) | All 43 rewards, 11 prices, payout examples and balance limits |
| [World Events](v1/WORLD_EVENTS.md) | Ten approved cards, triggers, collective conditions and timing |
| [Implementation plan](IMPLEMENTATION_PLAN.md) | M2 defaults, M4 checkpoints, persistence and acceptance checks |
| [Board JSON](v1/board.json), [CSV](v1/board.csv), [shop JSON](v1/shop.json) | Exact numeric data for the future duck profile |
| [Bounded audit](v1/balance-audit.json) | Scoped arithmetic and explicit assumptions, not full-match balance |
| [M3 closeout](m3-closeout/README.md) | Approved visual state, data reconciliation and evidence |

The 43 rows are synchronized with the accepted Unity layout. Havens are
**4, 10, 16, 21, 26, 32, 36 and 43**. Space 3 gives 5 Sleep/1 Twig; the Reed
hammock at 4 gives 6 Sleep/1 Twig/1 Feather. Space 5 starts the 2-Twig plateau.
The other six nonendpoint havens match both neighbours' Twigs. The oasis is the
endpoint exception at **21 Sleep/9 Twigs/2 Feathers**. Do not restore the earlier
50-space table or haven-3 placement.

## Accepted M3 presentation

The board is an illustrated tabletop with an incomplete nest at upper-left,
cool wetlands, warm meadow, harsh wasteland and the oasis at upper-right. Dense
foliage and terrain explain why the duck follows the route. Shelter entrances
stay visible; both bridges are clear of tiles and connect naturally to the path.
The wasteland bridge is the dramatic timber-and-rope crossing over a cleft.

The route divides into **14 wetlands / 14 meadow / 15 wasteland** spaces. The
42 normal/haven tiles sit along the painted paths, aligned to the seven on-board
shelters; space 43 uses the native oasis painting. Keep the approved centres,
spacing and clearance around trees, logs, shores and shelter entrances.

- Tiles are distinguishable, playful painted shapes with biome colours and
  richer leafy haven edges. Their 108 × 84 board-pixel footprint is the accepted
  reference; do not return to faint ground decals or external reward strips.
- Fifteen tile-art variants contain the correct scattered twig counts. Twigs
  are floor decoration and may be covered by a chip. The exact live Twig number
  stays at right-middle; the live Sleep number sits beside the bottom moon.
  There are no top-of-tile index labels or penny/T0 notation.
- Larger, bright haven Feathers occupy the lower-left tile pocket. The oasis has
  two slightly smaller Feathers in front of the pool, with winnings below.
- All chips and the resting duck use the shared 64-pixel frame, offset (16, -11)
  within a tile. The landing is more central and no longer shifts left to avoid
  twig artwork. Larger candidates covered rewards; preserve the tested fit.
- Fredoka SemiBold with black outlines is used throughout. The 86 board reward
  labels have a stronger dedicated outline and slightly fuller face; Twig
  numerals are 22 units. Sleep numerals are raised 0.84 board pixels to contain
  the stroke. The footer explains rest-where-you-land scoring and reward icons.
- Board taps open inspections. The full-screen Dream fixture has room for all
  11 offers and View adventure navigation. Its live purchases, nest growth and
  final Concept-B presentation are M5 work, driven by Core state.

The accepted source is `Assets/Art/DuckLayout/board.png` at **1536 × 1024**.
`board-layout.json`, `tile-art.json`, `tile-crops.json` and the scene builder
reproduce the layout. Native sprite outlines isolate approved art; mipmapped
filtering keeps the painted tile sheets clean at tablet scale. The detailed
3072 × 2048 painted master remains explicitly deferred, with a 4096 import cap
and resolution-independent UI. A larger render is not a higher-detail painting.

M3 evidence includes actual 1133 × 744 and 2732 × 2048 renders, 43 centre
interactions, all 16 chips checked at all 42 placement spaces, 19 rendered-mesh
comparisons, 150 labels without overflow and zero final compilation/Console
errors. These checks validate the fixed layout, not the new game's rules or
complete match. Earlier visual studies and rejected proofs remain historical.

## Token and future-art philosophy

Keep the approved [player ducks](concepts/2026-09-11/duck-player-tiles-v2.png),
[16 encounter designs](concepts/2026-09-13-agreed-token-set/README.md),
[zzz marker](concepts/2026-09-14-most-rested/README.md) and
[Dream Concept B](concepts/2026-09-13-dream-study/concept-b-nest-mat.png) as the
visual vocabulary. Dream concept prices and Feather-spending controls are obsolete.

Player ducks are goofy, distinct-coloured tabletop tokens. Each encounter type
keeps one recognizable silhouette across variants, a strong face/rim colour,
rounded sturdy edges and original illustration. Different types can use different
shapes but must fit the common landing and leave reward information visible.
Colour alone must not identify a type. Default movement one is unprinted;
movement arrows give total movement, while Reeds ×1/×2/×3 communicate Twig yield.
Separate those meanings in both art and data. The Companion is an encounter,
not another player marker. The zzz award reads differently from a permanent
Feather. Exact values and changing game state remain precise overlays.

Use the terms Day, Explore, Settle down, Exhaustion, Worn out, Sleep, Twigs,
Feather trail, Dream choices, Most Rested Duck, World Event and Dawn Delivery.
Production guidance remains in [ASSET_BRIEF.md](ASSET_BRIEF.md); current accepted
M3 assets and closeout evidence take precedence over older art-study coordinates.

## Build sequence from here

**Evolve the existing Core/API/CLI.** Keep match-session ownership, immutable
observations, deterministic randomness and the snapshot/legal-actions/execute
boundary. Refactor the changed state and rule policies; preserve the tested
original game as a reference profile. Do not restart the solution, copy the
whole match loop, or put rule arithmetic into Unity.

| Milestone | Status and result |
| --- | --- |
| M0 / initial M1 | Historical original baseline and art exploration |
| M2 — Rules sheet | Complete: rules, data, events, defaults and local save/resume scope approved |
| M3 — Layout | Complete: final 43-space visual proof approved by the user |
| **M4 — Core/CLI** | **Active through C3; C4/C5 await the next review.** C1 foundations/data → C2 Adventure/exact draws → C3 complete Day/Night → C4 ten-Day matches → C5 Normal AI/save-resume |
| M5 — Connected Unity | Bind the accepted board/Dream views to committed Core state/actions and complete a human/AI match |
| M6 — Balance/export | Review seeded match outcomes, tune with approval, finish readability/performance and validate iOS export |

The **current M4 checkpoint is C1**: establish the duck profile, import
all approved data, separate token movement from ability quantities, and define
saveable match state while keeping the classic regression suite passing. Update
the CLI alongside each slice. The [detailed C1 plan](IMPLEMENTATION_PLAN.md#c1--the-first-work-after-m4-approval)
sets its deliverables and stopping boundary.

C1 found a [reachable setting-3 overflow](m4/starting-feather-review.md): permanent
Day-10 start 43 plus Most Rested gives effective start 44. Default zero remains
proven safe. A user decision on this endpoint policy is pending; no cap,
conversion, lost Feather or removed setting is approved. Independent C2/C3
work continues. Actual full-match balance remains M6 work.

Stop for user review at each milestone. M4 source and focused tests are small,
separate checked commits with regular GitHub checkpoints; root owns Git and
Unity integration. Follow [AGENTS.md](../../AGENTS.md) and
[ARCHITECTURE.md](../ARCHITECTURE.md). M5, exports, device installation and public
release are separate steps, not implicit consequences of approving M4.

Shorter matches, alternate rule cards, more World Events/AI levels, networking,
test tubes, a separate AI-history pane and physical-device installation remain
deferred. Four duck identities do not expand the first human-versus-AI scope.
Release title remains undecided.
