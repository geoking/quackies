# Normal duck policy review

The v1 Normal opponent uses only its detached observation and current issued
actions. It has no access to the session, a save file, the random generator or
an opponent's private preview. Choosing an action consumes no randomness.

## Decisions checked

- Continue across an early equal-reward stretch when the known next Seed is
  safe; the neutral-weather fixture verifies the actual placement from 1 to 2.
- Continue from 3 into haven 4, but consider keeping that haven before drawing
  onward. Tests execute the move, so Rain cannot accidentally change the case.
- Stop before a known Goose that would cause wear-out. Splash can suppress
  the Goose nuisance but cannot remove its Exhaustion contribution.
- Value an attainable Most Rested reward when choosing whether to risk a
  settled lead. The temporary next-Day step and final Dream Twig have separate
  values. Worn-out opponents cannot qualify for Most Rested.
- Prefer useful movement earlier and direct Reeds Twigs later in a controlled
  offer comparison. Legal price/type/capacity constraints always come from Core.
- Make the same decision when unknown bag order changes or another duck has
  submitted an unrevealed final-Day commitment.

The policy compares the current rest against a bounded estimate of one further
placement, with an allowance for safe continuation. Known previews determine
the next candidate exactly; otherwise remaining physical chips weight the
estimate. Haven/Flower rewards, current flock, public collective conditions,
obstacle protection, standings and remaining Days contribute to that estimate.
Feathers have no decision value on Day 10. Purchases use remaining-Day value
and owned-token diversity; the policy does not search every possible shopping
bundle or long sequence of future draws.

## Interpretation

At committed snapshot `cc4c11a`, the full suite passes 286 tests. Three Normal
versus Normal runs exercise deliberate stops and several shop offers:

| Seed | Commands | Explore | Settle | Purchases | Distinct purchased offers | Winner |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| 4 | 149 | 78 | 18 | 26 | 7 | Human seat |
| 42 | 138 | 68 | 20 | 23 | 7 | AI seat |
| 941 | 161 | 91 | 20 | 23 | 7 | AI seat |

Both seats use Normal for these checks. Command totals also include private
preview choices and finishing Dreams. Distinct offers include variants; they
are not a count of all seven helpful token types. The decision-sensitive
Most Rested regression holds rival Sleep constant and changes only eligibility:
the safe rival can justify exploring, whereas the worn-out rival cannot block
the safe duck's award estimate.

These tests establish legitimate information use, coherent decisions and
determinism. They do not establish optimal play or game balance. Normal is
conservative about giving up a comfortable haven; the saved seed-42 example
finishes with both ducks at haven 10, so it is not evidence of adequate late
biome exploration. M6 should compare more adventurous policies, haven landing
frequency, purchases, wear-outs, Dawn recovery and region use before changing
the approved rules or reward values.

The [full CLI game](cli-full-game.txt), [saved result](full-game-save.json) and
[milestone validation](m4-final-validation.txt) provide the reproducible run.
The existing short Day-1 demonstration remains a scripted command-path smoke
test, not a Normal-policy demonstration.
