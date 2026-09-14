#!/usr/bin/env python3
"""Generate the bounded Quackies v1 balance audit.

This is exact bag arithmetic, not a complete-game simulator.  It intentionally
keeps rules, state, and output independent of Unity and the AI policy.

Run from the repository root:

    python3 -B docs/duck-migration/v1/balance-audit.py
"""

from __future__ import annotations

from collections import defaultdict
import csv
from fractions import Fraction
from functools import lru_cache
from math import comb
import json
from pathlib import Path


OUTPUT_PATH = Path(__file__).with_name("balance-audit.json")
BOARD_PATH = Path(__file__).with_name("board.csv")
SHOP_PATH = Path(__file__).with_name("shop.json")
SAFE_EXHAUSTION = 5
FIXED_RISK_DRAWS = 8
HAVENS = (7, 13, 21, 27, 32, 38, 44, 50)

# name, count, white, Log, Mud, fixed movement; movement None is Companion.
STARTING_TOKENS = (
    ("Log", 2, True, True, False, 1),
    ("Mud", 2, True, False, True, 1),
    ("Pebbles or Brambles", 4, True, False, False, 1),
    ("Seed", 2, False, False, False, 1),
    ("Tailwind 2", 1, False, False, False, 2),
    ("Signpost", 1, False, False, False, 2),
    ("Refreshing Splash", 1, False, False, False, 1),
)

EXPECTED_OFFERS = {
    "seeds": ("Seed", 3, 1, None),
    "tailwind_2": ("Tailwind 2", 5, 2, None),
    "tailwind_4": ("Tailwind 4", 10, 4, None),
    "tailwind_6": ("Tailwind 6", 15, 6, None),
    "signpost": ("Signpost", 7, 2, None),
    "splash": ("Refreshing Splash", 4, 1, None),
    "reeds_1": ("Reeds 1", 6, 1, 1),
    "reeds_2": ("Reeds 2", 11, 1, 2),
    "reeds_3": ("Reeds 3", 16, 1, 3),
    "companion": ("Companion", 7, None, None),
    "wildflowers": ("Wildflowers", 5, 1, None),
}


def measure(value: Fraction, *, percent: bool = False) -> dict[str, object]:
    result: dict[str, object] = {
        "fraction": f"{value.numerator}/{value.denominator}",
        "decimal": round(float(value), 6),
    }
    if percent:
        result["percent"] = round(float(value) * 100, 3)
    return result


def load_reference_data() -> tuple[dict[int, dict[str, object]], dict[str, int]]:
    with BOARD_PATH.open(newline="", encoding="utf-8") as board_file:
        rows = list(csv.DictReader(board_file))
    assert [int(row["space"]) for row in rows] == list(range(1, 51))
    board = {
        int(row["space"]): {
            "biome": row["biome"],
            "sleep": int(row["sleep"]),
            "twigs": int(row["twigs"]),
            "haven": row["haven"] == "True",
            "feathers": int(row["feathers"]),
            "haven_name": row["haven_name"],
        }
        for row in rows
    }
    assert tuple(space for space, row in board.items() if row["haven"]) == HAVENS
    assert [(board[space]["sleep"], board[space]["twigs"]) for space in HAVENS] == [
        (8, 2),
        (10, 3),
        (13, 4),
        (15, 5),
        (16, 6),
        (18, 7),
        (20, 8),
        (21, 9),
    ]
    for space in HAVENS[:-1]:
        assert board[space - 1]["twigs"] == board[space]["twigs"]
        assert board[space + 1]["twigs"] == board[space]["twigs"]
    assert board[50]["twigs"] == board[49]["twigs"] + 1

    shop = json.loads(SHOP_PATH.read_text(encoding="utf-8"))
    shop_prices = {offer["id"]: offer["sleep_price"] for offer in shop["offers"]}
    assert set(shop_prices) == set(EXPECTED_OFFERS)
    assert shop_prices == {
        offer_id: expected[1] for offer_id, expected in EXPECTED_OFFERS.items()
    }
    return board, shop_prices


def movement_outcomes(
    additions: tuple[tuple[str, int | None], ...] = (),
) -> dict[tuple[int, int, int], Fraction]:
    """Return exact outcomes for the deliberately scoped movement model.

    Log slowing and Mud flock reduction apply whenever drawn.  This traversal
    intentionally omits Splash blocking the next Log or Mud nuisance; Splash is
    represented only by its fixed movement one here.  The separate Goose-risk
    traversal below does model an immediately preceding Splash shielding Goose.
    """
    entries = [list(token) for token in STARTING_TOKENS]
    for name, movement in additions:
        matched = False
        for entry in entries:
            if entry[0] == name and entry[5] == movement and not entry[2]:
                entry[1] += 1
                matched = True
                break
        if not matched:
            entries.append([name, 1, False, False, False, movement])

    metadata = tuple(
        (entry[0], entry[2], entry[3], entry[4], entry[5]) for entry in entries
    )
    starting_counts = tuple(entry[1] for entry in entries)

    @lru_cache(maxsize=None)
    def visit(
        counts: tuple[int, ...],
        exhaustion: int,
        log_pending: bool,
        active_flock: int,
        companions_placed: int,
    ) -> tuple[tuple[tuple[int, int, int], Fraction], ...]:
        if exhaustion == SAFE_EXHAUSTION:
            return (((0, active_flock, companions_placed), Fraction(1)),)

        total = sum(counts)
        combined: defaultdict[tuple[int, int, int], Fraction] = defaultdict(Fraction)
        for index, count in enumerate(counts):
            if count == 0:
                continue
            name, white, is_log, is_mud, fixed_movement = metadata[index]
            next_counts = list(counts)
            next_counts[index] -= 1
            next_exhaustion = exhaustion
            next_log = log_pending
            next_flock = active_flock
            next_placed = companions_placed

            if white:
                movement = 1
                next_exhaustion += 1
                if is_log:
                    next_log = True
                if is_mud:
                    next_flock = max(0, next_flock - 1)
            else:
                if name == "Companion":
                    next_flock += 1
                    next_placed += 1
                    raw_movement = min(4, next_flock + 1)
                else:
                    assert fixed_movement is not None
                    raw_movement = fixed_movement
                movement = (raw_movement + 1) // 2 if log_pending else raw_movement
                next_log = False

            branch_probability = Fraction(count, total)
            child = visit(
                tuple(next_counts),
                next_exhaustion,
                next_log,
                next_flock,
                next_placed,
            )
            for (distance, final_flock, placed), probability in child:
                combined[(movement + distance, final_flock, placed)] += (
                    branch_probability * probability
                )
        return tuple(sorted(combined.items()))

    outcomes = dict(visit(starting_counts, 0, False, 0, 0))
    assert sum(outcomes.values(), start=Fraction(0)) == 1
    return outcomes


def distance_distribution(
    additions: tuple[tuple[str, int | None], ...] = (),
) -> dict[int, Fraction]:
    distribution: defaultdict[int, Fraction] = defaultdict(Fraction)
    for (distance, _flock, _placed), probability in movement_outcomes(additions).items():
        distribution[distance] += probability
    return dict(sorted(distribution.items()))


def mean_distance(distribution: dict[int, Fraction]) -> Fraction:
    return sum(
        (distance * probability for distance, probability in distribution.items()),
        start=Fraction(0),
    )


def probability_at_least(distribution: dict[int, Fraction], position: int) -> Fraction:
    return sum(
        (probability for distance, probability in distribution.items() if distance >= position),
        start=Fraction(0),
    )


def quantile(distribution: dict[int, Fraction], target: Fraction) -> int:
    cumulative = Fraction(0)
    for distance, probability in distribution.items():
        cumulative += probability
        if cumulative >= target:
            return distance
    raise AssertionError("distribution does not reach requested quantile")


def purchase_row(
    label: str,
    price: int,
    movement: int | None,
    base_mean: Fraction,
    *,
    reeds: int | None = None,
) -> dict[str, object]:
    outcomes = movement_outcomes(((label, movement),))
    distribution: defaultdict[int, Fraction] = defaultdict(Fraction)
    for (distance, _flock, _placed), probability in outcomes.items():
        distribution[distance] += probability
    result: dict[str, object] = {
        "price": price,
        "expected_next_day_distance": measure(mean_distance(distribution)),
        "expected_distance_gain_over_opening_bag": measure(
            mean_distance(distribution) - base_mean
        ),
    }
    if reeds is not None:
        # A specified added colour precedes white five with probability 5/(8+1).
        result["expected_gross_pending_twigs_before_rest_nuisance"] = measure(
            Fraction(5 * reeds, 9)
        )
    if label == "Companion":
        expected_placed = sum(
            (placed * probability for (_distance, _flock, placed), probability in outcomes.items()),
            start=Fraction(0),
        )
        expected_final_flock = sum(
            (flock * probability for (_distance, flock, _placed), probability in outcomes.items()),
            start=Fraction(0),
        )
        final_flock_positive = sum(
            (
                probability
                for (_distance, flock, _placed), probability in outcomes.items()
                if flock > 0
            ),
            start=Fraction(0),
        )
        result["companion_counter_check"] = {
            "expected_companions_placed": measure(expected_placed),
            "expected_final_active_flock": measure(expected_final_flock),
            "probability_final_active_flock_is_positive": measure(
                final_flock_positive, percent=True
            ),
            "note": "Mud reduces the active flock for later movement and the Night contest; earlier movement is unchanged.",
        }
    return result


def no_goose_draw_risk(colours: int, draws: int = FIXED_RISK_DRAWS) -> Fraction:
    denominator = comb(8 + colours, draws)
    return sum(
        (
            Fraction(comb(8, whites) * comb(colours, draws - whites), denominator)
            for whites in range(6, 9)
            if 0 <= draws - whites <= colours
        ),
        start=Fraction(0),
    )


def goose_draw_risk_without_shield(
    colours: int, draws: int = FIXED_RISK_DRAWS
) -> Fraction:
    denominator = comb(9 + colours, draws)
    with_goose = sum(
        comb(8, whites) * comb(colours, draws - 1 - whites)
        for whites in range(4, 9)
        if 0 <= draws - 1 - whites <= colours
    )
    without_goose = sum(
        comb(8, whites) * comb(colours, draws - whites)
        for whites in range(6, 9)
        if 0 <= draws - whites <= colours
    )
    return Fraction(with_goose + without_goose, denominator)


def goose_draw_risk_with_splash(
    colours: int, draws: int = FIXED_RISK_DRAWS
) -> Fraction:
    """Exact ordered-draw risk with one Splash among `colours`."""

    @lru_cache(maxsize=None)
    def visit(
        ordinary: int,
        goose: int,
        splash: int,
        other_colours: int,
        exhaustion: int,
        safe_maximum: int,
        shield: bool,
        drawn: int,
    ) -> Fraction:
        if exhaustion > safe_maximum or drawn == draws:
            return Fraction(exhaustion > safe_maximum)
        total = ordinary + goose + splash + other_colours
        result = Fraction(0)
        if ordinary:
            result += Fraction(ordinary, total) * visit(
                ordinary - 1,
                goose,
                splash,
                other_colours,
                exhaustion + 1,
                safe_maximum,
                False,
                drawn + 1,
            )
        if goose:
            goose_maximum = safe_maximum if shield else 4
            result += Fraction(goose, total) * visit(
                ordinary,
                0,
                splash,
                other_colours,
                exhaustion + 1,
                goose_maximum,
                False,
                drawn + 1,
            )
        if splash:
            result += Fraction(splash, total) * visit(
                ordinary,
                goose,
                0,
                other_colours,
                exhaustion,
                safe_maximum,
                True,
                drawn + 1,
            )
        if other_colours:
            result += Fraction(other_colours, total) * visit(
                ordinary,
                goose,
                splash,
                other_colours - 1,
                exhaustion,
                safe_maximum,
                False,
                drawn + 1,
            )
        return result

    return visit(8, 1, 1, colours - 1, 0, 5, False, 0)


def package_result(
    label: str,
    total_price: int,
    additions: tuple[tuple[str, int], ...],
) -> dict[str, object]:
    distribution = distance_distribution(additions)
    return {
        "label": label,
        "total_price": total_price,
        "expected_next_day_distance": measure(mean_distance(distribution)),
    }


def build_report() -> dict[str, object]:
    board, shop_prices = load_reference_data()
    base = distance_distribution()
    base_mean = mean_distance(base)
    first_haven = HAVENS[0]

    assert base_mean == Fraction(389198, 45045)
    assert min(base) == 5 and max(base) == 12

    reach_positions = (6, 7, 8, 10, 12, 13)
    reach = {
        str(position): measure(probability_at_least(base, position), percent=True)
        for position in reach_positions
    }
    exact_landings = {
        str(position): measure(base.get(position, Fraction(0)), percent=True)
        for position in (6, 7, 8, 13)
    }

    purchase_rows = {
        label: purchase_row(
            label,
            price,
            movement,
            base_mean,
            reeds=reeds,
        )
        for label, price, movement, reeds in EXPECTED_OFFERS.values()
    }

    opening_affordability = {}
    for offer_id, (label, price, _movement, _reeds) in EXPECTED_OFFERS.items():
        assert shop_prices[offer_id] == price
        affordable = sum(
            (
                probability
                for position, probability in base.items()
                if board[position]["sleep"] >= price
            ),
            start=Fraction(0),
        )
        opening_affordability[label] = {
            "price": price,
            "probability_affordable_from_opening_safe_rest": measure(
                affordable, percent=True
            ),
        }
    assert opening_affordability["Seed"][
        "probability_affordable_from_opening_safe_rest"
    ]["fraction"] == "1/1"
    assert opening_affordability["Reeds 1"][
        "probability_affordable_from_opening_safe_rest"
    ]["fraction"] == "1096/1287"
    assert opening_affordability["Signpost"][
        "probability_affordable_from_opening_safe_rest"
    ]["fraction"] == "6749/13860"

    # Stable exact results used in the written design review.
    assert purchase_rows["Seed"]["expected_next_day_distance"]["fraction"] == "581615/63063"
    assert purchase_rows["Tailwind 2"]["expected_next_day_distance"]["fraction"] == "67740/7007"
    assert purchase_rows["Tailwind 4"]["expected_next_day_distance"]["fraction"] == "672740/63063"
    assert purchase_rows["Tailwind 6"]["expected_next_day_distance"]["fraction"] == "735820/63063"
    companion_check = purchase_rows["Companion"]["companion_counter_check"]
    assert companion_check["expected_companions_placed"]["fraction"] == "5/9"
    assert companion_check["probability_final_active_flock_is_positive"]["fraction"] == "55/252"

    pressure_rows = []
    for added_colours in range(6):
        colours = 5 + added_colours
        pressure_rows.append(
            {
                "added_colours": added_colours,
                "total_colours": colours,
                "pre_goose": measure(no_goose_draw_risk(colours), percent=True),
                "goose_without_splash_effect": measure(
                    goose_draw_risk_without_shield(colours), percent=True
                ),
                "goose_with_one_immediate_next_chip_splash": measure(
                    goose_draw_risk_with_splash(colours), percent=True
                ),
            }
        )
    assert pressure_rows[0]["pre_goose"]["fraction"] == "107/429"
    assert pressure_rows[0]["goose_without_splash_effect"]["fraction"] == "19/33"
    assert pressure_rows[0]["goose_with_one_immediate_next_chip_splash"]["fraction"] == "479/858"
    assert pressure_rows[3]["goose_with_one_immediate_next_chip_splash"]["fraction"] == "25541/97240"

    next_draw_hazard = Fraction(8, 13)
    sleep_examples = []
    for position in (6, 7, 8):
        sleep = board[position]["sleep"]
        retained = sleep // 2
        loss = sleep - retained
        sleep_examples.append(
            {
                "position": position,
                "kind": "haven" if position == first_haven else "ordinary",
                "safe_sleep": sleep,
                "worn_out_sleep": retained,
                "sleep_lost": loss,
                "safe_branch_sleep_gain_needed_to_break_even": measure(
                    Fraction(8 * loss, 5)
                ),
            }
        )

    static_dawn = []
    for gap in (0, 1, 4, 5, 8, 9, 12, 13, 20):
        gift = min(3, (gap + 3) // 4)
        static_dawn.append(
            {
                "twig_gap": gap,
                "feathers_each_dawn": gift,
                "feathers_over_5_unchanged_dawns": gift * 5,
                "feathers_over_9_unchanged_dawns": gift * 9,
            }
        )
    assert [row["feathers_each_dawn"] for row in static_dawn] == [0, 1, 1, 2, 2, 3, 3, 3, 3]
    assert min(3, (0 + 3) // 4) == 0
    assert min(3, (4 + 3) // 4) == 1
    assert min(3, (5 + 3) // 4) == 2
    assert min(3, (8 + 3) // 4) == 2
    assert min(3, (9 + 3) // 4) == 3
    assert min(3, (100 + 3) // 4) == 3
    extreme_gifts = [min(3, ((8 * prior_days) + 3) // 4) for prior_days in range(1, 10)]
    assert extreme_gifts == [2, 3, 3, 3, 3, 3, 3, 3, 3]
    assert sum(extreme_gifts) == 26
    default_start_bound = {
        "initial_feathers": 0,
        "previous_haven_awards": 9,
        "maximum_feathers_from_previous_haven_awards": 9 * 2,
        "dawn_gifts_before_day_10": 9,
        "maximum_feathers_from_dawn_gifts": 9 * 3,
        "maximum_permanent_feathers_before_day_10": 9 * 2 + 9 * 3,
        "temporary_most_rested_steps": 1,
        "maximum_effective_start_before_day_10": 9 * 2 + 9 * 3 + 1,
        "trail_spaces": 50,
        "proves_default_start_not_beyond_route": 9 * 2 + 9 * 3 + 1 < 50,
        "note": "Conservative bound overestimates reachable early gains under current Feather sources; nonzero starting Feathers add directly, and source changes or round-length changes require re-audit.",
    }
    assert default_start_bound["maximum_permanent_feathers_before_day_10"] == 45
    assert default_start_bound["maximum_effective_start_before_day_10"] == 46
    assert default_start_bound["proves_default_start_not_beyond_route"] is True

    cap_two = (
        package_result("Tailwind 4", 10, (("Tailwind 4", 4),)),
        package_result("Seed + Signpost", 10, (("Seed", 1), ("Signpost", 2))),
    )
    cap_three = (
        package_result("Tailwind 6", 15, (("Tailwind 6", 6),)),
        package_result(
            "Seed + Tailwind 2 + Signpost",
            15,
            (("Seed", 1), ("Tailwind 2", 2), ("Signpost", 2)),
        ),
    )
    assert cap_two[0]["expected_next_day_distance"]["fraction"] == "672740/63063"
    assert cap_two[1]["expected_next_day_distance"]["fraction"] == "58796/5733"
    assert cap_three[0]["expected_next_day_distance"]["fraction"] == "735820/63063"
    assert cap_three[1]["expected_next_day_distance"]["fraction"] == "20571/1820"

    return {
        "study": "Quackies v1 bounded balance audit",
        "generated_by": "python3 -B docs/duck-migration/v1/balance-audit.py",
        "scope": {
            "days": 10,
            "trail_spaces": 50,
            "havens": list(HAVENS),
            "reference_inputs": [
                "docs/duck-migration/v1/board.csv",
                "docs/duck-migration/v1/shop.json",
            ],
            "classification": "exact bag arithmetic and counter-limited comparisons; not a full-game balance simulation",
        },
        "verification": {
            "arithmetic": "fractions.Fraction throughout; decimals are presentation-only",
            "opening_method": "memoized exhaustive traversal of every reachable multiset/counter state within the stated simplification that omits Splash blocking Log or Mud",
            "pressure_method": "exact hypergeometric counts without Goose and exhaustive ordered state traversal for Goose/Splash",
            "assertions": "The generator checks settled board/shop inputs and all headline fractions before writing JSON.",
        },
        "assumptions": [
            "Draws are uniformly random without replacement.",
            "The opening travel model starts at the nest with no Feather or temporary starting progress and places the fifth white before stopping safely.",
            "The opening bag has two each of Log, Mud, Pebbles and Brambles; two Seeds; one Tailwind 2; one Signpost; and one Refreshing Splash.",
            "Every white moves one and adds one Exhaustion. Five Exhaustion is safe before an unshielded Goose resolves.",
            "Log halves the next coloured movement, rounding up with a minimum of one; whites do not consume it and repeated Logs do not stack.",
            "Mud subtracts one from the active flock, floor zero. This changes later Companion movement and the final flock used at Night, but never changes movement already placed.",
            "The rules contract says Splash protects only the immediately following chip's extra nuisance. It does not cancel a white's movement or Exhaustion; if that chip is Goose it prevents the safe-maximum reduction.",
            "The opening travel, purchase and Companion traversals deliberately do not model Splash blocking a following Log or Mud; they always apply those counters. The separate Goose pressure traversal does model immediate Splash protection from Goose's maximum reduction.",
            "Each purchase comparison adds the named chip to the bag. Signpost preview, voluntary stopping, World Events, ordinary rest nuisances and all effects not explicitly reported are excluded.",
            "Reeds expectations are gross pending Twigs before a possible final Brambles nuisance.",
            "Fixed draw-eight pressure means continued drawing and is not an estimate of a player's wear-out rate.",
            "Dawn Delivery applies min(3, ceil(Twig gap / 4)); examples intentionally hold gaps or daily reward extremes fixed to expose the payment boundary.",
        ],
        "opening_travel": {
            "model_scope": "Exact for the listed safe-stop, Log and Mud model, with ordinary Splash shielding omitted. It is not an exact prediction for the complete new-power starting bag.",
            "expected_distance": measure(base_mean),
            "median_distance": quantile(base, Fraction(1, 2)),
            "middle_50_percent": {
                "lower": quantile(base, Fraction(1, 4)),
                "upper": quantile(base, Fraction(3, 4)),
            },
            "minimum": min(base),
            "maximum": max(base),
            "probability_reach_or_pass_position": reach,
            "probability_land_exactly_on_position": exact_landings,
            "haven_readout": {
                "first_haven": first_haven,
                "probability_reach_or_pass": reach[str(first_haven)],
                "probability_land_exactly": exact_landings[str(first_haven)],
                "second_haven": HAVENS[1],
                "probability_reach_or_pass_second_haven": reach[str(HAVENS[1])],
                "note": "Only an exact landing receives that space's rest reward; reach-or-pass is a travel-depth measure.",
            },
            "known_early_sleep_rows": [
                {
                    "space": space,
                    "kind": "haven" if board[space]["haven"] else "ordinary",
                    "sleep": board[space]["sleep"],
                }
                for space in (6, 7, 8)
            ],
            "haven_rewards_reference": [
                {
                    "space": space,
                    "sleep": board[space]["sleep"],
                    "twigs": board[space]["twigs"],
                    "feathers": board[space]["feathers"],
                    "name": board[space]["haven_name"],
                }
                for space in HAVENS
            ],
            "haven_twig_invariant": "Each non-endpoint haven has the same Twig reward as both adjacent spaces. Endpoint 50 is the explicit exception at one Twig above space 49.",
        },
        "single_purchase_movement_checks": {
            "model_scope": "Exact within the same scoped traversal as opening travel. Splash does not block Log or Mud, and nonmovement utility is not valued; the Companion active-flock result therefore also applies every Mud even when Splash immediately precedes it.",
            "rows": purchase_rows,
        },
        "opening_night_affordability": {
            "rows": opening_affordability,
            "method": "Map each exact opening rest-position probability to that row's Sleep, before encounter, flock, Most Rested or World Event changes.",
            "readout": "Prices 3–5 are affordable after every modelled safe opening rest; price 6 is affordable 85.159% of the time, price 7 is affordable 48.694%, and price 10 or more is not reached in this opening model.",
        },
        "purchase_cap_comparisons": {
            "two_purchase_tier_equal_budget_10": list(cap_two),
            "three_purchase_tier_equal_budget_15": list(cap_three),
            "note": "These compare only the listed, actually priced purchases and their counter-limited movement; utility effects are not valued.",
        },
        "draw_8_wear_out_pressure": {
            "rows": pressure_rows,
            "readout": "Three added colours bring the Goose-plus-Splash controlled pressure to 26.266%, near the original opening bag's 24.942%; zero or one addition leaves a much larger Day 5 spike.",
        },
        "safe_versus_worn_out": {
            "average_unknown_next_draw_hazard_after_placing_fifth_white": measure(
                next_draw_hazard, percent=True
            ),
            "worn_out_keeps_twigs": True,
            "known_early_sleep_examples": sleep_examples,
            "break_even_assumption": "Holds the printed reward and Twig value fixed. If the next draw is safe, its extra Sleep value must offset the chance of floor(Sleep / 2); movement jumps, preview information and retained Twigs can change a real decision.",
        },
        "dawn_delivery": {
            "rule": "Feathers = min(3, ceil(Twig gap / 4)); a zero gap gives zero Feathers",
            "static_gap_repetition": static_dawn,
            "candidate_reward_boundary": {
                "assumption": "One duck gains 9 Twigs and the other 1 each Day, so the persistent gap grows by the approved printed-board difference of 8 before each following dawn; the capped reward is shown through Day 10, while movement catch-up and the trail endpoint are deliberately ignored.",
                "gifts_on_days_2_through_10": extreme_gifts,
                "cumulative_feathers": sum(extreme_gifts),
            },
            "default_start_bound": default_start_bound,
            "boundary_assertions": {
                "gaps_checked": [0, 4, 5, 8, 9, 100],
                "expected_gifts": [0, 1, 2, 2, 3, 3],
            },
            "boundary_requiring_explicit_rules": "The cap limits each Dawn Delivery to three Feathers. This audit does not add a settings limit or endpoint clamp; nonzero starting Feathers, source changes, or round-length changes require re-audit.",
        },
        "limitations": [
            "The 50-row table is read only for haven metadata and opening endpoint affordability; later-day reward transitions, full purchase history and finite supply are not simulated.",
            "No AI or human stopping policy is modelled; Signpost knowledge is not acted upon.",
            "No full Goose-Day movement distribution, World Event, Most Rested, Feather feedback or opponent interaction is modelled.",
            "Wildflowers, Splash and Signpost are priced here only for computed movement comparisons; their strategic utility is not assigned a numeric value.",
            "The 8.640 opening mean is exact only for the scoped traversal that always applies Log and Mud. It omits Splash blocking either ordinary nuisance and is not a complete-new-power bag prediction.",
            "The 21.825% positive final Companion flock result likewise omits Splash blocking Mud. It covers one added Companion within that simplification; multiple-Night flock growth needs a complete match simulation.",
            "The Dawn section audits the accepted three-Feather cap. Nonzero starting Feathers, changed Feather sources, or changed round length require re-audit.",
        ],
    }


def main() -> None:
    report = build_report()
    OUTPUT_PATH.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {OUTPUT_PATH}")


if __name__ == "__main__":
    main()
