using Cysharp.Threading.Tasks;
using LGShuttle.Core;
using LGShuttle.Game;
using LGShuttle.SceneManagement;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace LGShuttle.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] float uiFadeTime = 1;
        [SerializeField] HidableUI levelFailedText;
        [SerializeField] HoldForActionUI restartUI;
        [SerializeField] HoldForActionUI escToMenuUI;
        [SerializeField] HidableUI goAnim;
        [SerializeField] float goAnimFrameLength;
        [SerializeField] Color goAnimPrimaryColor;
        [SerializeField] Color goAnimSecondaryColor;
        //[SerializeField] HidableUI controlsText;

        TextMeshProUGUI goText;
        LevelTimerUI timer;
        SurvivalCounter survivalCounter;
        LevelCompleteUI levelCompleteUI;
        GameOverUI gameOverUI;
        CumulativeStats lastStatsSent;

        public static event Action RequestStart;
        public static event Action RequestRestart;
        public static event Action RequestQuit;
        //public static event Action RequestNextLevel;

        private void Awake()
        {
            goText = goAnim.GetComponent<TextMeshProUGUI>();
            timer = GetComponentInChildren<LevelTimerUI>();
            survivalCounter = GetComponentInChildren<SurvivalCounter>();
            levelCompleteUI = GetComponentInChildren<LevelCompleteUI>();
            gameOverUI = GetComponentInChildren<GameOverUI>();
        }

        private void OnEnable()
        {
            LevelManager.LevelPrepared += OnLevelPrepared;
            //LevelManager.LevelStarted += OnLevelStarted;
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

            async UniTask a()
            {
                await MiscTools.DelayGameTime(SceneLoader.sceneFadeTime, GlobalGameTools.Instance.CTS.Token);
                await FadeInPrimaryUI(GlobalGameTools.Instance.CTS.Token);
            }

            var b = GoAnimationAndStart(goAnimFrameLength, GlobalGameTools.Instance.CTS.Token);

            await UniTask.WhenAll(a(), b);

        }

        //private async void OnLevelStarted(ILevelManager levelManager)
        //{
            
        //}

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
                    await FadeOutPrimaryUI(GlobalGameTools.Instance.CTS.Token);
                    break;
                case LevelCompletionResult.failed:
                    var a = LevelFailedAnimation(GlobalGameTools.Instance.CTS.Token);
                    var b = FadeOutPrimaryUI(GlobalGameTools.Instance.CTS.Token);
                    await UniTask.WhenAll(a, b);
                    break;
                case LevelCompletionResult.restart:
                    await FadeOutPrimaryUI(GlobalGameTools.Instance.CTS.Token);
                    break;
                case LevelCompletionResult.quit:
                    await FadeOutPrimaryUI(GlobalGameTools.Instance.CTS.Token);
                    await ShowGameOverUI(GlobalGameTools.Instance.CTS.Token);
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
                await ShowGameOverUI(GlobalGameTools.Instance.CTS.Token);
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

        private async UniTask ShowGameOverUI(CancellationToken token)
        {
            gameOverUI.DisplayStats(lastStatsSent);
            await gameOverUI.FadeShow(uiFadeTime / 2, token);
            await gameOverUI.Container.FadeShow(uiFadeTime / 4, token);
        }

        private async UniTask FadeInPrimaryUI(CancellationToken token)
        {
            var a = timer.FadeShow(uiFadeTime, token);
            var b = survivalCounter.FadeShow(uiFadeTime, token);
            var c = restartUI.FadeShow(uiFadeTime, token);
            var d = escToMenuUI.FadeShow(uiFadeTime, token);
            //var e = controlsText.FadeShow(uiFadeTime, GlobalGameTools.Instance.CTS.Token);
            await UniTask.WhenAll(a, b, c, d);
        }

        private async UniTask FadeOutPrimaryUI(CancellationToken token)
        {
            var a = timer.FadeHide(uiFadeTime / 2, token);
            var b = survivalCounter.FadeHide(uiFadeTime / 2, token);
            var c = restartUI.FadeHide(uiFadeTime / 2, token);
            var d = escToMenuUI.FadeHide(uiFadeTime / 2, token);
            //var e = controlsText.FadeHide(uiFadeTime / 2, GlobalGameTools.Instance.CTS.Token);
            await UniTask.WhenAll(a, b, c, d);
        }

        private async UniTask GoAnimationAndStart(float frameTime, CancellationToken token)
        {
            await UniTask.WhenAll(goAnim.FadeShow(frameTime, token),
                GoAnimation(frameTime, GlobalGameTools.Instance.CTS.Token));
            RequestStart?.Invoke();
            await MiscTools.DelayGameTime(3 * frameTime, GlobalGameTools.Instance.CTS.Token);
            await goAnim.FadeHide(3 * frameTime, GlobalGameTools.Instance.CTS.Token);
        }

        private async UniTask GoAnimation(float frameTime, CancellationToken token)
        {
            goAnim.Show();
            goText.color = goAnimPrimaryColor;
            for (int i = 0; i < 12; i++)
            {
                goText.text = GoText(i);
                await MiscTools.DelayGameTime(frameTime, token);
            }

            goText.color = goAnimSecondaryColor;
            goText.text = "GO!";
        }

        private string GoText(int frame)
        {
            if (frame >= 12)
            {
                return "GO!";
            }

            int q = 3 - (frame / 4);
            int r = frame % 4;
            var s = q.ToString();
            for (int i = 0; i < r; i++)
            {
                s += ".";
            }

            return s;
        }

        private async UniTask LevelFailedAnimation(CancellationToken token)
        {
            await levelFailedText.FadeShow(uiFadeTime / 3, token);
            await MiscTools.DelayGameTime(uiFadeTime, token);
            await levelFailedText.FadeHide(uiFadeTime / 3, token);
        }

        private void OnDisable()
        {
            LevelManager.LevelPrepared -= OnLevelPrepared;
            //LevelManager.LevelStarted -= OnLevelStarted;
            LevelManager.LGDeath -= OnLGDeath;
            LevelManager.LevelEnded -= OnLevelEnded;
            restartUI.Confirmed -= SendRestartRequest;
            escToMenuUI.Confirmed -= SendAbandonRequest;
            levelCompleteUI.ContinueButton.onClick.RemoveListener(LevelCompleteUIButtonHandler);
            gameOverUI.MainMenuButton.onClick.RemoveListener(GameOverUIButtonHandler);
        }
    }
}
