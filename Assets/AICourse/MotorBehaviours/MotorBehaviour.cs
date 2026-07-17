using UnityEngine;

namespace MotorBehaviours
{
    [RequireComponent(typeof(MotorManager))]
    public abstract class MotorBehaviour : MonoBehaviour
    {
        [Header("Base congfiguration")] [Range(0f, 1f)]
        public float blendingWeight = 1f; // weight for blending
        public int arbitrationPriority = 1; // arbitration priority 
        
        // convenience methods to enable/disable the motor behaviours without having to resort to
        // enabled = true/false in the code. 
        public void Enable()
        {
            enabled = true;
        }
        
        public void Disable()
        {
            Debug.Log($"Disabling MB {this.GetType().Name}");
            enabled = false;
        }

    }
}