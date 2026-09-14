# Quackies v1 board and Dream shop

14 September 2026. The user approved all 50 reward rows and 11 shop prices as
starting values. They are **not yet implemented or balanced through complete
matches**. The [overview](../RULES_AT_A_GLANCE.md) records the current mechanics;
M2 shop/housekeeping policies and payout timing are now approved. [Detailed encounters](../ENCOUNTER_RULES.md) and the
[ten approved World Events](WORLD_EVENTS.md) define the associated effects.

The board rows now follow the approved `board-layout.json` geometry authority.
Compared with the previous starting table, the changed rows are 3, 7, 10, 11,
13, 18, 19, 21, 29, 30, 32, 37 and 38; the exact synchronized values are in
the JSON and CSV below. The policy is 2/3/3 havens by biome, seven
nonendpoint haven rows whose Twig value matches both neighbours, haven Sleep
peaks at each shelter, and the endpoint exception of 21 Sleep / 9 Twigs / 2
Feathers. Shop prices and policies are unchanged.

## Board philosophy

Sleep measures comfort. Twigs reward travel and remain unchanged when a haven
interrupts a local Twig plateau. Thus a duck safely resting at haven 11 gets
10 Sleep/3 Twigs, while space 12 gives 8 Sleep/3 Twigs: the earlier duck wins
Most Rested if neither has other modifiers. Passed spaces never pay.

Wetlands occupy 1–16, meadow 17–33 and wasteland 34–50. Eight havens are split
2/3/3 across them at 3, 11, 19, 27, 29, 37, 44 and 50. Haven names below are
the semantic labels for the approved layout; the seven nonendpoint haven tiles
sit beside painted shelters and space 50 uses the oasis itself.

Ordinary wasteland spaces give 10–13 Sleep versus the later meadow's 13–14.
Wasteland havens jump to 18/20/21, making a comfortable destination valuable.
The endpoint has **21 Sleep, 9 Twigs and 2 Feathers**: just one more Sleep and
Twig than their previous printed maxima. Endpoint 50 is the explicit exception
to the neighbouring-Twig rule; the other seven havens match both neighbours.
This table refers to printed values; encounters/events can change total payouts.

Feathers below require a safe haven settle under the approved worn-out
rule. Every other space gives zero Feathers. Day 10's safe-haven +2
Sleep is **additional to** these printed values, and applies to all eight havens.

## Every space

The nest is separate and unscored. Indices are data references, not printed top
labels. Production reward strips use moon/Sleep, Twig and Feather icons.

| Space | Biome | Sleep | Twigs | Feathers if safe | Haven |
| --- | --- | ---: | ---: | ---: | --- |
| 1 | Wetlands | 3 | 1 | 0 | — |
| 2 | Wetlands | 3 | 1 | 0 | — |
| 3 | Wetlands | 6 | 1 | 1 | Reed hammock |
| 4 | Wetlands | 4 | 1 | 0 | — |
| 5 | Wetlands | 5 | 1 | 0 | — |
| 6 | Wetlands | 5 | 2 | 0 | — |
| 7 | Wetlands | 6 | 2 | 0 | — |
| 8 | Wetlands | 6 | 2 | 0 | — |
| 9 | Wetlands | 6 | 2 | 0 | — |
| 10 | Wetlands | 7 | 3 | 0 | — |
| 11 | Wetlands | 10 | 3 | 1 | Willow nest |
| 12 | Wetlands | 8 | 3 | 0 | — |
| 13 | Wetlands | 8 | 3 | 0 | — |
| 14 | Wetlands | 8 | 3 | 0 | — |
| 15 | Wetlands | 9 | 3 | 0 | — |
| 16 | Wetlands | 9 | 3 | 0 | — |
| 17 | Meadow | 10 | 3 | 0 | — |
| 18 | Meadow | 10 | 4 | 0 | — |
| 19 | Meadow | 13 | 4 | 1 | Clover hollow |
| 20 | Meadow | 11 | 4 | 0 | — |
| 21 | Meadow | 11 | 4 | 0 | — |
| 22 | Meadow | 11 | 4 | 0 | — |
| 23 | Meadow | 12 | 4 | 0 | — |
| 24 | Meadow | 12 | 4 | 0 | — |
| 25 | Meadow | 12 | 5 | 0 | — |
| 26 | Meadow | 13 | 5 | 0 | — |
| 27 | Meadow | 15 | 5 | 1 | Orchard shelter |
| 28 | Meadow | 13 | 5 | 0 | — |
| 29 | Meadow | 16 | 5 | 1 | Hayloft hideaway |
| 30 | Meadow | 14 | 5 | 0 | — |
| 31 | Meadow | 14 | 6 | 0 | — |
| 32 | Meadow | 14 | 6 | 0 | — |
| 33 | Meadow | 14 | 6 | 0 | — |
| 34 | Wasteland | 11 | 6 | 0 | — |
| 35 | Wasteland | 11 | 6 | 0 | — |
| 36 | Wasteland | 10 | 7 | 0 | — |
| 37 | Wasteland | 18 | 7 | 2 | Shaded rock nook |
| 38 | Wasteland | 10 | 7 | 0 | — |
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

[Shop data](shop.json) records the following **approved** shop policies:
all offers available from Night 1, unlimited stock for the first balance pass,
one purchase per token type per Night, subject to the shared calendar's individual
1/2/3 purchase limits. Different Tailwind or Reeds variants share a token type.
Sleep expires after that Night. Night 10 has no shopping; it converts Sleep.
These approved policies replace the earlier finite-stock suggestion. White obstacles, Goose, player ducks, Feathers and the zzz award
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
8. Rank total Twigs including Dream Twigs. For tied totals, compare frozen
   retained Night 10 Sleep before conversion, including eligible bonuses and
   worn-out halving. If that also ties, declare a draw. Do not compare conversion
   remainders, distance or Most Rested eligibility.

Examples without unrelated encounter/event modifiers:

- Safe haven 11: 10 Sleep, 3 Twigs, 1 Feather. Safe space 12: 8 Sleep, 3 Twigs.
  Haven 11 wins Most Rested. If 11 had been worn out, it keeps 5 Sleep/3 Twigs,
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

## Balance evidence and approved boundary rules

The reproducible [audit](balance-audit.py) and [results](balance-audit.json)
use exact arithmetic for bounded bag/counter comparisons. With the starting
bag, no World Event/start progress and a fixed stop-at-fifth-white policy,
mean travel is 8.640 spaces, median 9, middle half 7–10. Log is modelled, but Splash suppression of Log/Mud is omitted in this
movement comparison; Signpost information does not change the stopping decision.
Splash/Goose protection is included in the separate wear-out pressure comparison.

Under that policy, printed board Sleep makes offers costing 3–5 affordable at
every modelled opening finish, 6 affordable about 85% and 7 about 34.347%.
These are **printed-reward comparisons**, not promises after Pebbles or an
actual human stopping choice. The first haven is at 3 and is always crossed by
this fixed stop model, but no modeled opening finish lands there (0%). That is
a limitation of the audit's minimum-distance stopping assumption; gameplay can
still settle at space 3 under other stopping or preview choices.

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

Dawn Delivery is now capped at **three Feathers per duck per dawn**:
`min(3, ceil(Twig deficit / 4))`. A zero deficit gives none; 1–4 gives 1;
5–8 gives 2; **9+ gives 3**. Compare all scores before deliveries. Repeat
eligibility every dawn. Each Feather still permanently advances one step.

Under the current ten-Day rules and zero initial Feathers, before Day 10 a duck
can have at most nine earlier haven awards ×2 = 18 plus nine Dawn gifts ×3 = 27:
**45 permanent steps**, or **46** with the temporary Most Rested step. This is a
conservative upper bound, not a likely journey; early havens actually award less.
The cap therefore resolves default-start saturation. The approved shared starting
setting is 0–3 (default 0), so even the conservative maximum effective start is
49. Extra Feather sources or longer matches require a fresh bound. Unsupported
settings are rejected instead of clipping earned Feathers. The ten events add
no Feathers.

The M2 boundary rules are approved: no rewinds or separate flask; place at least
one chip before claiming route rewards; reaching/passing 50 places once at 50,
resolves the full chip and Exhaustion, then ends that duck's Day. No extra
placements or overshoot rewards. The empty bag also finishes after the last chip
fully resolves. Equal final Twigs use Night 10 retained Sleep, then a draw if
still equal. These rules are specified in the [implementation plan](../IMPLEMENTATION_PLAN.md).
M2 is complete; M3 remains paused until the user's command. No implementation
or Unity work occurs in this closure checkpoint.
