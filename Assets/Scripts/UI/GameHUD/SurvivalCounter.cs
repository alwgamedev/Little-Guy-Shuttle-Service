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

        Animation deathAnim;

        protected override void Awake()
        {
            base.Awake();
            deathAnim = GetComponentInChildren<Animation>();
        }

        public void UpdateUI(LevelParams p, LevelState s, bool animateDeath = false)
        {
            numerator.text = s.remaining.ToString();
            denominator.text = s.spawned.ToString();
            passCaption.text = $"PASS: {(int)(p.survivalRate * 100)}%";
            if (animateDeath)
            {
                deathAnim.Play();
            }
        }


    }
}