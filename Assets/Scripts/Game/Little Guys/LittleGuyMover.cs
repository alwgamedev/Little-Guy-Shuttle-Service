using UnityEngine;

namespace LGShuttle.Game
{
    public class LittleGuyMover : MonoBehaviour
    {
        [SerializeField] Transform frontFoot;
        [SerializeField] Transform backFoot;
        [SerializeField] float balanceStrength;
        [Range(0, 90)][SerializeField] float balanceBreakAngle = 60;
        [SerializeField] float acceleration;
        [SerializeField] float destinationTolerance = 0.1f;

        Rigidbody2D rb;

        float balanceBreakPoint;
        bool balanceBroken;
        //Vector2 frontFootRight;
        //Vector2 backFootRight;

        int boardLayer;
        Vector2 boardAnchorPtLocalPos;

        Vector2 BoardAnchorPt => boardAnchorPtLocalPos + (Vector2)SkateboardMover.Board.transform.position;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            balanceBreakPoint = Mathf.Cos(Mathf.Deg2Rad * (balanceBreakAngle + 90));
            //frontFootRight = frontFoot.right;
            //backFootRight = backFoot.right;
            boardLayer = LayerMask.GetMask("Skateboard");
        }

        private void Start()
        {
            var h = RaycastToBoard();
            boardAnchorPtLocalPos = h.point - (Vector2)SkateboardMover.Board.transform.position;
        }

        private void LateUpdate()
        {
            if (!balanceBroken)
            {
                KeepFeetLevel();
            }
        }

        private void FixedUpdate()
        {
            if (!balanceBroken)
            {
                MoveTowardsBoardAnchorPoint();
                Balance();
            }
        }

        //MOVEMENT

        public void MoveTowardsBoardAnchorPoint()
        {
            Move(acceleration, MoveDirectionAlongBoard(BoardAnchorPt));
        }

        private bool Move(float acceleration, Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                FaceDirectionWithoutRotating(direction);
                rb.AddForce(acceleration * rb.mass * direction);
                return true;
            }

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

        private Vector2 MoveDirectionAlongBoard(Vector2 targetPoint)
        {
            var d = targetPoint - (Vector2)transform.position;
            var r = SkateboardMover.Board.transform.right;
            var dot = Vector2.Dot(d, r);
            if (Mathf.Abs(dot) < destinationTolerance)
            {
                return Vector2.zero;
            }
            return Mathf.Sign(Vector2.Dot(d, r)) * r;
        }

        private RaycastHit2D RaycastToBoard()
        {
            return Physics2D.Raycast(transform.position, -SkateboardMover.Board.transform.up, boardLayer);
        }


        //BALANCE
        
        public void Balance()
        {
            var d = -Vector2.Dot(transform.right, SkateboardMover.Board.transform.up);
            if (d < balanceBreakPoint)
            {
                balanceBroken = true;
            }
            else
            {
                rb.AddTorque(d * balanceStrength * rb.mass);
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
