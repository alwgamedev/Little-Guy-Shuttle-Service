using TMPro;
using UnityEngine;

namespace LGShuttle.UI
{
    public class LevelTimerUI : HidableUI
    {
        [SerializeField] TextMeshProUGUI tmp;

        public void UpdateUI(float timeRemaining)
        {
            var mins = (int)(timeRemaining / 60);
            var secs = (int)timeRemaining % 60;
            string s = secs < 10 ? '0' + secs.ToString() : secs.ToString();
            tmp.text = $"{mins}:{s}";
        }
    }
}