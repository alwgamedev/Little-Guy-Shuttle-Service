using System;
using UnityEngine;

namespace LGShuttle.Game
{
    [Serializable]
    public struct LevelParams
    {
        public int lgToSpawn;
        public int timeLimit;
        [Range(0, 1)] public float survivalRate;
    }
}