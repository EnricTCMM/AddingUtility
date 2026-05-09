using UnityEngine;

namespace MotorBehaviours
{
    public abstract class MotorBehaviour : MonoBehaviour
    {
        [Header("Base congfiguration")] [Range(0f, 1f)]
        public float blendingWeight = 1f; // weight for blending
        public int arbitrationPriority = 1; // arbitration priority (2 is higher priority)
    }
}