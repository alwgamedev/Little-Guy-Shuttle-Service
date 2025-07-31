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
        [SerializeField] HoldForActionUI restartUI;
        [SerializeField] HoldForActionUI escToMenuUI;
        //[SerializeField] HidableUI controlsText;

        LevelTimerUI timer;
        SurvivalCounter survivalCounter;
        LevelCompleteUI levelCompleteUI;
        GameOverUI gameOverUI;
        CumulativeStats lastStatsSent;

        public static event Action RequestRestart;
        public static event Action RequestQuit;
        //public static event Action RequestNextLevel;

        private void Awake()
        {
            timer = GetComponentInChildren<LevelTimerUI>();
            survivalCounter = GetComponentInChildren<SurvivalCounter>();
            levelCompleteUI = GetComponentInChildren<LevelCompleteUI>();
            gameOverUI = GetComponentInChildren<GameOverUI>();
        }

        private void OnEnable()
        {
            LevelManager.LevelPrepared += OnLevelPrepared;
            LevelManager.LevelStarted += OnLevelStarted;
            LevelManager.LGDeath += OnLGDeath;
            LevelManager.LevelEnded += OnLevelEnded;
            restartUI.Confirmed += SendRestartRequest;
            escToMenuUI.Confirmed += SendAbandonRequest;
            levelCompleteUI.ContinueButton.onClick.AddListener(LevelCompleteUIButtonHandler);
            gameOverUI.MainMenuButton.onClick.AddListener(GameOverUIButtonHandler);
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
            await MiscTools.DelayGameTime(SceneLoader.sceneFadeTime, GlobalGameTools.Instance.CTS.Token);
            await FadeInPrimaryUI();
        }

        private void OnLevelStarted(ILevelManager levelManager)
        {
            //e.g. "GO!" animation
        }

        private void OnLGDeath(ILevelManager levelManager)
        {
            UpdateSurvivalCounter(levelManager.LevelParams, levelManager.LevelState, true);
        }

        private async void OnLevelEnded(ILevelManager levelManager)
        {
            lastStatsSent = levelManager.CumulativeStats;

            switch(levelManager.LevelState.result)
            {
                case LevelCompletionResult.passed:
                    levelCompleteUI.UpdateUI(levelManager);
                    levelCompleteUI.Show();
                    await FadeOutPrimaryUI();
                    break;
                case LevelCompletionResult.failed:
                    var a = LevelFailedAnimation();
                    var b = FadeOutPrimaryUI();
                    await UniTask.WhenAll(a, b);
                    break;
                case LevelCompletionResult.restart:
                    await FadeOutPrimaryUI();
                    break;
                case LevelCompletionResult.quit:
                    await FadeOutPrimaryUI();
                    await ShowGameOverUI();
                    break;
            }
        }

        private void SendRestartRequest()
        {
            RequestRestart?.Invoke();
        }

        private void SendAbandonRequest()
        {
            RequestQuit?.Invoke();
        }

        private async void LevelCompleteUIButtonHandler()
        {
            var next = await SceneLoader.LoadNextLevel();
            if (!next)
            {
                await ShowGameOverUI();
            }
            levelCompleteUI.Hide();
        }

        private async void GameOverUIButtonHandler()
        {
            async UniTask a()
            {
                await MiscTools.DelayGameTime(0.99f * SceneLoader.sceneFadeTime, GlobalGameTools.Instance.CTS.Token);
                gameOverUI.Container.Hide();//so that it's hidden for next time, because we used ignore parent groups
                gameOverUI.Hide();
            }
            var b = SceneLoader.LoadMainMenu();
            await UniTask.WhenAll(a(), b);
        }

        private async UniTask ShowGameOverUI()
        {
            gameOverUI.DisplayStats(lastStatsSent);
            await gameOverUI.FadeShow(uiFadeTime / 2, GlobalGameTools.Instance.CTS.Token);
            await gameOverUI.Container.FadeShow(uiFadeTime / 4, GlobalGameTools.Instance.CTS.Token);
        }

        private async UniTask FadeInPrimaryUI()
        {
            var a = timer.FadeShow(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            var b = survivalCounter.FadeShow(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            var c = restartUI.FadeShow(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            var d = escToMenuUI.FadeShow(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            //var e = controlsText.FadeShow(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            await UniTask.WhenAll(a, b, c, d);
        }

        private async UniTask FadeOutPrimaryUI()
        {
            var a = timer.FadeHide(uiFadeTime / 2, GlobalGameTools.Instance.CTS.Token);
            var b = survivalCounter.FadeHide(uiFadeTime / 2, GlobalGameTools.Instance.CTS.Token);
            var c = restartUI.FadeHide(uiFadeTime / 2, GlobalGameTools.Instance.CTS.Token);
            var d = escToMenuUI.FadeHide(uiFadeTime / 2, GlobalGameTools.Instance.CTS.Token);
            //var e = controlsText.FadeHide(uiFadeTime / 2, GlobalGameTools.Instance.CTS.Token);
            await UniTask.WhenAll(a, b, c, d);
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
            LevelManager.LevelStarted -= OnLevelStarted;
            LevelManager.LGDeath -= OnLGDeath;
            LevelManager.LevelEnded -= OnLevelEnded;
            restartUI.Confirmed -= SendRestartRequest;
            escToMenuUI.Confirmed -= SendAbandonRequest;
            levelCompleteUI.ContinueButton.onClick.RemoveListener(LevelCompleteUIButtonHandler);
            gameOverUI.MainMenuButton.onClick.RemoveListener(GameOverUIButtonHandler);
        }
    }
}
