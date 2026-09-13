# Encounter quantities, movement and conditional obstacles

13 September 2026, updated for plain Seeds and the proposed Companion flock.
This is the current design discussion, superseding the powers in the
[first obstacle study](concepts/2026-09-13-obstacle-study/README.md). Ten Days,
the artwork direction and the existing Day/Dream plan remain unchanged.
No Core, CLI, Unity or asset implementation is authorized by this discussion.

## Accepted direction and proposals

The user wants Reeds x1/x2/x3 to award one/two/three Twigs while each chip still
moves one space, more than one colour capable of movement, and ordinary white
nuisances that only hurt conditionally. They propose a Goose from Day 5 that
lowers the safe Exhaustion maximum by one, potentially causing immediate wear-out.

The latest direction removes both Seed movement choices and Seed Sleep rewards:
Seeds are cheap, non-Exhaustion chips with normal movement one and no ability.
The user proposes Companion interaction through a highest-count Night reward
and successive values 2/3/4. This document interprets those values as the movement
of successive Companions placed that Day, with a separate small Sleep payout.

The recommendations below specify a candidate interpretation, not tested prices
or a finalized full ruleset. They preserve the approved colour roles; movement
extensions, extra denominations, exact Mud and Goose lifecycle remain proposals.

## Numbers describe quantities, not universal strength

Keep movement, illustrated quantity and ability text separate. A Reeds x3 is
one physical encounter carrying three bundles: one placement, one trigger and
one normal step. It does not count as three drawn chips or multiply World Event
triggers. Each bundle grants one pending Twig in the initial ruleset, paid on a
safe rest. External named movement modifiers may still apply; the printed three
never sets the movement distance.

Put the small quantity beside a category symbol, such as a reed bundle x3. The
rule reference says what a bundle does in this match. If the face instead says
Twig +3, it promises that reward and is less reusable for future effect sets.
Reserve a forward arrow beside a number for movement: Tailwind →4 is four total steps before
modifiers. Ordinary movement one remains unprinted. Avoid an unexplained x3
badge that looks like a universal multiplier.

Select one rule card per colour for the whole match, visible to both ducks.
Future sets may change what quantities do, but never silently change a printed
movement instruction or make Reeds x3 three chips. Do not swap meanings during
a match. Initial play uses one fixed ruleset; customization is later work.

| Family and quantity | Initial candidate | Possible future rule card, not simultaneous |
| --- | --- | --- |
| Reeds: 1/2/3 bundles | Move one; gain one pending Twig per bundle. | Weaving: pool bundles drawn that Day; complete groups of three give an additional small comfort bonus on a safe rest. A three-bundle chip completes one group by itself. |
| Signpost: 1/2/3 sign panels | Preview the next N chips in order; continue or settle. The movement proposal below uses two steps and starts with N=1; larger previews are later options. No free draw, selection or reordering. | Route planner: reorder up to N previewed chips, then continue or settle; no chip is discarded. Stronger information, not extra movement. |
| Wildflowers: 1/2/3 blooms | Gain two pending Sleep per bloom if settling safely at a marked shelter. | Scent trail: blooms define a limited reach toward a nearby shelter instead of a Sleep payout; exact route reach and choice timing must be specified for that future card. |

The first Reeds variant is explicitly requested; other quantities and future
cards are options, not a requirement to stock every colour in three sizes.
Seeds need no denominations or quantity subsystem. Keep Splash and Companion
unnumbered initially: Splash has one rescue per Day, while Companion's movement
depends on its place in that Day's flock, not a purchased strength. Prices, finite stock and unlock
Days still need the full rules sheet; higher grades must earn their higher cost.

For Signpost, preview only available chips. Existing preview knowledge is kept
in the same bag order: another Signpost extends that known prefix if necessary,
not a free new independent selection. Each continued draw consumes the next
known chip normally. Opponents cannot see another duck's private preview, and
AI receives only its own legally revealed information.

## Simple Seeds and distinct movement roles

- **Seeds: cheap safe steps.** Move one, add no Exhaustion and have no ability.
  They dilute the obstacle share of the bag. They do not cancel Exhaustion already
  gained or make later draws automatically safe. Remove the prior food choice,
  kernel denominations and pending Sleep reward entirely.
- **Tailwind: reliable distance.** Its arrow gives total movement 2/4/6. No
  condition and no additional reward. It remains the large-distance specialist.
- **Companion: travel as a flock.** Successive Companions placed that Day move
  2, then 3, then 4 each for the third and every later Companion. The Night flock
  comparison below adds player interaction. This replaces the old shield rule;
  Companions do not also protect from nuisance effects.
- **Signpost: a suggested steady utility mover.** Move two and preview the next
  chip. This is the recommended additional movement-plus-small-benefit option,
  replacing its former normal one step if accepted. Existing suggestions for
  stronger preview quantities can remain later stock options; they do not raise
  this proposed base movement. Start with one preview and an explicit →2.

Reeds and Splash keep normal movement one. Wildflowers retains its initial
one-step shelter-comfort role. A later alternative could allow it to move two
instead of one only when the two-step destination is a marked shelter; specify
that as one optional step before placement, not a chain of shelter jumps. Do not
automatically add this option alongside every other movement proposal.

Keep Seeds cheapest. Tailwind →2 needs a price or availability advantage over
Signpost →2 plus a preview and over the first Companion. Higher-grade Tailwinds
offer reliable distance without needing repeated draws. Prices and quantities
are not settled by these roles; cheaper, slower options must remain worthwhile
under the Night purchase cap.

Determine final movement before placing each chip, place it once, then resolve
nonmovement effects. Do not collect rewards or triggers from intermediate
spaces. Previewed or cancelled encounters are not placements. Ordinary/World
Event modifier order, endpoint handling and every legal choice need the full
rules sheet. Removing Seed Sleep also means the early board rewards and Dream
prices must fund an enjoyable first purchase without relying on the old subsidy.

## Companion flock — candidate rule to test

Count only each duck's actually placed Companions this Day, not all encounters,
all Companions owned, or Companions drawn by other players. Reset the count at
dawn. They need not be consecutive; an intervening white or other colour does
not reset it. Each physical Companion counts once, even if Mud or Log reduces
its movement. Do not move earlier Companion placements again as the flock grows.

| Companion placed this Day | Total movement before modifiers | Total distance from all Companions so far |
| --- | --- | --- |
| First | 2 | 2 |
| Second | 3 | 5 |
| Third | 4 | 9 |
| Fourth | 4 | 13 |

Its own movement is normal one plus a flock bonus of one/two/three, capped at
three bonus steps. This makes the 2/3/4 progression compatible with Mud removing
bonus movement; Mud reduces it to one, while Log rounds halved total movement
up. Separate World Event movement must follow the shared modifier policy.
The component can show the next Companion's current movement in the live rule
reference rather than printing a fixed 2/3/4 denomination on every chip.

After every duck has ended the adventure, compare Companion counts among ducks
who settled safely. A worn-out duck is ineligible and does not block another
duck's reward. The highest positive count wins; tied eligible leaders all receive
the same reward. If every eligible count is zero, or no duck is safe, nobody earns it.

- One Companion in the largest safe flock: **+1 Sleep**.
- Two or more Companions in the largest safe flock: **+2 Sleep total**.
- Other ducks: no flock bonus. It is never paid once per Companion or per rival.

This min(count, 2) payout is the lead's bounded recommendation, not an accepted
balance value. It retains a count-dependent reward without also scaling the
Night payout to four or indefinitely. Each additional placed Companion still
contributes its capped four-step movement and helps win the count comparison.
Owning more copies increases the chance of drawing a flock; resolving a chip
does not itself improve the odds of later draws.

Apply the flock award to earned and available Sleep before freezing earned
Sleep and comparing Most Rested. It is spendable that Night and may change the
Most Rested result, which can grant tomorrow's temporary starting step. Do not
freeze Sleep first and then change that supposedly frozen comparison value.
For ordinary Sleep accounting, sum board Sleep, eligible encounter bonuses and
the flock award, apply the resting-chip Sleep deduction if any, then clamp the
total at zero once before freezing it. World Events must have explicit ordering
within this pre-freeze resolution.

Three Companions move nine raw spaces, versus six for three Tailwind →2 chips
and twelve for three Tailwind →4 chips. The escalating movement plus Sleep can
make black disproportionately attractive if equally cheap/common. Losing the
old shield, bounding the Night award, and pricing Companion above simple →2
movement are important starting constraints, not proof of balance. Test whether
one player can buy more Companions, win extra Sleep, and repeatedly finance the
next Companion and Most Rested advantage. If necessary, reduce the award to a
flat +1 before increasing other families' complexity to compensate.

Show each duck's placed Companion count publicly during exploration, together
with safe/worn status. The winner is not known until all ducks finish. This is
a shared race for a reward, not direct removal of another duck's resources.
For the initial human/AI match compare the two ducks once. A future multiplayer
Neighbourhood rule card could compare only distinct adjacent players; do not
double-count the same opponent as both left and right in a two-player game or
combine per-neighbour prizes with this all-player majority payout.

## Ordinary obstacles: conditional nuisances

Each resolved white still normally moves one and adds one Exhaustion. The
Exhaustion risk itself always applies. Only the additional nuisance is
conditional; ordinary obstacles do not impose automatic payout deductions.

| White | Current candidate nuisance |
| --- | --- |
| Fallen log | Halve the next coloured chip's movement, rounding up, minimum one. A one-step chip is unaffected; nonmovement ability still works. |
| Mud puddle | The next coloured chip cannot gain bonus movement from abilities or World Events. Its intrinsic movement and other powers still work. |
| Loose pebbles | Lose one Sleep only if this chip is the safe final resting place; floor the final Sleep total at zero. |
| Brambles | Lose one newly earned Twig only if this chip is the safe final resting place. Lose nothing if no Twig was earned today; earlier nest Twigs are untouched. |

Mud differs from Log: Tailwind →4 still moves four through Mud, but two through
a Log. A Companion's flock movement bonus is blocked, leaving one step, while
the chip still counts for the Day's flock. A rain event's Seed movement bonus
is also blocked for that next coloured chip. The proposed Signpost's intrinsic
→2 remains two and its preview still works. Mud never suppresses Twig/Sleep,
preview or rescue effects; an ordinary unmodified Seed is unaffected.

Repeated Logs or Muds do not accumulate. If both are pending, calculate each
restriction independently from the same proposed movement and use the lower
result; never apply one reduction to the other's result. Example: intrinsic
Tailwind four plus an event's extra two gives six before nuisances. Log alone
gives three, Mud alone four, both give three rather than two. Both clear on the
next coloured placement, even when movement remains one, or at Day's end.
World Events with other movement operations need their own explicit ordering.

The previous Companion nuisance shield is superseded, not retained as an extra
flock benefit. The two rest penalties cannot both apply from ordinary final-position
scoring: only one encounter occupies the duck's final resting position.
Moving beyond a Bramble/Pebble avoids its nuisance; it leaves no nightly tax.

Show the short icon key at the board edge and any armed slow effect beside the
draw control. No paragraphs or default movement numbers belong on chip faces.

## Grumpy Goose from Day 5

Candidate matching the user's introduction: add exactly one Goose to each
duck's owned bag during Day 5 preparation, before shuffling/drawing. It remains
in the inventory for Days 5–10; do not add another each dawn. This changes the
initial eight-white count to nine, before any other explicitly defined changes.
Announce the addition and the rule to both players before they choose to draw.

On a resolved Goose, add its one Exhaustion and set that Day's safe maximum
from five to four, then check safety. The maximum stays four for the rest of
that Day, resets to five next dawn and changes again only if Goose is drawn.
The reduction is not a permanent, cumulative loss across Days.

| Before Goose | After its one Exhaustion | New safe maximum | Result |
| --- | --- | --- | --- |
| 2 | 3 | 4 | Safe |
| 3 | 4 | 4 | Safe, but any further white causes wear-out |
| 4 | 5 | 4 | Worn out immediately |

Goose is a white Obstacle for all type-based rules; being special does not
create a separate helpful category. Plain Seeds have no previous-Obstacle condition.
The Goose replaces its old movement/ability-suppression rule. Its maximum
reduction is a special Exhaustion rule, not an ordinary nuisance: **Companion
does not block it**. When Goose resolves, it always lowers the maximum. The
Companion flock proposal has no shield mechanic.

Update Splash from “cancel the sixth white” to **cancel a draw that would leave
Exhaustion above the resulting safe maximum**. This covers a Goose that would
cause wear-out and a later ordinary white when Goose has already lowered the
maximum. With a rescue armed, evaluate that consequence before committing the
draw: cancel the whole encounter, then automatically settle safely at the last
occupied space. A cancelled Goose adds no Exhaustion and no maximum change;
an already-resolved Goose is not undone. No rescue permits another draw after
that safe settlement. Signpost preview alone never activates Goose.

Without a rescue, commit the Goose's placement, Exhaustion and maximum, then
end the adventure if worn out. Do not resolve any later drawn chip's effects.
Base worn-out payout remains an open rules-sheet decision; these changes do
not silently restore the old choose-coins-or-points rule.

## Risk evidence and open decisions

An independent effect-free enumeration checked eight continued draws, omitting
all powers, purchases, stopping decisions and permanent trail progress:

| Comparison bag | Worn-out by draw eight | Exact fraction |
| --- | --- | --- |
| Earlier eight ordinary whites + five colours, no Goose | 24.94% | 107/429 |
| Add Goose: eight ordinary + one Goose + five colours | 57.58% | 19/33 |
| Replace one: seven ordinary + one Goose + five colours | 52.14% | 61/117 |

These are deliberately controlled pressure comparisons, not forecasts for
actual Day 5 bags, which should contain purchased colours, or player loss rates.
For the Goose rows, wear-out means at least six whites without Goose, or at
least five whites including Goose. Enumerating all eight-chip subsets and Goose
positions agrees with the exact combinations (3,003 subsets for addition and
1,287 for replacement). This analysis changes no game code and does not certify
the chosen values as balanced.

The Goose is a meaningful difficulty increase even if it replaces a white.
Begin with the user's one-Goose addition as the candidate; replacing a regular
white is the gentler fallback if the Day 5 spike overwhelms purchased powers.
Test actual growing bags and stop behavior before choosing that fallback.
Show both current Exhaustion and the current maximum prominently; at four
Exhaustion with an undrawn Goose, the next draw is no longer guaranteed safe.

Review exact denominations, Companion flock and Signpost movement, Mud semantics and Goose
lifecycle alongside the 50-row rewards, prices, worn-out payout, final Night and
World Events. Feathers remain automatic permanent +1 starts, never a currency
or a relocation to yesterday's rest. No new art or runtime work was performed
for this discussion checkpoint.
