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
        [SerializeField] float colorSaturation;
        [SerializeField] RandomizableFloat randomScale;

        LittleGuyMover mover;
        Animator animator;
        ParticleSystem deathParticles;
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
            deathParticles = GetComponentInChildren<ParticleSystem>();
        }

        private void Start()
        {
            mover.Death += OnDeath;
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

        private void OnDeath(LittleGuyMover lg)
        {
            deathParticles.transform.SetParent(null);
            deathParticles.Play();
        }


        //SETTINGS

        public void SetSortingOrder(int order)
        {
            GetComponentInChildren<SortingLayerControl>().SortingOrder = 25 * order;
        }

        private void RandomizeColor()
        {
            var c = RandomizableColor.RandomColor(colorSaturation, 1);
            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color *= c;
            }

            var m = deathParticles.main;
            m.startColor = new ParticleSystem.MinMaxGradient(m.startColor.colorMin * c, m.startColor.colorMax * c);
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


        //ANIMATION

        private void AnimateMovement()
        {
            animator.SetFloat("moveSpeed", ComputeAnimatorMoveSpeed(), moveAnimationDampTime, Time.deltaTime);
            animator.SetFloat("panic", ComputeAnimatorPanic(), panicDampTime, Time.deltaTime);
        }

        private float ComputeAnimatorMoveSpeed()
        {
            var f = Mathf.Abs(mover.RelativeVelocity.magnitude) / mover.RunSpeed;
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