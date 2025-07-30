using static Unity.Burst.Intrinsics.X86.Avx;

namespace LGShuttle.Game
{
    public struct LevelState
    {
        public bool gameRunning;
        public int spawned;
        public int remaining;

        public float SurvivalRate => remaining / (float)spawned;
    }
}