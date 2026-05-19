

using UnityEngine;

namespace MotorBehaviours
{
    public class SB_Flocking : LinearMotorBehaviour
    {
        [Header("Flock Registry")]
        public GroupRegistry registry;

        [Header("Global Vision Settings")]
        public bool applyVision = false;
        [Range(0f, 360f)] public float coneOfVisionAngle = 270f;

        [Header("1. Separation Settings")]
        [Range(0f, 1f)] public float separationWeight = 0.6f;
        public float separationThreshold = 2f;

        [Header("2. Cohesion Settings")]
        [Range(0f, 1f)] public float cohesionWeight = 0.2f;
        public float cohesionThreshold = 5f;

        [Header("3. Alignment Settings")]
        [Range(0f, 1f)] public float alignmentWeight = 0.2f;
        public float alignmentThreshold = 5f;

        [Header("4. Attractor Settings (FlockingAround)")]
        [Tooltip("If assigned, the flock will seek this target collectively.")]
        public GameObject attractor;
        [Range(0f, 1f)] public float attractionWeight = 0.1f;

        // The method that is called by the MotorManager
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            if (registry == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Missing GroupRegistry on {GetType().Name} component of GameObject '{gameObject.name}'.");
                Debug.Break();
                return Vector3.zero;
            }

            Vector3 blendedVelocity = Vector3.zero;
            float totalActiveWeight = 0f;

            // --- 1. SEPARATION INGREDIENT ---
            Vector3? separationVelocity = SB_Separation.GetDesiredVelocity(
                me, registry, separationThreshold, applyVision, coneOfVisionAngle
            );
            
            if (separationVelocity.HasValue && separationWeight > 0f)
            {
                blendedVelocity += separationVelocity.Value * separationWeight;
                totalActiveWeight += separationWeight;
            }

            // --- 2. COHESION INGREDIENT ---
            Vector3? cohesionVelocity = SB_Cohesion.GetDesiredVelocity(
                me, registry, cohesionThreshold, applyVision, coneOfVisionAngle
            );
            
            if (cohesionVelocity.HasValue && cohesionWeight > 0f)
            {
                blendedVelocity += cohesionVelocity.Value * cohesionWeight;
                totalActiveWeight += cohesionWeight;
            }

            // --- 3. ALIGNMENT INGREDIENT ---
            Vector3? alignmentVelocity = SB_Alignment.GetDesiredVelocity(
                me, registry, alignmentThreshold, applyVision, coneOfVisionAngle
            );
            
            if (alignmentVelocity.HasValue && alignmentWeight > 0f)
            {
                blendedVelocity += alignmentVelocity.Value * alignmentWeight;
                totalActiveWeight += alignmentWeight;
            }

            // --- 4. ATTRACTOR INGREDIENT (FLOCKING AROUND) ---
            if (attractor != null && attractionWeight > 0f)
            {
                Vector3 attractorVelocity = SB_Seek.GetDesiredVelocity(me, attractor.transform.position);
                // Seek returns a Vector3, not nullable, so we apply it directly
                blendedVelocity += attractorVelocity * attractionWeight;
                totalActiveWeight += attractionWeight;
            }

            // If no behaviors contributed (or all active ones had 0 weight), abstain
            if (totalActiveWeight == 0f)
            {
                return null;
            }

            // NORMALIZATION POLICY
            // Divide by the sum of applied weights to obtain a true weighted average
            blendedVelocity /= totalActiveWeight;

            // Architectural safety clamp 
            if (blendedVelocity.magnitude > me.maxSpeed)
            {
                blendedVelocity = blendedVelocity.normalized * me.maxSpeed;
            }

            return blendedVelocity;
        }
    }
}