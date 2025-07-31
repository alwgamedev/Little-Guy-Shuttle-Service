using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace LGShuttle.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HidableUI : MonoBehaviour
    {
        public enum FadeState
        {
            none, fadeShow, fadeHide
        }

        [SerializeField] protected bool showOnEnable = true;
        [SerializeField] protected bool useShowOnEnable = true;
        [SerializeField] protected bool interactableWhenShown;
        [SerializeField] protected bool blockRaycastsWhenShown;

        protected FadeState fadeState = FadeState.none;

        public CanvasGroup CanvasGroup { get; protected set; }
        public bool Visible { get; protected set; }

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        protected virtual void OnEnable()
        {
            if (useShowOnEnable)
            {
                if (showOnEnable)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        public virtual void Show()
        {
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = interactableWhenShown;
            CanvasGroup.blocksRaycasts = blockRaycastsWhenShown;
            Visible = true;
        }

        public virtual void Hide()
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            Visible = false;
        }

        public async UniTask FadeShow(float time, CancellationToken token)
        {
            if (fadeState == FadeState.fadeShow || Visible) return;

            fadeState = FadeState.fadeShow;

            while (CanvasGroup.alpha < 1)
            {
                await Task.Yield();
                if (fadeState != FadeState.fadeShow) return;
                if (token.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
                CanvasGroup.alpha += Time.deltaTime / time;
            }

            fadeState = FadeState.none;
            Show();
        }

        public async UniTask FadeHide(float time, CancellationToken token)
        {
            if (fadeState == FadeState.fadeHide || !Visible) return;

            fadeState = FadeState.fadeHide;

            while (CanvasGroup.alpha > 0)
            {
                await Task.Yield();
                if (fadeState != FadeState.fadeHide) return;
                if (token.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
                CanvasGroup.alpha -= Time.deltaTime / time;
            }

            fadeState = FadeState.none;
            Hide();
        }

        public virtual void SetInteractable(bool val)
        {
            CanvasGroup.interactable = val;
        }
    }
}