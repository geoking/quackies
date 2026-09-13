# Day, dreams and nests — design study

13 September 2026. **Discussion proposal, not an approved rules specification.**
The user requested a critique and visual ideas. No gameplay, Unity scene or
approved board asset has been changed. The current [migration plan](../../PLAN.md)
remains the baseline reference while these new rules are worked out.

## Presentation options

[Concept A — integrated dream tray](concept-a-dream-tray.png) keeps the moonlit
adventure board and the duck's resting position visible. A fold-out tray holds
Dream choices, Feathers, the next-Day reward and the scored nest. This is the
recommended default: one continuous world with a clear Day/Night transition.

[Concept B — separate nest and dream mat](concept-b-nest-mat.png) gives the nest
and shopping more room, with a button back to the adventure board. It feels
like the reverse side of a personal board. It is a useful expanded view if the
full encounter catalogue is too cramped in a tray; it adds navigation and hides
the resting location until the player returns to the board.

Recommended combination: Concept A as the main flow, a persistent nest badge
during the Day, and a tap-to-expand nest view drawing on Concept B. No second
movement track is needed. Both are visual concepts, not exact 50-space layouts.

## Proposed direction from the user

- Rest and score at the final occupied encounter space, not the next empty one.
- Reduce the trail to 50 spaces. Working interpretation: 50 playable positions
  plus the separate starting nest; confirm before defining a new track.
- Rename Pond pennies to **Sleep score**. Day is for adventure; Night is a dream
  phase that resolves bonuses and buys encounters for the next Day.
- World Events describe shared conditions, such as yesterday's rain making
  Seeds move one extra space today.
- Most Rested Duck grants a more meaningful advantage next Day, potentially
  a start advance or extra movement.
- One Feather buys one permanent start space. Wasteland havens could award
  two Feathers, with three at the endpoint.

These proposals move the project beyond a presentation-only reskin. A new
Quackies rules profile needs its own reward table, effect definitions and
balance tests while the complete original baseline remains available.

## Design assessment and recommended first experiment

**Rest on the tile reached.** This makes the physical duck and the rewarded
location agree. Keep the reward strip visible below the final encounter and
place the duck above/on that token without hiding the values. If nothing was
drawn, rest at the effective starting position; after a flask rewind use the
new last occupied position. Define reaching/overshooting 50 explicitly.
A simple candidate is clamp to 50 and end exploration, but this makes the final
haven easier to reach and must be tested with its three-Feather reward.

**Author a new 50-row table.** Do not merely delete three spaces or move the
lookup back one. The existing maximum placement at 52 scores row 53
(35 buying points, 15 victory points); its own row is 33/14. Both reward timing
and track length are changing. The old eight-shelter indices also include 52
and must be remapped. Fifty spaces do not by themselves prove the artwork fits.

**Make sleep quality distinct from distance.** A steadily rising Sleep score
would still make the furthest duck sleep best. To create the intended choice,
exposed wasteland spaces can have lower Sleep, while sheltered stops have
higher Sleep. Twigs can reward exploration separately. That makes a nearer
comfortable stop compete with a farther exposed one. Define the exact curve
before combining it with Feather yields; no values in these mockups are a
proposed balanced table.

**Separate earned Sleep from spending.** Display a crescent moon rather than a
coin. For example, “Sleep score 18” stays fixed while “18 to spend tonight”
becomes “12 remaining” after a six-point purchase. Spending cannot change who
slept best. Twigs are persistent victory score; Sleep is the night's purchase
allowance; Feathers persist as upgrade resources. Confirm whether unspent
Sleep expires and how final-night conversion works.

**Most Rested should compare sleep, not distance.** Recommended metric:
highest frozen Sleep score among ducks who settled without becoming worn out,
before purchasing or receiving the winner's bonus. State exactly which
rest/event modifiers contribute. Give tied eligible ducks the same modest
reward; if everyone is worn out, nobody qualifies. The current baseline ranks
physical scoring position, so this is an explicit new rule.

Start with one temporary “Start +1 tomorrow” advantage, expiring after that
Day, and compare it against a bounded alternative such as “Your first three
helpful encounters move +1 tomorrow.” Avoid placing those beside “Every
encounter +1” as supposedly equivalent prizes. Ten placements with +1 each
give ten extra spaces; +1 start gives only one. Repeated winners can become
harder to catch. Extra movement also changes which shelters can be landed on,
so it is not always a benefit.

Keep movement modifiers separate from printed encounter strength and white
Exhaustion. Define stacking with World Events, eligible placement sources,
catch-up and duration. A next-Day reward has no use after Day nine; explicitly
choose a final-night replacement, such as a modest Twig award, rather than
showing an unusable dawn benefit. Its amount is unresolved.

**Feathers can accelerate the leader loop.** The original exchange is two
Feathers for one permanent start space. With a one-to-one exchange, a haven
paying two Feathers could buy two permanent spaces that help reach better
havens tomorrow. Three at the endpoint could buy three. Recommended first
test: retain the one-to-one price but allow at most one permanent start
upgrade per Night, with remaining Feathers banked. This is a proposed cap,
not an agreed restriction. Keep temporary dawn movement visibly separate
from permanent upgrades, and specify refill costs, worn-out shelter rewards
and final Feather conversion.

**The rain example fits well.** The baseline already contains the equivalent
orange-movement effect in Pumpkin Patch Party. “Rain-softened seeds — Seeds
move +1 today” is a clear original presentation of that mechanic. Apply it
once per qualifying placement, without changing printed strength. Global
weather can still benefit seed-heavy bags more; that is a useful strategy
difference to test, not a guarantee that every player benefits equally.

## Dream flow and Twig presentation

Suggested nightly sequence:

1. Freeze the duck's resting location and resolve rest/event results.
2. Show Sleep earned, Twigs gained and Feathers gained as distinct rewards.
   Freeze the Most Rested comparison before shopping and its own bonus.
3. Reveal any next-Day advantage, with its duration printed clearly.
4. Open Dream choices; spend the nightly Sleep allowance on encounters.
5. Spend/bank Feathers under the agreed upgrade rules, then confirm readiness.
6. At dawn, reset to the permanent start plus any valid temporary assistance
   and reveal the next World Event. Do not inherit last night's rest position.

Existing event hooks and worn-out reward choices must be reconciled with this
flow before implementation; this sequence is a player-facing proposal.

The scored nest should be tangible, gradually adding twigs and lining as the
match progresses. Always show an exact total such as “24 Twigs”, with
“+4 tonight” temporarily underneath. Nest growth is decorative feedback, not
a spendable construction tree or a requirement to count individual drawn twigs.
Use a small nest-and-total badge on the daytime board; tapping it shows larger
nests and exact rival totals. Keep player-identity medallions distinguishable
from the actual duck resting on the adventure trail.

## Open rules before implementation

Specify the 50-row Sleep/Twig/Feather table and shelter indices; no-draw,
rewind and endpoint behavior; Most Rested eligibility/metric/ties;
worn-out rewards; temporary boost stacking and expiry; Feather spending limits;
and the final-night treatment of Sleep, Feathers and dawn rewards.

Preserve all legal market categories, finite supplies and purchase restrictions
until deliberately changed. The two seed offers in the artwork demonstrate
layout only; they do not introduce random offers or permission to buy two
of the same category. Normal AI will need evaluation against the new shelter
trade-offs: its current safe-draw policy does not actively optimize sleep.

## Sources and visual evidence

The focused read-only review used
[BoardTrack.cs](../../../../src/Quackies.Core/Rules/BoardTrack.cs),
[EvaluationPhaseHandler.cs](../../../../src/Quackies.Core/Match/EvaluationPhaseHandler.cs),
[RubySpendingPhaseHandler.cs](../../../../src/Quackies.Core/Match/RubySpendingPhaseHandler.cs),
[OngoingFortunes.cs](../../../../src/Quackies.Core/Rules/Fortunes/OngoingFortunes.cs)
and [the track audit](../2026-09-11-v3/track-data.md).
No simulation or balance certification is claimed.

Both mockups were generated with built-in image_gen and copied unchanged to
this folder. [prompts.json](prompts.json) contains the complete prompt set and
reference paths; [inspection.json](inspection.json) records native dimensions,
hashes and visual limitations. Both images are 1536 × 1024. Values and prices
are illustrative. Concept B omits seed strength numerals; precise strength
overlays are required in any implementation.
