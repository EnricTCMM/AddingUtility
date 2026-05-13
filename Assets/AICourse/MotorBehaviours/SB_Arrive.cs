using UnityEngine;

namespace MotorBehaviours
{
    public class SB_Arrive : LinearMotorBehaviour
    {
        [Header("Arrive Settings")]
        public GameObject target;

        // The distance threshold to completely stop (in meters)
        public float toleranceRadius = 1f; 
        // The distance threshold to start slowing down (in meters)
        public float slowdownRadius = 20f; 

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Fail Fast: Check if target is missing
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Missing Target on {GetType().Name} component of GameObject '{gameObject.name}'.");
                Debug.Break();
                return Vector3.zero;
            }
            return SB_Arrive.GetDesiredVelocity(me, target.transform.position, toleranceRadius, slowdownRadius);
        }
        
        public static Vector3 GetDesiredVelocity(MotorManager me, Vector3 targetPosition, float tolerance, float slowdown)
        {
            Vector3 direction = targetPosition - me.transform.position;
            float distance = direction.magnitude;

            // A. If we are within the tolerance zone our desired velocity is zero
            if (distance < tolerance)
            {
                return Vector3.zero;
            }

            // B. DELEGATION: If we are outside the slowdown zone, we behave exactly like Seek
            if (distance > slowdown)
            {
                return SB_Seek.GetDesiredVelocity(me, targetPosition);
            }

            // C. Inside slowdown zone: calculate the proportional speed
            float targetSpeed = me.maxSpeed * (distance / slowdown);

            // Return the desired velocity vector
            return direction.normalized * targetSpeed;
        }
    }
}