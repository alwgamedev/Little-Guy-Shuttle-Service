namespace LGShuttle.Game
{
    public readonly struct LevelCompletionStats
    {
        public readonly int attempts;
        public readonly int completionBonus;
        public readonly float timeLimit;
        public readonly float timeRemaining;
        public readonly string formattedTimeRemaining;
        public readonly int timeBonus;
        public readonly int lgSpawned;
        public readonly int lgSurvived;
        public readonly int survivalPercent;
        public readonly int survivalBonus;
        public readonly int totalScore;
        public readonly int starRating;

        public LevelCompletionStats(ILevelManager levelManager)
        {
            attempts = levelManager.LevelState.attempts;
            completionBonus = levelManager.LevelParams.AdjustedCompletionBonus(attempts);
            timeLimit = levelManager.LevelParams.timeLimit;
            timeRemaining = levelManager.Timer.TimeRemaining;
            formattedTimeRemaining = levelManager.Timer.FormattedTimeRemaining();
            timeBonus = levelManager.LevelParams.TimeRemainingBonus(timeRemaining);
            lgSpawned = levelManager.LevelState.spawned;
            lgSurvived = levelManager.LevelState.remaining;
            var sr = levelManager.LevelState.SurvivalRate;
            survivalPercent = (int)(sr * 100);
            survivalBonus = levelManager.LevelParams.SurvivalRateBonus(sr);
            totalScore = completionBonus + timeBonus + survivalBonus;
            starRating = levelManager.LevelParams.StarRating(levelManager);
        }
    }
}