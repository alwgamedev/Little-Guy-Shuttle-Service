using LGShuttle.Core;
using NUnit.Framework.Constraints;
using System;
using UnityEngine;

namespace LGShuttle.Game
{
    [Serializable]
    public struct LevelParams
    {
        public int lgToSpawn;
        public int timeLimit;
        public int completionBonus;
        [Range(0, 1)] public float survivalRate;

        public int AdjustedCompletionBonus(int attempts)
        {
            if (attempts <= 7)
            {
                var f = 1 - (attempts / 7f);
                return (int)(f * completionBonus);
            }

            return 0;
        }

        public int TimeRemainingBonus(float timeRemaining)
        {
            return (int)(6 * timeRemaining);//6 pts per second remaining
        }

        public int SurvivalRateBonus(float survivalRate)
        {
            return (int)((survivalRate - this.survivalRate) * 900);//9 points per percentage point over required
        }

        public int StarRating(int score)
        {
            return MiscTools.rng.Next(5) + 1;//just for testing for now
        }
    }
}