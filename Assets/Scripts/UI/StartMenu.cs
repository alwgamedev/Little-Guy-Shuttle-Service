using LGShuttle.SceneManagement;
using LGShuttle.Game;
using UnityEngine;

namespace LGShuttle.UI
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] HidableUI container;

        //hooked up to button in inspector
        public async void StartGame()
        {
            await container.FadeHide(0.25f, GlobalGameTools.Instance.CTS.Token);
            await SceneLoader.LoadNextLevel();
        }
    }
}