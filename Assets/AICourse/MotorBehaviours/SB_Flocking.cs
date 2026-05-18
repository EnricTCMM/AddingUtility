using UnityEngine;

namespace MotorBehaviours
{
    public class SB_Flocking : LinearMotorBehaviour
    {
        [Header("Group Registry")]
        public GroupRegistry registry;

        [Header("Global Vision Settings")]
        public bool applyVision = false;
        [Range(0f, 360f)] public float coneOfVisionAngle = 270f;

        [Header("1. Separation Settings")]
        [Range(0f, 5f)] public float separationWeight = 1.5f;
        public float separationThreshold = 2f;

        [Header("2. Cohesion Settings")]
        [Range(0f, 5f)] public float cohesionWeight = 1.0f;
        public float cohesionThreshold = 5f;

        [Header("3. Alignment Settings")]
        [Range(0f, 5f)] public float alignmentWeight = 1.0f;
        public float alignmentThreshold = 5f;

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
            bool anyBehaviorActive = false;

            // --- 1. SEPARATION INGREDIENT ---
            Vector3? separationVelocity = SB_Separation.GetDesiredVelocity(
                me, registry, separationThreshold, applyVision, coneOfVisionAngle
            );
            
            if (separationVelocity.HasValue)
            {
                blendedVelocity += separationVelocity.Value * separationWeight;
                anyBehaviorActive = true;
            }

            // --- 2. COHESION INGREDIENT ---
            Vector3? cohesionVelocity = SB_Cohesion.GetDesiredVelocity(
                me, registry, cohesionThreshold, applyVision, coneOfVisionAngle
            );
            
            if (cohesionVelocity.HasValue)
            {
                blendedVelocity += cohesionVelocity.Value * cohesionWeight;
                anyBehaviorActive = true;
            }

            // --- 3. ALIGNMENT INGREDIENT ---
            Vector3? alignmentVelocity = SB_Alignment.GetDesiredVelocity(
                me, registry, alignmentThreshold, applyVision, coneOfVisionAngle
            );
            
            if (alignmentVelocity.HasValue)
            {
                blendedVelocity += alignmentVelocity.Value * alignmentWeight;
                anyBehaviorActive = true;
            }

            if (!anyBehaviorActive)
            {
                return null;
            }

            if (blendedVelocity.magnitude > me.maxSpeed)
            {
                blendedVelocity = blendedVelocity.normalized * me.maxSpeed;
            }

            return blendedVelocity;
        }
    }
}