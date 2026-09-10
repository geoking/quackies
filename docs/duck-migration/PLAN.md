# Duck migration plan

## Current scope and source

Work on **M0 only**, then pause for the user's reaction. The current branch is
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

- Happy, colourful cartoon art: expressive ducks, rounded shapes, clear outlines,
  soft shading and playful wetlands. A colourful playmat is a suitable first
  experiment. Exhaustion should look sleepily fed up or comically muddy.
- One continuous trail winds back and forth with rounded bends. Its layout is
  inspired by a winding board path, without adding ladders, snakes or shortcuts
  between visually adjacent rows. Preserve every physical track position.
- The duck is the permanent starting marker. It stays there while encounters
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
| M1 — Style test | Generate only playmat, happy duck and seed icon; assemble a separate `Assets/Scenes/DuckStyleTestScene.unity` through a new Editor builder | Native iPad-aspect Unity screenshot, full placeholder trail, representative labels, button and encounter sizes; ask how the style feels |
| M2 — Terminology | Add the smallest shared display contract in `src/Quackies.Core`; adapt `src/Quackies.Cli` and expose the same definitions to Unity | Duck CLI, preserved classic identities, same numerical results and legal actions for identical seeds/actions; ask about wording |
| M3 — Playable duck table | Connect a separate duck presentation to `MatchSession`; add essential shelter, feather, nest and catch-up visuals | Complete game with Explore, Settle down, exhaustion, rewards, market, next-day reset and retained nest score; ask for a playtest reaction |
| M4 — Essential art | Complete encounter icons and original help text; finish modest journey/result animations and UI | Readability at actual sizes, safe/worn-out endings, consistent assets and accurate previews; ask about feel |
| M5 — Shelter experiment | Only after a specific choice: separately selectable fewer/more rewarding shelters | Compare with unchanged rules; verify safe/exhausted stop, passing and ordinary spaces; review balance |
| M6 — Migration review | Review clients, AI, compatibility, assets, tests and remaining issues | Tested branch ready for the user's merge decision; no automatic merge |

The M1 filenames and builder are proposed additions, not existing implementation.
Keep `QuackiesInitialScene.unity`, its builder and source art usable throughout.
Do not add the style-test scene to shipping build settings by default.

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

For the new trail, generate coordinates by physical index independently of
`QuackiesArtCatalog`'s existing cauldron-art anchors. The standard board has 54
indexed spaces (0–53), with last encounter position 52 and scoring space 53.
Drive the resting preview from Core's scoring space after the last encounter;
never use the encounter's own position as its reward index.

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

Use the M1 preview to test a **6 × 9 winding placeholder trail**, accounting for
all 54 indexed spaces. Rounded bends and subtle row guides should make order
clear. Exact arrangement may change after the readability test.

Keep Twigs beside a nest, label pennies as **Spend today**, and show the resting
preview separately from the nest score. These are rules-neutral clarity choices.
Use placeholders for those displays during M1; do not generate the whole asset
pack before the first style reaction. Test tubes and AI-history UI remain outside
this migration's currently selected milestone.
