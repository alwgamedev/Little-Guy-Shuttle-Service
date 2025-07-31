using LGShuttle.Game;
using LGShuttle.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LGShuttle.UI
{
    public class GameOverUI : HidableUI
    {
        [SerializeField] TextMeshProUGUI statsBody;
        [SerializeField] TextMeshProUGUI score;
        [SerializeField] Image[] stars;
        [SerializeField] Button mainMenuButton;

        public Button MainMenuButton => mainMenuButton;

        public void DisplayStats(CumulativeStats stats)
        {
            statsBody.text = StatsBody(stats);
            score.text = $"SCORE: {stats.TotalScore}";
            var r = stats.AverageStarRating;
            for (int i = 0; i < 5; i++)
            {
                stars[i].gameObject.SetActive(i < r);
            }
        }

        private string StatsBody(CumulativeStats stats)
        {
            return $"Levels Completed: {stats.LevelsCompleted}/{SceneLoader.NumLevels}\n"
            + $"Average Attempts per Level: {stats.AverageAttemptsPerLevel:0.00}\n"
            + $"Little Guys Transported: {stats.TotalLGTransported}\n"
            + $"Little Guy Souls Lost: {stats.TotalLGKilled}\n"
            + $"Average Survival Rate: {stats.AverageSurvivalPercent}% (successful missions only)\n"
            + $"Completion Time: {stats.FormattedCompletionTime} (successful missions only)";
        }
    }
}