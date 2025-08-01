using LGShuttle.Core;
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
        //[Range(0, 1)] public float survivalRate;

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
            return (int)(survivalRate * 400);//4 points per percentage point 
        }

        //1 star min
        //0-2 stars for time
        //0-2 stars for survival
        //max 5 stars
        public int StarRating(ILevelManager lm)
        {
            float total = 1;
            var timeF = lm.Timer.FractionTimeRemaining;
            var survivalF = lm.LevelState.SurvivalRate;

            timeF = Mathf.Clamp(timeF / .375f, 0, 2);
            survivalF = Mathf.Clamp(survivalF / .2f, 0, 2);
            return (int)(total + timeF + survivalF);
        }
    }
}