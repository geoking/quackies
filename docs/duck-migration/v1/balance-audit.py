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
BOARD_JSON_PATH = Path(__file__).with_name("board.json")
SHOP_PATH = Path(__file__).with_name("shop.json")
SAFE_EXHAUSTION = 5
FIXED_RISK_DRAWS = 8
TRAIL_SPACES = 43
HAVENS = (4, 10, 16, 21, 26, 32, 36, 43)
EXPECTED_BIOME_COUNTS = {"wetlands": 14, "meadow": 14, "wasteland": 15}
DAWN_GIFT_CAP = 3
SUPPORTED_STARTING_FEATHERS = (0,)

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
    "tailwind_2": ("Tailwind 2", 4, 2, None),
    "tailwind_4": ("Tailwind 4", 8, 4, None),
    "tailwind_6": ("Tailwind 6", 12, 6, None),
    "signpost": ("Signpost", 7, 2, None),
    "splash": ("Refreshing Splash", 4, 1, None),
    "reeds_1": ("Reeds 1", 8, 1, 1),
    "reeds_2": ("Reeds 2", 14, 1, 2),
    "reeds_3": ("Reeds 3", 20, 1, 3),
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
    assert [int(row["space"]) for row in rows] == list(range(1, TRAIL_SPACES + 1))
    board = {
        int(row["space"]): {
            "biome": row["biome"],
            "sleep": int(row["sleep"]),
            "twigs": int(row["twigs"]),
            "haven": row["haven"].casefold() == "true",
            "feathers": int(row["feathers"]),
            "haven_name": row["haven_name"],
        }
        for row in rows
    }
    assert tuple(space for space, row in board.items() if row["haven"]) == HAVENS
    assert {
        biome: sum(row["biome"] == biome for row in board.values())
        for biome in EXPECTED_BIOME_COUNTS
    } == EXPECTED_BIOME_COUNTS
    assert all(board[space]["biome"] == "wetlands" for space in range(1, 15))
    assert all(board[space]["biome"] == "meadow" for space in range(15, 29))
    assert all(board[space]["biome"] == "wasteland" for space in range(29, 44))
    assert [(board[space]["sleep"], board[space]["twigs"]) for space in HAVENS] == [
        (6, 1),
        (10, 3),
        (13, 4),
        (15, 5),
        (16, 5),
        (18, 7),
        (20, 8),
        (21, 9),
    ]
    assert [board[space]["feathers"] for space in HAVENS] == [1, 1, 1, 1, 1, 2, 2, 2]
    assert sum(row["sleep"] for row in board.values()) == 469
    assert sum(row["twigs"] for row in board.values()) == 211
    assert board[HAVENS[0] - 1]["twigs"] == board[HAVENS[0]]["twigs"]
    assert board[HAVENS[0] + 1]["twigs"] == board[HAVENS[0]]["twigs"] + 1
    for space in HAVENS[1:-1]:
        assert board[space - 1]["twigs"] == board[space]["twigs"]
        assert board[space + 1]["twigs"] == board[space]["twigs"]
    assert board[TRAIL_SPACES]["twigs"] == board[TRAIL_SPACES - 1]["twigs"] + 1

    board_json = json.loads(BOARD_JSON_PATH.read_text(encoding="utf-8"))
    assert board_json["nest"] == {"index": 0, "scorable": False}
    assert board_json["rows"] == [
        {
            "space": space,
            "biome": row["biome"],
            "sleep": row["sleep"],
            "twigs": row["twigs"],
            "haven": row["haven"],
            "feathers": row["feathers"],
            "haven_name": row["haven_name"],
        }
        for space, row in board.items()
    ]

    shop = json.loads(SHOP_PATH.read_text(encoding="utf-8"))
    shop_prices = {offer["id"]: offer["sleep_price"] for offer in shop["offers"]}
    assert set(shop_prices) == set(EXPECTED_OFFERS)
    assert shop_prices == {
        offer_id: expected[1] for offer_id, expected in EXPECTED_OFFERS.items()
    }
    return board, shop_prices


def percent_text(value: Fraction) -> str:
    return f"{float(value) * 100:.3f}%"


def dawn_feathers_for_deficit(deficit: int) -> int:
    """Return the approved Dawn Delivery band for a frozen Twig deficit."""
    assert deficit >= 0
    if deficit <= 2:
        return 0
    if deficit <= 6:
        return 1
    if deficit <= 10:
        return 2
    return DAWN_GIFT_CAP


def affordability_readout(rows: dict[str, dict[str, object]]) -> str:
    parts = []
    for label, row in rows.items():
        probability = row["probability_affordable_from_opening_safe_rest"]
        parts.append(f"{label} at {row['price']} Sleep: {probability['percent']:.3f}%")
    return "Computed from the current board rows: " + "; ".join(parts) + "."


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

    reach_positions = tuple(sorted({first_haven, HAVENS[1], 6, 7, 8, 10, 11, 12, 13}))
    reach = {
        str(position): measure(probability_at_least(base, position), percent=True)
        for position in reach_positions
    }
    exact_landings = {
        str(position): measure(base.get(position, Fraction(0)), percent=True)
        for position in tuple(sorted({first_haven, HAVENS[1], 6, 7, 8, 10, 13}))
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
    ]["fraction"] == "2947/8580"
    assert opening_affordability["Signpost"][
        "probability_affordable_from_opening_safe_rest"
    ]["fraction"] == "1741/3276"
    assert opening_affordability["Tailwind 4"][
        "probability_affordable_from_opening_safe_rest"
    ]["fraction"] == "2947/8580"

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
    dawn_boundary_gaps = (0, 1, 2, 3, 6, 7, 10, 11, 2**31 - 1)
    for gap in dawn_boundary_gaps:
        gift = dawn_feathers_for_deficit(gap)
        static_dawn.append(
            {
                "twig_gap": gap,
                "feathers_each_dawn": gift,
                "feathers_over_5_unchanged_dawns": gift * 5,
                "feathers_over_9_unchanged_dawns": gift * 9,
            }
        )
    dawn_boundary_gifts = [0, 0, 0, 1, 1, 2, 2, 3, 3]
    assert [row["feathers_each_dawn"] for row in static_dawn] == dawn_boundary_gifts
    extreme_gifts = [
        dawn_feathers_for_deficit(8 * prior_days)
        for prior_days in range(1, 10)
    ]
    assert extreme_gifts == [2, 3, 3, 3, 3, 3, 3, 3, 3]
    assert sum(extreme_gifts) == 26

    # Analytic endpoint bound for the default zero-Feather setting.  It uses
    # only current encounter maxima, the mutually exclusive event cards and
    # board rows; it does not assume a stopping policy or favourable draws.
    starting_helpful_movement = sum(
        count * movement
        for _name, count, white, _log, _mud, movement in STARTING_TOKENS
        if not white and movement is not None
    )
    starting_seed_count = next(
        count for name, count, _white, _log, _mud, _movement in STARTING_TOKENS
        if name == "Seed"
    )
    day_1_safe_max_movement = (
        SAFE_EXHAUSTION + starting_helpful_movement + starting_seed_count
    )
    day_1_rain_max_movement_including_lethal_white = day_1_safe_max_movement + 1
    day_1_pocket_max_movement_including_lethal_white = (
        SAFE_EXHAUSTION + starting_helpful_movement + 1
    )
    maximum_purchased_movement = max(
        movement or 2 for _label, _price, movement, _reeds in EXPECTED_OFFERS.values()
    )
    day_2_safe_max_movement = day_1_safe_max_movement + maximum_purchased_movement
    first_two_feather_haven = min(
        position for position in HAVENS if board[position]["feathers"] == 2
    )
    prior_haven_days = 9
    later_prior_haven_days = prior_haven_days - 2
    dawns_before_or_on_day_10 = 9
    later_dawns = dawns_before_or_on_day_10 - 1
    temporary_most_rested_steps = 1

    setting_bound_rows = {}
    for setting in SUPPORTED_STARTING_FEATHERS:
        day_1_safe_reach = setting + day_1_safe_max_movement
        day_1_rain_max_rest = min(
            TRAIL_SPACES,
            setting + day_1_rain_max_movement_including_lethal_white,
        )
        day_1_pocket_max_rest = min(
            TRAIL_SPACES,
            setting + day_1_pocket_max_movement_including_lethal_white,
        )
        day_1_rain_max_twigs = max(
            board[position]["twigs"] for position in range(1, day_1_rain_max_rest + 1)
        )
        day_1_pocket_max_twigs = 1 + max(
            board[position]["twigs"] for position in range(1, day_1_pocket_max_rest + 1)
        )
        day_1_max_twigs = max(day_1_rain_max_twigs, day_1_pocket_max_twigs)
        day_2_max_dawn_gift = dawn_feathers_for_deficit(day_1_max_twigs)
        day_1_max_haven_feathers = max(
            board[position]["feathers"]
            for position in HAVENS
            if position <= day_1_safe_reach
        )
        day_2_effective_start = (
            setting
            + day_1_max_haven_feathers
            + day_2_max_dawn_gift
            + temporary_most_rested_steps
        )
        day_2_safe_reach = day_2_effective_start + day_2_safe_max_movement
        assert day_2_safe_reach < first_two_feather_haven
        day_2_max_haven_feathers = max(
            board[position]["feathers"]
            for position in HAVENS
            if position <= day_2_safe_reach
        )
        maximum_haven_feathers_before_day_10 = (
            day_1_max_haven_feathers
            + day_2_max_haven_feathers
            + later_prior_haven_days
            * max(board[position]["feathers"] for position in HAVENS)
        )
        maximum_dawn_feathers_before_day_10 = (
            day_2_max_dawn_gift + later_dawns * DAWN_GIFT_CAP
        )
        maximum_effective_start = (
            setting
            + maximum_haven_feathers_before_day_10
            + maximum_dawn_feathers_before_day_10
            + temporary_most_rested_steps
        )
        setting_bound_rows[str(setting)] = {
            "initial_feathers": setting,
            "day_1_safe_maximum_reach": day_1_safe_reach,
            "day_1_rain_maximum_final_position_including_lethal_sixth_white": day_1_rain_max_rest,
            "day_1_rain_maximum_printed_twigs_retained_when_worn_out": day_1_rain_max_twigs,
            "day_1_pocket_maximum_final_position_including_lethal_sixth_white": day_1_pocket_max_rest,
            "day_1_pocket_maximum_printed_plus_event_twigs": day_1_pocket_max_twigs,
            "day_1_maximum_twigs_for_day_2_dawn": day_1_max_twigs,
            "day_2_maximum_dawn_feathers": day_2_max_dawn_gift,
            "day_1_maximum_haven_feathers": day_1_max_haven_feathers,
            "day_2_effective_start_upper_bound": day_2_effective_start,
            "day_2_safe_reach_upper_bound": day_2_safe_reach,
            "day_2_maximum_haven_feathers": day_2_max_haven_feathers,
            "maximum_feathers_from_previous_haven_awards": maximum_haven_feathers_before_day_10,
            "maximum_feathers_from_dawn_gifts": maximum_dawn_feathers_before_day_10,
            "maximum_effective_start_before_day_10": maximum_effective_start,
            "proves_start_before_endpoint": maximum_effective_start < TRAIL_SPACES,
        }

    default_row = setting_bound_rows["0"]
    default_maximum_permanent_start = (
        default_row["maximum_feathers_from_previous_haven_awards"]
        + default_row["maximum_feathers_from_dawn_gifts"]
    )
    default_maximum_effective_start = default_row[
        "maximum_effective_start_before_day_10"
    ]
    assert starting_helpful_movement == 7
    assert day_1_safe_max_movement == 14
    assert day_2_safe_max_movement == 20
    assert first_two_feather_haven == 32
    assert list(setting_bound_rows) == ["0"]
    assert default_row["day_1_maximum_twigs_for_day_2_dawn"] == 4
    assert default_row["day_2_maximum_dawn_feathers"] == 1
    assert default_row["day_1_maximum_haven_feathers"] == 1
    assert default_row["day_2_effective_start_upper_bound"] == 3
    assert default_row["day_2_safe_reach_upper_bound"] == 23
    assert default_row["day_2_maximum_haven_feathers"] == 1
    assert default_row["maximum_feathers_from_previous_haven_awards"] == 16
    assert default_row["maximum_feathers_from_dawn_gifts"] == 25
    assert default_row["maximum_effective_start_before_day_10"] == 42
    assert default_maximum_permanent_start == 41
    assert default_maximum_effective_start == 42
    assert default_maximum_effective_start < TRAIL_SPACES

    default_start_bound = {
        "initial_feathers": 0,
        "day_1_safe_maximum_movement": day_1_safe_max_movement,
        "day_1_rain_maximum_final_position_including_lethal_sixth_white": default_row["day_1_rain_maximum_final_position_including_lethal_sixth_white"],
        "day_1_rain_maximum_printed_twigs_retained_when_worn_out": default_row["day_1_rain_maximum_printed_twigs_retained_when_worn_out"],
        "day_1_pocket_maximum_final_position_including_lethal_sixth_white": default_row["day_1_pocket_maximum_final_position_including_lethal_sixth_white"],
        "day_1_pocket_maximum_printed_plus_event_twigs": default_row["day_1_pocket_maximum_printed_plus_event_twigs"],
        "day_1_maximum_haven_feathers": default_row["day_1_maximum_haven_feathers"],
        "day_1_maximum_twigs_for_day_2_dawn": default_row["day_1_maximum_twigs_for_day_2_dawn"],
        "day_2_maximum_dawn_feathers": default_row["day_2_maximum_dawn_feathers"],
        "day_2_safe_maximum_movement_after_one_purchase": day_2_safe_max_movement,
        "supported_profile_day_2_effective_start_upper_bound": default_row["day_2_effective_start_upper_bound"],
        "supported_profile_day_2_safe_reach_upper_bound": default_row["day_2_safe_reach_upper_bound"],
        "first_two_feather_haven": first_two_feather_haven,
        "maximum_day_1_and_day_2_haven_feathers_each": 1,
        "maximum_feathers_from_previous_haven_awards": default_row["maximum_feathers_from_previous_haven_awards"],
        "maximum_feathers_from_dawn_gifts": default_row["maximum_feathers_from_dawn_gifts"],
        "maximum_permanent_feathers_before_day_10": default_maximum_permanent_start,
        "temporary_most_rested_steps": temporary_most_rested_steps,
        "maximum_effective_start_before_day_10": default_maximum_effective_start,
        "trail_spaces": TRAIL_SPACES,
        "proves_default_start_before_endpoint": default_maximum_effective_start < TRAIL_SPACES,
        "proof": "Day 1 can move at most 14 safely under Rain-Softened Seeds, so its haven award is at most 1 Feather. A lethal sixth white can then finish at 15; worn-out ducks retain that row's 4 Twigs. Under the mutually exclusive Pocket of Driftwood event there is no Rain movement: the lethal-white maximum is 13, whose printed reward is at most 3 Twigs, plus the event's 1. Thus every supported-profile Day 1 payout is at most 4 Twigs and the Day 2 Dawn gift is at most 1 under the 3-6 band. The zero-start profile's Day 2 effective start is at most 3, and it can move at most 20 safely after one purchase, reaching at most 23 before the first 2-Feather haven at 32. Both early haven awards are therefore at most 1. The remaining seven prior haven awards are at most 2 each and the remaining eight Dawn gifts are at most 3 each: 16 haven + 25 Dawn + 1 temporary = 42.",
    }

    cap_two = (
        package_result("Tailwind 4", shop_prices["tailwind_4"], (("Tailwind 4", 4),)),
        package_result("Seed + Signpost", shop_prices["seeds"] + shop_prices["signpost"], (("Seed", 1), ("Signpost", 2))),
    )
    cap_three = (
        package_result("Tailwind 6", shop_prices["tailwind_6"], (("Tailwind 6", 6),)),
        package_result(
            "Seed + Tailwind 2 + Signpost",
            shop_prices["seeds"] + shop_prices["tailwind_2"] + shop_prices["signpost"],
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
            "trail_spaces": TRAIL_SPACES,
            "supported_starting_feathers": list(SUPPORTED_STARTING_FEATHERS),
            "havens": list(HAVENS),
            "reference_inputs": [
                "docs/duck-migration/v1/board.csv",
                "docs/duck-migration/v1/board.json",
                "docs/duck-migration/v1/shop.json",
            ],
            "rules_reviewed": [
                "docs/duck-migration/ENCOUNTER_RULES.md",
                "docs/duck-migration/v1/WORLD_EVENTS.md",
            ],
            "classification": "exact bag arithmetic and counter-limited comparisons; not a full-game balance simulation",
        },
        "verification": {
            "arithmetic": "fractions.Fraction throughout; decimals are presentation-only",
            "opening_method": "memoized exhaustive traversal of every reachable multiset/counter state within the stated simplification that omits Splash blocking Log or Mud",
            "pressure_method": "exact hypergeometric counts without Goose and exhaustive ordered state traversal for Goose/Splash",
            "assertions": "The generator checks identical 43-row CSV/JSON board inputs, the 14/14/15 biome split, haven and endpoint rows, aggregate 469 Sleep/211 Twigs, shop prices, headline fractions and the analytic default-start bound before writing JSON.",
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
            "Dawn Delivery uses frozen Twig-deficit bands: 0-2 gives 0 Feathers, 3-6 gives 1, 7-10 gives 2, and 11 or more gives 3; examples intentionally hold gaps or daily reward extremes fixed to expose the payment boundary.",
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
                "probability_land_exactly_on_second_haven": exact_landings[str(HAVENS[1])],
                "computed_readout": f"Under this fixed safe-stop model, reach-or-pass is {percent_text(probability_at_least(base, first_haven))} for haven {first_haven} and {percent_text(probability_at_least(base, HAVENS[1]))} for haven {HAVENS[1]}; exact landing is {percent_text(base.get(first_haven, Fraction(0)))} and {percent_text(base.get(HAVENS[1], Fraction(0)))}, respectively.",
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
            "haven_twig_invariant": "First haven 4 matches space 3 at 1 Twig and space 5 begins the 2-Twig band. The other six nonendpoint havens match both adjacent spaces. Endpoint 43 is one Twig above space 42.",
        },
        "single_purchase_movement_checks": {
            "model_scope": "Exact within the same scoped traversal as opening travel. Splash does not block Log or Mud, and nonmovement utility is not valued; the Companion active-flock result therefore also applies every Mud even when Splash immediately precedes it.",
            "rows": purchase_rows,
        },
        "opening_night_affordability": {
            "rows": opening_affordability,
            "method": "Map each exact opening rest-position probability to that row's Sleep, before encounter, flock, Most Rested or World Event changes.",
            "computed_readout": affordability_readout(opening_affordability),
        },
        "purchase_cap_comparisons": {
            "two_purchase_tier_equal_budget_10": list(cap_two),
            "three_purchase_tier_equal_budget_15": list(cap_three),
            "note": "These compare the listed purchases under the same budget ceiling; actual Sleep spent can differ. Counter-limited movement is measured, but utility effects and unspent Sleep are not valued.",
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
            "rule": "Frozen Twig deficit 0-2 gives 0 Feathers; 3-6 gives 1; 7-10 gives 2; 11 or more gives 3",
            "static_gap_repetition": static_dawn,
            "candidate_reward_boundary": {
                "assumption": "One duck gains 9 Twigs and the other 1 each Day, so the persistent gap grows by the approved printed-board difference of 8 before each following dawn; the capped reward is shown through Day 10, while movement catch-up and the trail endpoint are deliberately ignored.",
                "gifts_on_days_2_through_10": extreme_gifts,
                "cumulative_feathers": sum(extreme_gifts),
            },
            "default_start_bound": default_start_bound,
            "supported_starting_feather_evidence": setting_bound_rows,
            "boundary_assertions": {
                "gaps_checked": list(dawn_boundary_gaps),
                "expected_gifts": dawn_boundary_gifts,
            },
            "endpoint_bound": "The supported zero-start profile is bounded to effective start 42 before endpoint 43. This audit does not add an endpoint clamp or another Feather rule.",
        },
        "limitations": [
            "The 43-row table is read for board assertions, haven metadata, opening affordability and the analytic default-start bound; later-day reward transitions and full purchase history are not simulated.",
            "No AI or human stopping policy is modelled; Signpost knowledge is not acted upon.",
            "No full Goose-Day movement distribution, World Event, Most Rested, Feather feedback or opponent interaction is modelled.",
            "Wildflowers, Splash and Signpost are priced here only for computed movement comparisons; their strategic utility is not assigned a numeric value.",
            "The 8.640 opening mean is exact only for the scoped traversal that always applies Log and Mud. It omits Splash blocking either ordinary nuisance and is not a complete-new-power bag prediction.",
            "The 21.825% positive final Companion flock result likewise omits Splash blocking Mud. It covers one added Companion within that simplification; multiple-Night flock growth needs a complete match simulation.",
            "The Dawn section proves only the supported zero-start profile starts before endpoint 43. Changed Feather sources, starting progress or round length require a fresh audit.",
        ],
    }


def main() -> None:
    report = build_report()
    OUTPUT_PATH.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {OUTPUT_PATH}")


if __name__ == "__main__":
    main()
