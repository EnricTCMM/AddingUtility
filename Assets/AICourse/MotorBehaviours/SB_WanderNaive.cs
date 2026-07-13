using UnityEngine;

namespace MotorBehaviours
{
    public class SB_WanderNaive : LinearMotorBehaviour
    {
        [Header("Naive Wander Settings")]
        [Tooltip("How much the agent can randomly turn per frame (in degrees)")]
        public float wanderRate = 5f; 

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Slightly change agent's orientation
            float angleChange = (Random.value - Random.value) * wanderRate;
            
            Vector3 currentRotation = me.transform.eulerAngles;
            currentRotation.z += angleChange;
            me.transform.eulerAngles = currentRotation;

            // delegate
            return SB_GoWhereYouLook.CalculateDesiredVelocity(me); 
        }
    }
}