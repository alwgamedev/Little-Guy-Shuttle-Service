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
        [SerializeField] float wheelTorque;
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

        bool Grounded => frontWheel.IsTouchingLayers(GlobalGameTools.Instance.GroundLayer)
            || backWheel.IsTouchingLayers(GlobalGameTools.Instance.GroundLayer)
            || Board.IsTouchingLayers(GlobalGameTools.Instance.GroundLayer);
        float TotalMass => Board.mass + frontWheel.attachedRigidbody.mass
            + backWheel.attachedRigidbody.mass + TotalLGMass;

        public static float TotalLGMass { get; set; }
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
            LevelManager.LevelStarted += OnLevelStarted;
            LevelManager.LevelEnded += OnLevelEnded;
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
                Accelerate(moveInput, moveSpeed, acceleration, wheelTorque);
            }

            if (rotateInput != 0)
            {
                Rotate(rotateInput, torque);
            }

            if (jumpInput == 1)
            {
                JumpImpulse();
                awaitingLanding = true;
                jumpCooldownTimer = jumpCooldown;
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

        private void Accelerate(int direction, float goalSpeed, float acceleration, float wheelTorque)
        {
            var u = Grounded ? board.transform.right : Vector3.right;
            //^otherwise you can rotate the board to a steep angle and fly through the air
            var s = Vector2.Dot(Board.linearVelocity, direction * u);
            var f = (goalSpeed - s) * acceleration * TotalMass * direction * u;
            Board.AddForce(f);
            var t = (s - goalSpeed) * wheelTorque * TotalMass * direction;
            frontWheel.attachedRigidbody.AddTorque(t);
            backWheel.attachedRigidbody.AddTorque(t);
            //driving the wheels helps when a wheel gets stuck on a ledge
            //but also need to drive the board, so that you can still move while midair
        }

        private void Rotate(int direction, float torque)
        {
            board.AddTorque(-direction * torque * TotalMass);
        }

        private bool CanJump()
        {
            return jumpInput == 0 && jumpCooldownTimer <= 0 && !awaitingLanding;
            //so you WILL be allowed to jump mid-air, but once you jump you have to hit ground 
            //before you can jump again
        }

        private void JumpImpulse()
        {
            board.AddForce(jumpImpulseForce * TotalMass * Vector2.up, ForceMode2D.Impulse);
        }

        private void JumpHold()
        {
            board.AddForce(jumpHoldForce * TotalMass * Vector2.up);
        }

        private void OnLevelStarted(ILevelManager lm)
        {
            inputEnabled = true;
        }

        private void OnLevelEnded(ILevelManager lm)
        {
            inputEnabled = false;
            moveInput = 0;
            jumpInput = 0;
            rotateInput = 0;
        }

        private void OnDisable()
        {
            LevelManager.LevelStarted -= OnLevelStarted;
            LevelManager.LevelEnded -= OnLevelEnded;
        }
    }
}
