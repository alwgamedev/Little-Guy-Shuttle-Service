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

        string[] completionBonusMessages = new string[] { "YEP!", "YAY!", "YES" };

        public Button ContinueButton => continueButton;

        public void UpdateUI(ILevelManager lm)
        {
            int total = 0;
            total += DisplayCompletionBonus(lm);
            total += DisplayTimeRemaining(lm);
            total += DisplaySurvivalRate(lm);
            DisplayScore(total);
            DisplayStarRating(lm.LevelParams.StarRating(total));
        }

        private int DisplayCompletionBonus(ILevelManager lm)
        {
            var i = MiscTools.rng.Next(completionBonusMessages.Length);
            int bonus = lm.LevelParams.completionBonus;
            completionBonus.text = $"Completion Bonus: {completionBonusMessages[i]}   (<b>+{bonus}</b> pts)";
            return bonus;
        }

        private int DisplayTimeRemaining(ILevelManager lm)
        {
            var formattedTime = lm.Timer.FormattedTimeRemaining();
            int bonus = lm.LevelParams.TimeRemainingBonus(lm.Timer.TimeRemaining);
            timeRemaining.text = $"Time Remaining: {formattedTime}   (<b>+{bonus}</b> pts)";
            return bonus;
        }

        private int DisplaySurvivalRate(ILevelManager lm)
        {
            float sr = lm.LevelState.SurvivalRate;
            int bonus = lm.LevelParams.SurvivalRateBonus(sr);
            survivalRate.text = $"Survival Rate: {(int)(sr * 100)}%   (<b>+{bonus}</b> pts)";
            return bonus;
        }

        private void DisplayScore(int points)
        {
            score.text = $"SCORE: {points} pts";
        }

        private void DisplayStarRating(int rating)
        {
            for (int i = 0; i < 5; i++)
            {
                stars[i].gameObject.SetActive(i < rating);
            }
        }
    }
}