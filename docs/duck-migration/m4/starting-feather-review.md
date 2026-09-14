# C1 starting-Feather endpoint review

14 September 2026. The approved shared starting setting 0–3 is **not safe under
the current 43-space endpoint contract**. Setting 3 has a constructive,
positive-probability two-duck witness whose permanent start is 43 before Day 10
and whose temporary Most Rested step makes its effective start 44. The rules
require at least one chip placement, but they do not say how a duck already at
or past 43 places that chip.

This is a reachable sequence, not an inference from the earlier loose upper
bound. [`starting-feather-review.py`](starting-feather-review.py) checks the
board, shop, bags, purchases, events, encounter counters, movement, settlement,
Twig scores, Dawn awards, haven Feathers and Most Rested results, then writes
the full replay to [`starting-feather-review.json`](starting-feather-review.json).

## Constructive witness

Both ducks use starting setting 3. `Target` maximizes automatic starting
progress while `Opponent` builds a Twig lead with Reeds. Every listed bag order
has positive probability under uniform draws without replacement.

| Day | Event | Target start → rest | Opponent start → rest | Twigs today; cumulative T/O | Dawn to target | Haven Feathers T/O | Night purchase T/O |
| ---: | --- | --- | --- | --- | ---: | --- | --- |
| 1 | A Pocket of Driftwood | 3 → haven 4, safe | 3 → haven 16, worn out | 0/5; 0/5 | 0 | 1/0 | Tailwind →2 / Reeds ×1 |
| 2 | Thick Morning Mist | 7 → haven 21 | 3 → haven 16 | 4/5; 4/10 | 2 | 1/1 | Tailwind →6 / Reeds ×2 |
| 3 | Rain-Softened Seeds | 10 → haven 32 | 4 → space 20 | 6/8; 10/18 | 2 | 2/0 | — / Reeds ×2 |
| 4 | Restless Night | 14 → haven 32 | 4 → space 19 | 6/9; 16/27 | 2 | 2/0 | — / Reeds ×2 |
| 5 | A Friendly Guide | 19 → haven 32 | 4 → space 20 | 6/12; 22/39 | 3 | 2/0 | — / Reeds ×2 |
| 6 | All Tucked In | 24 → haven 32 | 4 → haven 21 | 6/14; 28/53 | 3 | 2/1 | — / Reeds ×2 |
| 7 | Home Before Dark | 29 → haven 32 | 5 → space 23 | 6/16; 34/69 | 3 | 2/0 | — / Reeds ×2 |
| 8 | Still Air | 34 → haven 36 | 5 → space 23 | 7/18; 41/87 | 3 | 2/0 | — / Reeds ×2 |
| 9 | Shared Supper | 39 → endpoint 43 | 5 → space 25 | 8/20; 49/107 | 3 | 2/0 | — / Reeds ×2 |

The Day 10 event is Sunlit Signboards. The 58-Twig deficit awards Target three
more Dawn Feathers. Its progress is then:

```text
initial setting                         3
safe haven Feathers, Days 1–9          16
Dawn Feathers, Days 2–10               24
permanent Day 10 start                 43
Night 9 temporary Most Rested step      1
effective Day 10 start                 44
```

Target is the sole Most Rested duck on Day 1 because Opponent is worn out, then
has strictly more retained Sleep on Days 2–9. The temporary step is therefore
available every following Day. Day 1's final Brambles reduces Target's printed
Twig from one to zero; later final Brambles reduce only that Day's printed
Twigs and never remove the banked score.

Opponent's Night 1 worn-out payout is enough for Reeds ×1: final Pebbles reduces
13 Sleep to 12 before halving, leaving 6. Nights 2–9 each retain at least 11
Sleep and buy one Reeds ×2. This obeys the one-purchase limit on Nights 1–3,
the later larger limits, and the one-Reeds-family-per-Night rule. Reeds are
drawn before five ordinary whites and are retained while safe. The Day 5 Goose
is in both bags but is not drawn, which is a legal positive-probability order.

The replay assigns all ten World Events exactly once. Rain supplies the exact
Day 3 movement to haven 32. Still Air is placed on Day 8, when Target uses only
a Seed and Brambles; Opponent's reduced starting Tailwind still gives the
checked 18-space route. A Friendly Guide is consumed by Target's opening Mud
before its final Brambles, so the Twig penalty remains active. Splash never
protects a final penalty chip in the witness, and all helpful movement precedes
any Log that could slow it.

## Decision required before exposing settings 1–3

The current endpoint rule covers a chip that reaches or passes 43 during a
Draw. It does not cover an effective start already at or past 43. Setting 3 can
reach that state, so C1 cannot certify the approved 0–3 option set without a
rule decision.

Two bounded implementation choices follow from the evidence. The game could cap
only the effective start at 42 while continuing to record every earned Feather,
or v1 could expose only the proven-safe zero setting. The first changes the
approved one-Feather/one-step benefit at the endpoint; the second reduces the
approved setting range. Either choice therefore requires explicit user approval
before Core work. Clipping recorded progress, converting Feathers or suppressing
awards is not authorized by this review.

The existing proof remains sufficient for setting 0: its effective start is at
most 42. Separate exact classifications of settings 1 and 2 are unnecessary to
establish the contract gap because setting 3 is already a reachable witness;
they should be covered by whichever start-at/past-endpoint policy the user
chooses.

## Evidence boundary

The script is a deterministic verifier for one legal sequence. It does not
estimate the sequence's probability, player strategy, endpoint frequency or
full-match balance. It assumes the supported two-duck game, uniform bag orders
with every permutation possible, the approved unlimited shop stock, and no AI
policy constraint on legal choices. The witness uses only authoritative rules
data and has no Unity dependency.
