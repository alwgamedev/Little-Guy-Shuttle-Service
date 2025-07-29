using System;
using System.Reflection;
using UnityEngine;

namespace LGShuttle.Game
{
    public class LittleGuyMover : MonoBehaviour
    {
        [SerializeField] Transform frontFoot;
        [SerializeField] Transform backFoot;
        //[SerializeField] float balanceStrength;
        //[Range(0, 90)][SerializeField] float balanceBreakAngle = 60;
        //[SerializeField] float runThreshold;
        //[SerializeField] float walkSpeed;
        //[SerializeField] float runSpeed;
        //[SerializeField] float accelMultiplier;
        //[SerializeField] float accelerationDampTime;
        //[SerializeField] float destinationTolerance = 0.1f;
        [SerializeField] LittleGuyPhysicsSettings physicsSettings;

        Vector2 boardAnchorPtLocalPos;
        float accelerationTimer;
        float moveImpetus;
        float accelerationDampTime;

        public Vector2 BoardAnchorPt => boardAnchorPtLocalPos + (Vector2)SkateboardMover.Board.transform.position;
        public Rigidbody2D Rigidbody { get; private set; }  
        public Collider2D Collider { get; private set; }
        public float Height { get; private set; }
        public float MoveImpetus => moveImpetus;
        public Vector2 RelativeVelocity => BalanceBroken ? Rigidbody.linearVelocity
            : Rigidbody.linearVelocity - SkateboardMover.Board.linearVelocity;
        public float RelativeVelocityAlongBoard => Vector2.Dot(RelativeVelocity, SkateboardMover.Board.transform.right);
        public bool BalanceBroken { get; private set; }
        public bool Dead { get; private set; }
        public float RunThreshold => physicsSettings.runThreshold;
        public bool ShouldRun => /*!repositioning &&*/ Mathf.Abs(SkateboardMover.VelocityAlongBoard) > RunThreshold;
        public float MoveSpeed => ShouldRun ? RunSpeed : WalkSpeed;
        public float WalkSpeed => physicsSettings.walkSpeed;
        public float RunSpeed => physicsSettings.runSpeed;

        //public event Action BalanceBroke;

        public event Action<LittleGuyMover> Death;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            Height = Collider.bounds.extents.y * 2;
            //balanceBreakPoint = Mathf.Cos(Mathf.Deg2Rad * (physicsSettings.balanceBreakAngle + 90));
        }

        private void Start()
        {
            UpdateAngularDamping();
            SetAnchorPtToCurrentPosition();
            ResetAccelerationTimer();
        }


        //NAVIGATION

        public void ChooseNewAnchorPoint()
        {
            boardAnchorPtLocalPos = SkateboardMover.RandomBoardAnchorLocalPosition;
            //repositioning = true;
        }

        private void SetAnchorPtToCurrentPosition()
        {
            var h = RaycastToBoard();
            if (h)
            {
                boardAnchorPtLocalPos = h.point - (Vector2)SkateboardMover.Board.transform.position;
            }
        }


        //STATE

        private bool IsGrounded(out Vector2 point)
        {
            var r = RaycastToBoard(physicsSettings.groundednessToleranceFactor * Height);

            if (r)
            {
                point = r.point;
                return true;
            }

            point = default;
            return false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (Dead || !gameObject) return;

            var lm = 1 << collision.gameObject.layer;
            if (lm == GlobalGameTools.SkateboardLayer && BalanceBroken)
            {
                RegainBalance();
            }
            else if (lm == GlobalGameTools.GroundLayer)
            {
                Dead = true;
                Death?.Invoke(this);
                Destroy(gameObject);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            OnCollisionEnter2D(collision);
        }


        //MOVEMENT

        public void DefaultBehavior()
        {
            if (!IsGrounded(out _))
            {
                if (!BalanceBroken)
                {
                    BreakBalance();
                }
            }
            else
            {
                MoveTowardsBoardAnchorPoint();
                Balance();
            }
        }

        private void MoveTowardsBoardAnchorPoint()
        {
            if (accelerationTimer < accelerationDampTime)
            {
                accelerationTimer += Time.deltaTime;
            }

            var d = MoveDirectionAlongBoard(BoardAnchorPt, out moveImpetus);
            var c = Mathf.Clamp(Mathf.Sqrt(moveImpetus), 0.1f, 1);
            var a = physicsSettings.accelMultiplier * accelerationTimer * c / accelerationDampTime;

            if (!Move(MoveSpeed, a, d))
            {
                //repositioning = false;
                ResetAccelerationTimer();
            }
        }

        private bool Move(float goalSpeed, float accelFactor, Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                UpdateOrientation(direction);
                var s = Vector2.Dot(RelativeVelocity, direction);
                var f = (goalSpeed - s) * accelFactor * Rigidbody.mass * direction;
                Rigidbody.AddForce(f);
                return true;
            }

            return false;
        }

        private void ResetAccelerationTimer()
        {
            accelerationTimer = 0;
            accelerationDampTime = physicsSettings.randomAccelerationDampTime.Value;
        }

        private void UpdateOrientation(Vector2 direction)
        {
            var s = transform.localScale;
            var d = Vector2.Dot(direction, transform.right * Mathf.Sign(s.x));

            if (d < 0)
            {
                s.x *= -1;
                transform.localScale = s;
            }
        }

        private Vector2 MoveDirectionAlongBoard(Vector2 targetPoint, out float moveImpetus)
        {
            var d = targetPoint - (Vector2)transform.position;
            var r = SkateboardMover.Board.transform.right;
            var dot = Vector2.Dot(d, r);
            var absDot = Mathf.Abs(dot);
            if (absDot < physicsSettings.destinationTolerance)
            {
                moveImpetus = 0;
                return Vector2.zero;
            }
            moveImpetus = absDot;
            return Mathf.Sign(dot) * r;
        }

        private RaycastHit2D RaycastToBoard(float distance = Mathf.Infinity)
        {
            return Physics2D.Raycast(Collider.bounds.center, -SkateboardMover.Board.transform.up, distance,
                GlobalGameTools.SkateboardLayer);
        }


        //BALANCE
        
        public void Balance()
        {
            var d = -Vector2.Dot(transform.right, SkateboardMover.Board.transform.up);
            d *= Mathf.Sqrt(Mathf.Abs(d));//I like the feel this gives it (and * Abs(d) was too much at extreme angles)
            Rigidbody.AddTorque(d * physicsSettings.balanceStrength * Rigidbody.mass);
            //if (d < balanceBreakPoint)
            //{
            //    BreakBalance();
            //}
            //else
            //{
            //    Rigidbody.AddTorque(d * physicsSettings.balanceStrength * Rigidbody.mass);
            //}
        }

        public void KeepFeetLevel()
        {
            frontFoot.right = 
                Vector2.Dot(frontFoot.transform.right, transform.right) * SkateboardMover.Board.transform.right
                + Vector2.Dot(frontFoot.transform.right, transform.up) * SkateboardMover.Board.transform.up;
            backFoot.right = 
                Vector2.Dot(backFoot.transform.right, transform.right) * SkateboardMover.Board.transform.right
                + Vector2.Dot(backFoot.transform.right, transform.up) * SkateboardMover.Board.transform.up;
        }

        private void BreakBalance()
        {
            BalanceBroken = true;
            UpdateAngularDamping();
        }

        private void RegainBalance()
        {
            BalanceBroken = false;
            UpdateAngularDamping();
        }

        private void UpdateAngularDamping()
        {
            Rigidbody.angularDamping = BalanceBroken ?
                physicsSettings.fallenAngularDamping : physicsSettings.balancedAngularDamping;
        }

        private void OnDestroy()
        {
            Death = null;
        }
    }
}
