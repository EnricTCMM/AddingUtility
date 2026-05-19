

using UnityEngine;

namespace MotorBehaviours
{
    public class SB_Flocking : LinearMotorBehaviour
    {
        [Header("Flock Registry")]
        public GroupRegistry registry;

        [Header("Global Vision Settings")]
        [Tooltip("If not applied cone is of 360 deg. Never applied to separation")]
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

        [Header("Debug Settings")]
        public bool showFlockingGizmos = true;
        
        [Tooltip("Number of line segments used to draw the circles.")]
        [HideInInspector] public int gizmoSegments = 24;
        
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
                me, registry, separationThreshold, false, 360f
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
            // ESN 
            // and then maxSpeed 
            blendedVelocity = blendedVelocity.normalized * me.maxSpeed;

            return blendedVelocity;
        }
        
        // Draws visual debugging information in the Editor only when the agent is selected
        private void OnDrawGizmos()
        {
            if (!showFlockingGizmos || !Application.isPlaying) return;

            Vector3 pos = transform.position;

            // 1. Separation Threshold (red)
            Gizmos.color = Color.red;
            DrawGizmoCircle(pos, separationThreshold, gizmoSegments);

            // 2. Alignment Threshold (Orange)
            Gizmos.color = new Color(1.0f, 0.5f, 0.0f); 
            DrawGizmoCircle(pos, alignmentThreshold, gizmoSegments);

            // 3. Cohesion Threshold (blue)
            Gizmos.color = Color.blue;
            DrawGizmoCircle(pos, cohesionThreshold, gizmoSegments);

            // 4. Cone of Vision (Black Whiskers)
            // We only draw them if vision is applied and it's not a full 360 degree angle
            if (applyVision && coneOfVisionAngle < 360f)
            {
                Gizmos.color = Color.black;
                float halfAngle = coneOfVisionAngle / 2f;

                // Assuming the boid faces the local X axis (as is standard in XY plane trigonometry),
                // we use transform.right as the "Forward" vector.
                // We rotate this vector half the angle to each side around the Z axis.
                Vector3 rightWhisker = Quaternion.Euler(0, 0, -halfAngle) * transform.right * cohesionThreshold;
                Vector3 leftWhisker = Quaternion.Euler(0, 0, halfAngle) * transform.right * cohesionThreshold;

                // Draw the lines from the center to the tip of the whisker
                Gizmos.DrawLine(pos, pos + rightWhisker);
                Gizmos.DrawLine(pos, pos + leftWhisker);
            }
        }

        // Auxiliary method to draw circles on the XY plane
        private void DrawGizmoCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angleRad = i * angleStep * Mathf.Deg2Rad;
                Vector3 nextPoint = center + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * radius;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
        
    }
}