# Quackies working agreement

## Product and architecture

Follow docs/duck-migration/PLAN.md for the current duck migration. The completed
playable baseline is documented in docs/IMPLEMENTATION_GOAL.md. Read
docs/ARCHITECTURE.md before changing component boundaries. Readable, adaptable
architecture is the leading requirement. Core rules must never depend on Unity.
Do not simplify the agreed complete base-game scope to satisfy prototype tests.

## Model and usage policy

- Use Astra Ultra for architectural decisions, difficult rule ambiguities, and
  milestone review. Keep routine implementation with the workers below.
- Use GPT-5.6 Sol at high reasoning for core rules and complex rule tests.
- Use GPT-5.6 Terra at high reasoning for Unity presentation and editor tooling.
- Use GPT-5.6 Luna at medium reasoning for narrow, fully specified support work.
- Explicitly select both model and reasoning effort when spawning a worker;
  avoid inheriting the lead's expensive settings by accident.
- At most two workers may run concurrently. Workers must not delegate further.
- Give workers focused briefs and only relevant files or specifications. Assign
  disjoint file ownership. Return changed files, test evidence, and open issues.
- Work in bounded milestones. Save code and decisions before long investigations.
  Resume from written state after interruptions instead of repeating exploration.
- One agent owns Unity Editor mutations at a time. Coordinate compilation and
  scene construction with the lead; use the Unity connector before UI automation.
- Run focused tests during implementation and full checks at milestones. Repeat
  broad checks only when a change or unresolved concern warrants it.

These choices optimize usage; they do not authorize reduced rule coverage or
untested completion claims. Escalate a specific hard problem, not a whole task.

## Collaboration

Preserve pre-existing work and unrelated changes. The user has authorized regular
progress commits and pushes to GitHub. The lead owns Git operations: use the
`codex/duck-game-milestone-0` branch for this migration, commit each small coherent Core change,
then its unit tests as a separate commit, and each compiling Unity change as its
own checkpoint. Run appropriate checks and push each checkpoint immediately.
Use clear commit subjects and bodies explaining the problem and resulting
behavior. Do not collect unrelated Core, tests and Unity changes into one commit.
Workers must not stage or commit files. Keep unrelated pre-existing modifications
out of these commits. Do not force-push or merge to main without a request.
Record completed milestones and validation in docs/PROGRESS.md. Prefer tested
code checkpoints; if a work-in-progress checkpoint is needed, label it honestly
and record the known failing checks. Do not describe a partial milestone as the
complete playable game. No physical-device install or public release is requested.
A successful Editor run, iOS export, and device test are distinct evidence.

Stop at each completed migration milestone and ask the user how they feel about
the result before starting the next one. M0 establishes the baseline and plan;
M1 is the first generated-art and Unity visual test. Preserve existing mechanics
through the first playable duck version. Ask before adding rule or balance
changes; presentation choices within the selected milestone may proceed.
