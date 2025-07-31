using Cysharp.Threading.Tasks;
using LGShuttle.UI;
using UnityEngine.SceneManagement;
using LGShuttle.Game;
using System.Threading.Tasks;
using System;

namespace LGShuttle.SceneManagement
{
    public static class SceneLoader
    {
        public static float sceneFadeTime;
        public static HidableUI sceneFader;

        public static int NumLevels => SceneManager.sceneCountInBuildSettings - 1;

        public static event Action ReturnedToMainMenu;

        public static async UniTask ReloadScene()
        {
            await LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static async UniTask LoadMainMenu()
        {
            await LoadScene(0);
            ReturnedToMainMenu?.Invoke();
        }

        public static async UniTask<bool> LoadNextLevel()
        {
            var i = SceneManager.GetActiveScene().buildIndex + 1;
            if (i  < SceneManager.sceneCountInBuildSettings)
            {
                await LoadScene(i);
                return true;
            }

            return false;
        }

        public static async UniTask LoadScene(int buildIndex)
        {
            var ao = SceneManager.LoadSceneAsync(buildIndex);
            ao.allowSceneActivation = false;

            var s = sceneFader.FadeShow(sceneFadeTime, GlobalGameTools.Instance.CTS.Token);

            async UniTask t()
            {
                while (!ao.isDone)
                {
                    if (GlobalGameTools.Instance.CTS.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }
                    if (ao.progress >= 0.9f)
                    {
                        return;
                    }

                    await UniTask.Yield();
                }
            }

            await UniTask.WhenAll(s, t());

            ao.allowSceneActivation = true;

            //await sceneFader.FadeHide(sceneFadeTime, GlobalGameTools.Instance.CTS.Token);
        }
    }
}
