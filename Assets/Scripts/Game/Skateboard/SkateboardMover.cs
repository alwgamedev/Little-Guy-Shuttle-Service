using LGShuttle.Core;
using UnityEngine;

namespace LGShuttle.Game
{
    public class SkateboardMover : MonoBehaviour
    {
        [SerializeField] Rigidbody2D board;
        [SerializeField] Collider2D boardCollider;
        [SerializeField] float acceleration;
        [SerializeField] RandomizableVector2 randomBoardAnchorPosition;

        int input;

        public static Rigidbody2D Board { get; private set; }
        public static RandomizableVector2 RandomBoardAnchorPosition { get; private set; }
        public static Vector2 RandomBoardAnchorLocalPosition 
            => RandomBoardAnchorPosition.LerpValue - (Vector2)Board.transform.position;
        public static float VelocityAlongBoard => Vector2.Dot(Board.linearVelocity, Board.transform.right);

        private void Awake()
        {
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
                Accelerate(input, acceleration);
            }
        }

        private void Accelerate(int direction, float acceleration)
        {
            board.AddForce(direction * acceleration * board.mass * Vector2.right);
        }
    }
}
