using System.Collections.Generic;
using AICourse.Utilities;
using UnityEngine;
using Steerings; // Required for Utils.InCone

namespace MotorBehaviours
{
    public class SB_Separation : LinearMotorBehaviour
    {
        [Header("Flock Registry")]
        public GroupRegistry registry;

        [Header("Separation Settings")]
        public float repulsionThreshold = 2f;
        
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
            return SB_Separation.GetDesiredVelocity(me, registry, repulsionThreshold, applyVision, coneOfVisionAngle);
        }

        // Math is done in this method. Also available for external calls
        public static Vector3? GetDesiredVelocity(MotorManager me, GroupRegistry groupRegistry, float threshold, bool useVision, float visionAngle)
        {
            // Safe exit to prevent NullReferenceException if called dynamically without a valid registry
            if (groupRegistry == null)
            {
                Debug.LogWarning($"[MotorBehaviours] Warning: Separation invoked with a null GroupRegistry.");
                return null; // Abstention
            }

            // High-efficiency roster retrieval O(1)
            List<GameObject> targets = groupRegistry.GetMembers();
            
            Vector3 totalDesiredVelocity = Vector3.zero;
            int neighborsCount = 0;

            foreach (GameObject target in targets)
            {
                // Do not take yourself into account
                if (target == me.gameObject)
                {
                    continue;
                }

                Vector3 directionToTarget = target.transform.position - me.transform.position;
                float distanceToTarget = directionToTarget.magnitude;

                // Disregard distant targets outside our personal space
                if (distanceToTarget > threshold) 
                {
                    continue;
                }
                
                // Safety check: Avoid exact position overlap to prevent zero division on normalization
                if (distanceToTarget < 0.001f)
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

                // Calculate how strong the separation should be.
                // The closer the neighbor, the stronger the velocity away from them.
                float separationStrength = me.maxSpeed * (threshold - distanceToTarget) / threshold;
                
                // Accumulate the velocity pointing AWAY from the target
                totalDesiredVelocity -= directionToTarget.normalized * separationStrength;
                
                neighborsCount++;
                
            } // End of iteration over all registered group members

            // If there are no neighbors in our personal space, we abstain completely
            if (neighborsCount == 0)
            {
                return null;
            }

            // Clip the total velocity so we don't break the maxSpeed architectural rule
            if (totalDesiredVelocity.magnitude > me.maxSpeed)
            {
                totalDesiredVelocity = totalDesiredVelocity.normalized * me.maxSpeed;
            }

            return totalDesiredVelocity;
        }
    }
}