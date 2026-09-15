using Quackies.Core.Ducks.Definitions;
using Quackies.Core.Ducks.Persistence;
using Quackies.Core.Ducks.Runtime;
using Quackies.Core.Match;
using Quackies.Evaluation.Policies;

namespace Quackies.Evaluation;

public enum EvaluationSchedule
{
    Cli,
    Reversed,
    AlternatingDay
}

public sealed record EvaluationPolicyInfo(string Id, string Description);
public sealed record EvaluationPolicyPair(EvaluationPolicyInfo PolicyA, EvaluationPolicyInfo PolicyB);
public sealed record EvaluationSeatAssignment(string Human, string Ai, bool Swapped);

public sealed record EvaluationMatchRequest(
    int Seed,
    int MatchIndex,
    IEvaluationPolicy PolicyA,
    IEvaluationPolicy PolicyB,
    bool Swapped,
    EvaluationSchedule Schedule,
    string SourceLabel,
    bool IncludeActions = false);

public sealed record EvaluationMatchResult(
    int SchemaVersion,
    string SourceLabel,
    string CoreAssemblyVersion,
    string ProfileId,
    int RulesVersion,
    int SaveFormatVersion,
    int Seed,
    int MatchIndex,
    EvaluationPolicyPair Policies,
    EvaluationSeatAssignment SeatAssignment,
    EvaluationSchedule Schedule,
    IReadOnlyList<EvaluationDayMetrics> Days,
    IReadOnlyList<EvaluationTimingSummary> Timings,
    EvaluationFinalMetrics Final,
    IReadOnlyList<EvaluationActionTrace>? Actions);

public sealed record EvaluationRunOutcome(EvaluationMatchResult Result, DuckSaveData FinalSave);

public sealed record EvaluationDayMetrics(
    int Day,
    string EventDefinitionId,
    int NestLevel,
    IReadOnlyList<EvaluationPlayerDayMetrics> Players);

public sealed record EvaluationPlayerDayMetrics(
    string PlayerId,
    string PolicyId,
    EvaluationDayStartMetrics Start,
    EvaluationAdventureMetrics Adventure,
    int PreNightTotalTwigs,
    int EndOfNightTotalTwigs,
    EvaluationNightMetrics Night,
    IReadOnlyList<EvaluationPurchaseMetrics> Purchases,
    int UnspentSleep,
    IReadOnlyDictionary<string, int> EndInventoryComposition,
    EvaluationNextDayStartMetrics? NextDayStart);

public sealed record EvaluationDayStartMetrics(
    int EffectiveStart,
    int PermanentTrail,
    bool ActiveTemporaryStep,
    int DawnTwigDeficit,
    int DawnFeathersAwarded,
    IReadOnlyDictionary<string, int> BagComposition);

public sealed record EvaluationNextDayStartMetrics(
    int EffectiveStart,
    int PermanentTrail,
    bool ActiveTemporaryStep,
    int DawnTwigDeficit,
    int DawnFeathersAwarded);

public sealed record EvaluationAdventureMetrics(
    int DrawCount,
    int MaxSpace,
    int RestSpace,
    DuckBiome Biome,
    bool IsHaven,
    bool IsOasis,
    bool IsSafe,
    bool IsWornOut,
    int Exhaustion,
    int SafeMaximum,
    GameActionKind FinishAction,
    string FinishReason);

public sealed record EvaluationNightMetrics(
    int Day,
    int PrintedSleep,
    int PrintedTwigs,
    int ReedsTwigs,
    int EventTwigs,
    int BramblesPenalty,
    int FlowerSleep,
    int FinalHavenSleep,
    int RestlessNightPenalty,
    int CollectiveEventSleep,
    int FlockSleep,
    int PebblesPenalty,
    int SleepBeforeWear,
    int FrozenSleep,
    int TotalTwigsEarned,
    int FeathersAwarded,
    bool IsMostRested,
    int NextDayTemporaryStep,
    int DreamTwigs);

public sealed record EvaluationPurchaseMetrics(
    string OfferDefinitionId,
    DuckEncounterType Type,
    int Price,
    string Reason,
    long PolicyElapsedMicroseconds);

public sealed record EvaluationTimingSummary(
    string PolicyId,
    int DecisionCount,
    long TotalPolicyMicroseconds,
    long MaxPolicyMicroseconds,
    long TotalExecuteMicroseconds,
    long MaxExecuteMicroseconds);

public sealed record EvaluationActionTrace(
    int Sequence,
    int Day,
    DuckPhase Phase,
    string PlayerId,
    string PolicyId,
    string ActionId,
    GameActionKind Kind,
    string DefinitionId,
    string Label,
    string Reason,
    long PolicyElapsedMicroseconds,
    long ExecuteElapsedMicroseconds,
    bool HiddenFinalDayCommit);

public sealed record EvaluationFinalMetrics(
    IReadOnlyList<string> WinnerIds,
    IReadOnlyList<EvaluationFinalStanding> Standings);

public sealed record EvaluationFinalStanding(
    string PlayerId,
    string PolicyId,
    int Rank,
    int TotalTwigs,
    int FrozenNightTenSleep,
    int DreamTwigs,
    bool IsWinner);
