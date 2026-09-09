# Base-game Set 1 rules audit

Reviewed against the publisher base rules and the fortune clarification in the
Herb Witches booklet. This audit covers the initial human-versus-Normal-AI scope;
test tubes and expansions are deferred. Component data and all24 card texts are
indexed in RULES_REFERENCE.md.

## Corrections from the final sweep

| Finding | Result | Evidence |
| --- | --- | --- |
| Final-round exploded pot still offered shopping coins | Automatically award the better single legal point outcome; safe pots receive both printed VP and converted coins | Core14cecc3;10 FinalRoundScoringTests |
| Blue could place another chip after filling the pot | No direct or nested selection at physical52; spoon scoring remains | Core45991dd;2 focused boundary cases |
| Strong Ingredient selected chip still earned deferred effects | Suppress its own ingredient contribution, retaining physical placement and white total | Coredd0c274;green/purple/black regressions67ae4ba |
| Well Stirred originally ignored white selected through blue | First placed white from the bag receives the once-per-round return offer | Core7993bca;3 original focused cases |
| A Second Chance was missing | Protected first5 placements and one deferred keep/restart choice; dedicated brewing-start snapshot | Core5a89d48;4 focused tests5efc452 |

## Scoring and timing decisions

Round9 uses floor(coins / 5). A safe pot earns that plus printed victory points.
An exploded pot receives the maximum of those two alternatives, automatically;
it never receives both. This automates the optimal legal choice requested by the
user. Earlier rounds retain the strategic points-versus-coins choice. Rubies
still convert at2 per point through the final ruby action.

Strong Ingredient's selected chip occupies the final physical position and can
change which earlier chips are among the last two. It does not contribute its
own green, purple or black ingredient effect. This follows the publisher wording
that the selected chip's action is not carried out. The earlier implementation
that awarded its own deferred green ruby was corrected.

Second Chance resolves the fifth placement's ingredient-choice chain before the
restart offer. Actual placements count; discarded previews do not. Restart
restores the player's brewing-start flask state without replaying preparation or
rewinding random draws. These detailed timing choices are implementation
interpretations where the primary text does not specify every chained case.

## Scope and limits

The bounded code-to-rules comparison found no further definite mismatch in the
21 previously published fortunes or the Set1 ingredient handlers. A shortage
edge remains a documented interpretation: simultaneous immediate fortune gifts
use the current fixed player iteration order when stock cannot satisfy everyone;
the inspected primary text does not establish a separate shortage tie-break.

The tests above prove their named rules and boundaries. Complete-deck simulations,
Unity interaction, compilation and iOS export are separate integration evidence,
recorded in PROGRESS.md when performed. This audit alone does not declare the
playable milestone complete.

## Primary sources

- [Publisher base rules](https://www.schmidtspiele.de/files/Retail/72dpi_PNG/88220_Quack_rules_english_2024.pdf)
- [Publisher fortune clarifications, page4](https://www.schmidtspiele.de/files/Retail/72dpi_PNG/88232_Quacks_The_Herb_Witches_GB.pdf)
