using LGShuttle.Core;
using LGShuttle.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LGShuttle.UI
{
    public class LevelCompleteUI : HidableUI
    {
        [SerializeField] Button continueButton;
        [SerializeField] TextMeshProUGUI completionBonus;
        [SerializeField] TextMeshProUGUI timeRemaining;
        [SerializeField] TextMeshProUGUI survivalRate;
        [SerializeField] TextMeshProUGUI score;
        [SerializeField] Image[] stars;

        public Button ContinueButton => continueButton;

        public void UpdateUI(ILevelManager lm)
        {
            var stats = lm.LevelState.Stats;
            DisplayCompletionBonus(stats);
            DisplayTimeRemaining(stats);
            DisplaySurvivalRate(stats);
            DisplayScore(stats);
            DisplayStarRating(stats);
        }

        private void DisplayCompletionBonus(LevelCompletionStats stats)
        {
            completionBonus.text = $"Total Attempts: {stats.attempts}  (<b>+{stats.completionBonus}</b> pts)";
        }

        private void DisplayTimeRemaining(LevelCompletionStats stats)
        {
            timeRemaining.text = $"Time Remaining: {stats.formattedTimeRemaining}  (<b>+{stats.timeBonus}</b> pts)";
        }

        private void DisplaySurvivalRate(LevelCompletionStats stats)
        {
            survivalRate.text = $"Survival Rate: {stats.survivalPercent}%  (<b>+{stats.survivalBonus}</b> pts)";
        }

        private void DisplayScore(LevelCompletionStats stats)
        {
            score.text = $"SCORE: {stats.totalScore} pts";
        }

        private void DisplayStarRating(LevelCompletionStats stats)
        {
            int rating = stats.starRating;
            for (int i = 0; i < 5; i++)
            {
                stars[i].gameObject.SetActive(i < rating);
            }
        }
    }
}