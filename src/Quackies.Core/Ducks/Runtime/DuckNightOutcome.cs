using System;

namespace Quackies.Core.Ducks.Runtime
{
    /// <summary>Frozen display and history breakdown from one ordered Night resolution.</summary>
    public sealed class DuckNightOutcome
    {
        internal DuckNightOutcome(
            int day,
            int printedSleep,
            int printedTwigs,
            int reedsTwigs,
            int eventTwigs,
            int bramblesPenalty,
            int flowerSleep,
            int finalHavenSleep,
            int restlessNightPenalty,
            int collectiveEventSleep,
            int flockSleep,
            int pebblesPenalty,
            int sleepBeforeWear,
            int frozenSleep,
            int totalTwigsEarned,
            int feathersAwarded,
            bool isMostRested,
            int nextDayTemporaryStep,
            int dreamTwigs)
        {
            ValidateNonNegative(printedSleep, nameof(printedSleep));
            ValidateNonNegative(printedTwigs, nameof(printedTwigs));
            ValidateNonNegative(reedsTwigs, nameof(reedsTwigs));
            ValidateNonNegative(eventTwigs, nameof(eventTwigs));
            ValidateNonNegative(bramblesPenalty, nameof(bramblesPenalty));
            ValidateNonNegative(flowerSleep, nameof(flowerSleep));
            ValidateNonNegative(finalHavenSleep, nameof(finalHavenSleep));
            ValidateNonNegative(restlessNightPenalty, nameof(restlessNightPenalty));
            ValidateNonNegative(collectiveEventSleep, nameof(collectiveEventSleep));
            ValidateNonNegative(flockSleep, nameof(flockSleep));
            ValidateNonNegative(pebblesPenalty, nameof(pebblesPenalty));
            ValidateNonNegative(sleepBeforeWear, nameof(sleepBeforeWear));
            ValidateNonNegative(frozenSleep, nameof(frozenSleep));
            ValidateNonNegative(totalTwigsEarned, nameof(totalTwigsEarned));
            ValidateNonNegative(feathersAwarded, nameof(feathersAwarded));
            ValidateNonNegative(dreamTwigs, nameof(dreamTwigs));
            if (day < 1 || day > DuckMatchSettings.StandardDays) throw new ArgumentOutOfRangeException(nameof(day));
            if (nextDayTemporaryStep < 0 || nextDayTemporaryStep > 1)
                throw new ArgumentOutOfRangeException(nameof(nextDayTemporaryStep));

            Day = day;
            PrintedSleep = printedSleep;
            PrintedTwigs = printedTwigs;
            ReedsTwigs = reedsTwigs;
            EventTwigs = eventTwigs;
            BramblesPenalty = bramblesPenalty;
            FlowerSleep = flowerSleep;
            FinalHavenSleep = finalHavenSleep;
            RestlessNightPenalty = restlessNightPenalty;
            CollectiveEventSleep = collectiveEventSleep;
            FlockSleep = flockSleep;
            PebblesPenalty = pebblesPenalty;
            SleepBeforeWear = sleepBeforeWear;
            FrozenSleep = frozenSleep;
            TotalTwigsEarned = totalTwigsEarned;
            FeathersAwarded = feathersAwarded;
            IsMostRested = isMostRested;
            NextDayTemporaryStep = nextDayTemporaryStep;
            DreamTwigs = dreamTwigs;
        }

        public int Day { get; }
        public int PrintedSleep { get; }
        public int PrintedTwigs { get; }
        public int ReedsTwigs { get; }
        public int EventTwigs { get; }
        public int BramblesPenalty { get; }
        public int FlowerSleep { get; }
        public int FinalHavenSleep { get; }
        public int RestlessNightPenalty { get; }
        public int CollectiveEventSleep { get; }
        public int FlockSleep { get; }
        public int PebblesPenalty { get; }
        public int SleepBeforeWear { get; }
        public int FrozenSleep { get; }
        public int TotalTwigsEarned { get; }
        public int FeathersAwarded { get; }
        public bool IsMostRested { get; }
        public int NextDayTemporaryStep { get; }
        public int DreamTwigs { get; }

        private static void ValidateNonNegative(int value, string parameterName)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}
