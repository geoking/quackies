# Duck migration plan

Updated 12 September 2026. This is the current decision record. Iteration history
and validation live in [STATUS.md](STATUS.md), [PROGRESS.md](../PROGRESS.md)
and the individual concept folders.

## Current decision and stopping point

The user approved **exec-9d44cb08-9cb9-4367-8a00-4a5f8c60b78b** as the base board.
Its exact native image is [saved here](concepts/2026-09-12-approved/board-art-approved.png),
with [selection and provenance](concepts/2026-09-12-approved/README.md).
This is the first V10 candidate, selected ahead of the later clearance revisions.
The approved duck tiles and seed remain the token references.

This checkpoint consolidates the plan only. **Stop after saving and publishing
it; wait for the user's command before Unity work, further art generation or
the next milestone.** Art approval does not establish exact space fit or a
playable duck scene.

Work remains on `codex/duck-game-milestone-0`, based on the completed playable
baseline `74e40cfe04995d813428c3ff8390461235928a44`. Keep its scene, builder
and source art usable. Regular commits and pushes are authorized; main merge
and release require separate user decisions.

## Game identity and scope

Quackies is a playful tabletop game about ducks exploring a shared world and
building the biggest, cosiest nest. Explore for helpful encounters, settle
down, or push your luck and end the day worn out. Accumulated Twigs determine
the result; reaching the end of the trail does not automatically win.

Preserve the complete base game, ingredient Book Set 1, one human versus Normal
AI, nine Days and all 24 active fortune effects. Keep legal choices, exhaustion,
flask recovery, catch-up, evaluation, finite market, upgrades, unlocks, late-day
rules, final conversions and ties. Include Normal AI and starting-Feather
settings; target iPad mini landscape. Test tubes and separate AI-history UI
remain deferred. See [the full baseline](../IMPLEMENTATION_GOAL.md) and
[its evidence](BASELINE.md).

All ducks inhabit the same world while retaining their own bag, trail state,
resources and nest score. Theme changes preserve strengths, starting bags,
prices, supply, thresholds, effect timing, legal actions and Normal AI decisions.
Use original explanations for existing effects. New balance rules require an
explicit agreed specification.

### Most Rested Duck reward

Rename the bonus dice roll **Most Rested Duck reward**, framed as a small
end-of-day bonus. Initially retain the current rule: the duck furthest along
the physical scoring track among those who are not worn out qualifies; every
tied leader rolls. This happens as evaluation begins, before encounter
evaluation and normal scoring. A World Event can retain its existing
modification to the number of rolls.

Keep the six existing die faces: 1 Twig, 1 Twig, 2 Twigs, 1 Feather, a
strength-1 Seed, or one Trail upgrade, including supply and maximum-position
restrictions. The theme does not add a rest-quality statistic or use illustrated
shelter quality as a tie-breaker. A smaller or different bonus is a future
balance decision, not part of this rename.

### World Events

Rename fortune cards **World Events**. Reveal one shared situation at the start
of each Day, before exploration, for all ducks in the same world. Explain its
effect and duration clearly; consequences and choices can differ by player
under the existing conditions.

Retain the 24-event deck and draw without replacement. Resolve reveal-time
effects and choices before exploration; preserve event choices and effects
that occur later in the Day. Shared fiction does not change timing or create new
shared-board rules. During terminology/art work, give each existing rule
identity an original event name, explanation and presentation.

## Board and token design philosophy

Use an illustrated physical board with tangible cardboard pieces: goofy,
colourful cartoon ducks, bold silhouettes, expressive faces and softly painted
scenery. Functional pieces must be readable at actual iPad mini sizes.

| Reference | Agreed use |
| --- | --- |
| [Approved base board](concepts/2026-09-12-approved/board-art-approved.png) | Canonical composition, without tiles, numbers, rewards or legend |
| [V2 player ducks](concepts/2026-09-11/duck-player-tiles-v2.png) | Approved physical duck tiles with distinct colours and personalities |
| [V3 seed](concepts/2026-09-11-v3/seed-tile-v3.png) | One rounded-triangle shape, solid orange face and thick darker orange rim |
| [V5 painted components](concepts/2026-09-11-v5/painted-kit.png) | Bubbly wells, leafy rest frames and icons; style reference only, not the rejected placement coordinates |

**Board composition.** One continuous route crosses three connected loops:
cool teal wetlands, warm sunny grasslands and harsh sandy wasteland. Keep the
incomplete upper-left nest, far-upper-right oasis and eight shelters. Use
wetland banks, short meadow grass and desert sand for biome-specific routes.
Dense reeds, grasses, shrubs and trees explain blocked shortcuts while leaving
comparable usable token clearance throughout. Preserve clear shelter entries,
including left access to the upper meadow shelter. Bridges meet their paths
naturally; the wasteland crossing is a dramatic timber-and-rope bridge over a
rocky cleft.

**Layered tabletop construction.** Keep scenery in the base image. Add separate
painted placement wells, reward rows, shelter markers and movable tokens over
it. Later Unity work must explicitly place and inspect every indexed space on
the illustrated route. Numerical correctness alone does not establish good
alignment. Bridges may be connectors without tile spaces on their decks.

**Future tokens.** Use one consistent silhouette per encounter category across
all its strengths. Categories may differ in shape where useful, but share a
common footprint and edge treatment so every piece fits the same wells. Give
each a strong category colour through its face and/or thick rim, plus a
recognizable illustration; identification must not rely on colour alone.
Keep Seeds on their accepted single shape. Player ducks vary by colour and
personality; the four-design sheet does not expand the two-player scope.

Keep token faces uncluttered and recognizably flat printed pieces. Strength
values, changing state and selection highlights are precise overlays, not
generated text. Reserve space for them and check human and opponent views.
Distinguish player duck tiles from Companion duck encounters. Use original
art; the reference game informs rules and physical-board affordances, not
copied illustrations. Production/provenance guidance lives in
[ASSET_BRIEF.md](ASSET_BRIEF.md).

## Spaces, rewards and shelters

Keep 54 logical positions: internal start 0 and spaces 1–53. Encounters end at
52; 53 is the final scoring space. Score **the next empty space**, not the last
encounter. Preserve the exact Pond penny/Twig table in
[the track audit](concepts/2026-09-11-v3/track-data.md); biome appearance does not
change its values.

- The incomplete nest is the duck's starting well, without a number or start
  label. Other wells must obviously accept tokens. Keep indices in data only;
  no top numbers or separate SCORE flag at 53.
- Attach a readable bottom reward row to each well: gold coin icon + amount,
  then twig icon + amount. For zero Twigs, omit both the icon and zero and
  centre the coin pair. Use exact typesetting, consistent usable geometry
  and modest painted contour variation. Recheck spaces 11, 44 and 53.
- The eight scenic rests are distributed 2/3/3 by biome. Proposed overlay
  indices are 5, 13, 20, 28, 34, 40, 46 and 52. Each frames one well with a
  leafy Feather marker and connects visibly to its own adjacent shelter.
  Keep 40 a modest refuge and 46 a cosier cave. Decorative scenery must not
  imply extra playable spaces or additional Penny/Twig rewards.
- Restore the playful footer with large painted icons and slim separators:
  empty well + **TILE HERE**; coin + **Pond pennies**; twigs + **Twigs**;
  leafy Feather + **REST**; dashed arrow + **Score the next empty space**.
  Keep lettering separate from the base art.
- Show accumulated Twigs beside the scored nest, Pond pennies as **Spend
  today**, and today's resting preview separately.

Eight scenic rests are an approved visual choice; Core still awards Feathers
at all 15 original ruby spaces. Before playable integration, agree how to
represent every baseline reward without silently removing seven or making
the display disagree with Core. An eight-reward rules profile belongs to the
optional experiment milestone.

## Duck journey and daily reset

The duck marks its permanent start while encounters are drawn ahead. Show
existing catch-up assistance as a temporary lily-pad crossing to the effective
start and a landing pad. Preserve the score-marker calculation, not one space
per point behind.

After exploration, animate the duck along the crossing and completed route.
A safe duck settles happily; a worn-out duck flops down. Core determines the
outcome; animation adds no rule phase or reward. Hold the result pose during
review. Next Day resets to the legitimate permanent start, preserves Trail
upgrades and recalculates catch-up. Tonight's rest does not become tomorrow's
start. The scored nest persists across the match.

## Display vocabulary

Display terms change; underlying rule and serialization identities stay stable.

| Existing concept | Display term |
| --- | --- |
| Victory points | Twigs |
| Buying points / coins | Pond pennies |
| Rubies | Feathers |
| Round | Day |
| Token / chip | Encounter |
| Draw / Stop | Explore / Settle down |
| White total / Exploded | Exhaustion / Worn out! |
| Ruby scoring space | Shelter |
| Cauldron track | Wetland trail |
| Permanent droplet improvement | Trail upgrade |
| Rat-tail assistance / rat marker | Lily-pad shortcut / Landing pad |
| Shopping phase / shop | Prepare for tomorrow / Pond Market |
| Flask | Water flask |
| Bonus die | Most Rested Duck reward |
| Fortune cards | World Events |

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

Twigs cannot be spent. Pond pennies keep daily expiry and final-day
floor(amount / 5) conversion, including the worn-out either/or restriction.
Feathers retain their separate final conversion.

## Milestones and gates

| Milestone | Bounded work | Evidence and pause |
| --- | --- | --- |
| M0 — Baseline | Completed repository/tool audit and preserved playable game | Accepted; build, 129 tests and CLI smoke recorded |
| M1 — Art direction | Approved ducks, seed and selected base board; consolidated plan | Art selection complete; overlay/resolution remain open; stop here |
| M2 — Terminology | Small shared display contract, CLI/Unity wording, Most Rested Duck reward and World Events | Same legal actions and numerical results for identical seeds/actions; review wording |
| M3 — Playable duck table | Separate presentation bound to MatchSession, selected board, exact overlays, nest/catch-up visuals and full game flow | Verify all positions/rewards, iPad readability, interaction, daily reset and complete match; review playtest |
| M4 — Essential art | Remaining encounters/resources, original World Event presentation/help and modest journey/result animations | Actual-size readability, consistent assets and accurate previews; review feel |
| M5 — Optional rules experiment | Only after rules approval: compare shelter density and biome reward profiles | Preserve unchanged baseline; test stop, passing and exhaustion cases; review balance |
| M6 — Migration review | Review clients, AI, compatibility, assets, tests and remaining issues | Tested branch ready for user's merge decision |

The roadmap does not authorize starting its next row. Get the user's reaction
at each gate. The earlier V1 Unity style-test scene remains historical evidence,
not the accepted duck presentation.

## Implementation and validation

Read [ARCHITECTURE.md](../ARCHITECTURE.md) before changing boundaries.
`src/Quackies.Core/Match/MatchSession.cs` remains the gameplay engine,
independent of Unity. Unity uses the compiled Core DLL; `MatchPresenter`
adapts session/input, presentation views render it, and builders/importers
remain editor-only.

For M2, inventory action, observation, history and event text before adding a
small immutable shared vocabulary/formatter. Preserve action IDs, enums,
fields, assemblies and serialized bindings. Avoid a generic theme engine,
duplicated client name tables or rules derived from text; keep legacy callers
compatible. Bind board anchors, hitboxes and reward labels to authoritative
indexed data, not inferred pixels. Separate optional rules from presentation.

Follow [AGENTS.md](../../AGENTS.md) for lead/worker models, disjoint ownership,
one Editor owner and regular small commits/pushes. Keep Core source, its tests
and compiling Unity changes in separate checkpoints; preserve unrelated work.
Record focused tests and milestone checks in PROGRESS.md. Editor runs, iOS
exports and physical-device tests are distinct evidence.

## Outstanding decisions

- The approved image is native **1536 × 1024**. The requested detailed
  3072 × 2048 master remains outstanding; enlargement alone adds no detail.
- Exact 53-space placement and token clearance need a fit pass once work
  resumes. Earlier narrow-corridor findings remain relevant. The passing
  audit of the later V10 alternative does not apply to the selected image;
  preserve the approved composition when resolving layout.
- Resolve the eight-shelter/15-reward representation before playable integration.
  Biome reward curves and two-Feather safe / one-Feather worn-out rewards
  remain unapproved experiments.
- The release title is undecided. Keep repository, namespaces, assemblies
  and bundle identifiers as Quackies.
