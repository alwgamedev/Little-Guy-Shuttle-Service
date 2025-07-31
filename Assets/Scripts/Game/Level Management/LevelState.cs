namespace LGShuttle.Game
{

    public enum LevelCompletionResult
    {
        passed, failed, restart, quit
    }

    public struct LevelState
    {
        public bool gameRunning;
        public LevelCompletionResult result;
        public int spawned;
        public int remaining;
        public int attempts;

        public LevelCompletionStats Stats { get; private set; }

        public float SurvivalRate => remaining / (float)spawned;

        public void CalculateStats(ILevelManager levelManager)
        {
            Stats = new LevelCompletionStats(levelManager);
        }
    }
}