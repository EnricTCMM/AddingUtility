using UnityEngine;
using Steerings; // Required for Utils.InCone

namespace MotorBehaviours
{
    public class SB_Cohesion : LinearMotorBehaviour
    {
        [Header("Cohesion Settings")]
        public string idTag = "Boid"; // Tag used to find neighbors
        public float cohesionThreshold = 5f;
        
        [Header("Vision Settings")]
        public bool applyVision = false;
        public float coneOfVisionAngle = 90f;

        // The method that is called by the MotorManager
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // We pass the inspector variables to the static math method
            return SB_Cohesion.GetDesiredVelocity(me, idTag, cohesionThreshold, applyVision, coneOfVisionAngle);
        }

        // Math is done in this method. Also available for external calls
        public static Vector3? GetDesiredVelocity(MotorManager me, string targetTag, float threshold, bool useVision, float visionAngle)
        {
            // WARNING: FindGameObjectsWithTag is computationally expensive O(n^2).
            // Ideal candidate to be replaced by a centralized BoidSensor later.
            GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
            
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
                
            } // End of iteration over all cohesion targets 

            // If there are no mates around, we abstain completely
            if (mates == 0)
            {
                return null;
            }

            // Calculate the actual center of masses (average position)
            centreOfMasses /= mates;

            // EXPLICIT DELEGATION TO SEEK
            // We mathematically delegate the actual steering calculation to SB_Seek
            // asking it to go to the calculated center of masses.
            return SB_Seek.GetDesiredVelocity(me, centreOfMasses);
        }
    }
}