# Seed 20 final-Day regression

`seed20-day10-revision1.json` is an actual legal snapshot from frozen gameplay
source `32b5233`, rules revision 1. Seed 20, original seats, CLI scheduler:
human uses the frozen M4 baseline; AI uses the corrected Normal policy.
The snapshot is captured immediately before AI's next Day-10 decision after
one chip, at haven 10 with 32 Twigs versus the human's 40 at space 20.

The test restores the original rules revision and checks the decision directly.
This keeps the regression stable when approved shopping prices change the bags
that the same seed would build in a new game. It does not fabricate placements,
inspect hidden order for a decision, or assert that the recovery succeeds.
The initial complete replay and corrected trace are preserved under
`docs/duck-migration/m4-5/evidence/`.
