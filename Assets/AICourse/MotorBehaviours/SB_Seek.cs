using UnityEngine;
namespace MotorBehaviours
{
    public class SB_Seek : LinearMotorBehaviour
    {
        [Header("Seek Settings")]
        public GameObject target;

        public override Vector3 GetForce(MotorManager me)
        {
            // 1. If there's no target FAIL-FAST
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical error: No target in {GetType().Name} in GameObject '{gameObject.name}'.");
                Debug.Break(); // immediate pause. 
                return Vector3.zero;
            }

            // 2. Calculate the vector from the agent to the target
            Vector3 direction = target.transform.position - me.transform.position;

            // Optional: If we are exactly on the target, stop seeking
            if (direction.sqrMagnitude == 0.0f) return Vector3.zero;

            // 3. Desired velocity is reaching the target at maximum speed
            Vector3 desiredVelocity = direction.normalized * me.maxSpeed;

            // 4. The steering force is the difference between desired and current velocity
            Vector3 force = desiredVelocity - me.currentVelocity;

            return force;
        }
    }
}