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

        //idea for how this will work ultimately:
        //one level manager persisting between levels that gets the level params for each level
        //from a static class. UI will get data from LevelManager (e.g. remaining lg, timer, etc.)

        //RULES OF THE GAME:
        //* it's timed; if timeRemaing <= 0, you fail the level and restart
        //* if percent remaining falls below a threshold (e.g. 50%), you fail and restart
        //  (note: game will generally be harder with smaller starting spawn because
        //  you get less deaths before you fail -- but maybe we just fix the starting spawn (with little bit of random)
        //  for simplicity
        //* you can press R to restart the level at any time (e.g. if you get stuck)

        LittleGuySpawner spawner;
        LittleGuyController[] spawned;
        LevelTimer timer;
        LevelParams levelParams;
        LevelState levelState;

        public ILevelTimer Timer => timer;
        public LevelParams LevelParams => levelParams;
        public LevelState LevelState => levelState;

        public static event Action<ILevelManager> LevelPrepared;
        public static event Action<ILevelManager> GameStarted;
        public static event Action<ILevelManager> LGDeath;
        public static event Action<ILevelManager> GameEnded;
        //WE'LL ADD SUCH EVENTS LATER ONCE WE KNOW HOW/IF WE ACTUALLY NEED THEM

        //WE'LL PROBABLY USE THESE EVENTS FOR UI (so just static events rather than static LM)
        //the ui will probably just be survival counter and timer
        //maybe we just:
        //a) configure and show hud when game starts 
        //b) have events for level state changes during game (to update survival counter)
        //c) hide hud when game ends

        private void Awake()
        {
            spawner = GetComponent<LittleGuySpawner>();
            timer = GetComponent<LevelTimer>();
            //DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            timer.TimedOut += TimeOutHandler;
            GameHUD.RequestRestart += HandleRestartRequest;
            LevelParamsMessenger.SendLevelParams += ReceiveLevelParams;
            FinishLine.FinishLineTriggered += OnFinishLineCrossed;
        }

        //private void Start()
        //{
        //    //PrepareLevel(testLevelParams);
        //    StartGame();
        //}

        private void ReceiveLevelParams(LevelParams levelParams)
        {
            PrepareLevel(levelParams);
            StartGame();
        }

        public void PrepareLevel(LevelParams levelParams)
        {
            this.levelParams = levelParams;
            timer.SetTimer(levelParams.timeLimit);
            spawned = spawner.Spawn(this.levelParams.lgToSpawn);
            levelState.spawned = spawned.Length;
            levelState.remaining = spawned.Length;
            SubscribeDeathHandlers();
            LevelPrepared?.Invoke(this);
        }

        public void StartGame()
        {
            levelState.gameRunning = true;
            timer.StartTimer();
            GameStarted?.Invoke(this);
        }

        public async UniTask EndGame(bool restart = false)
        {
            timer.StopTimer();
            levelState.gameRunning = false;
            GameEnded?.Invoke(this);

            if (restart)
            {
                await MiscTools.DelayGameTime(1, GlobalGameTools.Instance.CTS.Token);
                await SceneLoader.ReloadScene();
            }
        }

        //public async void LoadNextLevel()
        //{
        //    Debug.Log("load next level here!");
        //    await SceneLoader.ReloadScene();
        //}

        //public void FailLevel()
        //{
        //    Debug.Log("Level failed");
        //    EndGame();
        //    //and maybe an event that will trigger some "LEVEL FAILED" UI,
        //    //and then scene manager will restart the scene
        //}

        private async void TimeOutHandler()
        {
            if (levelState.gameRunning)
            {
                await EndGame(true);
            }
        }

        private async void HandleRestartRequest()
        {
            if (levelState.gameRunning)
            { 
                await EndGame(true);
            }
        }

        private async void OnFinishLineCrossed()
        {
            if (levelState.gameRunning)
            {
                await EndGame();
            }
        }

        private async void LGDeathHandler(LittleGuyMover lg)
        {
            if (!lg) return;

            levelState.remaining--;
            lg.Death -= LGDeathHandler;

            LGDeath?.Invoke(this);

            if (levelState.gameRunning && levelState.SurvivalRate < levelParams.survivalRate)
            {
                //FailLevel();
                await EndGame(true);
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
            GameHUD.RequestRestart -= HandleRestartRequest;
            LevelParamsMessenger.SendLevelParams -= ReceiveLevelParams;
            FinishLine.FinishLineTriggered -= OnFinishLineCrossed;
        }

        private void OnDestroy()
        {
            UnsubscribeDeathHandlers();
        }
    }
}