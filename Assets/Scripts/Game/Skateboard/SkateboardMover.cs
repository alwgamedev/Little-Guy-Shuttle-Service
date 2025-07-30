using LGShuttle.Core;
using UnityEngine;

namespace LGShuttle.Game
{
    public class SkateboardMover : MonoBehaviour
    {
        [SerializeField] Rigidbody2D board;
        [SerializeField] Collider2D frontWheel;
        [SerializeField] Collider2D backWheel;
        [SerializeField] float acceleration;
        [SerializeField] float torque;
        [SerializeField] float moveSpeed;
        [SerializeField] float jumpCooldown;
        [SerializeField] float jumpImpulseForce;
        [SerializeField] float jumpHoldForce;
        [SerializeField] float jumpHoldMinUpwardVelocity;
        [SerializeField] RandomizableVector2 randomBoardAnchorPosition;

        int moveInput;
        int rotateInput;
        int jumpInput;//0 = no jumpInput, 1 = jumpJustPressed, 2 = jumpHeld
        bool awaitingLanding;
        bool inputEnabled;

        float jumpCooldownTimer;

        bool Grounded => frontWheel.IsTouchingLayers(GlobalGameTools.GroundLayer)
            || backWheel.IsTouchingLayers(GlobalGameTools.GroundLayer);


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

        private void OnEnable()
        {
            LevelManager.GameStarted += OnGameStarted;
            LevelManager.GameEnded += OnGameEnded;
        }

        private void Update()
        {
            if (inputEnabled)
            {
                UpdateInput();
            }

            if (jumpCooldownTimer > 0)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }
            else if (awaitingLanding && Grounded)
            {
                awaitingLanding = false;
            }
        }

        private void FixedUpdate()
        {
            if (moveInput != 0)
            {
                Accelerate(moveInput, moveSpeed, acceleration);
            }

            if (rotateInput != 0)
            {
                Rotate(rotateInput, torque);
            }

            if (jumpInput == 1)
            {
                JumpImpulse();
                awaitingLanding = true;
                jumpInput = 2;
            }
            else if (jumpInput == 2)
            {
                JumpHold();
                if (jumpCooldownTimer <= 0 && board.linearVelocityY < jumpHoldMinUpwardVelocity)
                    //otherwise holding jump slows the fall and looks unnatural
                {
                    jumpInput = 0;
                }
            }
        }

        private void UpdateInput()
        {
            moveInput = (Input.GetKey(KeyCode.A) ? -1 : 0)
                            + (Input.GetKey(KeyCode.D) ? 1 : 0);

            rotateInput = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0)
                + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);

            if (Input.GetKeyDown(KeyCode.Space) && CanJump())
            {
                jumpInput = 1;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                jumpInput = 0;
            }
        }

        private void Accelerate(int direction, float goalSpeed, float acceleration)
        {
            var s = Vector2.Dot(Board.linearVelocity, direction * Board.transform.right);
            var f = (goalSpeed - s) * acceleration * Board.mass * direction * Board.transform.right;
            Board.AddForce(f);
        }

        private void Rotate(int direction, float torque)
        {
            board.AddTorque(-direction * torque * board.mass);
        }

        private bool CanJump()
        {
            return jumpCooldownTimer <= 0 && !awaitingLanding;
            //so you WILL be allowed to jump mid-air, but once you jump you have to hit ground 
            //before you can jump again
        }

        private void JumpImpulse()
        {
            board.AddForce(jumpImpulseForce * board.mass * Vector2.up, ForceMode2D.Impulse);
            jumpCooldownTimer = jumpCooldown;
        }

        private void JumpHold()
        {
            board.AddForce(jumpHoldForce * board.mass * Vector2.up);
        }

        private void OnGameStarted(ILevelManager lm)
        {
            inputEnabled = true;
        }

        private void OnGameEnded(ILevelManager lm)
        {
            inputEnabled = false;
        }

        private void OnDisable()
        {
            LevelManager.GameStarted -= OnGameStarted;
            LevelManager.GameEnded -= OnGameEnded;
        }
    }
}
