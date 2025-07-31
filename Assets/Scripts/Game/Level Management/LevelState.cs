namespace LGShuttle.Game
{

    public enum LevelCompletionResult
    {
        passed, failed, restart
    }

    public struct LevelState
    {
        public bool gameRunning;
        public LevelCompletionResult result;
        public int spawned;
        public int remaining;

        public float SurvivalRate => remaining / (float)spawned;
    }
}