# Commands and checks

All repository reads used /Users/george/Repos/quackies. All mutations and run
outputs used /tmp/quackies-m45-price-probe-20260915-1845.

## Read-only preparation

    sed -n '1,240p' AGENTS.md
    sed -n '1,280p' docs/duck-migration/m4-5/PLAN.md
    git status --short --branch
    git rev-parse 3a2315f^{commit}
    git show --stat --oneline --decorate --no-renames 3a2315f
    git show 3a2315f:src/Quackies.Core/Ducks/Definitions/DuckRules.cs
    git show 3a2315f:tools/Quackies.Evaluation/Quackies.Evaluation.csproj
    git show 3a2315f:src/Quackies.Core/Quackies.Core.csproj
    python3 tools/analyze-duck-evaluation.py --help
    shasum -a 256 docs/duck-migration/m4-5/evidence/dev-followup-normal-vs-reeds-heavy-1000-1099.jsonl.gz docs/duck-migration/m4-5/evidence/dev-movement-heavy-vs-reeds-heavy-1000-1099.jsonl.gz tools/analyze-duck-evaluation.py

## Isolated source creation

    mkdir -p /tmp/quackies-m45-price-probe-20260915-1845/source-base /tmp/quackies-m45-price-probe-20260915-1845/candidate-a-tailwind-4-8-12 /tmp/quackies-m45-price-probe-20260915-1845/candidate-b-reeds-7-12-17 /tmp/quackies-m45-price-probe-20260915-1845/results /tmp/quackies-m45-price-probe-20260915-1845/controls
    git archive --format=tar 3a2315f src/Quackies.Core tools/Quackies.Evaluation > /tmp/quackies-m45-price-probe-20260915-1845/source-3a2315f.tar
    tar -xf /tmp/quackies-m45-price-probe-20260915-1845/source-3a2315f.tar -C /tmp/quackies-m45-price-probe-20260915-1845/source-base
    tar -xf /tmp/quackies-m45-price-probe-20260915-1845/source-3a2315f.tar -C /tmp/quackies-m45-price-probe-20260915-1845/candidate-a-tailwind-4-8-12
    tar -xf /tmp/quackies-m45-price-probe-20260915-1845/source-3a2315f.tar -C /tmp/quackies-m45-price-probe-20260915-1845/candidate-b-reeds-7-12-17
    gzip -n -9 /tmp/quackies-m45-price-probe-20260915-1845/source-3a2315f.tar
    cp docs/duck-migration/m4-5/evidence/dev-followup-normal-vs-reeds-heavy-1000-1099.jsonl.gz /tmp/quackies-m45-price-probe-20260915-1845/controls/
    cp docs/duck-migration/m4-5/evidence/dev-movement-heavy-vs-reeds-heavy-1000-1099.jsonl.gz /tmp/quackies-m45-price-probe-20260915-1845/controls/

Candidate changes were applied with apply_patch. The exact inputs are preserved
as candidate-a-tailwind-4-8-12.patch and candidate-b-reeds-7-12-17.patch.

## Source checks

    diff -ru --exclude=obj --exclude=bin source-base candidate-a-tailwind-4-8-12
    diff -ru --exclude=obj --exclude=bin source-base candidate-b-reeds-7-12-17
    diff -rq --exclude=DuckRules.cs --exclude=bin --exclude=obj source-base candidate-a-tailwind-4-8-12
    diff -rq --exclude=DuckRules.cs --exclude=bin --exclude=obj source-base candidate-b-reeds-7-12-17
    diff -U0 source-base/src/Quackies.Core/Ducks/Definitions/DuckRules.cs candidate-a-tailwind-4-8-12/src/Quackies.Core/Ducks/Definitions/DuckRules.cs | rg '^[+-]\s+Offer' | wc -l
    diff -U0 source-base/src/Quackies.Core/Ducks/Definitions/DuckRules.cs candidate-b-reeds-7-12-17/src/Quackies.Core/Ducks/Definitions/DuckRules.cs | rg '^[+-]\s+Offer' | wc -l
    git show 3a2315f:src/Quackies.Core/Ducks/Definitions/DuckRules.cs | shasum -a 256

The two wc checks each returned six lines: three removals and three additions.
The recursive comparisons excluding DuckRules.cs returned no differences.

## Clean builds

From each candidate directory:

    test ! -d tools/Quackies.Evaluation/bin
    test ! -d tools/Quackies.Evaluation/obj
    test ! -d src/Quackies.Core/bin
    test ! -d src/Quackies.Core/obj
    dotnet build tools/Quackies.Evaluation/Quackies.Evaluation.csproj -c Release --nologo

Both builds returned exit 0, zero warnings, and zero errors.

## Instrumentation checks

From candidate A:

    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'diagnostic-A;source=3a2315f9c64b99fde32f33b117e4991c5dcddde3;prices=tailwind_2:4,tailwind_4:8,tailwind_6:12;all-else-unchanged' --seed-start 1000 --seed-count 1 --policy-a normal --policy-b reeds-heavy --seat-mode original --schedule cli --include-actions --output /tmp/quackies-m45-price-probe-20260915-1845/results/check-a-normal-vs-reeds-heavy-seed1000.jsonl --trace /tmp/quackies-m45-price-probe-20260915-1845/results/check-a-normal-vs-reeds-heavy-seed1000-trace.json --progress-every 0

From candidate B:

    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'diagnostic-B;source=3a2315f9c64b99fde32f33b117e4991c5dcddde3;prices=reeds_1:7,reeds_2:12,reeds_3:17;all-else-unchanged' --seed-start 1000 --seed-count 1 --policy-a movement-heavy --policy-b reeds-heavy --seat-mode original --schedule cli --include-actions --output /tmp/quackies-m45-price-probe-20260915-1845/results/check-b-movement-heavy-vs-reeds-heavy-seed1000.jsonl --trace /tmp/quackies-m45-price-probe-20260915-1845/results/check-b-movement-heavy-vs-reeds-heavy-seed1000-trace.json --progress-every 0

Jq validation asserted one record, seed 1000, ten Days, two players per Day,
source label, final winner IDs, actions present, and trace output. Counts were
208 actions for A and 227 for B.

## Batch commands

From candidate A, with the A source label shown above:

    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'diagnostic-A;source=3a2315f9c64b99fde32f33b117e4991c5dcddde3;prices=tailwind_2:4,tailwind_4:8,tailwind_6:12;all-else-unchanged' --seed-start 1000 --seed-count 100 --policy-a normal --policy-b reeds-heavy --seat-mode both --schedule cli --output /tmp/quackies-m45-price-probe-20260915-1845/results/a-normal-vs-reeds-heavy-1000-1099.jsonl --progress-every 25
    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'diagnostic-A;source=3a2315f9c64b99fde32f33b117e4991c5dcddde3;prices=tailwind_2:4,tailwind_4:8,tailwind_6:12;all-else-unchanged' --seed-start 1000 --seed-count 100 --policy-a movement-heavy --policy-b reeds-heavy --seat-mode both --schedule cli --output /tmp/quackies-m45-price-probe-20260915-1845/results/a-movement-heavy-vs-reeds-heavy-1000-1099.jsonl --progress-every 25

From candidate B, with the B source label shown above:

    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'diagnostic-B;source=3a2315f9c64b99fde32f33b117e4991c5dcddde3;prices=reeds_1:7,reeds_2:12,reeds_3:17;all-else-unchanged' --seed-start 1000 --seed-count 100 --policy-a normal --policy-b reeds-heavy --seat-mode both --schedule cli --output /tmp/quackies-m45-price-probe-20260915-1845/results/b-normal-vs-reeds-heavy-1000-1099.jsonl --progress-every 25
    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'diagnostic-B;source=3a2315f9c64b99fde32f33b117e4991c5dcddde3;prices=reeds_1:7,reeds_2:12,reeds_3:17;all-else-unchanged' --seed-start 1000 --seed-count 100 --policy-a movement-heavy --policy-b reeds-heavy --seat-mode both --schedule cli --output /tmp/quackies-m45-price-probe-20260915-1845/results/b-movement-heavy-vs-reeds-heavy-1000-1099.jsonl --progress-every 25

All four returned exit 0 and 200/200 progress.

## Batch validation and analysis

Jq structural checks asserted, for every batch: 200 records, 100 unique seeds,
minimum 1000, maximum 1099, 100 records per seat assignment, one expected source
label, profile quackies.duck.v1, rules/save version 1, CLI schedule, ten Days,
and one expected policy pair. The collected purchase offer/price pairs matched
the candidate source. A filesystem search found no save artifacts.

    gzip -n -9 results/a-normal-vs-reeds-heavy-1000-1099.jsonl results/a-movement-heavy-vs-reeds-heavy-1000-1099.jsonl results/b-normal-vs-reeds-heavy-1000-1099.jsonl results/b-movement-heavy-vs-reeds-heavy-1000-1099.jsonl
    python3 /Users/george/Repos/quackies/tools/analyze-duck-evaluation.py controls/dev-followup-normal-vs-reeds-heavy-1000-1099.jsonl.gz controls/dev-movement-heavy-vs-reeds-heavy-1000-1099.jsonl.gz results/a-normal-vs-reeds-heavy-1000-1099.jsonl.gz results/a-movement-heavy-vs-reeds-heavy-1000-1099.jsonl.gz results/b-normal-vs-reeds-heavy-1000-1099.jsonl.gz results/b-movement-heavy-vs-reeds-heavy-1000-1099.jsonl.gz --json results/aggregate-summary.json --markdown results/aggregate-summary.md

The analyzer reported 1,200 complete matches in six separated comparisons.

## Source-only candidate archives

    tar --exclude='*/bin' --exclude='*/bin/*' --exclude='*/obj' --exclude='*/obj/*' -cf candidate-a-tailwind-4-8-12-source.tar -C candidate-a-tailwind-4-8-12 src/Quackies.Core tools/Quackies.Evaluation
    gzip -n -9 candidate-a-tailwind-4-8-12-source.tar
    tar --exclude='*/bin' --exclude='*/bin/*' --exclude='*/obj' --exclude='*/obj/*' -cf candidate-b-reeds-7-12-17-source.tar -C candidate-b-reeds-7-12-17 src/Quackies.Core tools/Quackies.Evaluation
    gzip -n -9 candidate-b-reeds-7-12-17-source.tar
    tar -tzf candidate-a-tailwind-4-8-12-source.tar.gz | rg '/(bin|obj)/'
    tar -tzf candidate-b-reeds-7-12-17-source.tar.gz | rg '/(bin|obj)/'

Both archive-content searches returned no bin or obj entries.
