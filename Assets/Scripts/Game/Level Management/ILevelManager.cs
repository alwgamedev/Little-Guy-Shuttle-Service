namespace LGShuttle.Game
{
    public interface ILevelManager
    {
        public LevelTimer Timer { get; }
        public LevelParams LevelParams { get; }
        public LevelState LevelState { get; }
        public bool LevelFailed => Timer.TimeRemaining <= 0 || LevelState.SurvivalRate < LevelParams.survivalRate;
    }
}