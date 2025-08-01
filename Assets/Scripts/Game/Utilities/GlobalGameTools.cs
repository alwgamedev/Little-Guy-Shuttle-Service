using System.Threading;
using UnityEngine;

namespace LGShuttle.Game
{
    public class GlobalGameTools : MonoBehaviour
    {
        int? littleGuyLayer;
        int? skateboardLayer;
        int? groundLayer;
        int? sceneBoundsLayer;

        public static GlobalGameTools Instance { get; private set; }

        public CancellationTokenSource CTS { get; private set; }
        public int LittleGuyLayer
        {
            get
            {
                if (!littleGuyLayer.HasValue)
                {
                    littleGuyLayer = LayerMask.GetMask("Little Guys");
                }

                return littleGuyLayer.Value;
            }
        }
        public int SkateboardLayer
        {
            get
            {
                if (!skateboardLayer.HasValue)
                {
                    skateboardLayer = LayerMask.GetMask("Skateboard");
                }

                return skateboardLayer.Value;
            }
        }
        public int GroundLayer
        {
            get
            {
                if (!groundLayer.HasValue)
                {
                    groundLayer = LayerMask.GetMask("Ground");
                }

                return groundLayer.Value;
            }
        }

        public int SceneBoundsLayer
        {
            get
            {
                if (!sceneBoundsLayer.HasValue)
                {
                    sceneBoundsLayer = LayerMask.GetMask("Scene Bounds");
                }

                return sceneBoundsLayer.Value;
            }
        }

        private void Awake()
        {
            CTS = new();
            Instance = this;
        }

        private void OnDestroy()
        {
            CTS.Cancel();
            CTS.Dispose();
            Instance = null;
        }
    }
}