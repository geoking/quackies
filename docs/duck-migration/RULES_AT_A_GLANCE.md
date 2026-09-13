# Quackies rules at a glance

Updated 14 September 2026. The user's latest mechanic changes are incorporated
below. Values in the [50-space board/shop proposal](v1/BOARD_AND_SHOP.md) and
[ten-card World Event deck](v1/WORLD_EVENTS.md) are **new candidates for review**.
They are not implemented or established as balanced through full games.

## The game

Play **ten Days**, initially one human versus Normal AI. Each duck has its own
bag and route progress in a shared world. Adventure by Day, then enter the
full-screen Dream/nest view at Night; View adventure returns to the board.
**Most Twigs wins**, including final Dream Twigs. A shorter game setting waits.

The incomplete nest is separate from **50 playable spaces** across wetlands,
meadow and wasteland. Score the duck's final occupied space, never the next
space or all spaces passed. Eight proposed havens are at **7, 13, 21, 27, 32,
38, 44 and 50**. They improve Sleep without increasing the local Twig plateau;
endpoint 50 is the exception, with the highest printed Sleep and Twigs by one.

## Dawn and adventure

Shuffle ten shared World Events once per game; reveal one each dawn without
replacement. Its effect lasts that Day. Days 1–10 therefore use all ten cards.

At dawn the stork delivers **one Feather for every four Twigs behind the
leader, rounding up**: 1–4 behind gives 1, 5–8 gives 2, 9–12 gives 3, and so on.
A tied leader gets none. Snapshot all Twig scores before delivery. Each Feather
permanently advances every later start by exactly one; it is never spent,
converted or redeemed through a cap. This uncapped formula replaces the old
thresholds. Start from the updated permanent trail plus any temporary Most
Rested step, not yesterday's resting place. Trail saturation at 50 remains open.

The opening bag has **13 chips**: two each of Log, Mud, Pebbles and Brambles;
two Seeds; one Tailwind →2; one Signpost; one Splash. Each white moves one and
adds one Exhaustion. **Five is normally safe; six wears the duck out.** Once on
Day 5, add one Grumpy Goose to each bag, where it remains for the rest of the game.
An unprotected Goose adds one Exhaustion and lowers today's safe maximum to
four before checking safety; five now wears the duck out. Reset the maximum
and other temporary state next dawn. Owned chips return to the bag each Day.

## Helpful encounters

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
rescue/rewind. Mud is interpreted to affect **both later Companion movement and
the Night flock contest**. A Pebbles/Brambles final-chip penalty also applies
when worn out under the proposed timing interpretation; a protected placement
has no such penalty. See [detailed timing](ENCOUNTER_RULES.md).

## Night, the award and the ending

Bank today's printed, Reeds and earned event Twigs after any Brambles deduction.
Safe ducks retain their full Sleep; **worn-out ducks retain half, rounded down**.
The proposed treatment keeps haven Feathers, Wildflowers, safe event bonuses
and the flock award safe-only. Worn-out ducks cannot win Most Rested.

Compare frozen earned Sleep among safe ducks, including their bonuses, before
spending. All tied leaders qualify; if all ducks wear out, nobody qualifies.
Nights 1–9 give each Most Rested duck **one temporary extra starting step next
Day**, represented by a passed-around [zzz tile](concepts/2026-09-14-most-rested/README.md).
Place it just beyond the updated nest/Feather trail; use duplicate display
markers for tied beneficiaries.
It never becomes a permanent Feather or stacks across Days.

The calendar gives both ducks the same nest capacity: Nights 1–3 allow one
purchase, 4–6 allow two, 7–9 allow three. Each has its own allowance. Proposed
shop policy is one chip per family per Night, all offers available when
affordable, unlimited initial stock, and no Sleep carried into another Night.
Purchases change remaining Sleep, never the frozen Most Rested score.

**Day 10:** safe havens give **+2 extra Sleep**, on top of printed rewards and
other earned bonuses. **Night 10:** no shopping; every duck converts retained
Sleep at **4 Sleep → 1 Dream Twig**, rounding down. The safe Most Rested duck
(or tied ducks) also gets **1 Dream Twig** instead of a tomorrow-start reward.
Twigs determine victory. Record safe-haven Feathers as usual on Day 10, but
there is no following Day and they add no final exchange or Dream Twig value.

## Still requiring review before implementation

The full 50-row table, all 11 prices and ten events now exist as reviewable data.
Remaining decisions are the proposed stock/housekeeping rules, final victory
ties, no-draw/empty-bag/overshoot handling, and especially what happens when
uncapped permanent Feathers reach or exceed the finite route. None may silently
break the one-Feather/one-step rule. Confirm interpretations and run full-match
balance work after the contract is accepted. No Core/CLI or Unity change is
part of this checkpoint; the existing engine will be evolved, not restarted.
