namespace LGShuttle.Game
{
    public struct LevelState
    {
        //public GameState gameState;
        public bool gameRunning;
        public int spawned;
        public int remaining;

        public float SurvivalRate => remaining / (float)spawned;
    }
}