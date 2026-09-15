# Final AI validation — 15 September 2026

Gameplay/evaluation source: `32b52333d49d99ecbce56dc25b91d490e30a74bc`.
Final test revision: `feebf2c`. Approved rules and prices remain unchanged.

- Focused policy, shared placement and evaluation checks: **54 passed**.
- Native-layout `dotnet test Quackies.sln --configuration Release --nologo`:
  **328 passed**, zero failed. [Log](final-ai-suite.txt).
- `dotnet build Quackies.sln --configuration Release --nologo`: zero warnings
  and errors. [Log](final-ai-build.txt).
- An earlier full-suite invocation with an external `--artifacts-path` failed
  22 file-dependent tests because their ancestor-path fixture lookup could not
  locate the repository. The ordinary repository-layout command above resolves
  that invocation limitation; no gameplay change was required.
- Final CLI demonstration: `dotnet src/Quackies.Cli/bin/Release/net10.0/Quackies.Cli.dll
  --profile ducks --seed 42 --demo-game --save /tmp/quackies-m45-final-ai-seed42-save.json`.
  Both ducks used Normal. Ten Days completed: **AI 56 Twigs / 15 Sleep,
  Human 46 Twigs / 15 Sleep**. [Full transcript](final-ai-seed42-cli.txt.gz).
- Repeating with `--profile ducks --continue --demo-game --save` and the same
  save path preserved the finished standings and did not award Night 10 again.
  [Continue transcript](final-ai-seed42-continue.txt.gz),
  [finished save](final-ai-seed42-save.json.gz).
- Seed 20's final-Day regression now explores beyond a certain losing rest,
  then wears out and loses 36–50. This checks decision intent, not a guaranteed
  successful recovery. [Trace](final-ai-seed20-trace.json.gz).
- Fresh approved-price evaluation: **3,600 completed matches**, six declared
  comparisons, 300 seeds per comparison, both seats. Each record passed the
  analyzer's completion/accounting checks. [Manifest](holdout-manifest.json),
  [summary](holdout-summary.md), [complete aggregates](holdout-summary.json).

Equal-policy swapped records duplicate the same seeded match; selfplay therefore
contains 300 distinct games, not 600 independent games. These automated games
are not user playtesting. No Unity or physical-device validation is claimed.
