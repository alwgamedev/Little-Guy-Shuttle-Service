using LGShuttle.Game;
using TMPro;
using UnityEngine;
using System;

namespace LGShuttle.UI
{
    public class HoldForActionUI : HidableUI
    {
        [SerializeField] string actionName;
        [SerializeField] string keyName;
        [SerializeField] KeyCode keyCode;
        [SerializeField] TextMeshProUGUI tmp;
        [SerializeField] float confirmTime;

        float confirmTimer;
        bool confirming;
        bool listenToInput;

        string BaseMessage => $"HOLD {keyName.ToUpper()} TO {actionName.ToUpper()}";

        public event Action Confirmed;

        protected override void OnEnable()
        {
            base.OnEnable();

            LevelManager.LevelStarted += OnLevelStarted;
            LevelManager.LevelEnded += OnLevelEnded;
        }

        private void Start()
        {
            DisplayDefaultMessage();
        }

        private void Update()
        {
            if (!listenToInput) return;

            if (Input.GetKeyDown(keyCode))
            {
                BeginConfirm();
            }
            else if (Input.GetKey(keyCode) && confirming)
            {
                confirmTimer = Mathf.Max(confirmTimer - Time.deltaTime, 0);
                DisplayConfirmMessage();

                if (confirmTimer == 0)
                {
                    ActionConfirmed();
                }
            }
            else if (confirming)
            {
                CancelConfirm();
            }
        }

        public void OnLevelStarted(ILevelManager lm)
        {
            listenToInput = true;
            DisplayDefaultMessage();
            Update();
        }

        public void OnLevelEnded(ILevelManager lm)
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

        private void ActionConfirmed()
        {
            listenToInput = false;
            confirming = false;
            //Debug.Log("restart confirmed. restarting...");
            Confirmed?.Invoke();
        }

        private void DisplayConfirmMessage()
        {
            tmp.text = $"{BaseMessage} ({Mathf.Ceil(confirmTimer)})";
        }

        private void DisplayDefaultMessage()
        {
            tmp.text = BaseMessage;
        }

        private void OnDisable()
        {
            LevelManager.LevelStarted -= OnLevelStarted;
            LevelManager.LevelEnded -= OnLevelEnded;
        }

        private void OnDestroy()
        {
            Confirmed = null;
        }
    }
}