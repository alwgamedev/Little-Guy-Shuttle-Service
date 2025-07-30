using System;
using UnityEngine;

namespace LGShuttle.Game
{
    public class LevelParamsMessenger : MonoBehaviour
    {
        [SerializeField] LevelParams levelParams;

        public static event Action<LevelParams> SendLevelParams;

        private void Start()
        {
            SendLevelParams?.Invoke(levelParams);
        }
    }
}