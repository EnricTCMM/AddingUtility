
using UnityEngine;
using Steerings; // need to access Utils

namespace MotorBehaviours
{
    public class SB_KeepPosition : LinearMotorBehaviour
    {
        [Header("Target Settings")]
        public GameObject target;
        public float distance = 2f;
        [Tooltip("Angle offset in degrees relative to the target's Z rotation.")]
        public float angle = 45f;

        [Header("Arrive Settings")]
        public float arriveRadius = 0.5f;
        public float slowDownRadius = 5f;

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Fail Fast: If there is no target, abstain so other behaviors can take over
            if (target == null)
            {
                return null;
            }
            
            // Calculate the absolute angle by adding the offset to the target's current Z rotation
            float desiredAngle = target.transform.rotation.eulerAngles.z + angle;
            
            // Convert the scalar angle into a normalized directional vector
            Vector3 desiredDirectionFromTarget = Utils.OrientationToVector(desiredAngle);
            
            // Scale the direction by the desired distance to get the final displacement vector
            Vector3 displacement = desiredDirectionFromTarget * distance;
            
            // Add the displacement to the target's origin to find the exact world coordinate
            Vector3 desiredPosition = target.transform.position + displacement;

            //  DELEGATION 
            // Pass the calculated coordinate to the static math function of SB_Arrive
            return SB_Arrive.GetDesiredVelocity(me, desiredPosition, arriveRadius, slowDownRadius);
        }
    }
}