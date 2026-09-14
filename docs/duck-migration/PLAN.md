# Quackies overall plan

Updated 14 September 2026. This is the current duck-game direction. It
supersedes the presentation-only Quacks migration; historical decisions and
validation remain in [PROGRESS.md](../PROGRESS.md). The latest user-requested
mechanics, all 50 board rewards and the 11 shop prices are accepted starting
values. Dawn gifts now have the user-selected maximum of three. Revised World
Events and explicitly labelled interpretations/policies remain **proposals for review**.

## What we are building

A ten-Day tabletop game for iPad mini landscape, initially one human versus
Normal AI. Ducks adventure through wetlands, meadow and wasteland by Day, then
enter a full-screen Dream/nest view at Night. Each has its own bag and progress;
World Events affect everyone. Most persistent Twigs, including final Dream
Twigs, wins. Keep starting-progress settings, rival-board inspection and restart.

Use the selected [board](concepts/2026-09-12-approved/board-art-approved.png),
[V2 player ducks](concepts/2026-09-11/duck-player-tiles-v2.png) and approved
[16 encounter designs](concepts/2026-09-13-agreed-token-set/README.md).
[Dream Concept B](concepts/2026-09-13-dream-study/concept-b-nest-mat.png) is the
chosen full-screen layout with **View adventure** navigation and room for the
whole shop. Its illustrative prices and Feather-spending controls are obsolete.

This checkpoint supplies rules/data and a Most Rested marker concept. It does
not change Core/CLI, modify the scene or import into Unity. Implementation waits
for the next explicit milestone command.

## Current rule references

Keep a single source for each kind of detail instead of duplicating long lists:

| Reference | Authority |
| --- | --- |
| [Rules at a glance](RULES_AT_A_GLANCE.md) | Current game loop, accepted changes and clearly marked interpretations |
| [Encounter timing](ENCOUNTER_RULES.md) | All helpful/white powers, protection and movement ordering |
| [Board and shop](v1/BOARD_AND_SHOP.md) | Every one of 50 rewards, 11 prices, Night examples, proposed policies and balance limits |
| [World Events](v1/WORLD_EVENTS.md) | Ten proposed cards, once-per-game shuffled deck, triggers and ordering |
| [Board data](v1/board.json), [CSV](v1/board.csv), [shop data](v1/shop.json) | Approved numeric values; not yet loaded by the game |
| [Bounded audit](v1/balance-audit.json) | Exact bag/counter arithmetic with explicit assumptions, not full-game balance |

## Design rules that guide implementation

- **Rest where the duck lands.** Fifty scorable spaces follow the separate,
  incomplete nest. No numbered-zero graphic or next-empty-space scoring.
  The approved table divides them 1–16 / 17–33 / 34–50, with havens at
  7, 13, 21, 27, 32, 38, 44 and 50. Haven names/indices need later art fit.
- **Comfort is not monotonic distance.** A haven improves Sleep over nearby
  spaces while keeping their Twigs. Exposed wasteland is less restful than late
  meadow, but its havens are especially comfortable. Endpoint printed values
  are just highest: 21 Sleep/9 Twigs/2 Feathers. All three wasteland havens now
  award two Feathers, superseding the old three-Feather endpoint.
- **Separate nightly comfort from victory.** Twigs persist. Sleep earned freezes
  for Most Rested; purchases reduce only remaining Sleep. Worn-out ducks keep
  earned Twigs and half Sleep rounded down. Safe-only reward interpretation is
  documented, not hidden inside the payout formula.
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
  reward. Feathers have no final conversion. Highest final Twigs wins.
- **Nest capacity follows the calendar.** Both ducks have level 1 on Days 1–3,
  level 2 on 4–6 and level 3 on 7–10, allowing 1/2/3 purchases on purchasing
  Nights. Individual decorative nest growth shows score; score cannot unlock
  exclusive purchase capacity for the leader. Use a readable exact Twig total.
- **Original, readable encounters.** Seven helpful token types and five white
  designs retain the approved shapes. Default movement 1 is unprinted; arrows
  state total movement; Reeds quantities state bundle count/Twig yield.
  Starting bag remains eight whites plus five helpful chips. Goose joins once
  on Day 5. Mud now reduces the active flock, including later movement and its
  Night comparison under the documented interpretation. Splash protects just
  the next chip's nuisance, not its Exhaustion, and supplies no rescue.
- **One shared world.** Shuffle the ten event cards once and reveal one per
  Day without replacement. Their effects expire that Day. Future deck expansion
  can add cards; the revised ten-card proposal mixes four helpful events, three
  collective goals and three mild setbacks. Collective rewards wait until all
  players finish and include the AI. The cards remain subject to review/testing.

## The short build sequence

1. **Review the revised events and remaining policies.** Board rewards/prices
   are accepted. Resolve start-setting/route-end rules and housekeeping choices.
2. **Upgrade Core and CLI.** Reuse the engine. Implement direct resting, new
   rewards, encounters, events, Dream purchases and ending in small tested pieces.
3. **Build the Unity table.** Fit all 50 spaces to the approved scenery, with
   clear haven links, readable tokens and a spacious Dream view.
4. **Play complete matches.** Connect the human and Normal AI through all ten
   Days, including dreams, deliveries, final scoring and restart.
5. **Balance and finish.** Check recovery, haven choices, purchase diversity,
   leader effects and endpoint frequency; polish iPad readability and verify export.

Stop for feedback at each milestone; these steps are not permission to begin
the next milestone automatically.

## Art and token philosophy

The board remains a physical illustrated tabletop: cool wetlands, warm meadow
and harsh wasteland, with an incomplete upper-left nest and upper-right oasis.
Dense natural barriers explain the route; keep clear shelter entries and similar
usable widths across all biomes. Bridges meet the route naturally; the dramatic
wasteland crossing remains a timber-and-rope bridge over a cleft.

Layer precise wells, reward strips, rest markers and movable tokens over the
approved base art. Use the playful [V5 painted components](concepts/2026-09-11-v5/painted-kit.png)
as style references, not their rejected coordinates. Each shelter visibly belongs
to one well. Use moon + Sleep, twigs + score, and any Feather yield in readable
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
rewind is proposed for v1; this housekeeping choice still needs review.
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

## Milestones and open boundaries

| Milestone | Work and evidence |
| --- | --- |
| M0 / initial M1 | Historical baseline and art exploration; recorded in PROGRESS |
| Current M2 proposal checkpoint | Approved 50-row table/prices, capped Dawn gifts, revised 10 event proposals and zzz concept; bounded math only |
| M2 completion | Review events/interpretations and resolve route end, starting settings, shop and housekeeping policies |
| M3 — Core/CLI | Focused rules/tests, original regressions and complete deterministic text matches |
| M4 — Unity layout | Actual-size 50-space fit, shelters, tokens, nest, full shop and touch/readability checks |
| M5 — Full game | Ten-Day human/AI loop, all choices, restart, scene reconstruction and Console checks |
| M6 — Balance/export | Recorded complete-match comparisons, visual polish and iOS export evidence |

With the three-Feather Dawn cap and **zero starting Feathers**, the ten-Day
start remains below the endpoint: at most nine prior haven rewards ×2 plus nine
Dawn gifts ×3 is 45 permanent steps; Most Rested adds at most one temporary step,
for a conservative maximum start of **46**. This overestimates early haven gains,
so it is a bound, not a normal-match forecast. It resolves the default-start
saturation concern under the current Feather sources and ten-Day length.

Starting-Feather settings add directly to that bound and need an explicit valid
range. Overshooting 50 during a draw, no-draw/empty-bag endings, no-rewind/no-flask,
final victory ties, shop stock/one-per-type rules and Sleep expiry remain review
items. Do not silently discard, bank, convert or reduce awarded Feathers. The
Dawn source cap is now explicitly authorized; no other source changes follow.
Safe-only haven/Flower/flock rewards and final-chip penalties when worn out are
still marked integration interpretations. No implementation starts here.

The selected board is native 1536 × 1024. Its requested detailed 3072 × 2048
master, exact 50-space alignment, token clearance and iPad fit remain outstanding.
Do not transfer a width audit of a later alternative to this selected image.

Short-match settings, alternate rule cards, test tubes, a separate AI-history
pane and device installation remain deferred. Four player duck identities do
not expand the initial human-versus-AI scope. Keep source identifiers as Quackies;
release title remains undecided.

Follow [AGENTS.md](../../AGENTS.md): root owns Git/integration, workers have
bounded disjoint files, one agent owns Editor mutations. Push coherent checkpoints
on `codex/duck-game-milestone-0`, preserving unrelated changes. No automatic
merge, physical-device install or release. Editor success, iOS export and device
testing are separate evidence. Stop after this planning checkpoint for review.
