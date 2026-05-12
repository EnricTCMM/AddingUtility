using UnityEngine;
namespace MotorBehaviours
{
    public class SB_Seek : LinearMotorBehaviour
    {
        [Header("Seek Settings")]
        public GameObject target;

        public override Vector3 GetDesiredVelocity(MotorManager me)
        {
            // If there's no target FAIL-FAST
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical error: No target in {GetType().Name} in GameObject '{gameObject.name}'.");
                Debug.Break(); // immediate pause. 
                return Vector3.zero;
            }
            
            // 1. Direction towards the target
            Vector3 direction = target.transform.position - me.transform.position;
            
            // Safety check to avoid normalizing a zero vector
            if (direction.sqrMagnitude == 0f) return Vector3.zero;

            // 2. The desired velocity is reaching the target at maximum speed
            Vector3 desiredVelocity = direction.normalized * me.maxSpeed;

            return desiredVelocity;
        }
    }
}