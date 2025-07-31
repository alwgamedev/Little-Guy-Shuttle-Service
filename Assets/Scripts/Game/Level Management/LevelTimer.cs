using LGShuttle.UI;
using System;
using UnityEngine;

namespace LGShuttle.Game
{
    public class LevelTimer : MonoBehaviour, ILevelTimer
    {
        float timeRemaining;

        public float TimeLimit { get; private set; }
        public float TimeRemaining
        {
            get => timeRemaining;
            private set
            {
                timeRemaining = value;
                UICanvas.Instance.GameHUD.UpdateTimer(this);
            }
        }
        public bool Running { get; private set; }   

        public event Action TimedOut;

        public void Update()
        {
            if (Running)
            {
                TimeRemaining = Mathf.Max(TimeRemaining - Time.deltaTime, 0);
                if (TimeRemaining <= 0)
                {
                    TimeOut();
                }
            }
        }

        public string FormattedTimeRemaining()
        {
            var mins = (int)(timeRemaining / 60);
            var secs = (int)timeRemaining % 60;
            return $"{mins:00}:{secs:00}";
        }

        public void SetTimer(float timeLimit)
        {
            if (Running)
            {
                StopTimer();
            }
            TimeLimit = timeLimit;
            TimeRemaining = TimeLimit;
        }

        public void StartTimer()
        {
            Running = true;
        }

        public void StopTimer()
        {
            Running = false;
        }

        public void ResetTimer()
        {
            Running = false;
            TimeLimit = 0;
            TimeRemaining = 0;
        }

        private void TimeOut()
        {
            StopTimer();
            TimedOut?.Invoke();
        }
    }
}