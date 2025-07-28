using LGShuttle.Core;
using UnityEngine;

namespace LGShuttle.Game
{
    public class SkateboardMover : MonoBehaviour
    {
        //[SerializeField] Rigidbody2D frontWheel;
        //[SerializeField] Rigidbody2D rearWheel;
        [SerializeField] Rigidbody2D board;
        [SerializeField] Collider2D boardCollider;
        //[SerializeField] float torque = 75;
        [SerializeField] float acceleration;
        [SerializeField] RandomizableVector2 randomBoardAnchorPosition;

        int input;

        public static int BoardLayer { get; private set; }
        public static Rigidbody2D Board { get; private set; }
        public static RandomizableVector2 RandomBoardAnchorPosition { get; private set; }
        public static Vector2 RandomBoardAnchorLocalPosition 
            => RandomBoardAnchorPosition.LerpValue - (Vector2)Board.transform.position;
        public static float VelocityAlongBoard => Vector2.Dot(Board.linearVelocity, Board.transform.right);

        private void Awake()
        {
            BoardLayer = LayerMask.GetMask("Skateboard");
            Board = board;
            RandomBoardAnchorPosition = randomBoardAnchorPosition;
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
