namespace LGShuttle.Game
{
    public interface ILevelTimer
    {
        public float TimeLimit { get; }
        public float TimeRemaining { get; }
        public float FractionTimeRemaining { get; }

        public string FormattedTimeRemaining();
    }
}