using LGShuttle.Core;
using UnityEngine;

namespace LGShuttle.Game
{
    public class LittleGuyController : MonoBehaviour
    {
        //[SerializeField] float moveAnimationScale = .5;
        //[SerializeField] float moveAnimationDampTime = .15f;
        [SerializeField] float panicAnimationScale = 1;
        [SerializeField] float panicDampTime = .15f;
        [SerializeField] float panicFromMoveImpetusMax;
        //[SerializeField] float panicBoardVelocityMin;
        [SerializeField] RandomizableFloat panicRandomizer;
        [SerializeField] float randomPanicLerpRate;
        [SerializeField] RandomizableFloat repositionTime;

        LittleGuyMover mover;

        Animator animator;
        float randomPanic;

        float _repositionTime;
        float repositionTimer;

        private void Awake()
        {
            mover = GetComponent<LittleGuyMover>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            ResetRepositionTimer();
        }

        private void Update()
        {
            AnimateMovement();
            UpdateRepositionTimer();
        }

        private void LateUpdate()
        {
            if (!mover.BalanceBroken)
            {
                mover.KeepFeetLevel();
            }
        }

        private void FixedUpdate()
        {
            if (!mover.BalanceBroken)
            {
                mover.DefaultBehavior();
            }

            //AnimateMovement();
        }

        private void ResetRepositionTimer()
        {
            _repositionTime = repositionTime.Value;
            repositionTimer = 0;
        }

        private void UpdateRepositionTimer()
        {
            repositionTimer += Time.deltaTime;
            if (repositionTimer > _repositionTime)
            {
                mover.ChooseNewAnchorPoint();
                ResetRepositionTimer();
            }
        }


        //ANIMATION

        private void AnimateMovement()
        {
            animator.SetFloat("moveSpeed", ComputeAnimatorMoveSpeed());
            animator.SetFloat("panic", ComputeAnimatorPanic(), panicDampTime, Time.deltaTime);
        }

        private float ComputeAnimatorMoveSpeed()
        {
            return Mathf.Abs(mover.RelativeVelocityAlongBoard) / mover.RunSpeed;
        }

        private float ComputeAnimatorPanic()
        {
            if (!mover.ShouldRun)
            {
                return 0;
            }

            var i = mover.MoveImpetus * panicAnimationScale;
            i = Mathf.Min(i, panicFromMoveImpetusMax);
            randomPanic = Mathf.Lerp(randomPanic, panicRandomizer.Value, randomPanicLerpRate * Time.deltaTime);
            return i + randomPanic;
        }
    }
}