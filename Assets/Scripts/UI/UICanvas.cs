using LGShuttle.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using LGShuttle.Game;

namespace LGShuttle.UI
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField] HidableUI sceneFader;
        [SerializeField] float sceneFadeTime = .25f;

        public StartMenu StartMenu { get; private set; }
        public GameHUD GameHUD { get; private set; }

        public static UICanvas Instance { get; private set; }

        private void Awake()
        {
            //we will never have competing instances thanks to Persistent Objects Spawner
            Instance = this;
            StartMenu = GetComponentInChildren<StartMenu>();
            GameHUD = GetComponentInChildren<GameHUD>();
            SceneLoader.sceneFader = sceneFader;
            SceneLoader.sceneFadeTime = sceneFadeTime;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var isStartMenu = SceneManager.GetActiveScene().buildIndex == 0;
            StartMenu.gameObject.SetActive(isStartMenu);
            GameHUD.gameObject.SetActive(!isStartMenu);
            if (sceneFader.Visible)
            {
                await sceneFader.FadeHide(sceneFadeTime, GlobalGameTools.Instance.CTS.Token);
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnDestroy()
        {
            Instance = null;
            SceneLoader.sceneFader = null;
        }
    }
}