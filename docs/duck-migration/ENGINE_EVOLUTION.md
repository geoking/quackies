# Evolve the existing engine

14 September 2026. Architecture recommendation for the accepted Day/Dream
direction in [PLAN.md](PLAN.md), now ten Days with original encounter powers
and World Events. M3 and M4 C1–C5 are complete. The implementation record is
in [m4/README.md](m4/README.md).

## Decision

Keep Quackies.Core, the MatchSession API and the CLI. Refactor the concentrated
rules that changed; do not restart the project or copy the complete game loop.

Core already separates legal actions, state, randomness, supply, rule effects,
AI and presentation. The CLI submits the same Core actions that Unity uses.
Those boundaries still fit Quackies. Starting again would recreate working
infrastructure and discard useful test coverage while the hard task—the new
rules specification—would remain.

Preserve the original game as a tested reference configuration. Build the new
duck profile on shared engine capabilities. A reference configuration is for
regression and development; it does not require a second mode in the game's UI.

## What remains useful

- [MatchSession](../../src/Quackies.Core/Match/MatchSession.cs) owns state and exposes
  GetSnapshot, GetLegalActions and Execute with opaque stale-safe action IDs.
- RuleSet registration, immutable observations, deterministic randomness,
  inventory ownership and configurable supply remain relevant.
- Ingredient and World Event lifecycle hooks already distinguish placement
  sources and can resolve legal choices without Unity implementing rules.
- IPlayerPolicy observes state and chooses legal actions; Normal's exhaustion
  reasoning remains useful, though shelter/Sleep strategy needs new evaluation.
- [The CLI loop](../../src/Quackies.Cli/Program.cs) remains a quick, repeatable
  text client for complete matches. Update its displays and exposed choices.
- Unity's presenter/view/editor separation remains useful. Build the new
  Adventure/Dream presentation around Core rather than copying rule arithmetic.

Do not extend the retained legacy QuackiesGame prototype. There should still
be one authoritative match engine.

## Concentrated changes

| Area | Required change |
| --- | --- |
| [BoardTrack](../../src/Quackies.Core/Rules/BoardTrack.cs) | Explicit playable endpoint and reward lookup policy; the duck profile has nest 0 and occupiable/scorable 1–43. Store authoritative biome, Sleep, Twigs, Feather yield and shelter identity. Remove the assumption that every profile scores position+1. |
| [Player state](../../src/Quackies.Core/Match/PlayerRoundState.cs) | Separate persistent Twigs, frozen Sleep earned, Sleep remaining, permanent trail, next-Day temporary advantage, effective start and frozen rest position. |
| Encounter identity and effects | Separate category, obstacle subtype, explicit movement instruction and ability parameters. Default movement one is not a universal printed strength. Add session-owned next-chip protection, per-placement nuisance suppression, pending Log, preview and active-flock state after the contract is approved. |
| Feather awards | Route all duck-profile Feather sources through one capability that advances the permanent start exactly once and records the source for display/history. No spendable Feather balance or Feather-spending action. |
| [Evaluation](../../src/Quackies.Core/Match/EvaluationPhaseHandler.cs) | Freeze final occupied rest; retain Twigs and floor(Sleep/2) when worn, resolve safe bonuses and compare safe ducks. Day 10 adds safe-haven Sleep then converts retained Sleep to Dream Twigs. |
| Dream phase | Replace the duck profile's old evaluation/shop/ruby sequence with explicit Night resolution and Dream purchasing. Full-screen layout is a Unity concern; phase legality belongs to Core. |
| [Purchasing](../../src/Quackies.Core/Match/ShoppingPhaseHandler.cs) | Use each player's Day-based 1/2/3 limit, remaining Sleep and approved stock/category restrictions; the approved policy uses unlimited stock and one per token type. Reopening a panel must not reset purchases. |
| Dawn preparation | Replace rat calculation with one pre-award Twig-deficit snapshot: 0–2 gives 0, 3–6 gives 1, 7–10 gives 2, and 11+ gives 3 stork parcels, plus temporary-bonus activation/expiry. No simultaneous old catch-up. |
| Observations/API | Add the new authoritative fields and phases without gratuitously breaking reference clients. Clients should not derive nest level, gifts or winner eligibility from labels. |
| AI | Keep legal-action separation and safe-draw reasoning; evaluate new Dream choices and whether the policy handles comfortable stops and recovery adequately. |

Use a small number of named rule policies/capabilities where behavior differs.
Do not build a generic rules language or spread profile checks through clients.
Classic state/behavior must remain valid for its tests while duck clients use
clear domain names. Compatibility is not a reason to label permanent progress
as a spendable currency.

## New rules rather than mandatory conversions

The user explicitly chose freedom from the Quacks effect structure. Write a
complete original encounter catalogue and World Event deck; neither the old
24-card count nor a one-to-one keep/adapt/replace mapping is required. Original
regression tests still describe the completed reference game.

Reuse technical capabilities where they fit. For example, the existing
[ongoing event hooks](../../src/Quackies.Core/Rules/Fortunes/OngoingFortunes.cs)
can inform a rain/Seed movement modifier. They do not mandate the old event's
text, deck membership or balance. Ruby spending, rat tails and bonus-die effects
must not leak into the new profile through default registration.

The current [encounter rules](ENCOUNTER_RULES.md) supersede the art study's
powers: separate thematic quantity from movement, support conditional
movement and ordinary nuisances, and specify a Day 5 Goose with a temporary
lower safe maximum. Splash now suppresses only the immediately next chip's
Obstacle nuisance; it never cancels movement or Exhaustion and supplies no
rescue. Mud reduces active flock for later movement and the Night comparison.
The current powers are summarized in [RULES_AT_A_GLANCE.md](RULES_AT_A_GLANCE.md).
Keep physical placement counts separate from active Companions. Protection of
a final Pebbles/Brambles placement must persist until settlement. Do not treat
Reeds x3 as three placements; keep its earned Twigs even when worn out.
AI and human must see the same information legally available to their duck;
a Signpost preview does not expose an unearned future draw order.

## Source audit and approved M2 contract

The latest read-only audit confirms reuse of the API shape, with a substantial
refactor of the match model. The [implementation plan](IMPLEMENTATION_PLAN.md)
records source anchors and checkpoints. Two important requirements are exact
private Signpost previews (current sampling does not reserve the next draw) and
separate token type/movement/ability quantities (current colour/value cannot
represent the agreed obstacles and Reeds faithfully).

M2 and the visual M3 gate are complete. The synchronized 43 rows (first haven
at 4), all prices/events, endpoint/empty-bag, shop, zero-start contract and
worn-out rules are approved. Every duck starts at nest 0 with zero Feathers;
safe havens and Dawn thresholds are the only permanent Feather sources, and
Most Rested is a temporary +1. Days 1–9 actions resolve
independently and publicly so players can react. Only Day 10 uses hidden
simultaneous commitments; adapt the old round-nine pattern through a final-Day
profile policy and fresh action IDs. Keep previews private on every Day.

Local autosave/resume is approved v1 scope. It does not exist today. Design
stable IDs, authoritative save state, resumable randomness and pending-decision
descriptors before coupling new effects to delegate-only state. Saveable Core
data stays separate from CLI/Unity filesystem adapters. A MatchView is not a
complete save, and scene reconstruction is not match restoration.

Night 10 still has no shopping: safe havens add +2 Sleep, retained Sleep converts
at floor(Sleep/4), safe Most Rested winners receive +1 Dream Twig. Compare final
total Twigs, then frozen retained Night 10 Sleep before conversion; equal values
on both mean a draw. Preserve that Sleep value in state and saves, including
bonuses and worn-out halving; do not reuse a distance or safe-eligibility tiebreak.

## Build and verification order

The isolated **M3 layout proof is complete and approved**. It uses fixed sample
data and remains separate from the changing Core DLL until M5. M4 C1–C5 is
complete. Within M4:

1. Establish duck profile/data/identity/state, preserving original regressions.
2. Implement Adventure, exact private previews and the selected decision rhythm.
3. Complete one Day/Night with encounters, all event fixtures and Dream purchases.
4. Complete ten Days, Dawn, Goose, final conversion and winners.
5. Refactor Normal AI and implement local save/resume support.

Update CLI observations/actions with every slice rather than leaving it until
the end. Bind the committed DLL to Unity only after the complete CLI contract
works. [IMPLEMENTATION_PLAN.md](IMPLEMENTATION_PLAN.md) lists the expected evidence.
The 15 September plan adds [M4.5](m4-5/PLAN.md) as a further gate before that
binding: improve AI and assess complete-game balance/playability in Core/CLI,
then review any proposed rule/data adjustments before proceeding into Unity.

Keep focused source/test checkpoints separate and push each. Retain meaningful
classic regression tests; add duck tests against the same engine for all 43
positions, frozen versus spent Sleep, every Feather source, Dawn thresholds,
tied Most Rested, purchase tiers, endpoint/rewind cases, each new encounter/event
and final Day 10. Verify independent Days 1–9 actions, hidden Day 10 decisions,
final Twig/Sleep ranking and restored action freshness. Verify obstacle subtypes, queued nuisances, private previews
and protection/Exhaustion boundaries against the agreed new specification.
Continue testing immutable snapshots, stale choices and configured supply policies.

Use seeded complete matches to inspect game length, leader retention, recovery,
shelter usage, upgrade accumulation and early arrival at the endpoint. These are
future balance checks, not claims that the approved starting values are balanced.
Unity then verifies layout/input, correct displayed state, scene reconstruction
and complete human/AI play. Editor success, iOS export and device testing remain
separate evidence.

## Audit basis

The read-only review inspected the source files linked above,
[RuleSet](../../src/Quackies.Core/Rules/RuleSet.cs),
[SetOneIngredients](../../src/Quackies.Core/Rules/Ingredients/SetOneIngredients.cs)
and [SetOneFortunes](../../src/Quackies.Core/Rules/Fortunes/SetOneFortunes.cs).
No Core/CLI implementation or runtime tests were performed for this planning
recommendation. The v1 bag/data audit is separate bounded design evidence.
