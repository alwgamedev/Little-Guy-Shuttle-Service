using Cysharp.Threading.Tasks;
using LGShuttle.Core;
using LGShuttle.UI;
using LGShuttle.SceneManagement;
using System;
using UnityEngine;

namespace LGShuttle.Game
{

    [RequireComponent(typeof(LevelTimer))]
    [RequireComponent(typeof(LittleGuySpawner))]
    public class LevelManager : MonoBehaviour, ILevelManager
    {
        [SerializeField] LevelParams testLevelParams;

        LittleGuySpawner spawner;
        LittleGuyController[] spawned;
        LevelTimer timer;
        LevelParams levelParams;
        LevelState levelState;
        CumulativeStats cumulativeStats;

        public ILevelTimer Timer => timer;
        public LevelParams LevelParams => levelParams;
        public LevelState LevelState => levelState;
        public CumulativeStats CumulativeStats => cumulativeStats;

        public static event Action<ILevelManager> LevelPrepared;
        public static event Action<ILevelManager> LevelStarted;
        public static event Action<ILevelManager> LGDeath;
        public static event Action<ILevelManager> LevelEnded;

        private void Awake()
        {
            spawner = GetComponent<LittleGuySpawner>();
            timer = GetComponent<LevelTimer>();
        }

        private void OnEnable()
        {
            timer.TimedOut += TimeOutHandler;
            GameHUD.RequestStart += StartLevel;
            GameHUD.RequestRestart += HandleRestartRequest;
            GameHUD.RequestQuit += HandleQuitRequest;
            LevelParamsMessenger.SendLevelParams += PrepareLevel;
            FinishLine.FinishLineTriggered += OnFinishLineCrossed;
            SceneLoader.ReturnedToMainMenu += ResetCumulativeStats;
        }

        public void ResetCumulativeStats()
        {
            cumulativeStats = new();
            levelState = new();
        }

        public void PrepareLevel(LevelParams levelParams)
        {
            this.levelParams = levelParams;
            timer.SetTimer(levelParams.timeLimit);
            spawned = spawner.Spawn(this.levelParams.lgToSpawn);
            levelState.spawned = spawned.Length;
            levelState.remaining = spawned.Length;
            SubscribeDeathHandlers();
            SkateboardMover.TotalLGMass = spawned[0].Mover.Rigidbody.mass * spawned.Length;
            LevelPrepared?.Invoke(this);
        }

        public void StartLevel()
        {
            levelState.gameRunning = true;
            timer.StartTimer();
            LevelStarted?.Invoke(this);
        }

        public async UniTask EndLevel(LevelCompletionResult result)
        {
            if (!levelState.gameRunning) return;

            timer.StopTimer();
            levelState.gameRunning = false;
            levelState.attempts++;
            levelState.result = result;

            if (result == LevelCompletionResult.passed)
            {
                levelState.CalculateStats(this);
                cumulativeStats.OnLevelPassed(levelState.Stats);
                levelState.attempts = 0;
                LevelEnded?.Invoke(this);
            }
            else
            {
                cumulativeStats.OnLevelFailed(this);
                LevelEnded?.Invoke(this);
                if (result != LevelCompletionResult.quit)
                {
                    await MiscTools.DelayGameTime(1, GlobalGameTools.Instance.CTS.Token);
                    await SceneLoader.ReloadScene();
                }
            }
        }

        private async void TimeOutHandler()
        {
            if (levelState.gameRunning)
            {
                await EndLevel(LevelCompletionResult.failed);
            }
        }

        private async void HandleRestartRequest()
        {
            if (levelState.gameRunning)
            { 
                await EndLevel(LevelCompletionResult.restart);
            }
        }

        private async void HandleQuitRequest()
        {
            if (levelState.gameRunning)
            {
                await EndLevel(LevelCompletionResult.quit);
            }
        }

        private async void OnFinishLineCrossed()
        {
            if (levelState.gameRunning)
            {
                await EndLevel(LevelCompletionResult.passed);
            }
        }

        private async void LGDeathHandler(LittleGuyMover lg)
        {
            if (!lg) return;

            levelState.remaining--;
            SkateboardMover.TotalLGMass -= lg.Rigidbody.mass;
            lg.Death -= LGDeathHandler;

            LGDeath?.Invoke(this);

            if (levelState.gameRunning && levelState.remaining == 0)
            {
                await EndLevel(LevelCompletionResult.failed);
            }
        }

        private void SubscribeDeathHandlers()
        {
            if (spawned != null)
            {
                foreach (var lg in spawned)
                {
                    if (lg && lg.Mover)
                    {
                        lg.Mover.Death += LGDeathHandler;
                    }
                }
            }
        }

        private void UnsubscribeDeathHandlers()
        {
            if (spawned != null)
            {
                foreach (var lg in spawned)
                {
                    if (lg && lg.Mover)
                    {
                        lg.Mover.Death -= LGDeathHandler;
                    }
                }
            }
        }

        private void OnDisable()
        {
            timer.TimedOut -= TimeOutHandler;
            GameHUD.RequestStart -= StartLevel;
            GameHUD.RequestRestart -= HandleRestartRequest;
            GameHUD.RequestQuit -= HandleQuitRequest;
            LevelParamsMessenger.SendLevelParams -= PrepareLevel;
            FinishLine.FinishLineTriggered -= OnFinishLineCrossed;
            SceneLoader.ReturnedToMainMenu -= ResetCumulativeStats;
        }

        private void OnDestroy()
        {
            UnsubscribeDeathHandlers();
        }
    }
}