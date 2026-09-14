# Quackies encounter rules and timing

14 September 2026. This is the current detailed encounter contract for the
first complete ruleset. It replaces the earlier rescue, Companion shield and
Mud movement rules. Board payouts, prices, stock policy and the ten shared
events are approved starting rules.

Numeric yields remain subject to later full-match balance testing; this document
does not claim implementation.

## Draw and placement order

Days 1–9 resolve each duck's complete Draw or Settle action independently and
publish it immediately. Other ducks may react to those completed actions. There
is no paired wait, frozen multi-player cohort or forced alternating turn order
on these Days. Signpost previews stay private, and Night/shared-event scoring
still waits for everyone to finish.

On Day 10 only, freeze the active cohort and its preceding public state for
each decision beat. Every active duck commits Draw or Settle using that state
plus its own known preview. Resolve in fixed player order and reveal the whole
beat atomically after all active ducks commit. A duck finishing or wearing out
leaves subsequent beats; it does not cancel another already-committed action.

For a Draw action, resolve one encounter at a time:

1. Reveal the next chip. A Signpost preview is information only, not a reveal
   or placement for encounter rules.
2. Determine its intrinsic movement and any ability or World Event additions.
   If a Fallen Log is pending, halve the resulting helpful-chip movement,
   rounding up to at least one.
3. Place the chip once on its final destination. Passed spaces do not pay.
4. Resolve the chip's nonmovement ability or nuisance, including Exhaustion.
5. If Exhaustion is above the current safe maximum, the duck becomes worn out
   immediately and draws no further chip. Otherwise it may continue or settle.

A cancelled preview never counts as placed. No current encounter cancels an
entire draw. Each duck must draw at least one chip before it may settle that
Day. An empty bag ends exploration only after its final chip fully resolves. If
movement reaches or overshoots space 43, place the chip at 43, fully resolve it
including Exhaustion, then finish with no further placements. The duck
eventually rests on its final occupied space, not the next empty space. World
Event movement is added before Log halves the total unless that event explicitly
says otherwise. The Still Air event and pending
Log halve Tailwind movement only once together; the Log is consumed normally.

## Helpful encounters

A token type means one of the seven helpful kinds listed below. Tailwind
strengths and Reeds quantities remain variants of their respective types.
Default movement one is unprinted. A number beside a forward arrow is total
movement. A pictured quantity such as Reeds x3 describes the pictured objects,
not movement or a universal strength.

| Encounter | Current rule |
| --- | --- |
| **Seeds** | Move 1. No ability, Sleep reward or Exhaustion. |
| **Tailwind →2/→4/→6** | Move the printed total. No other ability. |
| **Signpost →2** | Move 2, then privately preview the next chip. Settle and return it during cleanup, or continue with that exact chip next. No selection, discard or reorder. |
| **Refreshing splash** | Move 1, then protect only the immediately next placed chip as described below. |
| **Nesting reeds x1/x2/x3** | Move 1 and gain the pictured one, two or three Twigs. Each chip is one placement and one trigger. These Twigs are retained even if the duck later wears out. |
| **Companion duck** | Increment the active flock count, then move `min(active count + 1, 4)`. Mud can reduce that count before a later Companion. |
| **Wildflowers** | Move 1. At settlement, each placed Wildflowers grants +2 Sleep only if the duck settled safely at a haven. |

Stronger Signpost previews, numbered Wildflowers and shelter movement are later
alternatives, not part of this ruleset.

### Active Companion flock

Each duck starts every Day with active flock count zero. A placed Companion
increments the current count before its movement: count one moves 2, count two
moves 3, and count three or more moves 4. Companions need not be consecutive.

Mud reduces the active count by one, to a minimum of zero. It does not remove an
owned or placed Companion and never moves an earlier chip. The next Companion
increments the reduced count and uses the formula above. A Companion still
counts as physically placed for rules that inspect encountered token types, while
movement and the Night flock contest use the reduced active count.

After every duck's adventure ends, compare positive active flock counts among
ducks who settled safely. Worn-out ducks are ineligible and do not block a
smaller safe flock. All tied leaders qualify. Each leader gains
`min(active flock count, 2)` Sleep once: +1 at count one or +2 at count two or
more. Add it to earned and available Sleep before freezing earned Sleep for
Most Rested. Companion has no shield or nuisance-cancelling ability.

## Immediate-next Splash protection

Placing Splash arms protection for the **immediately next placed chip only**.
That protection expires after that chip regardless of its token type, or at Day
cleanup if no next chip is placed.

- If the next chip is helpful, protection does nothing and expires. A second
  consecutive Splash then resolves normally and arms fresh protection for its
  own next chip.
- If the next chip is an Obstacle, its movement and +1 Exhaustion still resolve,
  and it can still cause wear-out. Only its extra nuisance is suppressed.
- Suppressing Log prevents that Log from arming a new pending slowdown, but it
  does not remove a Log already pending.
- Suppressing Mud prevents its active-flock decrement.
- Suppressing Pebbles or Brambles permanently marks that placement as protected,
  so its final-position penalty remains suppressed if the duck later rests there.
- Suppressing Goose prevents its safe-maximum reduction. Goose still moves one
  and adds one Exhaustion; the current safe maximum stays unchanged.

Splash never cancels a chip, refunds a draw, settles the duck or rescues it from
lethal Exhaustion. It does not block World Event weather or other shared
conditions; those are not obstacle nuisances.

The **A Friendly Guide** World Event gives equivalent nuisance protection to
each duck's first placed Obstacle that Day. If Splash also protects that same
Obstacle, both protections are consumed; neither carries forward. Guide does
not remove an older pending Log. Like Splash, it never blocks movement or
Exhaustion.

## Ordinary white Obstacles

Every ordinary white moves one and adds one Exhaustion. Five Exhaustion is safe
under the normal maximum; the sixth causes wear-out even when its nuisance is
suppressed.

| Obstacle | Extra nuisance |
| --- | --- |
| **Fallen log** | Arm a slowdown that halves the next helpful chip's movement after World Event additions, rounding up to at least 1. Its other ability still resolves. |
| **Mud puddle** | Reduce this duck's active Companion flock count by 1, minimum 0. Earlier Companion positions and owned chips do not change. |
| **Loose pebbles** | If this placement is the duck's final occupied chip, subtract 1 Sleep that Day, minimum 0. This also applies after wear-out. |
| **Brambles** | If this placement is the duck's final occupied chip, subtract 1 Twig earned that Day, minimum 0. This also applies after wear-out and never removes an earlier Day's Twig. |

Only one Log slowdown can be pending. Another unprotected Log does not multiply
or strengthen it. The pending Log clears after the next helpful placement, even
if the result remains one, or at Day cleanup. Protection against a newly drawn
Log does not clear an older pending Log.

Pebbles and Brambles attach their penalty to their own placement. Moving beyond
that chip avoids the penalty. Splash or Guide suppression remains marked on the
placement for the rest of the adventure, rather than becoming deferred
protection.

## Starting bag

Each duck starts with 13 chips:

- eight ordinary white Obstacles: two Logs, two Muds, two Pebbles and two
  Brambles;
- two Seeds;
- one Tailwind →2;
- one Signpost →2; and
- one Refreshing splash.

Reeds, Companion, Wildflowers and stronger Tailwinds enter through the shop;
their prices, availability and approved stock policy are defined with the board data.

## Grumpy Goose from Day 5

During Day 5 preparation, add exactly one Goose to each duck's bag. It remains
for Days 5–10; do not add another each dawn. Goose is a white Obstacle, moves one
and adds one Exhaustion.

Unless its nuisance is protected by Splash or Friendly Guide, Goose also sets
that Day's safe Exhaustion maximum to four before checking for wear-out. The
maximum stays four for the rest of that Day and resets to five next dawn. The
effect is neither permanent nor cumulative. For example, an unprotected Goose
at four existing Exhaustion moves and raises Exhaustion to five, lowers the
maximum to four, and immediately wears the duck out. A protected Goose at four
raises Exhaustion to five but leaves the maximum at five, so the duck remains
safe; a later sixth white still causes wear-out.

Signpost preview alone never activates Goose. Goose has no former next-colour
suppression rule, and Companion provides no protection against it.

## Settlement and Night order

After every player has finished, evaluate any collective event condition once.
Night and shared-event conditions always wait for every active duck to finish;
the Day 1–9 public placement rhythm does not award them early.
Settlement uses the final occupied chip, including the white chip that caused
wear-out. Include each eligible event payout once in the following order:

1. Record the final space's printed Sleep/Twigs, Reeds Twigs and all earned
   event rewards. Worn-out ducks retain earned Twigs but cannot earn safe-only
   event rewards.
2. If the final chip is unsuppressed Brambles, subtract one from today's earned
   Twigs, floor zero. Previously banked Twigs cannot be lost.
3. Safe ducks receive haven Feathers, +2 Sleep for each placed Wildflowers at a
   haven, any other safe-only event Sleep, and Day 10's additional haven +2 Sleep.
   Count each earned event reward once, not again here if already included.
4. After everyone finishes, award the safe-only active-flock Sleep bonus.
5. Apply any unsuppressed final Pebbles deduction to total earned Sleep, floor
   zero. Safe ducks retain this Sleep. Worn-out ducks retain `floor(Sleep / 2)`;
   their total contains no safe-only bonuses.
6. Freeze retained earned Sleep and compare Most Rested among safe ducks only.
   All tied eligible leaders win; if none are safe, nobody wins.

On Nights 1–9, each Most Rested winner places their own **zzz marker** for a
temporary Start +1 on the next Day. It is used once, never becomes a Feather and
is not stored for a later Day.

On Day 10, a duck that settles safely at a haven gains an additional +2 Sleep
before Most Rested. After Sleep is frozen, each duck converts retained earned
Sleep into `floor(Sleep / 4)` Dream Twigs. Every safe duck tied for Most Rested
also gains +1 Dream Twig instead of a tomorrow-start bonus. Worn-out ducks can
convert their retained, already-halved Sleep but cannot win Most Rested. Brambles has already deducted from adventure-earned Twigs; it does not take a
second deduction or remove Dream Twigs created later. There is no Night 10 shopping.

Record safe-haven Feathers as usual on Day 10; with no Day 11 or conversion,
they add no remaining gameplay advantage or victory score.

For final victory, compare total Twigs including Dream Twigs. If tied, compare
the frozen retained Sleep from Final Night before conversion, including eligible
bonuses and a worn-out duck's rounded-down half. If still tied, declare the game a draw with tied winners. This is a final-victory tiebreak and does not replace the separate
Most Rested safe-duck eligibility rule.

All 11 Dream offers are available from Night 1 with unlimited stock. Each duck
may buy at most one chip per token type per Night, within the Night 1/2/3
purchase cap; variants share a type and unspent Sleep expires. Night 10 has no
shopping. There is no flask, rewind or redraw in this ruleset.

Local autosave and Continue are approved future functionality and remain
unimplemented. M2 and M3 are complete; M4 remains unstarted and waits for the
user's command. No Core/CLI gameplay implementation is claimed here.

Exact space rewards, haven Feather values, encounter prices and shop policies
belong to [v1/BOARD_AND_SHOP.md](v1/BOARD_AND_SHOP.md). The full World Event deck
and its payout timing are in [v1/WORLD_EVENTS.md](v1/WORLD_EVENTS.md).
