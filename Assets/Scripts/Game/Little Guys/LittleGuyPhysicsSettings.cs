using LGShuttle.Core;
using System;
using UnityEngine;

namespace LGShuttle.Game
{
    [Serializable]
    public struct LittleGuyPhysicsSettings
    {
        [Header("Balance")]
        public float balanceStrength;
        [Range(0, 90)] public float balanceBreakAngle;
        public float balancedAngularDamping;
        public float fallenAngularDamping;

        [Header("Movement")]
        public float runThreshold;
        public float walkSpeed;
        public float runSpeed;
        public float accelMultiplier;
        public RandomizableFloat randomAccelerationDampTime;

        [Header("Logic")]
        public float destinationTolerance;
        public float groundednessToleranceFactor;
    }
}