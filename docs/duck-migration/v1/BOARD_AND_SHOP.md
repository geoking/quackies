# Proposed v1 board and Dream shop

14 September 2026. Complete initial numeric proposal for review, **not yet
implemented or balanced through complete matches**. The user's requested
mechanics are in [the overview](../RULES_AT_A_GLANCE.md); this document supplies
candidate numbers. [Detailed encounters](../ENCOUNTER_RULES.md) and the
[ten proposed World Events](WORLD_EVENTS.md) define the associated effects.

## Board philosophy

Sleep measures comfort. Twigs reward travel and remain unchanged when a haven
interrupts a local Twig plateau. Thus a duck safely resting at haven 13 gets
10 Sleep/3 Twigs, while space 14 gives 8 Sleep/3 Twigs: the earlier duck wins
Most Rested if neither has other modifiers. Passed spaces never pay.

Wetlands occupy 1–16, meadow 17–33 and wasteland 34–50. Eight havens are split
2/3/3 across them. Haven names below are provisional visual names, not new art
or verified coordinates. Fit these semantic spaces to the approved board later.

Ordinary wasteland spaces give 10–13 Sleep versus the later meadow's 13–14.
Wasteland havens jump to 18/20/21, making a comfortable destination valuable.
The endpoint has **21 Sleep, 9 Twigs and 2 Feathers**: just one more Sleep and
Twig than their previous printed maxima. Endpoint 50 is the explicit exception
to the neighbouring-Twig rule; the other seven havens match both neighbours.
This table refers to printed values; encounters/events can change total payouts.

Feathers below require a safe haven settle under the proposed worn-out
interpretation. Every other space gives zero Feathers. Day 10's safe-haven +2
Sleep is **additional to** these printed values, and applies to all eight havens.

## Every space

The nest is separate and unscored. Indices are data references, not printed top
labels. Production reward strips use moon/Sleep, Twig and Feather icons.

| Space | Biome | Sleep | Twigs | Feathers if safe | Haven |
| --- | --- | ---: | ---: | ---: | --- |
| 1 | Wetlands | 3 | 1 | 0 | — |
| 2 | Wetlands | 3 | 1 | 0 | — |
| 3 | Wetlands | 4 | 1 | 0 | — |
| 4 | Wetlands | 4 | 1 | 0 | — |
| 5 | Wetlands | 5 | 1 | 0 | — |
| 6 | Wetlands | 5 | 2 | 0 | — |
| 7 | Wetlands | 8 | 2 | 1 | Reed hammock |
| 8 | Wetlands | 6 | 2 | 0 | — |
| 9 | Wetlands | 6 | 2 | 0 | — |
| 10 | Wetlands | 7 | 2 | 0 | — |
| 11 | Wetlands | 7 | 3 | 0 | — |
| 12 | Wetlands | 8 | 3 | 0 | — |
| 13 | Wetlands | 10 | 3 | 1 | Willow nest |
| 14 | Wetlands | 8 | 3 | 0 | — |
| 15 | Wetlands | 9 | 3 | 0 | — |
| 16 | Wetlands | 9 | 3 | 0 | — |
| 17 | Meadow | 10 | 3 | 0 | — |
| 18 | Meadow | 10 | 3 | 0 | — |
| 19 | Meadow | 11 | 4 | 0 | — |
| 20 | Meadow | 11 | 4 | 0 | — |
| 21 | Meadow | 13 | 4 | 1 | Clover hollow |
| 22 | Meadow | 11 | 4 | 0 | — |
| 23 | Meadow | 12 | 4 | 0 | — |
| 24 | Meadow | 12 | 4 | 0 | — |
| 25 | Meadow | 12 | 5 | 0 | — |
| 26 | Meadow | 13 | 5 | 0 | — |
| 27 | Meadow | 15 | 5 | 1 | Orchard shelter |
| 28 | Meadow | 13 | 5 | 0 | — |
| 29 | Meadow | 14 | 5 | 0 | — |
| 30 | Meadow | 14 | 6 | 0 | — |
| 31 | Meadow | 14 | 6 | 0 | — |
| 32 | Meadow | 16 | 6 | 1 | Hayloft hideaway |
| 33 | Meadow | 14 | 6 | 0 | — |
| 34 | Wasteland | 11 | 6 | 0 | — |
| 35 | Wasteland | 11 | 6 | 0 | — |
| 36 | Wasteland | 10 | 7 | 0 | — |
| 37 | Wasteland | 10 | 7 | 0 | — |
| 38 | Wasteland | 18 | 7 | 2 | Shaded rock nook |
| 39 | Wasteland | 10 | 7 | 0 | — |
| 40 | Wasteland | 11 | 7 | 0 | — |
| 41 | Wasteland | 11 | 7 | 0 | — |
| 42 | Wasteland | 11 | 8 | 0 | — |
| 43 | Wasteland | 12 | 8 | 0 | — |
| 44 | Wasteland | 20 | 8 | 2 | Spring-fed refuge |
| 45 | Wasteland | 11 | 8 | 0 | — |
| 46 | Wasteland | 12 | 8 | 0 | — |
| 47 | Wasteland | 12 | 8 | 0 | — |
| 48 | Wasteland | 13 | 8 | 0 | — |
| 49 | Wasteland | 13 | 8 | 0 | — |
| 50 | Wasteland | 21 | 9 | 2 | Oasis sanctuary |

Editable data: [CSV](board.csv) and [JSON](board.json), with identical rows.

## Every shop offer

Prices are Sleep and buy **one owned chip**, not one use. Reeds quantity changes
Twig yield; it never changes movement or how many chips are purchased.

| Offer | Sleep price |
| --- | ---: |
| Seeds | 3 |
| Tailwind →2 | 5 |
| Tailwind →4 | 10 |
| Tailwind →6 | 15 |
| Signpost →2 / preview 1 | 7 |
| Refreshing splash | 4 |
| Nesting reeds ×1 | 6 |
| Nesting reeds ×2 | 11 |
| Nesting reeds ×3 | 16 |
| Companion duck | 7 |
| Wildflowers | 5 |

[Shop data](shop.json) also labels the following **proposed** shop policies:
all offers available from Night 1, unlimited stock for the first balance pass,
one purchase per family per Night, subject to the shared calendar's individual
1/2/3 purchase limits. Different Tailwind or Reeds variants share a family.
Sleep expires after that Night. Night 10 has no shopping; it converts Sleep.
These policies replace the previously unconfirmed finite-stock suggestion only
if accepted. White obstacles, Goose, player ducks, Feathers and the zzz award
are not shop offers. Reeds are not retroactively upgraded: each variant is a
separate owned chip and pays only when placed on a later Day.

Cheap Seeds support early bag improvement. Tailwind's 5/10/15 prices keep the
measured movement gained per Sleep similar. Signpost's extra cost buys private
information. Companion costs 7 because Mud reduces its active flock and Night
award; Splash costs 4 for its narrow immediate-next-chip protection. Reeds cost
more because their Twigs directly contribute to victory and now survive wear-out.
Wildflowers are inexpensive but require an exact safe haven finish.

## Night resolution and examples

1. Freeze the final occupied space and whether the duck is worn out.
2. Add printed Sleep, already-triggered event Sleep, and any earned safe-only
   event/Flower bonuses. A safe haven on Day 10 adds another 2 Sleep.
3. After everyone finishes, safe positive active-flock leaders receive the
   Companion award: +1 Sleep for one Companion, +2 total for two or more.
4. Apply an unsuppressed final Pebbles deduction to earned Sleep, floor 0.
   Safe ducks retain this Sleep. Worn-out ducks retain half rounded down, have
   no safe-only bonuses and cannot win Most Rested. Freeze retained earned Sleep.
5. Keep printed, Reeds and earned event Twigs, even when worn out. Apply an
   unsuppressed final Brambles deduction to today's earned Twigs only, floor 0.
   Award listed haven Feathers only to safe ducks. They never move today's rest.
6. Compare frozen Sleep among safe ducks; all tied leaders are Most Rested.
   Nights 1–9 give each a temporary +1 start tomorrow. Purchases spend remaining
   Sleep without changing the comparison. Refill owned bags for the next Day.
7. Night 10 instead converts every duck's retained Sleep to `floor(Sleep / 4)`
   Dream Twigs. Each safe Most Rested duck also gains **1 Dream Twig**. No extra
   +1 Sleep, no Day 11 start, no shopping and no final Feather-to-Twig conversion.
   Dream Twigs are ordinary victory Twigs displayed as a distinct final gain.
   Record safe-haven Feathers as usual on Day 10, but they affect no remaining
   Day and add nothing to the final score.

Examples without unrelated encounter/event modifiers:

- Safe haven 13: 10 Sleep, 3 Twigs, 1 Feather. Safe space 14: 8 Sleep, 3 Twigs.
  Haven 13 wins Most Rested. If 13 had been worn out, it keeps 5 Sleep/3 Twigs,
  gets no Feather and cannot win that award.
- Worn out at 48: `floor(13/2) = 6` Sleep and 8 Twigs. If a placed Reeds ×2
  also earned 2 Twigs, retain 10 Twigs. A final unsuppressed Brambles instead
  reduces today's Twig total by one; it never takes earlier banked Twigs.
- Final safe haven 44: `20 + 2 = 22` Sleep → 5 Dream Twigs, plus 8 printed
  Twigs. If also Most Rested, the final gain is **14 Twigs** (8 + 5 + 1).
- Final safe endpoint 50: `21 + 2 = 23` Sleep → 5 Dream Twigs, plus 9 printed
  Twigs. If also Most Rested, the final gain is **15 Twigs**. Without further
  modifiers, endpoint wins the Sleep comparison over haven 44 by one.
- Final worn-out endpoint: no safe-haven +2; `floor(21/2)=10` Sleep → 2 Dream
  Twigs, plus 9 printed Twigs = 11. No Most Rested award or haven Feathers.

## Balance evidence and remaining decisions

The reproducible [audit](balance-audit.py) and [results](balance-audit.json)
use exact arithmetic for bounded bag/counter comparisons. With the starting
bag, no World Event/start progress and a fixed stop-at-fifth-white policy,
mean travel is 8.640 spaces, median 9, middle half 7–10. Log is modelled, but Splash suppression of Log/Mud is omitted in this
movement comparison; Signpost information does not change the stopping decision.
Splash/Goose protection is included in the separate wear-out pressure comparison.

Under that policy, printed board Sleep makes offers costing 3–5 affordable at
every modelled opening finish, 6 affordable about 85% and 7 about 49%.
These are **printed-reward comparisons**, not promises after Pebbles or an
actual human stopping choice. The first haven is reachable at 7, but the
model's exact final landing there is only about 14%; crossing it does not pay.

The controlled eight-draw wear-out comparison rises from about 25% with the
opening bag to about 56% with an added Goose and no purchased chips. Adding
three helpful chips brings the latter to about 26%. This is draw pressure,
not an observed Day 5 loss rate: actual players can stop and use previews.
It supports allowing affordable bag improvement before Day 5, not compulsory
buying or a claim that the Goose has been balanced.

The data does not simulate ten-Day purchasing, opponents, safe haven targeting,
Most Rested, World Event order or permanent Feather feedback. Full retained
Twigs can make risky travel especially attractive; safe-only Feathers, Flowers,
flock bonuses and Most Rested must be assessed alongside half-Sleep. Large
wasteland Sleep jumps and Reeds investment also need complete-match comparison.

Dawn Delivery is **uncapped** `ceil(Twig deficit / 4)`: 0 gives 0; 1–4 gives 1;
5–8 gives 2; 9–12 gives 3; and so on. Compare all scores before deliveries.
Repeated equal deficits pay again each dawn and give permanent progress, so
this may eventually exceed the 50-space route. The endpoint and start-saturation
contract remains unresolved; do not silently cap, discard, bank or convert
Feathers. This is a real specification boundary, not evidence the proposed
catch-up formula fails in ordinary play.

Suggested housekeeping, still for review: no rewinds or separate flask in v1;
require a placed chip before claiming route rewards; when a draw reaches/passes
50, place once at 50, resolve the full chip and Exhaustion, then finish the Day
with its safe/worn outcome. This does **not** solve starts already at/beyond 50.
Empty-bag finish and tied final Twigs can respectively use automatic settling
and shared victory. Confirm these policies, stock rules and the numeric proposal
before the Core milestone. No implementation or Unity work is authorized here.
