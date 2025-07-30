using Cysharp.Threading.Tasks;
using LGShuttle.Game;
using System;
using TMPro;
using UnityEngine;

namespace LGShuttle.UI
{
    public class RestartUI : HidableUI
    {
        [SerializeField] TextMeshProUGUI tmp;
        [SerializeField] float confirmTime;

        float confirmTimer;
        bool confirming;
        bool gameRunning;

        public event Action ConfirmedRestart;

        private void Start()
        {
            DisplayDefaultMessage();
        }

        private void Update()
        {
            if (!gameRunning) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                BeginConfirm();
            }
            else if (Input.GetKeyUp(KeyCode.R) && confirming)
            {
                CancelConfirm();
            }
            else if (confirming)
            {
                confirmTimer = Mathf.Max(confirmTimer - Time.deltaTime, 0);
                DisplayConfirmMessage();

                if (confirmTimer == 0)
                {
                    RestartConfirmed();
                }
            }
        }

        public void OnGameStarted(ILevelManager lm)
        {
            DisplayDefaultMessage();
            gameRunning = true;
        }

        public void OnGameEnded(ILevelManager lm)
        {
            gameRunning = false;
            if (confirming)
            {
                CancelConfirm();
            }
        }

        private void BeginConfirm()
        {
            confirmTimer = confirmTime;
            confirming = true;
        }

        private void CancelConfirm()
        {
            confirming = false;
            DisplayDefaultMessage();
        }

        private void RestartConfirmed()
        {
            confirming = false;
            ConfirmedRestart?.Invoke();
        }

        private void DisplayConfirmMessage()
        {
            tmp.text = $"HOLD R TO RESTART ({Mathf.Ceil(confirmTimer)})";
        }

        private void DisplayDefaultMessage()
        {
            tmp.text = "HOLD R TO RESTART";
        }

        private void OnDestroy()
        {
            ConfirmedRestart = null;
        }
    }
}