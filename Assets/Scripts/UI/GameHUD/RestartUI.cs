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
        bool listenToInput;

        public event Action ConfirmedRestart;

        private void Start()
        {
            DisplayDefaultMessage();
        }

        private void Update()
        {
            if (!listenToInput) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                BeginConfirm();
            }
            else if (Input.GetKey(KeyCode.R) && confirming)
            {
                confirmTimer = Mathf.Max(confirmTimer - Time.deltaTime, 0);
                DisplayConfirmMessage();

                if (confirmTimer == 0)
                {
                    RestartConfirmed();
                }
            }
            else if (confirming)
            {
                CancelConfirm();
            }
        }

        public void OnGameStarted(ILevelManager lm)
        {
            listenToInput = true;
            DisplayDefaultMessage();
            Update();
        }

        public void OnGameEnded(ILevelManager lm)
        {
            listenToInput = false;
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
            listenToInput = false;
            confirming = false;
            //Debug.Log("restart confirmed. restarting...");
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