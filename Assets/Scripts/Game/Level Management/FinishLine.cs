using System;
using UnityEngine;

namespace LGShuttle.Game
{
    public class FinishLine : MonoBehaviour
    {
        bool finishLineTriggered;

        public static Action FinishLineTriggered;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!gameObject.activeInHierarchy || finishLineTriggered)
                return;

            if (collider.gameObject.CompareTag("Skateboard"))
            {
                finishLineTriggered = true;
                FinishLineTriggered?.Invoke();
            }
        }
    }
}