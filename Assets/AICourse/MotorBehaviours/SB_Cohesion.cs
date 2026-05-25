using System.Collections.Generic;
using AICourse.Utilities;
using UnityEngine;
using Steerings; // Required for Utils.InCone

namespace MotorBehaviours
{
    public class SB_Cohesion : LinearMotorBehaviour
    {
        [Header("Flock Registry")]
        public GroupRegistry registry;

        [Header("Cohesion Settings")]
        public float cohesionThreshold = 5f;
        
        [Header("Vision Settings")]
        public bool applyVision = false;
        public float coneOfVisionAngle = 90f;

        // The method that is called by the MotorManager
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Fail Fast: Check if registry is missing
            if (registry == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Missing GroupRegistry on {GetType().Name} component of GameObject '{gameObject.name}'.");
                Debug.Break();
                return Vector3.zero;
            }

            // We pass the inspector variables to the static math method
            return SB_Cohesion.GetDesiredVelocity(me, registry, cohesionThreshold, applyVision, coneOfVisionAngle);
        }

        // Math is done in this method. Also available for external calls
        public static Vector3? GetDesiredVelocity(MotorManager me, GroupRegistry groupRegistry, float threshold, bool useVision, float visionAngle)
        {
            // Safe exit to prevent NullReferenceException if called dynamically without a valid registry
            if (groupRegistry == null)
            {
                Debug.LogWarning($"[MotorBehaviours] Warning: Cohesion invoked with a null GroupRegistry.");
                return null; // Abstention
            }

            // High-efficiency roster retrieval O(1)
            List<GameObject> targets = groupRegistry.GetMembers();
            
            Vector3 centreOfMasses = Vector3.zero;
            int mates = 0;

            foreach (GameObject target in targets)
            {
                // Do not take yourself into account
                if (target == me.gameObject)
                {
                    continue;
                }

                float distanceToTarget = (target.transform.position - me.transform.position).magnitude;

                // Disregard distant targets
                if (distanceToTarget > threshold) 
                {
                    continue;
                }

                // Disregard targets outside cone of vision if necessary
                if (useVision)
                {
                    if (!Utils.InCone(me.gameObject, target, visionAngle)) 
                    {
                        continue;
                    }
                }

                // Accumulate positions to calculate the center later
                centreOfMasses += target.transform.position;
                mates++;
                
            } // End of iteration over all registered group members

            // If there are no mates around, we abstain completely
            if (mates == 0)
            {
                return null;
            }

            // Calculate the actual center of masses (average position)
            centreOfMasses /= mates;

            // EXPLICIT DELEGATION TO SEEK
            // We mathematically delegate the actual steering calculation to SB_Seek
            return SB_Seek.GetDesiredVelocity(me, centreOfMasses);
        }
    }
}