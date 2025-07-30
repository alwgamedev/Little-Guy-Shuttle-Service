using LGShuttle.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LGShuttle.UI
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField] HidableUI sceneFader;
        [SerializeField] float sceneFadeTime = .25f;

        public GameHUD GameHUD { get; private set; }

        public static UICanvas Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            GameHUD = GetComponentInChildren<GameHUD>();
            SceneTransitionManager.sceneFader = sceneFader;
            SceneTransitionManager.sceneFadeTime = sceneFadeTime;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameHUD.gameObject.SetActive(SceneManager.GetActiveScene().name != "HomeScene");
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnDestroy()
        {
            Instance = null;
            SceneTransitionManager.sceneFader = null;
        }
    }
}