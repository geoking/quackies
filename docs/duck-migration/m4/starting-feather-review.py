#!/usr/bin/env python3
"""Replay a concrete setting-3 endpoint collision under the approved v1 rules."""

from __future__ import annotations

from collections import Counter
import json
from pathlib import Path


ROOT = Path(__file__).resolve().parents[3]
BOARD_PATH = ROOT / "docs/duck-migration/v1/board.json"
SHOP_PATH = ROOT / "docs/duck-migration/v1/shop.json"
OUTPUT_PATH = Path(__file__).with_suffix(".json")
ENDPOINT = 43
STARTING_FEATHERS = 3

STARTING_BAG = Counter(
    {
        "Log": 2,
        "Mud": 2,
        "Pebbles": 2,
        "Brambles": 2,
        "Seed": 2,
        "Tailwind2": 1,
        "Signpost": 1,
        "Splash": 1,
    }
)
WHITE = {"Log", "Mud", "Pebbles", "Brambles", "Goose"}
HELPFUL_TYPE = {
    "Seed": "Seed",
    "Tailwind2": "Tailwind",
    "Tailwind6": "Tailwind",
    "Signpost": "Signpost",
    "Splash": "Splash",
    "Reeds1": "Reeds",
    "Reeds2": "Reeds",
}
MOVEMENT = {
    "Log": 1,
    "Mud": 1,
    "Pebbles": 1,
    "Brambles": 1,
    "Goose": 1,
    "Seed": 1,
    "Tailwind2": 2,
    "Tailwind6": 6,
    "Signpost": 2,
    "Splash": 1,
    "Reeds1": 1,
    "Reeds2": 1,
}
REEDS_TWIGS = {"Reeds1": 1, "Reeds2": 2}
FAMILY = {"Tailwind2": "Tailwind", "Tailwind6": "Tailwind", "Reeds1": "Reeds", "Reeds2": "Reeds"}

BASE_HELPFUL = ["Seed", "Seed", "Tailwind2", "Signpost", "Splash"]
SAFE_WHITES_FINAL_BRAMBLES = ["Log", "Log", "Mud", "Mud", "Brambles"]
SAFE_WHITES_FINAL_MUD = ["Log", "Log", "Brambles", "Pebbles", "Mud"]

EVENTS = [
    "A Pocket of Driftwood",
    "Thick Morning Mist",
    "Rain-Softened Seeds",
    "Restless Night",
    "A Friendly Guide",
    "All Tucked In",
    "Home Before Dark",
    "Still Air",
    "Shared Supper",
    "Sunlit Signboards",
]

TARGET_DRAWS = [
    ["Brambles"],
    BASE_HELPFUL + ["Tailwind2"] + SAFE_WHITES_FINAL_BRAMBLES,
    BASE_HELPFUL + ["Tailwind2", "Tailwind6"] + SAFE_WHITES_FINAL_BRAMBLES,
    ["Seed", "Seed", "Tailwind2", "Splash", "Tailwind2", "Tailwind6"]
    + SAFE_WHITES_FINAL_BRAMBLES,
    ["Mud", "Tailwind6", "Tailwind2", "Tailwind2", "Seed", "Brambles"],
    ["Tailwind6", "Seed", "Brambles"],
    ["Tailwind2", "Brambles"],
    ["Seed", "Brambles"],
    ["Tailwind2", "Seed", "Brambles"],
]
TARGET_RESTS = [4, 21, 32, 32, 32, 32, 32, 36, 43]
TARGET_PURCHASES = ["Tailwind2", "Tailwind6", None, None, None, None, None, None, None]

OPPONENT_REEDS = [[], ["Reeds1"]]
for count in range(2, 9):
    OPPONENT_REEDS.append(["Reeds1"] + ["Reeds2"] * (count - 1))
OPPONENT_DRAWS = [
    BASE_HELPFUL + ["Log", "Log", "Mud", "Mud", "Pebbles", "Pebbles"],
    BASE_HELPFUL + OPPONENT_REEDS[1] + SAFE_WHITES_FINAL_MUD,
    BASE_HELPFUL + OPPONENT_REEDS[2] + SAFE_WHITES_FINAL_MUD,
    BASE_HELPFUL + OPPONENT_REEDS[3] + SAFE_WHITES_FINAL_MUD,
    BASE_HELPFUL + OPPONENT_REEDS[4] + SAFE_WHITES_FINAL_MUD,
    BASE_HELPFUL + OPPONENT_REEDS[5] + SAFE_WHITES_FINAL_MUD,
    BASE_HELPFUL + OPPONENT_REEDS[6] + SAFE_WHITES_FINAL_MUD,
    BASE_HELPFUL + OPPONENT_REEDS[7] + SAFE_WHITES_FINAL_MUD,
    BASE_HELPFUL + OPPONENT_REEDS[8] + SAFE_WHITES_FINAL_MUD,
]
OPPONENT_RESTS = [16, 16, 20, 19, 20, 21, 23, 23, 25]
OPPONENT_PURCHASES = ["Reeds1"] + ["Reeds2"] * 8


def purchase_cap(night: int) -> int:
    return 1 if night <= 3 else 2 if night <= 6 else 3


def replay_draw(draws: list[str], event: str) -> dict[str, object]:
    exhaustion = 0
    distance = 0
    reeds_twigs = 0
    log_pending = False
    splash_armed = False
    guide_available = event == "A Friendly Guide"
    helpful_types: set[str] = set()
    final_bramble_suppressed = False
    final_pebbles_suppressed = False
    distance_after_each_draw = []

    for index, token in enumerate(draws):
        assert token in MOVEMENT
        protected = splash_armed
        splash_armed = False
        if token in WHITE:
            exhaustion += 1
            assert exhaustion <= 5 or index == len(draws) - 1
            if guide_available:
                protected = True
                guide_available = False
            if token == "Log" and not protected:
                log_pending = True
            if index == len(draws) - 1 and token == "Brambles":
                final_bramble_suppressed = protected
            if index == len(draws) - 1 and token == "Pebbles":
                final_pebbles_suppressed = protected
            distance += 1
            distance_after_each_draw.append(distance)
            continue

        movement = MOVEMENT[token]
        if event == "Rain-Softened Seeds" and token == "Seed":
            movement += 1
        if event == "Still Air" and token.startswith("Tailwind"):
            movement = (movement + 1) // 2
        if log_pending:
            if not (event == "Still Air" and token.startswith("Tailwind")):
                movement = max(1, (movement + 1) // 2)
            log_pending = False
        distance += movement
        reeds_twigs += REEDS_TWIGS.get(token, 0)
        helpful_types.add(HELPFUL_TYPE[token])
        if token == "Splash":
            splash_armed = True
        distance_after_each_draw.append(distance)

    return {
        "distance": distance,
        "distance_after_each_draw": distance_after_each_draw,
        "exhaustion": exhaustion,
        "safe": exhaustion <= 5,
        "reeds_twigs": reeds_twigs,
        "helpful_types": sorted(helpful_types),
        "pocket_twigs": int(event == "A Pocket of Driftwood" and len(helpful_types) >= 3),
        "final_token": draws[-1],
        "final_bramble_penalty": int(draws[-1] == "Brambles" and not final_bramble_suppressed),
        "final_pebbles_penalty": int(draws[-1] == "Pebbles" and not final_pebbles_suppressed),
    }


def main() -> None:
    board_rows = json.loads(BOARD_PATH.read_text(encoding="utf-8"))["rows"]
    board = {row["space"]: row for row in board_rows}
    shop = json.loads(SHOP_PATH.read_text(encoding="utf-8"))
    prices = {
        "Tailwind2": next(row["sleep_price"] for row in shop["offers"] if row["id"] == "tailwind_2"),
        "Tailwind6": next(row["sleep_price"] for row in shop["offers"] if row["id"] == "tailwind_6"),
        "Reeds1": next(row["sleep_price"] for row in shop["offers"] if row["id"] == "reeds_1"),
        "Reeds2": next(row["sleep_price"] for row in shop["offers"] if row["id"] == "reeds_2"),
    }
    assert len(board) == ENDPOINT
    assert [space for space, row in board.items() if row["haven"]] == [4, 10, 16, 21, 26, 32, 36, 43]
    assert len(EVENTS) == len(set(EVENTS)) == 10

    players = {
        "target": {
            "bag": STARTING_BAG.copy(),
            "trail": STARTING_FEATHERS,
            "twigs": 0,
            "temporary": 0,
            "draws": TARGET_DRAWS,
            "rests": TARGET_RESTS,
            "purchases": TARGET_PURCHASES,
        },
        "opponent": {
            "bag": STARTING_BAG.copy(),
            "trail": STARTING_FEATHERS,
            "twigs": 0,
            "temporary": 0,
            "draws": OPPONENT_DRAWS,
            "rests": OPPONENT_RESTS,
            "purchases": OPPONENT_PURCHASES,
        },
    }
    days = []
    for day, event in enumerate(EVENTS[:9], start=1):
        before = {name: player["twigs"] for name, player in players.items()}
        leader = max(before.values())
        for player in players.values():
            deficit = leader - player["twigs"]
            dawn = 0 if deficit == 0 else min(3, (deficit + 3) // 4)
            player["dawn"] = dawn
            player["trail"] += dawn
            player["effective_start"] = player["trail"] + player["temporary"]
            player["temporary"] = 0

        outcomes = {}
        for name, player in players.items():
            if day == 5:
                player["bag"]["Goose"] += 1
            draws = player["draws"][day - 1]
            assert all(count <= player["bag"][token] for token, count in Counter(draws).items())
            draw = replay_draw(draws, event)
            assert all(
                player["effective_start"] + distance < ENDPOINT
                for distance in draw["distance_after_each_draw"][:-1]
            )
            assert player["effective_start"] + draw["distance"] <= ENDPOINT
            rest = min(ENDPOINT, player["effective_start"] + draw["distance"])
            assert rest == player["rests"][day - 1]
            assert draw["safe"] is (name != "opponent" or day != 1)
            row = board[rest]
            today_twigs = max(
                0,
                row["twigs"]
                + draw["reeds_twigs"]
                + draw["pocket_twigs"]
                - draw["final_bramble_penalty"],
            )
            sleep = max(0, row["sleep"] - draw["final_pebbles_penalty"])
            haven_feathers = row["feathers"] if draw["safe"] and row["haven"] else 0
            outcomes[name] = {
                "effective_start": player["effective_start"],
                "draws": draws,
                **draw,
                "rest": rest,
                "printed_sleep": row["sleep"],
                "printed_twigs": row["twigs"],
                "today_twigs": today_twigs,
                "haven_feathers": haven_feathers,
                "sleep_before_collective_event": sleep,
            }

        all_safe = all(row["safe"] for row in outcomes.values())
        all_havens = all(board[row["rest"]]["haven"] for row in outcomes.values())
        all_seed = all("Seed" in row["helpful_types"] for row in outcomes.values())
        for name, outcome in outcomes.items():
            sleep = outcome["sleep_before_collective_event"]
            if event == "Restless Night" and outcome["safe"] and board[outcome["rest"]]["haven"]:
                sleep = max(0, sleep - 1)
            if event == "All Tucked In" and all_safe and all_havens:
                sleep += 2
            if event == "Home Before Dark" and all_safe:
                sleep += 1
            if event == "Shared Supper" and all_seed:
                sleep += 1
            if not outcome["safe"]:
                sleep //= 2
            outcome["retained_sleep"] = sleep
            players[name]["twigs"] += outcome["today_twigs"]
            players[name]["trail"] += outcome["haven_feathers"]
            outcome["cumulative_twigs"] = players[name]["twigs"]
            outcome["permanent_trail_after_night"] = players[name]["trail"]

        safe_sleep = {
            name: outcome["retained_sleep"]
            for name, outcome in outcomes.items()
            if outcome["safe"]
        }
        most_rested = [
            name for name, sleep in safe_sleep.items() if sleep == max(safe_sleep.values())
        ]
        for name in most_rested:
            players[name]["temporary"] = 1
        assert most_rested == ["target"]

        for name, player in players.items():
            purchase = player["purchases"][day - 1]
            if purchase is None:
                outcomes[name]["purchase"] = None
                continue
            assert purchase_cap(day) >= 1
            assert prices[purchase] <= outcomes[name]["retained_sleep"]
            assert sum(1 for value in [purchase] if value is not None) <= purchase_cap(day)
            player["bag"][purchase] += 1
            outcomes[name]["purchase"] = {
                "token": purchase,
                "family": FAMILY[purchase],
                "price": prices[purchase],
            }

        days.append(
            {
                "day": day,
                "event": event,
                "twig_scores_before_dawn": before,
                "dawn_feathers": {name: player["dawn"] for name, player in players.items()},
                "outcomes": outcomes,
                "most_rested": most_rested,
            }
        )

    def series(player: str, field: str) -> list[object]:
        return [day["outcomes"][player][field] for day in days]

    assert series("target", "effective_start") == [3, 7, 10, 14, 19, 24, 29, 34, 39]
    assert series("opponent", "effective_start") == [3, 3, 4, 4, 4, 4, 5, 5, 5]
    assert series("target", "distance") == [1, 14, 22, 18, 13, 8, 3, 2, 4]
    assert series("opponent", "distance") == [13, 13, 16, 15, 16, 17, 18, 18, 20]
    assert series("target", "rest") == TARGET_RESTS
    assert series("opponent", "rest") == OPPONENT_RESTS
    assert series("target", "exhaustion") == [1, 5, 5, 5, 2, 1, 1, 1, 1]
    assert series("opponent", "exhaustion") == [6, 5, 5, 5, 5, 5, 5, 5, 5]
    assert series("target", "today_twigs") == [0, 4, 6, 6, 6, 6, 6, 7, 8]
    assert series("opponent", "today_twigs") == [5, 5, 8, 9, 12, 14, 16, 18, 20]
    assert series("target", "cumulative_twigs") == [0, 4, 10, 16, 22, 28, 34, 41, 49]
    assert series("opponent", "cumulative_twigs") == [5, 10, 18, 27, 39, 53, 69, 87, 107]
    assert series("target", "haven_feathers") == [1, 1, 2, 2, 2, 2, 2, 2, 2]
    assert series("target", "retained_sleep") == [6, 15, 18, 17, 18, 20, 19, 20, 22]
    assert series("opponent", "retained_sleep") == [6, 13, 12, 11, 12, 17, 14, 13, 15]
    assert all(day["most_rested"] == ["target"] for day in days)

    before_day_10 = {name: player["twigs"] for name, player in players.items()}
    leader = max(before_day_10.values())
    day_10_dawn = {}
    for name, player in players.items():
        deficit = leader - player["twigs"]
        dawn = 0 if deficit == 0 else min(3, (deficit + 3) // 4)
        player["trail"] += dawn
        day_10_dawn[name] = dawn

    target = players["target"]
    target_effective_day_10 = target["trail"] + target["temporary"]
    assert [day["dawn_feathers"]["target"] for day in days] == [0, 2, 2, 2, 3, 3, 3, 3, 3]
    assert day_10_dawn == {"target": 3, "opponent": 0}
    assert target["trail"] == 43
    assert target_effective_day_10 == 44
    assert target_effective_day_10 >= ENDPOINT
    assert target["twigs"] == 49
    assert players["opponent"]["twigs"] == 107

    report = {
        "study": "Quackies M4 C1 starting-Feather reachability witness",
        "classification": "constructive positive-probability rules witness; not a balance simulation",
        "inputs": [
            "docs/duck-migration/v1/board.json",
            "docs/duck-migration/v1/shop.json",
            "docs/duck-migration/ENCOUNTER_RULES.md",
            "docs/duck-migration/v1/WORLD_EVENTS.md",
        ],
        "starting_feathers_shared": STARTING_FEATHERS,
        "event_order": EVENTS,
        "days_1_through_9": days,
        "day_10": {
            "event": EVENTS[9],
            "twig_scores_before_dawn": before_day_10,
            "dawn_feathers": day_10_dawn,
            "target_permanent_start": target["trail"],
            "target_temporary_most_rested_step": target["temporary"],
            "target_effective_start": target_effective_day_10,
            "endpoint": ENDPOINT,
            "collision": "The effective start is already past endpoint 43 before the required first draw.",
        },
        "conclusion": "Approved starting setting 3 has a reachable start-at/past-endpoint state under the current rules. A product rule decision is required; a loose upper bound is not the evidence for this conclusion.",
    }
    OUTPUT_PATH.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {OUTPUT_PATH}")


if __name__ == "__main__":
    main()
