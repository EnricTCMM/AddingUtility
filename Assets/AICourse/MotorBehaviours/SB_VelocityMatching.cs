using UnityEngine;

namespace MotorBehaviours
{
    public class SB_VelocityMatch : LinearMotorBehaviour
    {
        [Header("Velocity Match Settings")]
        public GameObject target;

        // The method that is called by the MotorManager
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Fail Fast: Check if target is missing
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Missing Target on {GetType().Name} component of GameObject '{gameObject.name}'.");
                Debug.Break();
                return Vector3.zero;
            }
            
            // Fail Fast: Check if target lacks a MotorManager
            MotorManager targetManager = target.GetComponent<MotorManager>();
            if (targetManager == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: target '{target.name}' lacks a MotorManager in {GetType().Name} component of GameObject '{gameObject.name}'.");
                Debug.Break();
                return Vector3.zero;
            }

            return SB_VelocityMatch.GetDesiredVelocity(me, target);
        }

        // Intermediate method. Retrieves target's info
        public static Vector3? GetDesiredVelocity(MotorManager me, GameObject target)
        {
            // Safe exit to prevent NullReferenceException if called dynamically
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: VelocityMatching with null target");
                return null; // Abstention
            }

            MotorManager targetManager = target.GetComponent<MotorManager>();
            if (targetManager == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Target '{target.name}' in VelocityMatch contains no MotorManager.");
                Debug.Break();
                return Vector3.zero;
            }

            // We extract the target's current velocity and pass it to the math method
            return SB_VelocityMatch.GetDesiredVelocity(me, targetManager.currentVelocity);
        }

        // Math is done in this method. Also available for external calls (like Alignment)
        public static Vector3? GetDesiredVelocity(MotorManager me, Vector3 targetVelocity)
        {
            // In a velocity-based steering architecture, matching velocity means our 
            // desired velocity is simply the target's current velocity.
            
            // We just clamp the target's velocity to our own maximum speed to ensure
            // we don't ask the motor to do something impossible.
            if (targetVelocity.magnitude > me.maxSpeed)
            {
                return targetVelocity.normalized * me.maxSpeed;
            }
            
            return targetVelocity;
        }
    }
}