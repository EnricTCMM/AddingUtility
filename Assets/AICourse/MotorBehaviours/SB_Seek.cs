using UnityEngine;
namespace MotorBehaviours
{
    public class SB_Seek : LinearMotorBehaviour
    {
        [Header("Seek Settings")]
        public GameObject target;

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // If there's no target FAIL-FAST
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical error: No target in {GetType().Name} in GameObject '{gameObject.name}'.");
                Debug.Break(); // immediate pause. 
                return Vector3.zero;
            }
            
            return SB_Seek.GetDesiredVelocity(me, target.transform.position);
        }
        
        // Math is done in this method
        public static Vector3 GetDesiredVelocity(MotorManager me, Vector3 targetPosition)
        {
            Vector3 directionToTarget = targetPosition - me.transform.position;
            
            // Safety check. Don't try to normalize a zero vector (not strictly necessary
            // since Unity returns Vector3.zero for zero vectors) 
            if (directionToTarget.sqrMagnitude == 0f) return Vector3.zero;

			// intended velocity is full speed towards the target.
            return directionToTarget.normalized * me.maxSpeed;
        }
    }
}