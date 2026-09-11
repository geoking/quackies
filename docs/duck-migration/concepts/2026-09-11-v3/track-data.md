# Board track data audit (2026-09-11)

This is a verified transcription of the current Core track for the v3 board illustration. It preserves the source's physical indices and reward values. The illustration proposal at the end is explicitly image-only and does not change gameplay.

## What the current rules mean

`BoardTrack.Standard()` creates 54 contiguous physical spaces, indices 0 through 53. Index 0 is the permanent duck/droplet start, not an encounter placement space. Positive chip strengths advance from that start into ordinary encounter positions 1–52. The board displays numbered spaces 1–53, with index 53 reserved as the final scoring space. At evaluation, `ScoringSpace(lastChipPosition)` returns the following space, capped at the end.

The source ruby positions are 15 physical indices: **5, 9, 13, 16, 20, 24, 28, 30, 34, 36, 40, 42, 46, 50, 52**. A scoring ruby is awarded when the following scoring space has `HasRuby`; it can still apply after an explosion. Ordinary non-exploded evaluation grants the scoring space's points and coins. In round 9, an exploded player automatically receives the better of printed points and `coins / 5` (integer division), with no shopping.

Source references:

- [BoardTrack.cs](/Users/george/Repos/quackies/src/Quackies.Core/Rules/BoardTrack.cs:7) defines the arrays, ruby set, contiguous indices, `LastChipPosition`, and following-space lookup.
- [MatchView.cs](/Users/george/Repos/quackies/src/Quackies.Core/Match/MatchView.cs:79) defines the physical position, coin, point, and ruby fields.
- [BrewingPhaseHandler.cs](/Users/george/Repos/quackies/src/Quackies.Core/Match/BrewingPhaseHandler.cs:17) and `:54` enforce the last placeable position.
- [EvaluationPhaseHandler.cs](/Users/george/Repos/quackies/src/Quackies.Core/Match/EvaluationPhaseHandler.cs:17) records the following scoring space; `:57`–`:86` applies ruby, points, coins, and final-round exploded scoring.
- [PLAN.md](/Users/george/Repos/quackies/docs/duck-migration/PLAN.md:132) records the same 0–53 / last-encounter-52 baseline.

## Exact source table

| index | coins (pennies) | VP (twigs) | source ruby | board role |
|---:|---:|---:|:---:|:---|
| 0 | 0 | 0 | no | permanent start |
| 1 | 1 | 0 | no | tile placement |
| 2 | 2 | 0 | no | tile placement |
| 3 | 3 | 0 | no | tile placement |
| 4 | 4 | 0 | no | tile placement |
| 5 | 5 | 0 | yes | tile placement |
| 6 | 6 | 1 | no | tile placement |
| 7 | 7 | 1 | no | tile placement |
| 8 | 8 | 1 | no | tile placement |
| 9 | 9 | 1 | yes | tile placement |
| 10 | 10 | 2 | no | tile placement |
| 11 | 11 | 2 | no | tile placement |
| 12 | 12 | 2 | no | tile placement |
| 13 | 13 | 2 | yes | tile placement |
| 14 | 14 | 3 | no | tile placement |
| 15 | 15 | 3 | no | tile placement |
| 16 | 15 | 3 | yes | tile placement |
| 17 | 16 | 3 | no | tile placement |
| 18 | 16 | 4 | no | tile placement |
| 19 | 17 | 4 | no | tile placement |
| 20 | 17 | 4 | yes | tile placement |
| 21 | 18 | 4 | no | tile placement |
| 22 | 18 | 5 | no | tile placement |
| 23 | 19 | 5 | no | tile placement |
| 24 | 19 | 5 | yes | tile placement |
| 25 | 20 | 5 | no | tile placement |
| 26 | 20 | 6 | no | tile placement |
| 27 | 21 | 6 | no | tile placement |
| 28 | 21 | 6 | yes | tile placement |
| 29 | 22 | 7 | no | tile placement |
| 30 | 22 | 7 | yes | tile placement |
| 31 | 23 | 7 | no | tile placement |
| 32 | 23 | 8 | no | tile placement |
| 33 | 24 | 8 | no | tile placement |
| 34 | 24 | 8 | yes | tile placement |
| 35 | 25 | 9 | no | tile placement |
| 36 | 25 | 9 | yes | tile placement |
| 37 | 26 | 9 | no | tile placement |
| 38 | 26 | 10 | no | tile placement |
| 39 | 27 | 10 | no | tile placement |
| 40 | 27 | 10 | yes | tile placement |
| 41 | 28 | 11 | no | tile placement |
| 42 | 28 | 11 | yes | tile placement |
| 43 | 29 | 11 | no | tile placement |
| 44 | 29 | 12 | no | tile placement |
| 45 | 30 | 12 | no | tile placement |
| 46 | 30 | 12 | yes | tile placement |
| 47 | 31 | 12 | no | tile placement |
| 48 | 31 | 13 | no | tile placement |
| 49 | 32 | 13 | no | tile placement |
| 50 | 32 | 13 | yes | tile placement |
| 51 | 33 | 14 | no | tile placement |
| 52 | 33 | 14 | yes | tile placement; last legal encounter |
| 53 | 35 | 15 | no | scoring-only following space |

## Image-only half-count shelter suggestion

There are 15 original ruby spots, so half is 7.5. The nearest possible image counts are 7 or 8. Keep the source ruby positions above as the rules data; use the following only as labelled shelter/resting visual selections tied to existing tile indices.

For a three-region visual partition, keep start index 0 separate, then use indices 1–18, 19–36, and 37–53. The source ruby counts are 4, 6, and 5. The user-confirmed 8-shelter image subset distributes 2/3/3 at indices **5, 13 / 20, 28, 34 / 40, 46, 52**. This spatial subset is pending visual review. It uses only original ruby indices and preserves their source coin/VP values; it does not implement a Core rules change or treat index 53 as an encounter placement.

## Issues to carry forward

- Index 0 is the permanent start. Ordinary encounter placement ends at index 52; index 53 is scoring-only.
- The user confirmed 8 image shelters; this is a visual design choice pending review and does not change Core ruby rules.
- The concept image is not evidence of exact count or mapping. Any future authored anchor map should retain these exact Core values and explicitly identify the scoring-only index.
