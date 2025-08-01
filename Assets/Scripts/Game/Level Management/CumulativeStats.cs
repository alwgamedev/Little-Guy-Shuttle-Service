using LGShuttle.SceneManagement;

namespace LGShuttle.Game
{
    public struct CumulativeStats
    {
        public int TotalAttempts { get; private set; }
        public int LevelsCompleted { get; private set; }
        public int TotalLGKilled { get; private set; }
        public int TotalLGKilledOnPassedLevels { get; private set; }
        public int TotalLGTransported { get; private set; }
        public float CompletionTime { get; private set; }
        //public float TotalTimeLimit { get; private set; }
        //public float TotalTimeRemaining { get; private set; }
        public int TotalScore { get; private set; }
        public int TotalStars { get; private set; }

        public float AverageAttemptsPerLevel
        {
            get
            {
                //if ended game without completing all levels, then we should include last attempted level
                //in the denominator
                float l = LevelsCompleted < SceneLoader.NumLevels ? LevelsCompleted + 1 : LevelsCompleted;
                return TotalAttempts / l;
            }
        }

        public int AverageSurvivalPercent
        {
            get
            {
                float spawned = TotalLGKilledOnPassedLevels + TotalLGTransported;
                if (spawned == 0)
                {
                    return 0;
                }

                return (int)(100 * TotalLGTransported / spawned);
            }
        }

        public float AverageStarRating
        {
            get
            {
                if (LevelsCompleted == 0)
                {
                    return 5;
                }

                return TotalStars / (float)(5 * LevelsCompleted);
            }
        }

        public string FormattedCompletionTime
        {
            get
            {
                var totalSeconds = (int)CompletionTime;
                var hrs = totalSeconds / 3600;
                totalSeconds -= 3600 * hrs;
                var mins = totalSeconds / 60;
                totalSeconds -= mins * 60;
                if (hrs > 0)
                {
                    return $"{hrs:00}:{mins:00}:{totalSeconds:00}";
                }

                return $"{mins:00}:{totalSeconds:00}";
            }
        }

        public void OnLevelPassed(LevelCompletionStats stats)
        {
            TotalAttempts++;
            LevelsCompleted++;
            var killed = stats.lgSpawned - stats.lgSurvived;
            TotalLGKilled += killed;
            TotalLGKilledOnPassedLevels += killed;
            TotalLGTransported += stats.lgSurvived;
            CompletionTime += stats.timeLimit - stats.timeRemaining;
            //TotalTimeLimit += stats.timeLimit;
            //TotalTimeRemaining += stats.timeRemaining;
            TotalScore += stats.totalScore;
            TotalStars += stats.starRating;
        }

        public void OnLevelFailed(ILevelManager lm)
        {
            TotalAttempts++;
            TotalLGKilled += lm.LevelState.spawned - lm.LevelState.remaining;
        }
    }
}