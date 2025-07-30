using LGShuttle.Game;
using TMPro;
using UnityEngine;

namespace LGShuttle.UI
{
    public class LevelTimerUI : HidableUI
    {
        [SerializeField] TextMeshProUGUI tmp;

        public void UpdateUI(ILevelTimer lt)
        {
            tmp.text = lt.FormattedTimeRemaining();
        }
    }
}