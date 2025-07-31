using Cysharp.Threading.Tasks;
using LGShuttle.Core;
using LGShuttle.SceneManagement;
using LGShuttle.Game;
using UnityEngine;

namespace LGShuttle.UI
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] HidableUI container;
        //hook up to button in inspector

        //**just set container to show on enable**
        //private void OnEnable()
        //{
        //    if (!container.Visible)
        //    {
        //        container.Show();
        //    }
        //}

        public async void StartGame()
        {
            await container.FadeHide(0.25f, GlobalGameTools.Instance.CTS.Token);
            await SceneLoader.LoadNextLevel();
        }
    }
}