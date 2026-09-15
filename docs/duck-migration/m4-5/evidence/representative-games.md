# Representative games on the approved prices

Sources: `docs/duck-migration/m4-5/evidence/holdout-normal-vs-normal-10000-10299.jsonl.gz` and `holdout-summary.json`. The records report `sourceLabel`/approved build `32b52333d49d99ecbce56dc25b91d490e30a74bc;Release;holdout-approved-prices`; the normal policy is the final AI candidate. I treated “original seat” as `seatAssignment.swapped == false`, where `human == policyA` and `ai == policyB`. These are AI selfplay examples, not human feedback. Pre-Night Twigs already include Reeds and events banked during adventure; the arrows are not full-Day gains. No reruns or causal claims.

## Three original-seat examples

The compact fields below are exact raw values: `event; player: drawCount/restSpace, isSafe/isWornOut, pre→end Twigs, sleepBeforeWear→frozenSleep, feathers, dreamTwigs, finishAction`. Raw paths are `days[i].eventDefinitionId`, `days[i].players[j].adventure.*`, `days[i].players[j].preNightTotalTwigs`, `...endOfNightTotalTwigs`, and `...night.*`.

1. **Comeback, seed 10042 (`matchIndex` 42):** On Day 5 the original-seat human trails by four Twigs (13–17), then finishes ahead 62–45. The per-Day outcome summary is:

   ```text
   D5 a_friendly_guide | human 5/10 true/false 10→13 10→10 F1 dream0 settle | AI 9/16 true/false 13→17 14→14 F1 dream0 settle
   D6 rain_softened_seeds | human 5/16 true/false 13→17 13→13 F1 dream0 settle | AI 9/18 false/true 17→20 11→5 F0 dream0 explore
   D7 sunlit_signboards | human 15/32 true/false 18→25 21→21 F2 dream0 settle | AI 4/10 true/false 21→24 9→9 F1 dream0 settle
   D8 still_air | human 8/21 true/false 28→33 15→15 F1 dream0 settle | AI 4/10 true/false 24→27 12→12 F1 dream0 settle
   D9 all_tucked_in | human 14/36 false/true 33→41 20→10 F0 dream0 explore | AI 11/20 false/true 30→34 12→6 F0 dream0 explore
   D10 a_pocket_of_driftwood | human 15/32 true/false 49→62 21→21 F2 dream6 settle | AI 15/31 false/true 38→45 11→5 F0 dream1 explore
   ```
   Final raw standings: `final.standings`: human `totalTwigs=62,frozenNightTenSleep=21,dreamTwigs=6,rank=1`; AI `45,5,1,rank=2`. Human Day 6 purchases were `companion` price 7 and `wildflowers` price 5; Day 7 `reeds_3` 16 and `splash` 4; Day 8 `reeds_2` 11 and `splash` 4; Day 9 `reeds_1` 6 and `splash` 4. AI Day 6 bought `wildflowers` 5; Day 7 `reeds_1` 6 and `seeds` 3; Day 8 `reeds_2` 11; Day 9 `reeds_1` 6. Purchases are raw `players[j].purchases[].{offerDefinitionId,price}`.

2. **Safe oasis arrival, seed 10006 (`matchIndex` 6):** On Day 10, event `shared_supper`, original-seat human reaches the safe oasis: `drawCount=19,restSpace=43,isHaven=true,isOasis=true,isSafe=true,isWornOut=false`, Twigs `49→66`, sleep `28→28`, `feathersAwarded=2`, `dreamTwigs=8`, `finishAction=explore`. AI that day is safe haven but not oasis: `11/21,true/false`, Twigs `46→54`, sleep `18→18`, F1, dream4, settle. Final standings are human 66/frozenSleep28 (winner) to AI 54/18. This is raw `days[9]` plus `final`.

3. **Tied final Twigs resolved by final Sleep, seed 10009 (`matchIndex` 9):** Day 10 `shared_supper`; human `drawCount=7,restSpace=21,isSafe=true,isWornOut=false`, Twigs `38→48`, sleep `18→18`, F1, dream5, settle. AI `8/21,true/false`, Twigs `39→48`, sleep `17→17`, F1, dream4, settle. `final.standings` ties `totalTwigs=48`, then ranks human first on `frozenNightTenSleep=18` versus 17. This is raw `days[9]` and `final.standings`.

## Wear-out rates and action fields

The aggregate `holdout-summary.json` normal-vs-normal record is `matchups[3]`; each event/day has 1,200 player-days (600 matches counting both original/swapped seats). `players[0].byDay[3]` (Day 4) reports `wornOut=110/1200 = 9.17%`; `players[0].byDay[4]` (Day 5) reports `88/1200 = 7.33%`. Day 5 also has the Goose in the recorded bag progression, so the observed day comparison is confounded by day and bag composition; no causation is inferred.

Per-event observed wear-out (`matchups[3].players[0].byEvent[].wornOut / playerDays`):

```text
a_friendly_guide       128/1200 = 10.67%
a_pocket_of_driftwood  180/1200 = 15.00%
all_tucked_in          126/1200 = 10.50%
home_before_dark       116/1200 =  9.67%
rain_softened_seeds    160/1200 = 13.33%
restless_night         178/1200 = 14.83%
shared_supper          130/1200 = 10.83%
still_air              150/1200 = 12.50%
sunlit_signboards      116/1200 =  9.67%
thick_morning_mist     142/1200 = 11.83%
```

The JSONL player object has no `optionalActions` field. The exact action-like fields available are `adventure.finishAction`/`finishReason` and `purchases[]` (offer ID, type, price, reason, policy timing), plus `unspentSleep`; no additional optional-action sequence can be reconstructed from this evidence.
