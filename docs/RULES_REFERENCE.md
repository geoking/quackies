# Base game, Set 1: implementation reference

This is a source map and factual implementation checklist, not a replacement
rulebook. It records the selected classic edition, two players, standard board.
It does not enable any expansion or the optional test-tube board variant.

## Sources

- [Publisher's English base rules, 2024](https://www.schmidtspiele.de/files/Retail/72dpi_PNG/88220_Quack_rules_english_2024.pdf).
- Supplied `unity/Quackies.Unity/Assets/Art/raw/cards.jpg`: all 24 original
  English fortune cards, arranged in five rows of five; final cell is the back.
- Supplied `Assets/Art/raw/books/<color>/1.png`, with `black/2_player.png`:
  ingredient text, prices and two-player comparison rule.
- Supplied `Assets/Art/raw/cauldron/blue/cauldron.png` and `board.png`: exact
  spiral rewards, physical positions, score-track rat boundaries. The pot art
  includes test tubes; the scene crops those out for the selected standard rules.
- [Publisher's Herb Witches rules, page 4](https://www.schmidtspiele.de/files/Retail/72dpi_PNG/88232_Quacks_The_Herb_Witches_GB.pdf):
  the section clarifying base fortune cards, without importing expansion content.
  It explicitly protects fortune-card draws from explosions and suppresses the
  chip action for Strong Ingredient (called Overpowering Ingredient there).

## Match invariants

Nine rounds. Each starting bag contains white 1 x4, white 2 x2, white 3 x1,
orange 1 x1 and green 1 x1; players begin with one ruby and a full flask.
Yellow unlocks in round 2; purple in round 3; add white 1 in round 6.
Draws are without replacement. Exceeding white 7 explodes, except when a card
changes/protects the threshold. The exploding chip remains placed. A full flask
returns the most recent white chip only when it did not cause an explosion.
Scoring uses the following physical space. Evaluation order is die, ingredients,
rubies, points/purchase choice, shopping, ruby spending. Purchase at most two
chips of different colors; leftover coins expire. Ruby cost is two per droplet
step or flask refill. Round 9 uses committed simultaneous draws/stops, and allows
conversion at five coins or two rubies per point. Highest score wins; ties compare
final physical pot position, then remain tied.

## Physical board indexing

Physical index 0 is the starting droplet. Chip positions end at index 52;
index 53 is the spoon reward (35 coins, 15 points). Printed coin labels repeat
after 15 and must never be used as unique position identifiers. A chip landing
at or beyond the end is clamped to 52, and the pot is full.

The reviewed source for the arrays is the supplied cauldron image. The first
implementation's synthetic 0..33 board is obsolete and must not define tests.

Rat boundaries occur after scores 1, 4, 7, then every even score from 10 to 48.
These repeat each 50-point lap. Count boundaries strictly after the trailing
player and before the leading player's space: e.g. 4 to 5 crosses one boundary,
5 to 7 crosses none, 7 to 8 crosses one. Determine rats after the fortune card,
then place the temporary rat relative to the permanent droplet.

## Component quantities and prices

Quantities below are total base-box chips, excluding three replacements. Remove
starting allocations from the shared supply. There is no purchase price for white.

| Color | Values | Total stock by value | Set 1 prices by value | Available from |
|---|---|---|---|---|
| White | 1, 2, 3 | 20, 8, 4 | — | Setup / named effects |
| Orange | 1 | 20 | 3 | Round 1 |
| Green | 1, 2, 4 | 15, 10, 13 | 4, 8, 14 | Round 1 |
| Blue | 1, 2, 4 | 14, 10, 10 | 5, 10, 19 | Round 1 |
| Red | 1, 2, 4 | 12, 8, 10 | 6, 10, 16 | Round 1 |
| Yellow | 1, 2, 4 | 13, 6, 10 | 8, 12, 18 | Round 2 |
| Purple | 1 | 15 | 9 | Round 3 |
| Black | 1 | 18 | 10 | Round 1 |

## Ingredient handlers

| Color | Timing | Set 1 behavior |
|---|---|---|
| White | Placement | Adds its value to the white total; normally explodes only above 7. |
| Orange | Placement | Normal movement; existing orange count supports red. |
| Blue | After placement | Preview up to its value (1/2/4) from the bag; optionally place one, return the others. Resolve the selected chip's ability, including another blue selection. |
| Red | Placement | Existing 1–2 oranges permit +1 movement; 3+ permit +2. |
| Yellow | After placement | May return the immediately preceding chip if white; yellow stays on its reached position and gaps remain. |
| Green | Evaluation | Each green among the final two placed chips earns one ruby. Count chips, not empty spaces. |
| Purple | Evaluation | Count 1: one point. Count 2: one point plus one ruby. Count 3+: two points plus a droplet step. |
| Black | Evaluation, two players | A positive tie earns each a droplet step. Greater count earns a droplet step plus ruby. Zero black earns nothing. |

Effects with player discretion must be expressed as legal choices. Effects from
green/black/purple apply even after an explosion. The bonus die compares physical
scoring positions among non-exploded players; all tied leaders roll. Die faces
are one point (two faces), two points, one ruby, one orange 1, and a droplet step.

## All fortune cards

Rows correspond to supplied atlas order. P means immediate preparation; R means
an effect lasting during the round or evaluated at its end. Descriptions below
are paraphrased mechanics; use the supplied image in the player-facing reference.

| # | Card | Timing | Required behavior |
|---|---|---|---|
| 1 | A Second Chance | R | After the first five placements, offer one choice to keep the potion or start the brewing round again. See clarification below. |
| 2 | Just in Time | P | Choose four points or permanently remove one white 1 from the bag. |
| 3 | Well Stirred | R | Offer to return the first white drawn this round, without spending the flask. |
| 4 | Lucky Devil | R | A ruby scoring space adds two points even if exploded. |
| 5 | It's Shining Extra Bright | R | A ruby scoring space awards one extra ruby. |
| 6 | Seasoned Perfectly | R | White total exactly seven at round end grants a droplet step. |
| 7 | Less Is More | P | Everyone previews five chips. Lowest summed value, ties included, receives blue 2; other players receive one ruby. Return all previewed chips. |
| 8 | The Pot Is Full | R | Anyone entitled to the evaluation die rolls twice. |
| 9 | Choose Wisely | P | Choose two droplet steps or purple 1, respecting unlock availability. |
| 10 | Living in Luxury | R | White explosion threshold becomes nine for this round. |
| 11 | An Opportunistic Moment | P | Preview four; may exchange one for the next value of the same color. If no exchange is possible, receive green 1. Return unexchanged previews. |
| 12 | Roll the Die | P | Each player rolls once and receives that face's reward. |
| 13 | Pumpkin Patch Party | R | Every placed orange travels an extra space. |
| 14 | You Only Get to Choose One | P | Choose black 1, any available 2-chip, or three rubies. |
| 15 | Strong Ingredient | R | In start-player order, non-exploded players preview up to five after stopping and may add one last chip. Return the rest. See clarification below. |
| 16 | Magic Potion | R | All flasks refill free at round end. |
| 17 | A Good Start | P | Choose ordinary rats, or exchange 1–3 of this round's rat steps for that many rubies. |
| 18 | Rat Infestation | P | Double the round's rat steps. |
| 19 | Rats Are Your Friends | P | Choose any available 4-chip, or one point per rat step due this round. |
| 20 | Charity | P | All players tied for fewest rubies receive one ruby. Compare before awarding. |
| 21 | The Pot Is Filling Up | P | Every player advances their droplet one step. |
| 22 | Wheel and Deal | P | Optional exchange of one ruby for any available 1-chip except purple/black. |
| 23 | Beginner's Bonus | P | All tied for fewest points receive green 1. Compare before awarding. |
| 24 | Schadenfreude | R | A player's explosion gives their left neighbor a choice of an available 2-chip. In two-player mode, this is the other player. |

Shared rules: shuffle without replacement; draw one card per round; alternate
start player; honor ingredient unlocks, finite stock, and the active card's timing.
Fortune gifts are distinct from purchased chips and do not consume purchase slots.

## Clarifications and verification questions

The publisher's supplementary clarification is authoritative that card-driven
draws cannot cause explosions, even if white totals exceed the usual limit.
Therefore Strong Ingredient can place a protected white chip after stopping.
Its placement has no ingredient ability. It still occupies the final pot position
and affects which earlier chips are among the final two.

Second Chance needs explicit state to count the first five placements, offer a
single restart, and prevent preview-only draws from counting as placements.
The implementation must retain a brewing-start snapshot to restore state without
repeating preparation rewards or the round-six setup. Tests must distinguish
protection on those card-driven placements from the next ordinary draw.

Details requiring an explicit, documented interpretation during final audit:

- Second Chance timing when a fifth placement also opens a blue/yellow choice,
  and flask availability after restarting.
- Whether the suppressed Strong Ingredient chip contributes to its own deferred
  color-count benefit; source says its action is not performed, without spelling
  out every deferred-count interaction. Do not silently treat an implementation
  interpretation as a separately verified publisher ruling.
- Limits on repeated Wheel and Deal exchanges: supplied card describes one
  ruby/one chip, without an explicit repeat clause. Use a single exchange unless
  a stronger source establishes otherwise.

These questions must remain visible until reviewed; ordinary component data and
the other card effects do not depend on resolving them.
