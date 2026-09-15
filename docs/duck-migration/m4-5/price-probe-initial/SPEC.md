# M4.5 bounded price probe specification

Status: exploratory development evidence only. This is not a holdout run, final
recommendation, or approved rules change.

Base source: committed Core and evaluation runner from
3a2315f9c64b99fde32f33b117e4991c5dcddde3.

Candidate A changes only these shop prices:

- tailwind_2: 5 -> 4
- tailwind_4: 10 -> 8
- tailwind_6: 15 -> 12

Candidate B changes only these shop prices:

- reeds_1: 6 -> 7
- reeds_2: 11 -> 12
- reeds_3: 16 -> 17

Every other rule, source file, policy, schedule, and price remains the 3a2315f
version. No other price was explored.

Each candidate runs the real Quackies.Core session through
Quackies.Evaluation for two declared CLI-schedule matchups:

- normal vs reeds-heavy
- movement-heavy vs reeds-heavy

The development seed set is 1000 through 1099 inclusive. Each seed is played
with both seat assignments, giving 200 matches per candidate/matchup. Existing
controls use the same commit, seeds, seats, and CLI schedule.

The design target supplied during the probe is that Reeds-focused and
movement-focused bags should have fairly even strategic viability across
opponents and bag combinations; choosing one should not imply automatic wins.
Oasis arrival is optional. A single targeted-policy matchup need not be exactly
50/50.

Known limitations are part of the specification: the 3a2315f initial AI still
has remaining-horizon and last-Day limitations, the improved policy is under
construction, and this development set is not the fresh 300-seed validation.
No save was generated or used as a policy input.
