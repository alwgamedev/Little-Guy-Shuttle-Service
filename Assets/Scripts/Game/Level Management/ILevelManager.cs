namespace LGShuttle.Game
{
    public interface ILevelManager
    {
        public ILevelTimer Timer { get; }
        public LevelParams LevelParams { get; }
        public LevelState LevelState { get; }
        public CumulativeStats CumulativeStats { get; }
    }
}