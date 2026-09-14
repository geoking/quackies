# Quackies rules at a glance

Updated 14 September 2026. The user's latest mechanic changes are incorporated
below. The user approved the [43-space rewards and shop prices](v1/BOARD_AND_SHOP.md),
the three-Feather Dawn cap, the [ten-card World Event deck](v1/WORLD_EVENTS.md),
the remaining M2 defaults, and local autosave/Continue as a future feature. This
rules sheet is closed for M2; it is a specification, not evidence of implementation
or full-match balance.

## The game

Play **ten Days**, initially one human versus Normal AI. Each duck has its own
bag and route progress in a shared world. Adventure by Day, then enter the
full-screen Dream/nest view at Night; View adventure returns to the board.
**Most Twigs wins**, including final Dream Twigs. A shorter game setting waits.

The incomplete nest is separate from **43 playable spaces** across wetlands,
meadow and wasteland. Score the duck's final occupied space, never the next
space or all spaces passed. Eight havens are at **4, 10, 16, 21, 26, 32, 36
and 43**. They improve Sleep without increasing the local Twig plateau;
endpoint 43 is the exception, with the highest printed Sleep and Twigs by one.

## Dawn and adventure

Shuffle ten shared World Events once per game; reveal one each dawn without
replacement. Its effect lasts that Day. Days 1–10 therefore use all ten cards.

**Days 1–9:** Draw or Settle independently. Each completed action is visible,
so ducks can react to others' progress before choosing to continue or stop.
**Day 10 only:** active ducks commit their choices privately, then reveal each
decision beat together. Finished ducks leave later beats. Signpost previews
remain private on every Day; shared Night rewards wait for everyone to finish.

At dawn the stork gives **0 Feathers for a 0–2 Twig deficit, 1 for 3–6, 2
for 7–10, and 3 for 11 or more**. Tied leaders get none. Snapshot all scores
before delivery; repeat the check each dawn. Every duck starts at nest 0 with
zero Feathers. Permanent Feathers come only from safe haven rewards and these
Dawn thresholds. Each awarded Feather permanently advances later starts by
exactly one and is never spent or converted. Most Rested is a temporary +1
starting step, never a Feather.

The opening bag has **13 chips**: two each of Log, Mud, Pebbles and Brambles;
two Seeds; one Tailwind →2; one Signpost; one Splash. Each white moves one and
adds one Exhaustion. **Five is normally safe; six wears the duck out.** Once on
Day 5, add one Grumpy Goose to each bag, where it remains for the rest of the game.
An unprotected Goose adds one Exhaustion and lowers today's safe maximum to
four before checking safety; five now wears the duck out. Reset the maximum
and other temporary state next dawn. Owned chips return to the bag each Day.

## Helpful encounters

A **token type** is Seeds, Tailwind, Signpost, Splash, Reeds, Companion or
Wildflowers. Different strengths/quantities of one type still count as one type:
Seed + Tailwind + Signpost is three types; Tailwind →2 + →4 + →6 is one.
“Helpful” means a non-obstacle chip. This replaces the unclear word “family”.

| Chip | Rule |
| --- | --- |
| Seeds | Move 1; no ability or Exhaustion. Cheap bag improvement. |
| Tailwind →2 / →4 / →6 | Move the printed total before modifiers; no other ability. |
| Signpost →2 | Move 2, then privately preview the next chip. Continue with that exact chip or settle. No selecting/reordering. |
| Refreshing splash | Move 1. If the **immediately next chip** is an obstacle, ignore its nuisance; its movement and Exhaustion still apply. Protection expires after that chip even if helpful. |
| Nesting reeds ×1 / ×2 / ×3 | Move 1 and earn 1/2/3 Twigs. Quantity never changes movement or chip count. Keep these Twigs when worn out. |
| Companion duck | Add one to today's active flock; the first moves 2, second 3, later ones 4. Mud reduces this running total, without altering earlier movement. |
| Wildflowers | Move 1. Each placed Flower gives +2 Sleep if the duck settles safely at a haven. |

After everyone finishes, the largest **safe, positive active flock** gets one
Night award: +1 Sleep with one Companion or +2 total with two or more. All tied
leaders qualify. It enters Sleep before Most Rested is decided.

## White obstacles

| Obstacle | Extra nuisance, in addition to movement 1 and Exhaustion 1 |
| --- | --- |
| Fallen log | Halve the next helpful chip's movement, rounding up, minimum 1. Repeated Logs do not stack. Its other ability still works. |
| Mud puddle | Reduce today's active Companion total by 1, minimum 0. Never remove an owned chip or change earlier placements. No deferred debt if the flock is empty. |
| Loose pebbles | Lose 1 Sleep, minimum 0, only if this is the final occupied chip. |
| Brambles | Lose 1 Twig earned today, minimum 0, only if this is the final occupied chip. Earlier banked Twigs are untouched. |
| Grumpy Goose | Lower today's safe maximum to 4. Added to each bag once on Day 5. |

Splash prevents the covered obstacle's extra nuisance, including Goose's limit
reduction. It cannot prevent wear-out from the count itself and provides no
flask, rescue or rewind. Mud affects **both later Companion movement and the
Night flock contest**. A final unprotected Pebbles/Brambles penalty also applies
when worn out; a protected placement has no such penalty. See [detailed timing](ENCOUNTER_RULES.md).

## Night, the award and the ending

Bank today's printed, Reeds and earned event Twigs after any Brambles deduction.
Safe ducks retain their full Sleep; **worn-out ducks retain half, rounded down**.
The approved treatment keeps haven Feathers, Wildflowers, safe event bonuses
and the flock award safe-only. Worn-out ducks cannot win Most Rested.

Compare frozen earned Sleep among safe ducks, including their bonuses, before
spending. All tied leaders qualify; if all ducks wear out, nobody qualifies.
Nights 1–9 give each Most Rested duck **one temporary extra starting step next
Day**, represented by a passed-around [zzz tile](concepts/2026-09-14-most-rested/README.md).
Place it just beyond the updated nest/Feather trail; use duplicate display
markers for tied beneficiaries.
It never becomes a permanent Feather or stacks across Days.

The calendar gives both ducks the same nest capacity: Nights 1–3 allow one
purchase, 4–6 allow two, 7–9 allow three. Each has its own allowance. All 11
offers are available from Night 1, with unlimited stock; each Night allows one
chip per token type within that duck's 1/2/3 cap. Variants share a type, and no
Sleep carries into another Night.
Purchases change remaining Sleep, never the frozen Most Rested score.

**Day 10:** safe havens give **+2 extra Sleep**, on top of printed rewards and
other earned bonuses. **Night 10:** no shopping; every duck converts retained
Sleep at **4 Sleep → 1 Dream Twig**, rounding down. The safe Most Rested duck
(or tied ducks) also gets **1 Dream Twig** instead of a tomorrow-start reward.
Twigs determine victory. Record safe-haven Feathers as usual on Day 10, but
there is no following Day and they add no final exchange or Dream Twig value.

For final victory, compare each duck's total Twigs, including Dream Twigs. If
the totals tie, compare the frozen retained Sleep from Final Night before Sleep
conversion, including every eligible bonus and a worn-out duck's rounded-down
half. If that still ties, declare the game a draw with tied winners. This final tiebreak is
separate from the Most Rested award's safe-duck eligibility.

## M2 and M3 closure

All 43 board rewards, 11 prices, ten events, draw rhythm, shop/housekeeping
defaults, endpoint handling, final tiebreak, starting-Feather setting and local
autosave/Continue scope are approved for the rules sheet. Each Day requires at
least one draw before settling. An empty bag ends exploration only after its
final chip fully resolves. Reaching or overshooting space 43 places at 43,
fully resolves that chip including Exhaustion, then finishes with no further
placements. Local autosave/Continue is approved but unimplemented.

M2 and M3 are complete. C1–C3 are complete for the bounded Day 1 → Night 1 →
Day 2 slice; C4/C5 and Unity remain later work. No ten-Day game, Normal AI or
save/resume implementation is claimed here.
