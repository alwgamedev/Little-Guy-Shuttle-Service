using System.Reflection;
using UnityEngine;

namespace LGShuttle.Game
{
    public class LittleGuyMover : MonoBehaviour
    {
        [SerializeField] Transform frontFoot;
        [SerializeField] Transform backFoot;
        [SerializeField] float balanceStrength;
        [Range(0, 90)][SerializeField] float balanceBreakAngle = 60;
        [SerializeField] float runThreshold;
        //[SerializeField] float walkAcceleration;
        //[SerializeField] float runAcceleration
        [SerializeField] float walkSpeed;
        [SerializeField] float runSpeed;
        [SerializeField] float accelMultiplier;
        //[SerializeField] float boardAccelBoostRate;
        //[SerializeField] float boardAccelBoostMax;
        [SerializeField] float accelerationDampTime;
        [SerializeField] float destinationTolerance = 0.1f;

        float balanceBreakPoint;
        //Vector2 frontFootRight;
        //Vector2 backFootRight;

        //int boardLayer;
        Vector2 boardAnchorPtLocalPos;
        float accelerationTimer;
        float moveImpetus;

        public Vector2 BoardAnchorPt => boardAnchorPtLocalPos + (Vector2)SkateboardMover.Board.transform.position;
        public Rigidbody2D Rigidbody { get; private set; }
        public float MoveImpetus => moveImpetus;
        public Vector2 RelativeVelocity => Rigidbody.linearVelocity - SkateboardMover.Board.linearVelocity;
        public float RelativeVelocityAlongBoard => Vector2.Dot(RelativeVelocity, SkateboardMover.Board.transform.right);
        public bool BalanceBroken { get; private set; }
        public float RunThreshold => runThreshold;
        public bool ShouldRun => Mathf.Abs(SkateboardMover.VelocityAlongBoard) > RunThreshold;
        public float MoveSpeed => ShouldRun ?
            runSpeed : walkSpeed;
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            balanceBreakPoint = Mathf.Cos(Mathf.Deg2Rad * (balanceBreakAngle + 90));
            //frontFootRight = frontFoot.right;
            //backFootRight = backFoot.right;
            //boardLayer = LayerMask.GetMask("Skateboard");
        }

        private void Start()
        {
            var h = RaycastToBoard();
            boardAnchorPtLocalPos = h.point - (Vector2)SkateboardMover.Board.transform.position;
        }

        //NAVIGATION

        public void ChooseNewAnchorPoint()
        {
            boardAnchorPtLocalPos = SkateboardMover.RandomBoardAnchorLocalPosition;
            Debug.Log($"new anchor pt local pos: {boardAnchorPtLocalPos}");
        }



        //MOVEMENT

        public void DefaultBehavior()
        {
            MoveTowardsBoardAnchorPoint();
            Balance();
        }

        private void MoveTowardsBoardAnchorPoint()
        {
            if (accelerationTimer < accelerationDampTime)
            {
                accelerationTimer += Time.deltaTime;
            }

            var d = MoveDirectionAlongBoard(BoardAnchorPt, out moveImpetus);
            var c = Mathf.Clamp(Mathf.Sqrt(moveImpetus), 0.1f, 1);
            var a = accelMultiplier * accelerationTimer * c / accelerationDampTime;

            if (!Move(MoveSpeed, a, d))
            {
                accelerationTimer = 0;
            }
        }

        private bool Move(float goalSpeed, float accelFactor, Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                FaceDirectionWithoutRotating(direction);
                var s = Vector2.Dot(RelativeVelocity, direction);
                var f = (goalSpeed - s) * accelFactor * Rigidbody.mass * direction;
                Debug.Log($"adding force {f}");
                Rigidbody.AddForce(f);
                return true;
            }

            Debug.Log("no move");
            return false;
        }

        private void FaceDirectionWithoutRotating(Vector2 direction)
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
            if (absDot < destinationTolerance)
            {
                moveImpetus = 0;
                return Vector2.zero;
            }
            moveImpetus = absDot;
            return Mathf.Sign(dot) * r;
        }

        private RaycastHit2D RaycastToBoard()
        {
            return Physics2D.Raycast(transform.position, -SkateboardMover.Board.transform.up, 
                SkateboardMover.BoardLayer);
        }


        //BALANCE
        
        public void Balance()
        {
            var d = -Vector2.Dot(transform.right, SkateboardMover.Board.transform.up);
            if (d < balanceBreakPoint)
            {
                BalanceBroken = true;
            }
            else
            {
                Rigidbody.AddTorque(d * balanceStrength * Rigidbody.mass);
            }
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
    }
}
