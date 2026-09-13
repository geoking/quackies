#!/usr/bin/env python3
"""Exact, dependency-free audit of the proposed Quackies opening bags.

The probability results treat draws as sampling without replacement and ask
what happens if a duck keeps drawing.  The stopping results use the separate,
conservative policy of placing the fifth obstacle and stopping immediately.
"""

from __future__ import annotations

import json
from fractions import Fraction
from itertools import combinations
from math import comb
from pathlib import Path


OUTPUT_PATH = Path(__file__).with_name("bag-analysis.json")
CHECKPOINT_DRAWS = (6, 8, 10)
SAFE_OBSTACLES = 5


def measure(value: Fraction, *, percent: bool = False) -> dict[str, object]:
    result: dict[str, object] = {
        "fraction": f"{value.numerator}/{value.denominator}",
        "decimal": round(float(value), 6),
    }
    if percent:
        result["percent"] = round(float(value) * 100, 3)
    return result


def formula_sixth_by_draw(obstacles: int, coloured: int, draws: int) -> Fraction:
    """Hypergeometric P(at least six obstacles among the first `draws`)."""
    total = obstacles + coloured
    denominator = comb(total, draws)
    return sum(
        (
            Fraction(comb(obstacles, white) * comb(coloured, draws - white), denominator)
            for white in range(6, min(obstacles, draws) + 1)
            if 0 <= draws - white <= coloured
        ),
        start=Fraction(0),
    )


def enumerate_binary_orders(obstacles: int, coloured: int) -> dict[str, object]:
    """Enumerate every obstacle/colour order as an independent exact check."""
    total = obstacles + coloured
    orders = list(combinations(range(total), obstacles))
    order_count = len(orders)

    risks: dict[int, Fraction] = {}
    for draws in CHECKPOINT_DRAWS:
        worn_orders = sum(
            sum(position < draws for position in obstacle_positions) >= 6
            for obstacle_positions in orders
        )
        risks[draws] = Fraction(worn_orders, order_count)

    stop_positions = [positions[SAFE_OBSTACLES - 1] + 1 for positions in orders]
    expected_placements = Fraction(sum(stop_positions), order_count)
    expected_helpful = expected_placements - SAFE_OBSTACLES

    return {
        "order_count": order_count,
        "risks": risks,
        "expected_placements": expected_placements,
        "expected_helpful": expected_helpful,
    }


def analyse_bag(name: str, obstacles: int, coloured: int) -> dict[str, object]:
    total = obstacles + coloured
    enumeration = enumerate_binary_orders(obstacles, coloured)

    # Negative-hypergeometric expectations for the position of obstacle five.
    formula_placements = Fraction(SAFE_OBSTACLES * (total + 1), obstacles + 1)
    formula_helpful = Fraction(SAFE_OBSTACLES * coloured, obstacles + 1)
    assert enumeration["expected_placements"] == formula_placements
    assert enumeration["expected_helpful"] == formula_helpful

    probability_output: dict[str, object] = {}
    for draws in CHECKPOINT_DRAWS:
        formula = formula_sixth_by_draw(obstacles, coloured, draws)
        assert enumeration["risks"][draws] == formula
        probability_output[str(draws)] = measure(formula, percent=True)

    return {
        "label": name,
        "composition": {
            "obstacles": obstacles,
            "coloured_helpful_chips": coloured,
            "total": total,
        },
        "raw_obstacle_fraction": measure(Fraction(obstacles, total), percent=True),
        "sixth_obstacle_probability_by_draw_if_continuing": probability_output,
        "stop_immediately_after_placing_fifth_obstacle": {
            "expected_helpful_draws": measure(formula_helpful),
            "expected_total_placements": measure(formula_placements),
            "obstacles_placed": SAFE_OBSTACLES,
        },
        "verification": {
            "method": "enumerated every distinct obstacle/colour order and matched hypergeometric and negative-hypergeometric formulas",
            "distinct_orders": enumeration["order_count"],
        },
    }


def starting_bag_distance() -> dict[str, object]:
    obstacles = 8
    coloured = 5
    expected_placements = Fraction(SAFE_OBSTACLES * (obstacles + coloured + 1), obstacles + 1)

    # A named coloured chip appears before obstacle five with probability 5/(W+1).
    tailwind_drawn = Fraction(SAFE_OBSTACLES, obstacles + 1)
    sensitivity: dict[str, object] = {}
    for tailwind_total_movement in (2, 4, 6):
        distance = expected_placements + tailwind_drawn * (tailwind_total_movement - 1)
        sensitivity[str(tailwind_total_movement)] = {
            "expected_distance": measure(distance),
            "fraction_of_50_space_trail": measure(distance / 50, percent=True),
        }

    return {
        "composition": [
            "8 Obstacles",
            "2 Seeds",
            "1 Tailwind 2",
            "1 Signpost",
            "1 Refreshing splash",
        ],
        "movement_model": "Every placed chip moves 1; Tailwind's total movement replaces its ordinary 1.",
        "probability_tailwind_is_placed_before_stopping": measure(tailwind_drawn, percent=True),
        "tailwind_total_movement_sensitivity": sensitivity,
        "opening_tailwind_2_result": sensitivity["2"],
    }


def build_report() -> dict[str, object]:
    return {
        "study": "Quackies obstacle-bag probability audit",
        "scope": {
            "days": 10,
            "playable_trail_spaces": 50,
            "purpose": "bounded opening-bag arithmetic, not a full-game balance simulation",
        },
        "assumptions": [
            "Draws are uniformly random without replacement.",
            "Every obstacle adds exactly 1 Exhaustion.",
            "Exhaustion 5 is safe; placing a sixth obstacle makes the duck worn out.",
            "The safe stopping policy places the fifth obstacle, then stops immediately.",
            "Helpful means any non-obstacle draw; it does not claim every coloured effect is beneficial in every state.",
            "Distance uses movement only and excludes effect chains, negative effects, recovery, voluntary stop strategy, World Events, Dream purchases and permanent Feather starting progress.",
        ],
        "bags": {
            "proposal_8_obstacles_5_coloured": analyse_bag(
                "Proposal: 8 Obstacles + 5 coloured", 8, 5
            ),
            "alternative_8_obstacles_7_coloured": analyse_bag(
                "Alternative: 8 Obstacles + 7 coloured", 8, 7
            ),
            "alternative_6_obstacles_5_coloured": analyse_bag(
                "Alternative: 6 Obstacles + 5 coloured", 6, 5
            ),
        },
        "starting_bag_distance": starting_bag_distance(),
        "design_readout": [
            "The proposal is 8/13 Obstacles (61.538%). If drawing continues, worn-out risk rises from 1.632% by draw 6 to 24.942% by draw 8 and 80.420% by draw 10.",
            "Stopping on obstacle five yields 25/9 (2.778) helpful draws and 70/9 (7.778) total placements on average.",
            "With ordinary movement 1 and Tailwind 2, the bounded opening estimate is 25/3 (8.333) spaces, one sixth of the 50-space trail. Later strong movement and/or Feather progression is therefore needed for deep-route access.",
            "The 8+7 alternative gives the safest draw-10 risk (42.657%) and most helpful draws (3.889) of these options. The 6+5 bag has lower early risk, but with only one obstacle left after the safe limit its draw-10 risk is 45.455%.",
            "A safe outing reaches five obstacle placements under this policy, so eight individually unique inconveniences would create frequent stacking, timing and explanation overhead. Exact obstacle effects and their stacking rules remain unresolved.",
        ],
        "unresolved_for_full_balance": [
            "The effects and movement totals of Seeds, Signpost and Refreshing splash.",
            "The eight obstacle inconveniences, including timing, duration and stacking.",
            "Recovery rules and any reason to stop before five Exhaustion.",
            "Dream supply, prices, later bag composition and Tailwind 4/6 acquisition pace.",
            "World Events, Feather gain cadence and the 50-space reward curve.",
        ],
    }


def main() -> None:
    report = build_report()
    OUTPUT_PATH.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {OUTPUT_PATH}")


if __name__ == "__main__":
    main()
