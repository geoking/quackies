# Duck art production brief

Updated 14 September 2026. The selected board remains the canonical scenery
reference. The user approved the current 16 encounter designs, including numberless
faces and obstacle variants. They remain concept sheets, not production sprites. Rule values,
placement geometry and interaction copy belong in [PLAN.md](PLAN.md); this
brief records the asset direction and the accepted M3 presentation boundary.

## Approved references

- [Selected board](concepts/2026-09-12-approved/board-art-approved.png) —
  unchanged three-biome composition and source reference. The accepted M3 board
  uses the stable 1536 × 1024 `Assets/Art/DuckLayout/board.png` presentation;
  a larger painted master is explicitly deferred.
- [V2 player ducks](concepts/2026-09-11/duck-player-tiles-v2.png) — approved
  player identities and silhouette language.
- [V3 Seed](concepts/2026-09-11-v3/seed-tile-v3.png) — approved rounded-triangle
  Seed shape, orange face and darker rim; this is the style reference for the
  new family, not a reason to change Seed rules.
- [V5 painted components](concepts/2026-09-11-v5/painted-kit.png) — approved
  visual language for wells, shelter frames and icons. Its old coordinates are
  not authoritative.
- [Dream Concept B](concepts/2026-09-13-dream-study/concept-b-nest-mat.png) —
  accepted full-screen Dream/nest presentation direction, with View adventure
  navigation and room for every legal offer.

## New token family

The seven new category designs are documented in
[concepts/2026-09-13-token-family/README.md](concepts/2026-09-13-token-family/README.md)
alongside the approved Seed reference. The categories are Obstacles, Tailwind,
Signpost, Refreshing splash, Nesting reeds, Companion duck and Wildflowers.
Each category needs one stable silhouette across variants, a strong category
colour plus an identifying illustration, and a shared footprint and edge
treatment that fits every legal well. The user approved this visual family.

The [revised study](concepts/2026-09-13-obstacle-study/README.md) removes generic
‘1’ badges. Default movement is one, unprinted; Tailwind uses explicit
total-movement arrows →2/→4/→6. The subsequent [quantity discussion](ENCOUNTER_RULES.md)
allows meaningful counts such as reed bundles x1/x2/x3, visually separate from
movement. A quantity represents contents, not copies, placements or triggers.
If future rule cards may change its reward, avoid baking a specific Twig/Sleep
payout into that quantity icon. Conditional movement belongs in clear rule text,
not a misleading fixed arrow. Keep instructions, quantities, changing state
and selection highlights as precise production overlays. White obstacles share
their category shape/rim and differ through clear nuisance illustrations; use
a short board-edge key and active-effect indicator rather than face paragraphs.
The Goose needs a visible current-Exhaustion/current-maximum display; the older
concept-sheet footer “Five is safe” is not an always-valid runtime instruction.
Plain Seeds need no quantity/movement badge or power text. The accepted Companion
flock uses one unnumbered design and a public active-flock/next-movement display;
2/3/4 is a Day-state progression, not three printed denominations. Its older
shield treatment is superseded. The fixed-movement Signpost uses
an explicit →2, separate from any future preview-quantity symbol.
Artwork must leave room for readable exceptional values at actual iPad mini board and
opponent-inspection sizes. Player duck pieces must remain distinct from
Companion duck encounters.

## Remaining asset work

The [current token set](concepts/2026-09-13-agreed-token-set/README.md) contains
four approved concept sheets covering all 16 encounter designs/variants: five everyday
tokens, three Tailwinds, three Reeds quantities and five whites. Signpost →2,
Reeds quantities, unnumbered Companion and Day 5 Goose labels match the accepted
rules. Native dimensions, hashes, prompts and visual findings are saved there.

M3 accepts the 43-space board presentation with 15 scattered-Twig tile-art
variants. Twigs are floor decoration and may be covered by a chip; the exact
live Twig count stays at right-middle, while the exact live Sleep count sits
beside the bottom moon. Larger bright Feathers occupy the lower-left haven
pocket; the oasis uses two slightly smaller Feathers in front of the pool.
All chips and the resting duck use the shared 64-pixel frame at offset
`(16, -11)` within a tile. Fredoka SemiBold with black outlines is used
throughout, with a stronger dedicated outline and fuller face for the 86 board
reward labels; Twig numerals are 22 units.

Production polish deferred to M5/M6 includes sprites and resource icons for
Twigs, Sleep and Feathers, the scored nest and shared nest-level presentation,
shelter markers, endpoint and wasteland details, the Dawn Delivery stork,
original World Event art, and the final Dream view. The Dream view must show
the full legal catalogue without implying extra purchases or altered rules.
Twigs remain persistent nest score; Feathers are automatic permanent trail
advances; Sleep earned and Sleep remaining need separate visual treatments.

The new [Most Rested marker](concepts/2026-09-14-most-rested/README.md) is a
lavender cloud with a dark blue rim, crescent and “zzz”. It is for review and
covers the temporary start +1 beyond the updated Feather trail. Use duplicate
displays for tied beneficiaries. Distinguish it from permanent trail resources;
Night 10 changes the award to a Dream Twig and does not indicate another start.

The accepted M3 presentation is already represented in the stable
`Assets/Art/DuckLayout/` board, layout data, tile-art data, crops and scene
builder. Preserve those existing assets, editable source, stable `.meta`
identities and source/prompt metadata. Do not replace the accepted 43-space
layout with the historical 50-space proof or require a larger board master
before production work can continue.

Preserve approved native images and Assets/Art/raw. Use original illustrations;
reference game assets inform rules and component affordances, not copied art.
Concept B's old Feather balance/exchange treatment must become a display of
Feathers laid and permanent trail progress, with no spending controls.

Possible later import roots, relative to `unity/Quackies.Unity`:

- `Assets/Art/DuckTheme/Backgrounds/`
- `Assets/Art/DuckTheme/Characters/`
- `Assets/Art/DuckTheme/Encounters/`
