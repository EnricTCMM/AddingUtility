using UnityEngine;
using Steerings; // Required for Utils.InCone

namespace MotorBehaviours
{
    public class SB_Separation : LinearMotorBehaviour
    {
        [Header("Separation Settings")]
        public string idTag = "Boid"; // Tag used to find neighbors
        public float repulsionThreshold = 2f;
        
        [Header("Vision Settings")]
        public bool applyVision = false;
        public float coneOfVisionAngle = 90f;

        // The method that is called by the MotorManager
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // We pass the inspector variables to the static math method
            return SB_Separation.GetDesiredVelocity(me, idTag, repulsionThreshold, applyVision, coneOfVisionAngle);
        }

        // Math is done in this method. Also available for external calls
        public static Vector3? GetDesiredVelocity(MotorManager me, string targetTag, float threshold, bool useVision, float visionAngle)
        {
            // WARNING: FindGameObjectsWithTag is mathematically correct but computationally expensive.
            // In a full Flocking scenario with many agents, this should eventually be replaced 
            // by a centralized "BoidSensor" that provides a pre-calculated list of neighbors.
            GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
            
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
                
            } // End of iteration over all repulsive targets 

            // If there are no neighbors in our personal space, we abstain completely
            // so we don't interfere with other behaviors like Alignment or Cohesion.
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