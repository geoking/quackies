# Duck migration plan

## Current scope and source

The user accepted M0 and authorized **M1 only** on 10 September 2026. On 11
September the first M1 style was rejected as a visual direction. The subsequent
duck tiles and three-biome style were approved, and the V3 seed token is now
accepted. V5’s painted style remains preferred, but its painted spaces did not
align convincingly with the illustrated paths. The V9 image-only pass edits the
base board only: preserve V8’s blocked shortcuts and three open meadow entries,
then sweep bridge integration and image quality using a clean high-resolution
render. Bridges should have earth/stone bank abutments, open walk-on approaches,
grounded supports/contact shadows, wet moss on the wetland bridge and drier
stone/wood on the desert bridge. Preserve the cool wetlands, warm grasslands,
desert, eight shelters, nest, oasis and broad routes.
The selected candidate measures 1536 × 1024; the requested 3072 × 2048 master
remains unresolved after two built-in attempts. Enlargement alone would not add
the requested detail. Do not add tiles, text or a legend. No Unity import,
source/settings change or M2 is authorized.

The earlier V8 image-only pass edited the
base board to distinguish the middle grasslands from wetlands with cooler
teal/sage damp banks and reeds, versus warm sunny open grasslands with a broad
short-grass route, taller soft grasses, meadow flowers and leafy deciduous cover
around the three existing shelters. V8 added dense tall meadow grasses and
leafy shrubs to block apparent shortcuts while keeping the main U route and
shelter entry pockets open, plus a reed/willow thicket blocking the nest-to-bridge
shortcut while keeping the nest’s downward route and returning lane open.
Preserve the desert composition,
far-upper-right oasis, nest, eight shelters and bridges, the upper middle-meadow
shelter’s left access, and broad corridors for future token spaces. Do not add
tiles, tokens, numbers, reward text or a legend. Preserve the full rules
baseline. Pause for image review; Unity positioning was discussed only and no
Unity import, source/settings changes or M2 are authorized by this pass.

The user explicitly approved precise typesetting over generated artwork after
the first labelled image skipped positions and corrupted reward values. For a
later authorized tile-overlay pass, use the accepted revised background,
reuse the painted stone/leaf/icon components in the preferred earlier
style, compose the 53 wells from those components, then typeset reward rows and
the full illustrated legend from verified track data. The early labelled-board
reference is for style, not numerical data. Position indices remain internal and
are not printed. V9 is only the background edit described above; no overlay is
being composed in this pass. Static design rendering is outside Unity and does
not alter the rules engine.
The current branch is
`codex/duck-game-milestone-0`, created from merged playable commit
`74e40cfe04995d813428c3ff8390461235928a44` on 10 September 2026.

This repository plan reconciles the accessible messages in **Design Tabletop
Unity** through the agreed Revision 4 decisions. The linked downloadable plan
was not exposed by the conversation reader; the M0 checklist, milestone table
and subsequent agreed amendments were available in the messages themselves.
Conversation: `6aa27592-b550-83ed-85a2-f231b9ca2d3c`.

The current user request and working agreement authorize regular commits and
pushes, superseding the earlier example prompt's instruction not to push.
Do not merge, publish a release, or start the next milestone without feedback.

## Agreed game identity

Explore the wetlands, discover helpful encounters and build the biggest,
cosiest nest in the pond. Settle somewhere comfortable, or push your luck and
end the day worn out.

- Goofy, flat cartoon tabletop art: ducks should read as tangible cardboard
  player tiles or tokens, with oversized bills and eyes and original artwork.
  Player ducks differ in colour and styling. The approved V2 sheet explores four
  identities; it does not change the current human-versus-AI player count.
  Encounter tokens also feel physical, with one consistent silhouette per
  category and unmistakable category colour. The accepted V3 seeds use the rounded
  triangle with a solid orange face and thick darker orange border.
- The board is illustrated artwork with one continuous, readable winding path
  across three connected biome loops. Bridges make the region transitions clear;
  there are no unintended branches or shortcuts. Irregular illustrated spaces
  and inviting rest places replace a rigid repeated-circle treatment. Every
  space has an obvious token placement well and an attached reward
  strip that remains readable when a tile covers the well. Decorative paths and
  rocks must not look like extra playable spaces.
- The approved biome art direction is a pleasant
  pond/grassland beginning, a lush comfortable middle with many inviting rests,
  and a barren unpleasant final region with rare exceptionally cosy havens.
  This replaces the quiet-background-only art brief: the path and rest places
  belong in the illustration, while precise positions remain data in software.
- The board starts at an incomplete twig nest with an empty bowl for the duck
  token. Do not print a start number or start label. The duck is the permanent
  starting marker. It stays there while encounters
  are drawn and placed ahead to build today's route.
- Lily-pad shortcuts provide the existing temporary catch-up assistance. Show
  a crossing from the permanent start to the effective start, with a landing pad.
  Preserve the existing score-marker calculation, not one space per point behind.
- After drawing is finished, animate the duck along the crossing and completed
  route. A safe duck settles happily; a worn-out duck flops down. The outcome
  comes from Core; the animation introduces no additional rule phase or reward.
- Keep the result pose during review. Next day resets to the current permanent
  starting marker, retaining legitimate trail upgrades and recalculating the
  temporary crossing. Do not turn tonight's resting spot into tomorrow's start.
- The scored nest persists across the match and shows accumulated Twigs. Today's
  resting spot is separate. Reaching the far end does not automatically win.

## Shared terminology

| Existing concept | Agreed display term | Invariant |
| --- | --- | --- |
| Victory points | Twigs | Persistent score, never spendable |
| Buying points / coins | Pond pennies | Today's buying allowance; existing expiry and final conversion |
| Rubies | Feathers | Existing costs, recovery, upgrades and final conversion |
| Round | Day | Nine-day structure |
| Token / chip | Encounter | Existing strengths and effects |
| Draw | Explore | Same legal draw action |
| Stop | Settle down | Same voluntary stopping action |
| White total | Exhaustion | Same threshold and exceptions |
| Exploded | Worn out! | Same reward restrictions |
| Ruby scoring space | Shelter | Existing reward initially; preview uses the scoring space |
| Cauldron track | Wetland trail | Same indexed positions and rewards |
| Permanent droplet improvement | Trail upgrade | Permanent starting-position change |
| Rat-tail assistance | Lily-pad shortcut | Temporary catch-up, recalculated each day |
| Rat marker | Landing pad | Effective encounter starting point |
| Shopping phase | Prepare for tomorrow | Same phase and purchase restrictions |
| Shop | Pond Market | Same finite supply and prices |
| Flask | Water flask | Same legal recovery effect |
| Bonus die | Lucky find | Same resolved die outcomes |
| Fortune cards | Pond happenings | Same active 24-card deck initially |

| Encounter identity | Display term |
| --- | --- |
| White | Obstacles |
| Orange | Seeds |
| Red | Tailwind |
| Blue | Signpost |
| Yellow | Refreshing splash |
| Green | Nesting reeds |
| Black | Companion duck |
| Purple | Wildflowers |

Write original explanations matching the active Set 1 effects. Theme changes
must not alter starting bags, strengths, effects, prices, supply, thresholds,
scoring, fortune timing, resource lifetimes or Normal AI decisions. Final-day
Pond pennies still convert by floor(amount / 5), with the existing worn-out
either/or restriction. Feathers retain their separate final conversion.

## Repository-specific sequence and review gates

| Milestone | Bounded work | Evidence and pause |
| --- | --- | --- |
| M0 — Baseline | Inspect actual API, CLI, Unity, rules, AI and persistence; preserve dirty work; branch; run existing checks; record tools, risks, plan and art brief | Repository map and current evidence; ask for reaction |
| M1 — Art redirection | Preserve approved tokens/biome style; refine board nest, unnumbered wells, icon rewards and eight visibly linked shelters | Image review only; inspect count/value legibility and ask how the result feels before Unity work |
| M2 — Terminology | Add the smallest shared display contract in `src/Quackies.Core`; adapt `src/Quackies.Cli` and expose the same definitions to Unity | Duck CLI, preserved classic identities, same numerical results and legal actions for identical seeds/actions; ask about wording |
| M3 — Playable duck table | Connect a separate duck presentation to `MatchSession`; add essential shelter, feather, nest and catch-up visuals | Complete game with Explore, Settle down, exhaustion, rewards, market, next-day reset and retained nest score; ask for a playtest reaction |
| M4 — Essential art | Complete encounter icons and original help text; finish modest journey/result animations and UI | Readability at actual sizes, safe/worn-out endings, consistent assets and accurate previews; ask about feel |
| M5 — Shelter/biome reward experiment | Only after an explicit rules specification: compare shelter density and proposed biome reward profiles as selectable experiments | Preserve the old shelter comparison and unchanged baseline; verify safe/exhausted stop, passing and ordinary spaces; review balance |
| M6 — Migration review | Review clients, AI, compatibility, assets, tests and remaining issues | Tested branch ready for the user's merge decision; no automatic merge |

The original M1 scene and builder remain superseded visual-prototype evidence;
see [STATUS.md](STATUS.md). Keep `QuackiesInitialScene.unity`, its builder and
source art usable throughout. Do not add new concepts to Unity or alter the
shipping build settings at this gate.

## Implementation boundaries

The current engine is `src/Quackies.Core/Match/MatchSession.cs`; Unity binds the
compiled Core DLL from `Assets/Plugins`. `MatchPresenter` handles session/input
adaptation; `Presentation` views render it. `InitialSceneBuilder` and
`QuackiesArtImporter` live under `Assets/Editor`. Build on these boundaries.

M2 should add a small immutable display vocabulary/formatter at a shared Core
boundary, not a new service or a generic theme engine. Inventory text in action
labels, observations, history and fortune explanations before choosing the exact
contract. Preserve action IDs, enums, fields, assembly names and serialized
bindings. Do not derive rules from translated text or duplicate token-name tables
in CLI and Unity. The detailed baseline audit records the existing coupling.

For any later board implementation, retain the standard 54 indexed spaces
(0–53), with last encounter position 52 and scoring space 53. The current
next-scoring-space semantics remain the baseline until rules are approved.
Illustrated artwork should sit above invisible indexed anchors and hitboxes; the
image concept is not proof of an exact space count or mapping. Drive the resting
preview from Core's scoring space after the last encounter; never use the
encounter's own position as its reward index.

Separate presentation identity from the optional M5 rule profile. Preserve the
current API and tests while introducing narrowly tested display changes. Keep
legacy prototype callers working; they are not the migration's gameplay engine.

## Agent and checkpoint workflow

- Lead: architecture, scope, Git, integration review and milestone feedback.
- Sol/high: bounded Core/API/CLI work and meaningful rule/compatibility tests.
- Terra/high: bounded Unity presentation/editor work; sole Editor mutation owner.
- Luna/medium: fully specified asset manifests, documentation and data checks.
- Use one worker normally and a second only for independent work. Workers get
  short briefs and disjoint ownership, no full history or further delegation.
- Keep `.codex` agent settings; do not change global settings. Art creation and
  import can be separate tasks but only one agent controls the Editor.
- Lead commits source, its tests, compiling Unity changes and documentation as
  coherent checkpoints, pushing each. Preserve unrelated local changes.
- Run focused checks during implementation and full relevant checks at gates.
  Record commands and results so interruptions do not trigger repeated discovery.
- Pause at each milestone and ask how the user feels before starting the next.

## Open choices and proposed improvements

The release title is undecided. Keep repository, namespaces, assembly and bundle
identifiers as Quackies. Shelter density and 2-feather safe / 1-feather exhausted
rewards are unapproved experiment candidates, not requirements.

Retain the approved four-player duck identity sheet and V3 seed tile. Review
the V9 base board for biome distinction, route readability and shelter
connections; it has no playable tiles or labels. Preserve the V5 three-biome
style as visual context, but do not treat the V9 image as a mapping of the 54
indexed source positions.
Static image checks do not establish Unity behaviour.

Keep Twigs beside a nest, label pennies as **Spend today**, and show the resting
preview separately from the nest score. These are rules-neutral clarity choices.
The latest request explicitly retains the current Quacks Pond penny/Twig table,
superseding the earlier biome-dependent reward curve for this design. That
earlier idea remains a future experiment only. V5 retained eight clearly assigned
resting spaces, proposed at physical indices 5,13,20,28,34,40,46,52: a subset of
the original ruby positions, distributed 2/3/3 across the illustrated regions.
The count is confirmed; their illustrated placement awaits review. Each rest
frames its own token well and has an attached feather marker; scenery nearby
must not imply a separate resting space. No extra Penny/Twig bonuses are added.

The rules baseline retains internal start 0 and spaces 1–53; this V9 base image
has no visible index badges.
Each reward row uses a gold coin icon and amount, then a twig icon and amount.
For zero Twigs, omit both twig icon and zero and center the coin pair. Use the
same usable pad/reward geometry with modest painted contour variations; no
separate SCORE flag at 53. Use the earlier full lower-border key: empty stone
and TILE HERE, gold coin and Pond pennies, twig bundle and Twigs, leafy white
feather and REST, then dashed arrow and Score the next empty space. Keep its
icons large and lettering playful, separated by slim vertical rules. Each of the
eight rest pads has an adjacent illustrated shelter and a short clear entry spur.
The barren-region shelter at 40 is visibly modest compared with the cave at 46.
Ordinary encounter placement ends at 52; space 53 is the existing final scoring
space. Rewards and the resting
spot are read from the next empty scoring space. The exact source table and
original ruby flags are in [the V3 data audit](concepts/2026-09-11-v3/track-data.md).
Core still has all 15 original ruby positions: the image-only selection does
not itself implement the eight-rest rule profile. No new reward amount, timing,
AI or balance code is part of this review.
