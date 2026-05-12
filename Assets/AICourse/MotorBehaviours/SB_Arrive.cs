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
        // How fast we want to reach the desired speed
        public float timeToDesiredSpeed = 0.1f;

        public override Vector3 GetDesiredVelocity(MotorManager me)
        {
            // 1. Fail Fast: Check if target is missing
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Missing Target on {GetType().Name} component of GameObject '{gameObject.name}'.");
                Debug.Break();
                return Vector3.zero;
            }

            // 2. Calculate the direction vector and the distance to the target
            Vector3 direction = target.transform.position - me.transform.position;
            float distance = direction.magnitude;
            
            float targetSpeed;
            
            // 3. Check if we have arrived within the tolerance radius
            if (distance < toleranceRadius)
            {
                me.StopCompletely(); // off-physics, but ensures no jittering
                return Vector3.zero;
            }

            // 4. Calculate desired speed based on distance
            
            else if (distance > slowdownRadius)
            {
                // Outside the slowdown radius, we go at maximum speed
                targetSpeed = me.maxSpeed;
            }
            else
            {
                // Linear falloff inside the slowdown radius
                targetSpeed = me.maxSpeed * (distance / slowdownRadius);
            }

            // 5. Calculate desired velocity
            // Optimization: (direction / distance) is mathematically identical to direction.normalized
            // but saves the CPU from calculating the square root twice.
            Vector3 desiredVelocity = Vector3.zero;
            // avoid division by zero
            if (distance > 0) 
                desiredVelocity = (direction / distance) * targetSpeed;

            // 6. The steering force is the difference between desired and current velocity
            Vector3 force = (desiredVelocity - me.currentVelocity) / timeToDesiredSpeed;

            return force;
        }
    }
}