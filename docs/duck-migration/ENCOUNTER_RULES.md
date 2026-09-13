# Encounter quantities, movement and conditional obstacles

13 September 2026, following approval of the helpful colour concepts. This is
the current design discussion, superseding the powers in the
[first obstacle study](concepts/2026-09-13-obstacle-study/README.md). Ten Days,
the artwork direction and the existing Day/Dream plan remain unchanged.
No Core, CLI, Unity or asset implementation is authorized by this discussion.

## Accepted direction and proposals

The user wants Reeds x1/x2/x3 to award one/two/three Twigs while each chip still
moves one space, more than one colour capable of movement, and ordinary white
nuisances that only hurt conditionally. They propose a Goose from Day 5 that
lowers the safe Exhaustion maximum by one, potentially causing immediate wear-out.

The recommendations below specify a candidate interpretation, not tested prices
or a finalized full ruleset. They preserve the approved colour roles; movement
choices, extra denominations, exact Mud and Goose lifecycle remain proposals.

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
| Seeds: 1/2/3 kernels | Gain one pending Sleep per kernel, with the conditional food choice below. | Packed lunch: kernels instead fuel a capped movement choice after an obstacle; unused kernels have an explicitly defined reward. This changes the rule card, not the printed quantity. |
| Signpost: 1/2/3 sign panels | Preview the next N chips in order; continue or settle. No free draw, selection or reordering. | Route planner: reorder up to N previewed chips, then continue or settle; no chip is discarded. Stronger information, not extra movement. |
| Wildflowers: 1/2/3 blooms | Gain two pending Sleep per bloom if settling safely at a marked shelter. | Scent trail: blooms define a limited reach toward a nearby shelter instead of a Sleep payout; exact route reach and choice timing must be specified for that future card. |

The first Reeds variant is explicitly requested; other quantities and future
cards are options, not a requirement to stock every colour in three sizes.
Keep Splash and Companion unnumbered initially. More rescues or stacked shields
would change risk much more than another Twig. Prices, finite stock and unlock
Days still need the full rules sheet; higher grades must earn their higher cost.

For Signpost, preview only available chips. Existing preview knowledge is kept
in the same bag order: another Signpost extends that known prefix if necessary,
not a free new independent selection. Each continued draw consumes the next
known chip normally. Opponents cannot see another duck's private preview, and
AI receives only its own legally revealed information.

## Three distinct ways to gain movement

Use these modest candidate extensions in the initial ruleset:

- **Tailwind: reliable distance.** Its arrow gives total movement 2/4/6. No
  condition and no additional reward. It remains the large-distance specialist.
- **Seeds: food after a difficult step.** If the previous placed chip is an
  Obstacle, optionally eat one kernel from this Seed for +1 movement. Only the
  remaining kernels award pending Sleep. Otherwise gather every kernel for
  Sleep. This is at most one extra step, regardless of quantity, and trades
  comfort for position. No previously earned Sleep is spent.
- **Companion: company keeps you moving.** With no nuisance shield held, move
  one and arm a shield. If a shield is already held, move two total and keep the
  existing shield; do not add another. This makes a repeated Companion useful
  without turning it into a larger stock of protection.

Reeds, Signpost, Splash and the initial Wildflowers have normal movement one.
Future cards can offer other thematic movement; not every denomination needs
it. Seed/Companion precision is conditional and modest, while Tailwind reaches
farther predictably. Check whether a cheap Seed or Companion makes Tailwind →2
an unattractive purchase; frequency, cost and useful exact-rest opportunities
need play evidence rather than assuming the identities alone guarantee balance.

Determine movement and choose any food option after revealing the chip and
before placing it. Compute the final destination, place once, then resolve
nonmovement effects. Do not collect rewards or triggers from intermediate
spaces. A previewed or cancelled reveal is not a placement. The current
ordinary/World Event movement modifiers, endpoint handling and every legal
choice must be made explicit before implementation.

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
a Log. A normally one-step Seed cannot eat for an extra step through Mud, so it
keeps all its kernels for Sleep. A second Companion in Mud moves one but retains
the existing shield. A rain event's Seed movement bonus is also blocked for
that next coloured chip. Mud never suppresses Twig/Sleep/preview/rescue effects.

Repeated Logs or Muds do not accumulate. If both are pending, calculate each
restriction independently from the same proposed movement and use the lower
result; never apply one reduction to the other's result. Example: intrinsic
Tailwind four plus an event's extra two gives six before nuisances. Log alone
gives three, Mud alone four, both give three rather than two. Both clear on the
next coloured placement, even when movement remains one, or at Day's end.
World Events with other movement operations need their own explicit ordering.

A Companion shield prevents a new Log/Mud slow status from arming and is then
consumed; an already-pending duplicate does not consume it. For Pebbles/Brambles,
keep the shield while exploring and consume it only if safe settlement on that
chip would actually deduct Sleep/a newly earned Twig. Moving on or having zero
of that reward to lose does not spend the shield. A shield cannot undo an earlier
armed slow status. All unspent shields expire during Day cleanup, after settlement.
The two rest penalties cannot both apply from ordinary final-position
scoring: only one encounter occupies the duck's final resting position.
Moving beyond a Bramble/Pebble avoids its nuisance; it leaves no nightly tax.

If a Seed elects to eat despite a pending Log, its kernel is used even if the
halving removes the extra step. Show the resulting movement before confirming
the food choice; gathering remains available. Mud instead disables the extra
movement option, so the chip gathers all kernels.

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

Goose is a white Obstacle for all type-based rules, including Seeds' previous
Obstacle condition; being special does not create a separate helpful category.
The Goose replaces its old movement/ability-suppression rule. Its maximum
reduction is a special Exhaustion rule, not an ordinary nuisance: **Companion
does not block it**. When Goose resolves, it always lowers the maximum. A held
shield remains for a subsequent ordinary nuisance.

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

Review exact denominations, Seed/Companion movement, Mud semantics and Goose
lifecycle alongside the 50-row rewards, prices, worn-out payout, final Night and
World Events. Feathers remain automatic permanent +1 starts, never a currency
or a relocation to yesterday's rest. No new art or runtime work was performed
for this discussion checkpoint.
