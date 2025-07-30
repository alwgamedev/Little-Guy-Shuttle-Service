using Cysharp.Threading.Tasks;
using LGShuttle.Core;
using LGShuttle.Game;
using System;
using System.Threading;
using UnityEngine;

namespace LGShuttle.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] float uiFadeTime = 1;
        [SerializeField] HidableUI levelFailedText;

        LevelTimerUI timer;
        SurvivalCounter survivalCounter;
        RestartUI restartUI;

        public static event Action RestartRequested;

        private void Awake()
        {
            timer = GetComponentInChildren<LevelTimerUI>();
            survivalCounter = GetComponentInChildren<SurvivalCounter>();
            restartUI = GetComponentInChildren<RestartUI>();
        }

        private void OnEnable()
        {
            LevelManager.LevelPrepared += OnLevelPrepared;
            LevelManager.GameStarted += OnGameStarted;
            LevelManager.LevelStateUpdate += OnLevelStateUpdate;
            LevelManager.GameEnded += OnGameEnded;
            restartUI.ConfirmedRestart += SendRestartRequest;
        }

        public void UpdateUI(ILevelManager levelManager)
        {
            UpdateTimer(levelManager.Timer.TimeRemaining);
            UpdateSurvivalCounter(levelManager.LevelParams, levelManager.LevelState);
        }

        public void UpdateTimer(float timeRemaining)
        {
            timer.UpdateUI(timeRemaining); 
        }

        public void UpdateSurvivalCounter(LevelParams p, LevelState s)
        {
            survivalCounter.UpdateUI(p, s);
        }

        private async void OnLevelPrepared(ILevelManager levelManager)
        {
            UpdateUI(levelManager);
            await FadeInPrimaryUI();
        }

        private void OnGameStarted(ILevelManager levelManager)
        {
            restartUI.OnGameStarted(levelManager);
            //e.g. "GO!" animation
        }

        private void OnLevelStateUpdate(ILevelManager levelManager)
        {
            UpdateUI(levelManager);
        }

        private async void OnGameEnded(ILevelManager levelManager)
        {
            restartUI.OnGameEnded(levelManager);
            //interpret result of game and display appropriate ui
            if (levelManager.LevelFailed)
            {
                var a = LevelFailedAnimation();
                var b = FadeOutPrimaryUI();
                await UniTask.WhenAll(a, b);
            }
            else
            {
                await FadeOutPrimaryUI();
            }
        }

        private void SendRestartRequest()
        {
            RestartRequested?.Invoke();
        }

        private async UniTask FadeInPrimaryUI()
        {
            var a = timer.FadeShow(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            var b = survivalCounter.FadeShow(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            var c = restartUI.FadeShow(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            await UniTask.WhenAll(a, b, c);
        }

        private async UniTask FadeOutPrimaryUI()
        {
            var a = timer.FadeHide(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            var b = survivalCounter.FadeHide(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            var c = restartUI.FadeHide(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            await UniTask.WhenAll(a, b, c);
        }

        public async UniTask LevelFailedAnimation()
        {
            await levelFailedText.FadeShow(uiFadeTime / 3, GlobalGameTools.Instance.CTS.Token);
            await MiscTools.DelayGameTime(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            await levelFailedText.FadeHide(uiFadeTime / 3, GlobalGameTools.Instance.CTS.Token);
        }

        private void OnDisable()
        {
            LevelManager.LevelPrepared -= OnLevelPrepared;
            LevelManager.GameStarted -= OnGameStarted;
            LevelManager.LevelStateUpdate -= OnLevelStateUpdate;
            LevelManager.GameEnded -= OnGameEnded;
            restartUI.ConfirmedRestart -= SendRestartRequest;
        }
    }
}
