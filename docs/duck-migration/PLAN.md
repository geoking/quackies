# Quackies overall plan

Updated 13 September 2026. This is the accepted direction following the user's
approval of the full-screen Dream view, shared nest levels, Feather trails,
Dawn Delivery and token style, followed by ten Days and fresh encounter rules.
It supersedes the earlier presentation-only migration and Quacks rule mapping.
Historical decisions and evidence remain in [PROGRESS.md](../PROGRESS.md).

## What we are building

A complete ten-Day tabletop game for iPad mini landscape, initially one human
versus Normal AI. Ducks explore a shared world by Day, then dream at Night.
Twigs build their nests and determine the winner. Each duck has its own bag,
route state and score; shared World Events affect the same world.

The selected [three-biome board](concepts/2026-09-12-approved/board-art-approved.png),
[V2 duck tiles](concepts/2026-09-11/duck-player-tiles-v2.png) and
[V3 seed](concepts/2026-09-11-v3/seed-tile-v3.png) remain the art references.
The **full-screen nest and Dream mat, Concept B**, is the chosen layout direction:
[view concept](concepts/2026-09-13-dream-study/concept-b-nest-mat.png).
A clear **View adventure** button returns to the board. The integrated tray is
a historical alternative, not the default.

The user has accepted the current encounter rules, including plain Seeds,
Companion flock movement and Night competition, Signpost movement, Reeds
quantities, conditional nuisances and the Day 5 Goose. Read
[RULES_AT_A_GLANCE.md](RULES_AT_A_GLANCE.md) for the complete agreed overview and
remaining decisions, with detailed timing in [ENCOUNTER_RULES.md](ENCOUNTER_RULES.md).
The [current token set](concepts/2026-09-13-agreed-token-set/README.md) covers all
16 encounter designs/variants for review. This checkpoint does not implement
rules, modify the CLI or import into Unity. The full rules sheet is not finished.

## The short build sequence

1. **Finish the rules sheet.** Define all 50 rewards, shelter locations, prices,
   event changes and end-of-game details using the accepted direction below.
2. **Upgrade Core and the CLI.** Reuse the existing engine, implement the new
   rules in small tested pieces, and play complete games in the text client.
3. **Build the Unity table.** Fit all 50 spaces to the selected artwork and
   build the spacious Dream screen, nest display and token catalogue.
4. **Connect and play the full game.** Run all ten Days with the human and AI,
   including rest, dreams, upgrades, deliveries and the final result.
5. **Balance and finish.** Test whether players can recover from falling behind,
   tune rewards, inspect iPad readability, polish feedback and verify the iOS export.

Stop for feedback at each milestone. These are future steps, not permission
to start implementation automatically after this art/planning checkpoint.

## Accepted game rules and presentation

### Adventure and resting

Use **50 playable/scorable spaces plus the separate starting nest**. The duck
rests and receives the rewards of its final occupied encounter space, not the
next empty space. Keep rewards readable when both encounter and duck occupy it.
The nest has no printed zero or start label; position indices stay in data.

Author a new 50-row Sleep/Twig/Feather table. The old 53-row table is a reference,
not target data to truncate or relabel. Keep eight clearly linked shelters,
distributed 2/3/3 across the biomes, and remap their indices to the new route.
The original 15 ruby flags do not define this new profile.

Sleep reflects comfort as well as distance. A candidate reward curve gives
exposed wasteland less Sleep than a nearer comfortable shelter, while Twigs
reward exploration separately; confirm the curve in the 50-row table. The intended
wasteland haven rewards are two Feathers at the earlier havens and three at the
end. “Shelter” and “haven” refer to the same marked resting-place concept.
Exact amounts elsewhere, indices, prices and the complete reward curve
are the next rules-sheet milestone.

The duck marks its effective start during exploration and follows the completed
route when drawing ends. It settles or flops down on its final occupied space.
Its pose reflects Core's outcome. Next Day starts from its permanent Feather
trail plus any valid temporary advantage, not yesterday's resting place.

### Sleep, Twigs and Feathers

| Concept | Agreed role |
| --- | --- |
| Sleep score | Comfort earned tonight; frozen for the Most Rested comparison |
| Sleep remaining | The portion of that night's allowance still available for purchases |
| Twigs | Persistent victory score, shown through an exact total and growing nest |
| Feathers | Each one automatically extends the permanent starting trail by one space |

Use a moon icon for Sleep, twigs for score and a Feather trail for permanent
progress. A purchase changes remaining Sleep, never the earned Sleep score.
Do not call Sleep Pond pennies or illustrate it with coins.

**Feathers have one purpose only.** One Feather always represents one permanent
start advance. They are not banked or spent through a menu, cannot refill a flask,
and are not converted into Twigs. There is no one-upgrade-per-night redemption
cap. Tune the sources/yields rather than changing that exchange rate.

Gaining a Feather updates the permanent trail; it must not relocate today's
encounters or change a frozen resting reward. The old ruby currency phase and
its alternative purchases are obsolete for the new game. Starting-Feather
settings represent initial trail progress. The exact timing of animations and
the track-end/last-Night cases must be specified before implementation.

### Full-screen Dream view and nest levels

Use a dedicated Dream view with room for all legal encounter categories and
thematic variants, clear costs and remaining Sleep, plus a persistent route-preview
button. Do not add a second movement track. The earlier mockup's two seed offers
and prices illustrate layout only; they are not a random market or price table.
Obstacles are a bag category, not automatically a purchasable offer. Adapt
Concept B's Feather panel to show Feathers laid and permanent trail progress,
not a spendable balance or manual exchange button.

Both ducks unlock the same nest level with the calendar:

| Days | Nest level | Maximum purchases per Night |
| --- | --- | --- |
| 1–3 | 1 — Little nest | 1 |
| 4–6 | 2 — Comfortable nest | 2 |
| 7–10 | 3 — Cosy nest | 3 |

Each duck has its own allowance; this is not a shared match-wide purchase cap.
Extending the third tier through Day 10 is the initial schedule adjustment.
These are initial settings to test, not claims of balance. Limits apply across
the whole Night, not separately each time the purchase panel is opened.
Sleep still limits affordability. The starting proposal for M2 is to retain
finite supplies and one purchase per category, allowing level 3 to buy three
different legal categories. Confirm those policies in the rules sheet rather
than silently carrying over the old global cap of two.
Final-Night shopping has no following Day and requires an explicit ending rule.

Nest capacity is not unlocked by being ahead in Twigs. Shared levels prevent
the leader alone gaining extra buying power. Decorative nest growth and exact
Twig totals show individual progress; do not require counting drawn branches.
Show a brief gain such as “+4 tonight”, with rival totals readily accessible.

### Most Rested Duck

Compare **frozen earned Sleep among ducks who are not worn out**, before
spending or awarding the winner's bonus. All tied eligible leaders qualify;
if everyone is worn out, nobody does.

Start with a bounded **Start +1 tomorrow** reward. It is temporary for the next
Day, distinct from a permanent Feather, and is not stored for later Days.
It replaces the old default bonus die in the new profile. “Every encounter +1”
is not an equivalent prize and is deferred; varied bonus choices can follow
balance evidence. Define the final-Night reward and contributing Sleep modifiers
in the rules sheet.

The Companion flock Sleep award resolves before this comparison and
before earned Sleep freezes. It can therefore change both Dream buying power
and the Most Rested result; test the combined advantage, not just the raw bonus.

Movement bonuses affect distance, not white Exhaustion. Default movement and
explicit movement instructions are separate from ability amounts; there is no
universal printed strength. Define ordering with World Events and other effects.

### Dawn Delivery

A gift stork visits trailing ducks at the start of the Day. This replaces rat
tails and the temporary lily-pad catch-up system; do not apply both.

Use the Twig deficit from the leader at dawn, before deliveries, with a public
explanation of eligibility. Check at each dawn; Day 1 normally has equal zero
Twig scores and no parcel. Initial settings:

| Twig deficit | Dawn gift |
| --- | --- |
| 0–4 | None |
| 5–9 | 1 Feather |
| 10 or more | 2 Feathers |

Each eligible duck receives at most one parcel per dawn. Its Feathers extend
the permanent trail one space each, exactly like other Feathers. These thresholds
are accepted starting values for tuning, not guaranteed fairness. Test repeated
deliveries, deliberate under-scoring, large gaps and combined leader bonuses.
Borrowed encounter parcels remain a deferred alternative.

### World Events and encounter effects

Reveal one shared **World Event** at the start of each Day. Explain its effect
and duration; resolve reveal-time choices before exploration, while preserving
the correct timing of later effects. For example:
“Rain-softened seeds — Seeds move +1 today.”

Design original encounter powers and a World Event catalogue around the duck
game. Reuse useful engine hooks, not a required Set 1 effect structure, card
count or one-to-one conversion of the old 24 events. The complete new rules
contract must list every included effect, timing, choice and restriction.
The original game remains a tested reference, not a content requirement.

### Accepted encounters — first ruleset to test

Use [ENCOUNTER_RULES.md](ENCOUNTER_RULES.md) as the current detailed reference;
it supersedes the earlier study's powers. The user likes the helpful colour
roles and requests Reeds x1/x2/x3 for one/two/three Twigs, with normal movement
still one. Numbers describe thematic quantities, not universal strength.
Other quantity variants and future shared rule cards are suggestions to review.

**Seeds are cheap, plain one-step non-Exhaustion chips.** Remove their previous
Sleep reward, kernel denominations and food/movement choice. They dilute the
bag's obstacle proportion without cancelling Exhaustion already gained.

Tailwind →2/→4/→6 denotes total movement. Companion movement is
two for the first placed that Day, three for the second and four for all later
ones, plus a Night bonus for the most Companions. This replaces the old
shield and compares placed counts among safe ducks after everyone finishes.
Tied positive leaders receive +1 Sleep with one Companion or +2 total with two
or more, before earned Sleep freezes. This capped scaling and eligibility are
accepted starting rules to test; the bonus is not paid per chip or per neighbour.

Signpost moves two and previews one chip, offering modest
reliable movement with information. Stronger preview quantities and optional
Wildflower shelter movement remain alternatives to review, not automatic scope.
Reeds quantity never sets movement. Do not force every colour into movement
grades or change token meanings during a match. Revisit early reward/prices
without Seed Sleep, and ensure Companion/Signpost have an appropriate cost
relative to Tailwind →2. The initial comparison is between one human and one AI;
an adjacent-player rule card belongs to a future multiplayer option.

The agreed opening bag is eight white Obstacles plus five colours:
two each of Log, Mud, Pebbles and Brambles, with two Seeds,
one Tailwind →2, one Signpost and one Splash. The normal safe maximum is five
Exhaustion. Every white adds one, while ordinary extra nuisances are conditional:
Log affects fast movement; Mud blocks added movement; Pebbles/Brambles lose
Sleep/a newly earned Twig only when the duck rests on that chip.

Goose joins from Day 5 and lowers the safe maximum to four when
drawn, which can immediately cause wear-out at five Exhaustion. The rule
adds one per bag once, keeps it thereafter and resets the maximum next dawn.
Companion does not block this special Exhaustion rule. Splash cancels
any draw that would exceed the resulting maximum and then settles the duck safely.
Goose replaces its older next-colour ability suppression. These powers and the
starting composition are accepted working rules; prices, final payouts and
complete rule interactions still need specification and balance evidence.

The effect-free bag audit gives 7.78 placements and 2.78 coloured placements on
average when stopping at the fifth white; it excludes powers and player strategy.
Test whether early trips offer enough useful coloured draws. Keep nuisance
penalties bounded, avoid stacking movement penalties, and show a concise
board-edge obstacle key with active effects near the draw control. The
Goose pressure comparison is separate and excludes actual Day 5 purchases;
neither audit certifies full-game balance. No Goose starts in the Day 1 bag.

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
The [current token set](concepts/2026-09-13-agreed-token-set/README.md) has plain
faces, explicit movement arrows, Reeds quantities and all five white nuisances. These remain
concept sheets, not production sprites or final effect specifications. Player ducks remain
distinct colours/personalities and die-cut silhouettes; the Companion duck is a
small illustrated encounter tile, not another player marker. Four duck identities
do not expand the initial human-versus-AI scope.

Other display terms: Day; Explore; Settle down; Exhaustion; Worn out!; Shelter;
Feather trail; Dream choices; Most Rested Duck; World Event; Dawn Delivery.
Splash provides the agreed rescue; any separate flask/recovery mechanic remains
an unresolved inclusion decision, not a required Quacks feature or Feather purchase.
Production guidance is in [ASSET_BRIEF.md](ASSET_BRIEF.md).

## Reuse the engine; replace the changed rules

Retain one Core engine and evolve its existing snapshot/legal-action/execute
API. Keep deterministic randomness, ownership of state, bags, supply, event
hooks and the separation of AI from legality. Refactor the parts tied to old
scoring, phases and currency. Keep the CLI as a quick way to exercise the same
new rules before Unity integration.

Preserve the completed original rules as a regression reference and keep its
tests meaningful. New Quackies rules belong in a distinct configuration and
focused rule policies where needed, using the same engine. Do not fork an entire
second engine, grow the legacy prototype, or scatter theme conditionals through
Unity. A classic reference configuration does not require another user-facing
game mode.

Read [ARCHITECTURE.md](../ARCHITECTURE.md) and
[ENGINE_EVOLUTION.md](ENGINE_EVOLUTION.md) for the implementation rationale.
Core remains independent of Unity; clients render authoritative state rather
than calculating rewards. Preserve existing serialized assets until intentionally
migrated, and validate old and new behavior separately.

## Milestones and remaining decisions

M0 (baseline) and the initial M1 art exploration are recorded history. The
remaining roadmap replaces the earlier “terminology first, balance later” order:

| Milestone | Work | Review evidence |
| --- | --- | --- |
| M1 checkpoint — accepted rules overview and full token set | Agreed rule reference and 16 encounter designs/variants | Review current art; complete remaining rules before implementation |
| M2 — Rules sheet | 50-row table, prices, shelter indices, original encounter/event definitions, worn-out/recovery and final-Night rules | One readable, complete rules contract and example Days |
| M3 — Core and CLI | Implement the new profile in bounded source/test checkpoints; update observations, actions and Normal AI | Focused rule tests, original regression checks and complete deterministic CLI matches |
| M4 — Unity board and Dream layout | Fit 50 indexed spaces, tokens, nest, full catalogue and navigation to iPad landscape | Actual-size layout, reward visibility, alignment and touch checks |
| M5 — Full playable game | Bind every phase/choice to Core, add deliveries and journey feedback, restart and complete ten Days | End-to-end human/AI match, scene reconstruction and error checks |
| M6 — Balance and release preparation | Tune recovery/leader loops, finish essential assets/help and verify export | Recorded balance evidence, full relevant checks, iOS export; user decides merge/release |

M2 must settle: stopping before a draw; rewinds; endpoint/overshoot and saturated
Feather trails; worn-out Sleep/Twig/Feather treatment; Sleep expiry and final-Night
disposition, including whether any Sleep conversion exists; final-Day rewards
with no tomorrow; recovery/flask mechanics; event/bonus ordering; finite supply,
category uniqueness/unlocks, later quantity variants, event/nuisance ordering
and the final price table. Preserve the accepted starting bag, five-safe maximum,
Day 5 Goose and initial encounter powers while specifying their edge cases. Define when
each Day's effective start freezes, including Dawn gifts and reveal-time event
Feathers. Endpoint bookkeeping may not add Feather banking, spending, conversion
or a nightly redemption cap as a convenient substitute.

The approved board is native 1536 × 1024; the requested detailed 3072 × 2048
master and exact 50-space fit remain outstanding. A passing width audit of a
later V10 alternative does not apply to the user-selected image. Preserve the
selected composition while resolving the layout.

Short-match settings are deferred; standard play is ten Days. Test tubes,
additional encounter sets, a separate AI-history pane and physical-device
installation also remain deferred. Keep the initial AI/starting-progress
settings, fixed starting-bag reference, rival-board inspection and restart.
Keep source art, identifiers and namespaces as Quackies; release title is undecided.

Follow [AGENTS.md](../../AGENTS.md): lead owns Git/integration, bounded workers
have disjoint ownership, one agent owns Editor mutations, and every coherent
checkpoint is pushed on `codex/duck-game-milestone-0`. Preserve unrelated local
changes. Review each milestone before starting the next. Editor execution,
iOS export and physical-device testing are distinct evidence; no automatic
merge, device install or public release.
