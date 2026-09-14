# Quackies overall plan

Updated 14 September 2026. This is the current duck-game direction. It
supersedes the presentation-only Quacks migration; historical decisions and
validation remain in [PROGRESS.md](../PROGRESS.md). The latest user-requested
mechanics, all 50 board rewards and the 11 shop prices are accepted starting
values. Dawn gifts have the user-selected maximum of three, and the revised
ten World Events are approved. **M2 is complete:** all remaining defaults and
local autosave/resume are approved in the [implementation plan](IMPLEMENTATION_PLAN.md).
Drawing is independent on Days 1–9 and simultaneous only on Day 10; tied total
Twigs are decided by final-Night retained Sleep, then a draw if still equal.

## What we are building

A ten-Day tabletop game for iPad mini landscape, initially one human versus
Normal AI. Ducks adventure through wetlands, meadow and wasteland by Day, then
enter a full-screen Dream/nest view at Night. Each has its own bag and progress;
World Events affect everyone. Most persistent Twigs, including final Dream
Twigs, wins; tied Twigs use frozen retained Night 10 Sleep, then a draw. Keep
starting-progress settings, rival-board inspection, restart and local Continue game.

Use the selected [board](concepts/2026-09-12-approved/board-art-approved.png),
[V2 player ducks](concepts/2026-09-11/duck-player-tiles-v2.png) and approved
[16 encounter designs](concepts/2026-09-13-agreed-token-set/README.md).
[Dream Concept B](concepts/2026-09-13-dream-study/concept-b-nest-mat.png) is the
chosen full-screen layout with **View adventure** navigation and room for the
whole shop. Its illustrative prices and Feather-spending controls are obsolete.

**M2 is complete. The revised M3 layout is ready for user visual review.** The
earlier fixed-data proof and its [historical review evidence](m3/README.md)
remain available, but the user rejected its board concept and asset-resolution
assumption. M4 remains unstarted and waits for the user's command; no Core
gameplay work has begun.

## Current rule references

Keep a single source for each kind of detail instead of duplicating long lists:

| Reference | Authority |
| --- | --- |
| [Rules at a glance](RULES_AT_A_GLANCE.md) | Approved current game loop and rule details |
| [Encounter timing](ENCOUNTER_RULES.md) | All helpful/white powers, protection and movement ordering |
| [Board and shop](v1/BOARD_AND_SHOP.md) | Every one of 50 rewards, 11 prices, Night examples, approved policies and balance limits |
| [World Events](v1/WORLD_EVENTS.md) | Ten approved cards, once-per-game shuffled deck, triggers and ordering |
| [Implementation plan](IMPLEMENTATION_PLAN.md) | Approved M2 defaults, source audit, build checkpoints and acceptance checks |
| [Board data](v1/board.json), [CSV](v1/board.csv), [shop data](v1/shop.json) | Approved numeric values; not yet loaded by the game |
| [Bounded audit](v1/balance-audit.json) | Exact bag/counter arithmetic with explicit assumptions, not full-game balance |

## Design rules that guide implementation

- **Rest where the duck lands.** Fifty scorable spaces follow the separate,
  incomplete nest. No numbered-zero graphic or next-empty-space scoring.
  The approved table divides them 1–16 / 17–33 / 34–50, with havens at
  3, 11, 19, 27, 29, 37, 44 and 50. Seven nonendpoint haven tiles sit beside
  painted shelters; space 50 uses the oasis itself.
- **Comfort is not monotonic distance.** A haven improves Sleep over nearby
  spaces while keeping their Twigs. Exposed wasteland is less restful than late
  meadow, but its havens are especially comfortable. Endpoint printed values
  are just highest: 21 Sleep/9 Twigs/2 Feathers. All three wasteland havens now
  award two Feathers, superseding the old three-Feather endpoint.
- **Separate nightly comfort from victory.** Twigs persist. Sleep earned freezes
  for Most Rested; purchases reduce only remaining Sleep. Worn-out ducks keep
  earned Twigs and half Sleep rounded down. Safe-only rewards and final-chip
  deductions follow the approved timing contract.
- **Feathers have one job.** Each automatically extends the permanent starting
  trail by one. Never bank, spend, cap redemption or convert them. Awarding one
  cannot move today's rest or re-score a placement. Dawn Delivery now gives
  `min(3, ceil(Twig deficit / 4))`: zero when tied, 1 for deficits 1–4,
  2 for 5–8 and 3 for 9+. Snapshot scores before delivery. Repeated deficits
  pay again on later dawns. This caps the gift source, not Feather redemption
  or the one-Feather/one-step benefit.
- **Most Rested means highest Sleep among safe ducks.** All eligible ties share
  the award; if all wear out, there is none. Nights 1–9 grant a temporary start
  +1 tomorrow represented by the zzz marker beyond the updated Feather trail.
  Flock/event bonuses resolve before the comparison, and spending never changes it.
- **The ending uses Dreams.** On Day 10 each safe haven gives another +2 Sleep.
  On Night 10 convert retained Sleep at floor(Sleep/4) Dream Twigs and give
  safe Most Rested winners one extra Dream Twig. No Night 10 shopping or Day 11
  reward. Feathers have no final conversion. Rank total Twigs first, then
  frozen retained Night 10 Sleep before conversion; if both tie, declare a draw.
- **Nest capacity follows the calendar.** Both ducks have level 1 on Days 1–3,
  level 2 on 4–6 and level 3 on 7–10, allowing 1/2/3 purchases on purchasing
  Nights. Individual decorative nest growth shows score; score cannot unlock
  exclusive purchase capacity for the leader. Use a readable exact Twig total.
- **Original, readable encounters.** Seven helpful token types and five white
  designs retain the approved shapes. Default movement 1 is unprinted; arrows
  state total movement; Reeds quantities state bundle count/Twig yield.
  Starting bag remains eight whites plus five helpful chips. Goose joins once
  on Day 5. Mud now reduces the active flock, including later movement and its
  Night comparison. Splash protects just
  the next chip's nuisance, not its Exhaustion, and supplies no rescue.
- **Respond during the adventure.** Days 1–9 resolve each duck's Draw/Settle
  immediately, with completed actions visible for others to react to. Only Day 10
  uses hidden simultaneous decision beats. Signpost previews remain private.
- **One shared world.** Shuffle the ten event cards once and reveal one per
  Day without replacement. Their effects expire that Day. Future deck expansion
  can add cards; the approved ten-card deck mixes four helpful events, three
  collective goals and three mild setbacks. Collective rewards wait until all
  players finish and include the AI. Full-match testing remains outstanding.

## The short build sequence

1. **M2 is complete.** Rules, rewards, prices, events, settings, Night tie-break
   and local autosave/resume are approved.
2. **M3 revised layout is ready for visual review.** The board geometry, haven
   treatment, tile sizing and dual-viewport runtime audit are recorded in
   [m3-revision/README.md](m3-revision/README.md) and its
   [validation record](m3-revision/validation.md). The earlier proof is
   historical validation only; the revised two-rebuild and Console checks pass.
3. **Evolve Core with the CLI alongside it.** Keep the engine/API boundary;
   refactor duck identity, state and phases. Build a complete Day/Night slice,
   then all ten Days, the Normal AI and local autosave/resume.
4. **Connect the Unity game.** Reuse the measured layout and existing UI
   infrastructure, bind the committed Core build, and play a complete match.
5. **Balance and finish.** Inspect recovery, haven/purchase choices, event effects,
   touch/readability, restart/resume and export. Review before changing numbers.

The detailed [implementation plan](IMPLEMENTATION_PLAN.md) splits these into
reviewable checkpoints. M3 revision remains the visual gate and M4 the Core/CLI
build; this deliberately brings the highest visual risk forward. Stop for
feedback at each milestone. M4 starts only on the user's command.

## Art and token philosophy

The board remains a physical illustrated tabletop: cool wetlands, warm meadow
and harsh wasteland, with an incomplete upper-left nest and upper-right oasis.
Dense natural barriers explain the route; keep clear shelter entries and similar
usable widths across all biomes. Bridges meet the route naturally; the dramatic
wasteland crossing remains a timber-and-rope bridge over a cleft.

Layer precise wells, reward strips, rest markers and movable tokens over the
approved base art. The revision must place all 50 spaces in one centered route
following the painted path, with near-even arclength spacing across wetlands,
meadow and wasteland. Do not use a zigzag route or sidebar/stacked wells; keep
the descending and ascending wetland arms approximately even in count. Make
tiles and token faces larger while preserving usable paths and shelter entries.
Seven haven tiles sit on the path next to their painted shelters and never cover
the shelter artwork; space 50 uses the oasis itself and has no separate tile.
Integrate a single-Feather haven treatment and the two-Feather wasteland
treatment into the haven artwork instead of floating Feather decorations. Use
the playful
[V5 painted components](concepts/2026-09-11-v5/painted-kit.png)
as style references, not their rejected coordinates. Each shelter visibly belongs
to one well. Replace the old coin treatment with a Moon/Sleep icon, and use
twigs + score and any Feather yield in readable
reward strips. Omit zero-Twig clutter. Update the footer to explain scoring
**where the duck rests**; remove the old next-empty-space instruction.

Each token category has a consistent silhouette across variants, a strong
face/rim colour and an identifiable illustration. Different categories can
have different shapes but must fit a common well footprint. Keep rounded sturdy
edges, matte cardboard depth and playful original illustrations. Omit default
‘1’ movement badges. Meaningful quantities such as reed bundles x1/x2/x3 are
allowed beside their category symbol, with their effect explained on the shared
rule reference. Keep them visually distinct from explicit movement arrows.
Production instructions, quantities and changing state remain precise overlays.
Colour alone must not be required to recognize a category.

| Identity | Encounter name |
| --- | --- |
| White | Obstacles |
| Orange | Seeds |
| Red | Tailwind |
| Blue | Signpost |
| Yellow | Refreshing splash |
| Green | Nesting reeds |
| Black | Companion duck |
| Purple | Wildflowers |

The user approved the [token-family style](concepts/2026-09-13-token-family/README.md).
The approved [current token set](concepts/2026-09-13-agreed-token-set/README.md) has plain
faces, explicit movement arrows, Reeds quantities and all five white nuisances. These remain
concept sheets, not production sprites or final effect specifications. Player ducks remain
distinct colours/personalities and die-cut silhouettes; the Companion duck is a
small illustrated encounter tile, not another player marker. Four duck identities
do not expand the initial human-versus-AI scope.

Other display terms: Day; Explore; Settle down; Exhaustion; Worn out!; Shelter;
Feather trail; Dream choices; Most Rested Duck; World Event; Dawn Delivery.
Splash provides immediate-next-chip nuisance protection. No separate flask or
rewind is included in v1, as approved.
Production guidance is in [ASSET_BRIEF.md](ASSET_BRIEF.md).

The [Most Rested cloud tile](concepts/2026-09-14-most-rested/README.md) is a new
review concept. A lavender “zzz” marker covers the temporary extra start space
beyond the nest/updated Feather trail. It must read differently from a permanent
Feather or encounter. Tied eligible ducks receive the same gameplay advantage.

## Reuse the engine; replace the changed rules

Keep Quackies.Core, MatchSession's snapshot/legal-action/execute API, deterministic
randomness and ownership of authoritative state. Keep CLI as a rapid client of
the same engine and Normal AI as a consumer of legal actions. Refactor changed
phases, scoring and effects in focused policies instead of restarting or
spreading rule arithmetic into Unity. Preserve the original tested game as a
reference profile, not a mandatory user-facing mode or content template.

Read [ARCHITECTURE.md](../ARCHITECTURE.md) before boundary changes and
[ENGINE_EVOLUTION.md](ENGINE_EVOLUTION.md) for implementation rationale.
Preserve serialized identities until deliberately migrated. Old and new rules
need distinct validation; this planning audit is not a runtime test.

## Milestone status

| Milestone | Work and evidence |
| --- | --- |
| M0 / initial M1 | Historical baseline and art exploration; recorded in PROGRESS |
| **M2 — Complete** | Rules, all data, events, defaults and local save/resume approved; final-Day-only simultaneous drawing and Night Sleep tiebreak recorded |
| **M3 — Ready for visual review** | Revised centered 50-space route, seven adjacent haven wells, larger tiles/tokens, endpoint data and passing 1133 × 744 / 2732 × 2048 runtime audits; two stable rebuilds and zero Console errors recorded in [m3-revision/README.md](m3-revision/README.md) and [validation](m3-revision/validation.md) |
| M4 — Core/CLI (unstarted) | New profile/state, exact private previews, complete Day/Night and ten-Day matches, Normal AI and local save/resume; starts only on the user's command |
| M5 — Connected Unity game | Committed Core handoff, complete human/AI play, all phases, restart and local match restoration |
| M6 — Balance/export | Match evidence, approved tuning, readability/performance and iOS export |

The three-Feather Dawn cap bounds the default ten-Day start to 46. The approved
shared starting setting of 0–3 bounds the latest start to 49. First draw,
empty-bag and overshoot rules are fixed, as are shop limits, Sleep expiry,
recovery exclusion, worn-out payouts and final ranking. There are no remaining
M2 rule decisions. Production polish and actual balance are later milestone work.

The current M3 revision finishes layout at the approved board source size. The
detailed 3072 × 2048 production master is explicitly deferred; retain the
higher-resolution authoring plan and 4096 import cap. UI remains
resolution-independent and production sprites use native assets at their
intended scale. The revised layout passes runtime audits at 1133 × 744 and
2732 × 2048. The board uses seven native-alpha haven wells beside shelters,
larger 90 × 66 wells and 66-unit token faces (previously 74 × 58 and 54), with
havens at 3, 11, 19, 27, 29, 37, 44 and 50; endpoint remains 21 Sleep / 9
Twigs / 2 Feathers. The earlier 1536 × 1024 proof and its captures remain
historical validation only.

Short-match settings, alternate rule cards, test tubes, a separate AI-history
pane and device installation remain deferred. Four player duck identities do
not expand the initial human-versus-AI scope. Keep source identifiers as Quackies;
release title remains undecided.

Follow [AGENTS.md](../../AGENTS.md): root owns Git/integration and the Unity
Editor mutation for this milestone; workers have bounded disjoint files. Push coherent checkpoints
on `codex/duck-game-milestone-0`, preserving unrelated changes. No automatic
merge, physical-device install or release. Editor success, iOS export and device
testing are separate evidence. Historical M3 target-size verification, two
rebuilds, Console check and captures remain in [the old review evidence](m3/validation.md);
the revision needs its own evidence checklist in [m3-revision/README.md](m3-revision/README.md).
