# Fresh encounters and obstacle study

13 September 2026. The user approved the previous token art style and requested
a fresh rules design rather than inherited Quacks effects. Ten Days is now the
standard direction; shorter match lengths are deferred. This study proposes
rules to review, not a complete or balance-certified implementation.

## Updated images

- [Ordinary encounters](ordinary-encounters.png): Seeds, Signpost, Refreshing
  splash and Nesting reeds, with no generic strength badges.
- [Tailwind and companions](tailwind-and-companions.png): Companion duck and
  Wildflowers without badges; Tailwind shows total movement →2, →4 or →6.
- [White obstacles](obstacle-variants.png): Fallen log, Mud puddle, Loose pebbles,
  Brambles and optional Grumpy goose.

The approved shapes/colours/cardboard style remain the basis. Default movement
is one space and is not printed. Tailwind's number is an explicit instruction,
not a universal chip strength: →4 means move four in total, not one plus four.
Other upgraded movement chips should exist only where the story warrants them;
the first test needs only Tailwind as the movement specialist.

## Proposed starting bag and risk

Start with 13 encounters: **8 Obstacles and 5 coloured chips**.

- Obstacles: two Logs, two Mud puddles, two Loose pebbles and two Brambles.
- Coloured: two Seeds, one Tailwind →2, one Signpost and one Refreshing splash.
- The Grumpy goose is an optional later test, not an extra starting obstacle.

Working interpretation: each resolved Obstacle moves one space and adds one
Exhaustion. Five Exhaustion is safe; the sixth makes the duck worn out. A
prevented/cancelled reveal has not resolved and adds neither movement nor
Exhaustion. Helpful movement never changes an Obstacle's Exhaustion value.

The [exact bag audit](bag-analysis.json), reproduced by
[bag-analysis.py](bag-analysis.py), checks every obstacle/colour draw order:

| Bag | Mean placements when stopping on obstacle five | Mean coloured placements | Sixth obstacle by draw eight if continuing |
| --- | --- | --- | --- |
| 8 Obstacles + 5 colours | 7.78 | 2.78 | 24.94% |
| 8 Obstacles + 7 colours | 8.89 | 3.89 | 10.02% |
| 6 Obstacles + 5 colours | 8.57 | 3.57 | 6.06% |

For 8+5, the sixth-obstacle probability by draw ten is 80.42% if the player
continues. That is not an 80% game loss rate: players can stop, and the proposed
powers below are excluded. A one-Tailwind→2 opening bag has a mean raw distance
of 8.33 when stopping on obstacle five, excluding all other abilities/penalties.

This is enough to justify a first prototype, not to call it balanced. The
50-space table needs early rewarding stops, affordable Dream options and later
progress from stronger Tailwinds and Feather trails. Test whether players see
too few fun coloured effects; 8+7 is a gentler alternative if needed.

## Candidate helpful effects

All ordinary tokens move one before their text resolves. Red replaces this
movement with its printed total. No helpful effect awards permanent Feathers;
existing haven/Dawn sources remain governed by the overall plan.

| Token | Proposed rule | Role |
| --- | --- | --- |
| Seeds | Each resolved Seed adds 1 pending Sleep for tonight, paid on a safe settle. | Simple, inexpensive Dream funding |
| Tailwind | Move exactly 2, 4 or 6 before applicable movement modifiers. No additional ability. | Reach distant spaces and risk skipping a good rest |
| Signpost | Reveal the next encounter privately without resolving it. Choose to settle or continue with that exact encounter. | Information for a stop/draw decision |
| Refreshing splash | Arm one rescue. If a later obstacle would be the sixth, cancel that reveal and automatically settle safely at the previous occupied space. Once per Day. | A bounded safety net |
| Nesting reeds | Each resolved Reeds adds 1 pending Twig, banked on a safe settle. | Direct nest-building |
| Companion duck | Arm one shield against the next applicable Obstacle nuisance; its movement and Exhaustion still apply. One shield held at a time. | Protection from inconveniences, not from exhaustion |
| Wildflowers | Each resolved Wildflowers adds 2 pending Sleep if the duck settles safely at a marked shelter. | Comfort-focused shelter play |

Seed/Reeds/Wildflower rewards are deliberately different strategies; future
prices must reflect their value. Duplicate ordinary tokens remain useful.
Additional Splashes improve the chance of finding a rescue but never grant
more than one rescue in a Day. A later Companion can re-arm a spent nuisance
shield; unspent shields cannot stack.

The Signpost preview does not grant a free placement or let the player put back
a bad result and redraw. Continuing must resolve the previewed chip; settling
returns it during the normal end-of-Day cleanup. With no remaining chip, there
is no preview. Chained Signposts each require a normal explicit continue decision.

A Splash cannot undo an already-resolved worn-out result or revive the duck
after stopping. A cancelled sixth obstacle is set aside until normal cleanup;
it is not placed and its nuisance does not trigger. Without a rescue, the sixth
Obstacle is placed normally and fixes the worn-out resting position; no seventh
draw follows. Exhaustion itself is never cancelled by the Companion.

Seeds, Reeds and Wildflower bonuses are pending until a safe settle. The base
board payout on a worn-out ending still requires the full rules-sheet decision;
do not inherit the old choose-score-or-coins rule accidentally.

## Candidate obstacle effects

Each variant adds the same one Exhaustion. Only its modest extra inconvenience
differs. No obstacle removes existing Feathers or already-banked nest Twigs.

| Obstacle | Extra inconvenience |
| --- | --- |
| Fallen log | Halve the next coloured token's movement, round up, minimum one. Its nonmovement ability still works. |
| Mud puddle | Lose 1 Sleep from tonight's earned total, minimum zero. Mud applies at most once per Day. |
| Loose pebbles | If you settle safely on this chip, lose 1 Sleep, minimum zero. Moving past it avoids the nuisance. |
| Brambles | Lose 1 of today's newly earned Twigs, minimum zero. At most once per Day; earlier Days' nest score is untouched. |
| Grumpy goose — optional | The next coloured token moves only one and its nonmovement ability is skipped. Still only one Exhaustion from the Goose. |

The Log often leaves an ordinary one-space chip unaffected, which is intentional:
the obstacle counter supplies the risk and the extra annoyance is situational.
It changes Tailwind→6 to 3, →4 to 2 and →2 to 1. Halving a generic “effect” would
be ambiguous for previews, protection or choices, so only movement is halved.

Keep one pending next-colour nuisance, not a stack: repeated Logs do not
multiply. The optional Goose takes precedence over a Log; neither stacks with it.
The next coloured token consumes the pending nuisance even if its minimum-one
movement means no loss. White draws do not consume it. Unused nuisances expire
at Day's end. Mud and Brambles are separate once-per-Day penalty flags, each
applied to its named reward pool before the result is frozen. Pebbles applies
only when that unsuppressed chip is the safe final resting position; earlier
Pebbles do not accumulate penalties. Its one-point loss can combine with Mud,
with the final Sleep total floored at zero.

A Companion shield consumes itself only when the next Obstacle's nuisance
would arm/change a status or apply a new penalty. An already-capped duplicate
Mud/Brambles or redundant pending Log does not waste it. A blocked Mud/Brambles
does not use that nuisance's daily cap; a later unshielded copy can still apply.
Shielding Log/Goose prevents its pending status; shielding Pebbles marks that
placement as free of its rest penalty. A shield does not undo earlier effects.
Pending effects resolve before the next coloured ability. On a Goose-suppressed Splash, no rescue is
armed. If a sixth Obstacle causes a worn-out ending, its ordinary extra nuisance
does not create another future-draw effect; follow final payout rules.

A board-edge reference should show the four regular obstacle icons and short
rules, with the currently pending one highlighted beside the draw control.
Tap for full wording. Do not squeeze several paragraphs onto token faces.
The optional Goose can be explained separately when introduced.

## Why this is a starting ruleset

- Keep most draws simple: one move, at most one clear ability.
- Avoid backwards/zero-distance piles, permanent score loss, instant defeat
  from a rare nuisance, and endless rescue loops.
- Let purchases support distance, Dream funding, information, protection,
  direct Twigs or shelter comfort rather than making every colour “move more”.
- Keep the Goose out of the first balance run. It can later replace a normal
  obstacle symmetrically, with a visible explanation; do not secretly add it.

The opening bag cannot test every role: Reeds, Companion and Wildflowers enter
through later purchases, and Brambles needs that Day's board/encounter Twig
earnings to matter. Evaluate those effects in focused examples as well as full
games; opening draw math alone does not compare them.

All amounts, starting composition and powers here are proposals. The baseline
math excludes the powers, penalties, prizes, bag growth, weather, stop strategy
and permanent starts. It cannot establish purchase prices or whether players
reach the wasteland at the intended time. A complete ten-Day simulation comes
after the 50-row table and endgame rules are fixed.

## Architecture and next work

The existing session/bag/action/AI separation still fits. Reuse those foundations
while writing new encounter and World Event rules. The new event deck has no
required Quacks card count or one-to-one conversion mapping. Preserve the old
game only as reference/regression evidence.

The next rules pass should decide these candidate powers, the 50-row rewards,
obstacle threshold, worn-out base payout, Night 10, shelter award timing and
Dream prices. Each Feather's effect is already fixed: permanently add one to
later starting positions, never use the final rest as tomorrow's start anchor.
The open questions concern award sources/timing and endpoint handling, not
that exchange or permission to bank/spend Feathers.
Extend the shared nest-level schedule to Days 7–10 for the third
tier as the initial ten-Day adjustment; the final Night's purchase behavior
remains a separate decision. Short-match settings are later work.

## Image provenance and validation

All three 1536 × 1024 PNGs came from built-in image_gen, using the approved family
sheets as references, and were copied byte-for-byte. [prompts.json](prompts.json)
contains the complete inputs/prompts; [inspection.json](inspection.json) records
hashes and visual checks. Generic ‘1’ badges are absent and red movement arrows
read 2/4/6. Prior approved source files remain unchanged.

These are opaque concept sheets, not isolated sprites or game-size validation.
The exact bag script ran independently of Core, with enumeration/formula
agreement. No Core/CLI changes, Unity calls, imports or full-game balance tests
were performed.
