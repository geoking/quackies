# CLI usability checkpoint validation

15 September 2026. Source checkpoint `b175e78`; test checkpoint `c3525b4`.
This is the user-requested CLI and documentation work before M5, not Unity
integration or a change to the approved game rules.

- CLI-focused checks: **23/23 pass**. Classic settings checks: **9/9 pass**.
- [Full Release suite](full-suite.txt): **339/339 pass**, including both profiles.
- [Full Release solution build](full-build.txt): **zero warnings and errors**.
- Exact-state regression: reading references and entering invalid/empty input
  produces the same save as the control, including AI progress and random state.
- Help exits without loading or creating a save. New launches default to ducks;
  classic round/setting tests explicitly select `--profile classic`.
- The interactive shop displays revision 1 prices from a restored match.
  Current-price, exact Continue, purchase, backup, restart and pending final-Day
  persistence regressions remain passing.

The full-suite check initially identified four classic tests using the old
implicit profile. Their invocation/usage expectations were updated while their
round and settings assertions were retained. The final full suite above passes.
The temporary default save created by that obsolete invocation was identified
by its creation time and exact test state and moved into the temporary review
folder; it is not left in the player's default save location.

## Interactive game smoke check

A separate driver entered actual numbered menu choices through the interactive
CLI, rather than using `--demo-game`. It used seed 42 and an isolated temporary
save. Decisions came from the terminal's displayed actions and public state;
this was a scripted usability-flow check, not human gameplay-feel testing or a
new balance comparison.

- Thirteen reference/invalid commands left the save bytes unchanged.
- **78 gameplay commands**, **12 purchases** and **10 settlements** completed
  all ten Days. The run observed hidden final-Day decisions.
- Final result: **AI 41 Twigs / 17 Sleep; Human 35 Twigs / 17 Sleep**.
- Finished-game Continue, including reading Night/shop information, preserved
  both the result and the save bytes.

[Summary](interactive-summary.json), [interactive transcript](interactive.txt.gz)
and [Continue transcript](continue.txt.gz) record the run. Local filesystem roots
in the transcripts are normalized as `$REPO` and `$REVIEW`.

[Play guide](../CLI_GUIDE.md) · [Codebase map](../CODEBASE_MAP.md) ·
[Architecture](../ARCHITECTURE.md). M5 remains unstarted.
