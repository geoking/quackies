# Reproducibility commands

Repository reads used /Users/george/Repos/quackies. All generated or modified
files are under /tmp/quackies-m45-price-probe-finalai-20260915-32b5233.

## Source

    git rev-parse 32b5233^{commit}
    git archive --format=tar 32b5233 src/Quackies.Core tools/Quackies.Evaluation > source-32b5233.tar
    tar -xf source-32b5233.tar -C source-base
    tar -xf source-32b5233.tar -C profile-control-approved
    tar -xf source-32b5233.tar -C profile-c-tailwind-4-8-12-reeds-7-12-17
    tar -xf source-32b5233.tar -C profile-d-reeds-8-14-20
    gzip -n -9 source-32b5233.tar
    tar -xzf source-32b5233.tar.gz -C profile-e-tailwind-4-8-12-reeds-8-14-20

Profiles C, D, and E were changed with apply_patch; profile-c.patch,
profile-d.patch, and profile-e.patch preserve the exact edits. Successful
verification commands:

    diff -rq source-base profile-control-approved
    diff -rq --exclude=DuckRules.cs --exclude=bin --exclude=obj source-base profile-c-tailwind-4-8-12-reeds-7-12-17
    diff -rq --exclude=DuckRules.cs --exclude=bin --exclude=obj source-base profile-d-reeds-8-14-20
    diff -rq --exclude=DuckRules.cs --exclude=bin --exclude=obj source-base profile-e-tailwind-4-8-12-reeds-8-14-20
    diff -U0 source-base/src/Quackies.Core/Ducks/Definitions/DuckRules.cs profile-c-tailwind-4-8-12-reeds-7-12-17/src/Quackies.Core/Ducks/Definitions/DuckRules.cs
    diff -U0 source-base/src/Quackies.Core/Ducks/Definitions/DuckRules.cs profile-d-reeds-8-14-20/src/Quackies.Core/Ducks/Definitions/DuckRules.cs
    diff -U0 source-base/src/Quackies.Core/Ducks/Definitions/DuckRules.cs profile-e-tailwind-4-8-12-reeds-8-14-20/src/Quackies.Core/Ducks/Definitions/DuckRules.cs

One E diff attempt used the E profile as its working directory while addressing
sibling paths and returned “source-base: No such file or directory.” The
successful experiment-root checks above were run before any E match.

## Builds and instrumentation

From each profile directory, after asserting Core and runner bin/obj directories
did not exist:

    dotnet build tools/Quackies.Evaluation/Quackies.Evaluation.csproj -c Release --nologo

All four builds succeeded with zero warnings and errors.

Each profile ran a seed-1000 original-seat check before its batches using its
exact source label, --include-actions, --output, --trace, and --progress-every 0.
No --save option was used. The full commands and outputs are represented by the
check-*.jsonl and check-*-trace.json artifacts.

## Development batches

Common arguments:

    --seed-start 1000 --seed-count 100 --seat-mode both --schedule cli --progress-every 25

For each profile control/C/D/E, the isolated Release runner was invoked once for
each pair:

    --policy-a normal --policy-b reeds-heavy
    --policy-a normal --policy-b movement-heavy
    --policy-a movement-heavy --policy-b reeds-heavy

Exact labels:

    control-final-ai;source=32b52333d49d99ecbce56dc25b91d490e30a74bc;prices=tailwind_2:5,tailwind_4:10,tailwind_6:15,reeds_1:6,reeds_2:11,reeds_3:16;all-else-unchanged
    diagnostic-C;source=32b52333d49d99ecbce56dc25b91d490e30a74bc;prices=tailwind_2:4,tailwind_4:8,tailwind_6:12,reeds_1:7,reeds_2:12,reeds_3:17;all-else-unchanged
    diagnostic-D;source=32b52333d49d99ecbce56dc25b91d490e30a74bc;prices=tailwind_2:5,tailwind_4:10,tailwind_6:15,reeds_1:8,reeds_2:14,reeds_3:20;all-else-unchanged
    diagnostic-E;source=32b52333d49d99ecbce56dc25b91d490e30a74bc;prices=tailwind_2:4,tailwind_4:8,tailwind_6:12,reeds_1:8,reeds_2:14,reeds_3:20;all-else-unchanged

Representative exact E development command:

    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'diagnostic-E;source=32b52333d49d99ecbce56dc25b91d490e30a74bc;prices=tailwind_2:4,tailwind_4:8,tailwind_6:12,reeds_1:8,reeds_2:14,reeds_3:20;all-else-unchanged' --seed-start 1000 --seed-count 100 --policy-a movement-heavy --policy-b reeds-heavy --seat-mode both --schedule cli --output /tmp/quackies-m45-price-probe-finalai-20260915-32b5233/results/e-movement-heavy-vs-reeds-heavy-1000-1099.jsonl --progress-every 25

The other output paths follow the exact profile-policy filename pattern in
results/. Every file was validated before compression.

## Pre-output selection

SELECTION.md was written and hashed before fresh E files were generated or read.
Its initial SHA-256 was
e9a28a52d374fca81a190371bec557023f609be8a4a598a4aef78c74b60100c3.

## Fresh validation

Exact frozen E commands from the E profile directory:

    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'validation-E-selected-before-output;source=32b52333d49d99ecbce56dc25b91d490e30a74bc;prices=tailwind_2:4,tailwind_4:8,tailwind_6:12,reeds_1:8,reeds_2:14,reeds_3:20;all-else-unchanged' --seed-start 10000 --seed-count 300 --policy-a normal --policy-b reeds-heavy --seat-mode both --schedule cli --output /tmp/quackies-m45-price-probe-finalai-20260915-32b5233/results/e-validation-normal-vs-reeds-heavy-10000-10299.jsonl --progress-every 50
    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'validation-E-selected-before-output;source=32b52333d49d99ecbce56dc25b91d490e30a74bc;prices=tailwind_2:4,tailwind_4:8,tailwind_6:12,reeds_1:8,reeds_2:14,reeds_3:20;all-else-unchanged' --seed-start 10000 --seed-count 300 --policy-a normal --policy-b movement-heavy --seat-mode both --schedule cli --output /tmp/quackies-m45-price-probe-finalai-20260915-32b5233/results/e-validation-normal-vs-movement-heavy-10000-10299.jsonl --progress-every 50
    dotnet tools/Quackies.Evaluation/bin/Release/net10.0/Quackies.Evaluation.dll --source-label 'validation-E-selected-before-output;source=32b52333d49d99ecbce56dc25b91d490e30a74bc;prices=tailwind_2:4,tailwind_4:8,tailwind_6:12,reeds_1:8,reeds_2:14,reeds_3:20;all-else-unchanged' --seed-start 10000 --seed-count 300 --policy-a movement-heavy --policy-b reeds-heavy --seat-mode both --schedule cli --output /tmp/quackies-m45-price-probe-finalai-20260915-32b5233/results/e-validation-movement-heavy-vs-reeds-heavy-10000-10299.jsonl --progress-every 50

Each file was checked with jq before outcome analysis: 600 records, 300 unique
seeds, range 10000–10299, 300 original plus 300 swapped seats, ten Days, CLI
schedule, exact pair, exact source label, and exact observed prices.

## Compression and analysis

    gzip -n -9 results/e-validation-{normal-vs-reeds-heavy,normal-vs-movement-heavy,movement-heavy-vs-reeds-heavy}-10000-10299.jsonl
    python3 /Users/george/Repos/quackies/tools/analyze-duck-evaluation.py holdout-controls/holdout-{normal-vs-reeds-heavy,normal-vs-movement-heavy,movement-heavy-vs-reeds-heavy}-10000-10299.jsonl.gz --json results/approved-holdout-summary.json --markdown results/approved-holdout-summary.md
    python3 /Users/george/Repos/quackies/tools/analyze-duck-evaluation.py results/e-validation-{normal-vs-reeds-heavy,normal-vs-movement-heavy,movement-heavy-vs-reeds-heavy}-10000-10299.jsonl.gz --json results/e-validation-summary.json --markdown results/e-validation-summary.md
    python3 /Users/george/Repos/quackies/tools/analyze-duck-evaluation.py holdout-controls/holdout-{normal-vs-reeds-heavy,normal-vs-movement-heavy,movement-heavy-vs-reeds-heavy}-10000-10299.jsonl.gz results/e-validation-{normal-vs-reeds-heavy,normal-vs-movement-heavy,movement-heavy-vs-reeds-heavy}-10000-10299.jsonl.gz --json results/holdout-comparison-summary.json --markdown results/holdout-comparison-summary.md

The analyzer SHA-256 is
727819d3a95847430abf5c9a87e188aba03e5d6d989cc30430a0149300f9e96c.
It reported 1,800 matches in each separate profile summary and 3,600 in the
six-comparison holdout view.
