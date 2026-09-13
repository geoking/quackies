# Evolve the existing engine

13 September 2026. Architecture recommendation for the accepted Day/Dream
direction in [PLAN.md](PLAN.md), now ten Days with original encounter powers
and World Events. This document changes no implementation.

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
  inventory ownership and finite shared supply remain relevant.
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
| [BoardTrack](../../src/Quackies.Core/Rules/BoardTrack.cs) | Explicit playable endpoint and reward lookup policy; the duck profile has nest 0 and occupiable/scorable 1–50. Store authoritative biome, Sleep, Twigs, Feather yield and shelter identity. Remove the assumption that every profile scores position+1. |
| [Player state](../../src/Quackies.Core/Match/PlayerRoundState.cs) | Separate persistent Twigs, frozen Sleep earned, Sleep remaining, permanent trail, next-Day temporary advantage, effective start and frozen rest position. |
| Encounter identity and effects | Separate category, obstacle subtype, explicit movement instruction and ability parameters. Default movement one is not a universal printed strength. Add session-owned nuisance/preview/rescue state only after the candidate rules are agreed. |
| Feather awards | Route all duck-profile Feather sources through one capability that advances the permanent start exactly once and records the source for display/history. No spendable Feather balance or Feather-spending action. |
| [Evaluation](../../src/Quackies.Core/Match/EvaluationPhaseHandler.cs) | Freeze final occupied rest and Sleep at the agreed point; resolve Night effects and compare safe ducks by earned Sleep, preserving correct event ordering. |
| Dream phase | Replace the duck profile's old evaluation/shop/ruby sequence with explicit Night resolution and Dream purchasing. Full-screen layout is a Unity concern; phase legality belongs to Core. |
| [Purchasing](../../src/Quackies.Core/Match/ShoppingPhaseHandler.cs) | Use each player's Day-based 1/2/3 limit, remaining Sleep, finite stock and category restrictions. Reopening a panel must not reset purchases. |
| Dawn preparation | Replace rat calculation with one pre-award Twig-deficit snapshot, bounded stork parcels and temporary-bonus activation/expiry. No simultaneous old catch-up. |
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

The [fresh encounter study](concepts/2026-09-13-obstacle-study/README.md) proposes
eight whites plus five colours, five safe Exhaustion, mild bounded nuisances,
explicit Tailwind movement and preview/shield/rescue powers. These are review
candidates. Specify cancellation versus placement, private previews, nuisance
priority/expiry, daily caps and pending versus banked rewards before coding.
AI and human must see the same information legally available to their duck;
a Signpost preview does not expose an unearned future draw order.

## Before coding

Finish the 50-row table and eight shelter locations. Define no-draw rest,
rewinds, overshoot/settling, saturated permanent trails, worn-out results,
Sleep expiry, bonus ordering, whether any separate recovery/flask exists,
the exact starting bag, threshold, prices and final-Night behavior.
The accepted one-to-one Feather rule cannot silently become a cap, bank or
alternate conversion to work around an unresolved boundary.

Standard play ends after Day 10: purchasing encounters and awarding tomorrow's advantage
need an explicit ending treatment. Twigs determine the winner; reaching 50
does not automatically win. Record any final tie-break instead of inheriting
one accidentally from the old physical scoring position.

## Build and verification order

1. Publish the readable rules contract and data.
2. Evolve track, state and observations, then direct rest/Sleep/Feather behavior.
3. Implement Most Rested, Dawn Delivery, Dream purchases and the ending.
4. Implement every newly specified encounter/event and update CLI/Normal AI.
5. Verify complete deterministic text matches, then bind Unity.

Keep focused source/test checkpoints separate and push each. Retain meaningful
classic regression tests; add duck tests against the same engine for all 50
positions, frozen versus spent Sleep, every Feather source, Dawn thresholds,
tied Most Rested, purchase tiers, endpoint/rewind cases, each new encounter/event
and final Day 10. Verify obstacle subtypes, queued nuisances, private previews
and cancellation/Exhaustion boundaries against the agreed new specification.
Continue testing immutable snapshots, stale choices and shared supply.

Use seeded complete matches to inspect game length, leader retention, recovery,
shelter usage, upgrade accumulation and early arrival at the endpoint. These are
future balance checks, not claims that the currently proposed values work.
Unity then verifies layout/input, correct displayed state, scene reconstruction
and complete human/AI play. Editor success, iOS export and device testing remain
separate evidence.

## Audit basis

The read-only review inspected the source files linked above,
[RuleSet](../../src/Quackies.Core/Rules/RuleSet.cs),
[SetOneIngredients](../../src/Quackies.Core/Rules/Ingredients/SetOneIngredients.cs)
and [SetOneFortunes](../../src/Quackies.Core/Rules/Fortunes/SetOneFortunes.cs).
No tests or implementations were performed for this planning recommendation.
