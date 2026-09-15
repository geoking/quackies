# Frozen baseline reproduction

An isolated archive of `3a2315f` built the evaluation runner in Release. Both
seats used the unchanged M4 baseline on seed 42 with the CLI schedule.
The result was AI 29 Twigs, Human 27 Twigs, matching the preserved M4 game.

The entire captured save equals `m4/full-game-save.json` after normalizing JSON
property-name casing and the two enum representations present in this save
(`phase`: 4 / `finished`; the AI's placed helpful type: 3 / `splash`). All other
fields, including bag order, RNG, history, Night outcomes and final scores,
match exactly. This verifies this fixture's runtime/evaluation parity; the
82 original runtime regressions provide broader transition coverage.

The adjacent compressed JSONL, trace and save preserve the reproduction.
Command: `dotnet <isolated Release Quackies.Evaluation.dll> --source-label
3a2315f;baseline-reproduction --seed-start 42 --policy-a baseline --policy-b
baseline --output <jsonl> --save <save.json> --trace <trace.json>`.
