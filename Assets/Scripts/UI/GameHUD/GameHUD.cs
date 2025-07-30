using Cysharp.Threading.Tasks;
using LGShuttle.Core;
using LGShuttle.Game;
using LGShuttle.SceneManagement;
using System;
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
        LevelCompleteUI levelCompleteUI;

        public static event Action RequestRestart;
        //public static event Action RequestNextLevel;

        private void Awake()
        {
            timer = GetComponentInChildren<LevelTimerUI>();
            survivalCounter = GetComponentInChildren<SurvivalCounter>();
            restartUI = GetComponentInChildren<RestartUI>();
            levelCompleteUI = GetComponentInChildren<LevelCompleteUI>();
        }

        private void OnEnable()
        {
            LevelManager.LevelPrepared += OnLevelPrepared;
            LevelManager.GameStarted += OnGameStarted;
            LevelManager.LGDeath += OnLGDeath;
            LevelManager.GameEnded += OnGameEnded;
            restartUI.ConfirmedRestart += SendRestartRequest;
            levelCompleteUI.ContinueButton.onClick.AddListener(ContinueToNextLevel);
        }

        public void UpdateUI(ILevelManager levelManager)
        {
            UpdateTimer(levelManager.Timer);
            UpdateSurvivalCounter(levelManager.LevelParams, levelManager.LevelState);
        }

        public void UpdateTimer(ILevelTimer lt)
        {
            timer.UpdateUI(lt); 
        }

        public void UpdateSurvivalCounter(LevelParams p, LevelState s, bool animateDeath = false)
        {
            survivalCounter.UpdateUI(p, s, animateDeath);
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

        private void OnLGDeath(ILevelManager levelManager)
        {
            UpdateSurvivalCounter(levelManager.LevelParams, levelManager.LevelState, true);
        }

        private async void OnGameEnded(ILevelManager levelManager)
        {
            restartUI.OnGameEnded(levelManager);
            if (levelManager.LevelFailed)
            {
                var a = LevelFailedAnimation();
                var b = FadeOutPrimaryUI();
                await UniTask.WhenAll(a, b);
            }
            else
            {
                levelCompleteUI.UpdateUI(levelManager);
                levelCompleteUI.Show();
                await FadeOutPrimaryUI();
            }
        }

        private void SendRestartRequest()
        {
            RequestRestart?.Invoke();
        }

        private async void ContinueToNextLevel()
        {
            //RequestNextLevel?.Invoke();
            Debug.Log("load next level here!");
            levelCompleteUI.Hide();
            await SceneLoader.ReloadScene();
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
            LevelManager.LGDeath -= OnLGDeath;
            LevelManager.GameEnded -= OnGameEnded;
            restartUI.ConfirmedRestart -= SendRestartRequest;
            levelCompleteUI.ContinueButton.onClick.RemoveListener(ContinueToNextLevel);
        }
    }
}
