using Cysharp.Threading.Tasks;
using LGShuttle.Game;
using System.Threading;
using UnityEngine;

namespace LGShuttle.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] float uiFadeTime = 1;

        LevelTimerUI timer;
        SurvivalCounter survivalCounter;
        RestartUI restartUI;
        CancellationTokenSource cts;

        public static GameHUD Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                timer = GetComponentInChildren<LevelTimerUI>();
                survivalCounter = GetComponentInChildren<SurvivalCounter>();
                restartUI = GetComponentInChildren<RestartUI>();
                cts = new();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            LevelManager.LevelPrepared += OnLevelPrepared;
            LevelManager.GameStarted += OnGameStarted;
            LevelManager.LevelStateUpdate += OnLevelStateUpdate;
            LevelManager.GameEnded += OnGameEnded;
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
            await FadeInUI();
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
            await FadeOutUI();
        }

        private async UniTask FadeInUI()
        {
            var a = timer.FadeShow(uiFadeTime, cts.Token);
            var b = survivalCounter.FadeShow(uiFadeTime, cts.Token);
            var c = restartUI.FadeShow(uiFadeTime, cts.Token);
            await UniTask.WhenAll(a, b, c);
        }

        private async UniTask FadeOutUI()
        {
            var a = timer.FadeHide(uiFadeTime, cts.Token);
            var b = survivalCounter.FadeHide(uiFadeTime, cts.Token);
            var c = restartUI.FadeHide(uiFadeTime, cts.Token);
            await UniTask.WhenAll(a, b, c);
        }

        private void OnDisable()
        {
            LevelManager.LevelPrepared -= OnLevelPrepared;
            LevelManager.GameStarted -= OnGameStarted;
            LevelManager.LevelStateUpdate -= OnLevelStateUpdate;
            LevelManager.GameEnded -= OnGameEnded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                cts.Cancel();
                cts.Dispose();
            }
        }
    }
}
