#!/usr/bin/env python3
"""Independent geometry/data checks; does not infer paths or shelters from pixels."""
import csv
import json
import math
import statistics
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
LAYOUT = ROOT / "unity/Quackies.Unity/Assets/Art/DuckLayout/board-layout.json"
d = json.loads(LAYOUT.read_text())
rows = d["rows"]
guide = json.loads((HERE / "route-guide.json").read_text())["controlPoints"]
assert [r["space"] for r in rows] == list(range(1, 51))
assert len({r["id"] for r in rows}) == 50
points = [(r["x"] * d["boardWidth"], r["y"] * d["boardHeight"]) for r in rows]
segments = [math.dist(a, b) for a, b in zip(guide, guide[1:])]
arcs, deviations = [], []
for q in points:
    candidates, cumulative = [], 0.0
    for a, b, length in zip(guide, guide[1:], segments):
        v = (b[0] - a[0], b[1] - a[1])
        t = max(0, min(1, sum((q[k] - a[k]) * v[k] for k in (0, 1)) / length**2))
        closest = (a[0] + t * v[0], a[1] + t * v[1])
        candidates.append((math.dist(q, closest), cumulative + t * length))
        cumulative += length
    deviation, arc = min(candidates)
    deviations.append(deviation)
    arcs.append(arc)
intervals = [b - a for a, b in zip(arcs, arcs[1:])]
assert min(intervals) > 0, "Route doubles back or stacks spaces"
assert max(deviations) < 0.1, "A space left the measured centerline"
assert statistics.pstdev(intervals) / statistics.mean(intervals) < .12

# Full well and reward bounds, including both crossed well/reward cases.
# The one logical UI pixel gap is converted through the fitted board scale.
board_scale = min((1133 - 26) / d["boardWidth"], (744 - 57 - 5) / d["boardHeight"])
w, h, rh, gap = d["wellWidth"], d["wellHeight"], d["rewardHeight"], 1 / board_scale
rw, c = w * .86, (h + rh) / 2 + gap
minimum_clearance = float("inf")
for i, a in enumerate(points):
    for b in points[i + 1:]:
        dx, dy = abs(a[0] - b[0]), a[1] - b[1]
        clearance = min(max(dx - w, abs(dy) - h),
                        max(dx - rw, abs(dy) - rh),
                        max(dx - (w + rw) / 2, abs(dy - c) - (h + rh) / 2),
                        max(dx - (w + rw) / 2, abs(dy + c) - (h + rh) / 2))
        minimum_clearance = min(minimum_clearance, clearance)
assert minimum_clearance > .95, "Well/reward bounds touch or overlap"

havens = [r["space"] for r in rows if r["haven"]]
assert havens == [3, 11, 19, 27, 29, 37, 44, 50]
assert [r["space"] for r in rows if r["useBoardArt"]] == [50]
for n in havens[:-1]:
    before, haven, after = rows[n-2:n+1]
    assert before["twigs"] == haven["twigs"] == after["twigs"]
    assert haven["sleep"] > max(before["sleep"], after["sleep"])
assert (rows[-1]["sleep"], rows[-1]["twigs"], rows[-1]["feathers"]) == (21, 9, 2)
canonical = json.loads((HERE.parent / "v1/board.json").read_text())["rows"]
for row, reference in zip(rows, canonical):
    for key in ("space", "biome", "sleep", "twigs", "haven", "feathers"):
        assert row[key] == reference[key], (row["space"], key)
    assert row["havenName"] == reference["haven_name"]
with (HERE.parent / "v1/board.csv").open() as f:
    csv_rows = list(csv.DictReader(f))
assert len(csv_rows) == 50
for row, reference in zip(csv_rows, canonical):
    for key in ("space", "sleep", "twigs", "feathers"):
        assert int(row[key]) == reference[key]

# Thresholds describe the straight arms of the independently measured guide.
arms = {
    "wetlands": ([n+1 for n,(x,y) in enumerate(points) if x < 150 and 200 < y < 710],
                 [n+1 for n,(x,y) in enumerate(points) if 400 < x < 480 and 230 < y < 730]),
    "meadow": ([n+1 for n,(x,y) in enumerate(points) if 590 < x < 650 and 230 < y < 730],
               [n+1 for n,(x,y) in enumerate(points) if 900 < x < 960 and 210 < y < 730]),
    "wasteland": ([n+1 for n,(x,y) in enumerate(points) if 1060 < x < 1160 and 280 < y < 790],
                  [n+1 for n,(x,y) in enumerate(points) if x > 1340 and 250 < y < 770]),
}
assert all(len(a) == len(b) == 6 for a,b in arms.values())
result = {
    "status": "PASS",
    "spaces": 50, "wells": 49, "havens": havens,
    "armSpaces": {k: {"descending": a, "ascending": b} for k,(a,b) in arms.items()},
    "arclengthSpacingDesignPixels": {
        "min": min(intervals), "mean": statistics.mean(intervals),
        "max": max(intervals), "standardDeviation": statistics.pstdev(intervals)},
    "maximumCenterlineDeviation": max(deviations),
    "minimumWellRewardClearanceDesignPixels": minimum_clearance,
    "scope": "Numerical geometry and data consistency. Visual captures establish alignment with painted art; this is not gameplay or balance validation."
}
(HERE / "route-audit.json").write_text(json.dumps(result, indent=2) + "\n")
print(json.dumps(result, indent=2))
