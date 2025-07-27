using LGShuttle.Core;
using UnityEngine;

namespace LGShuttle.Game
{
    public class SkateboardMover : MonoBehaviour
    {
        //[SerializeField] Rigidbody2D frontWheel;
        //[SerializeField] Rigidbody2D rearWheel;
        [SerializeField] Rigidbody2D board;
        //[SerializeField] float torque = 75;
        [SerializeField] float acceleration;

        int input;

        public static Rigidbody2D Board { get; private set; }

        private void Awake()
        {
            Board = board;
        }

        private void Update()
        {
            input = (Input.GetKey(KeyCode.A) ? -1 : 0)
                + (Input.GetKey(KeyCode.D) ? 1 : 0);
        }

        private void FixedUpdate()
        {
            if (input != 0)
            {
                //Accelerate(input, torque);
                Accelerate(input, acceleration);
            }
        }

        //void Accelerate(int direction, float torque)
        //{
        //    //(front wheel was locking up and getting dragged along; rear-axle drive got rid of the issue)
        //    torque *= -direction;
        //    if (torque < 0) //forward movement
        //    {
        //        rearWheel.AddTorque(torque);
        //    }
        //    else //backward movement
        //    {
        //        frontWheel.AddTorque(torque);
        //    }
        //}

        private void Accelerate(int direction, float acceleration)
        {
            board.AddForce(direction * acceleration * board.mass * Vector2.right);
        }
    }
}
