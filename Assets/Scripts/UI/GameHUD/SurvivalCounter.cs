using LGShuttle.Game;
using TMPro;
using UnityEngine;

namespace LGShuttle.UI
{
    public class SurvivalCounter : HidableUI
    {
        [SerializeField] TextMeshProUGUI numerator;
        [SerializeField] TextMeshProUGUI denominator;
        [SerializeField] TextMeshProUGUI passCaption;

        public void UpdateUI(LevelParams p, LevelState s)
        {
            numerator.text = s.remaining.ToString();
            denominator.text = s.spawned.ToString();
            passCaption.text = $"PASS: {(int)(p.survivalRate * 100)}%";
        }
    }
}