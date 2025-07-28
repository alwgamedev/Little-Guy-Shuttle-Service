using LGShuttle.Core;
using UnityEngine;

namespace LGShuttle.Game
{
    public class LittleGuyController : MonoBehaviour
    {
        [SerializeField] float moveAnimationDampTime;
        [SerializeField] float panicAnimationScale = 1;
        [SerializeField] float panicDampTime = .15f;
        [SerializeField] float panicFromMoveImpetusMax;
        [SerializeField] RandomizableFloat panicRandomizer;
        [SerializeField] float randomPanicLerpRate;
        [SerializeField] RandomizableFloat repositionTime;
        [SerializeField] RandomizableColor[] randomColors;
        [SerializeField] RandomizableFloat randomScale;

        LittleGuyMover mover;
        Animator animator;
        float randomPanic;

        float _repositionTime;
        float repositionTimer;

        public LittleGuyMover Mover
        {
            get
            {
                if (mover == null)
                {
                    mover = GetComponent<LittleGuyMover>();
                }

                return mover;
            }
        }

        private void Awake()
        {
            if (mover == null)
            {
                mover = GetComponent<LittleGuyMover>();
            }
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            ResetRepositionTimer();
            RandomizeColor();
            RandomizeScale();
        }

        private void Update()
        {
            AnimateMovement();
            if (!mover.BalanceBroken)
            {
                UpdateRepositionTimer();
            }
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
        }


        //SETTINGS

        public void SetSortingOrder(int order)
        {
            GetComponentInChildren<SortingLayerControl>().SortingOrder = 25 * order;
        }

        private void RandomizeColor()
        {
            var i = MiscTools.rng.Next(randomColors.Length);
            var c = randomColors[i].Value;
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color *= c;
            }
        }

        private void RandomizeScale()
        {
            transform.localScale = randomScale.Value * transform.localScale;
        }


        //POSITIONING

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


        ////STATE CHANGE

        //private void OnBalanceBroken()
        //{

        //}


        //ANIMATION

        private void AnimateMovement()
        {
            animator.SetFloat("moveSpeed", ComputeAnimatorMoveSpeed(), moveAnimationDampTime, Time.deltaTime);
            animator.SetFloat("panic", ComputeAnimatorPanic(), panicDampTime, Time.deltaTime);
        }

        private float ComputeAnimatorMoveSpeed()
        {
            var f = Mathf.Abs(mover.RelativeVelocityAlongBoard) / mover.RunSpeed;
            return Mathf.Clamp(f, 0, 1);
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