using UnityEngine;

namespace MotorBehaviours
{
    /// <summary>
    /// Tries to place the agent at the midpoint between two targets.
    /// </summary>
    public class SB_Interpose : LinearMotorBehaviour
    {
        [Header("Interpose Targets")]
        public GameObject targetA;
        public GameObject targetB;

        [Header("Arrive Settings")]
        [Tooltip("Distance from the midpoint at which the agent stops.")]
        public float toleranceRadius = 1f;

        [Tooltip("Distance from the midpoint at which the agent starts slowing down.")]
        public float slowdownRadius = 5f;

        [Header("Debug Settings")]
        [Tooltip("Draws the segment between the targets and its midpoint.")]
        public bool showGizmos = true;

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Fail fast: both targets are required.
            if (targetA == null || targetB == null)
            {
                Debug.LogError(
                    $"[MotorBehaviours] Critical Error: Missing target A or B " +
                    $"on {GetType().Name} component of GameObject " +
                    $"'{gameObject.name}'."
                );

                Debug.Break();
                return null;
            }

            return SB_Interpose.GetDesiredVelocity(
                me,
                targetA,
                targetB,
                toleranceRadius,
                slowdownRadius
            );
        }

        /// <summary>
        /// Calculates the midpoint between the targets and delegates
        /// the movement calculation to SB_Arrive.
        /// </summary>
        public static Vector3? GetDesiredVelocity(
            MotorManager me,
            GameObject targetA,
            GameObject targetB,
            float toleranceRadius,
            float slowdownRadius)
        {
            // Safe exit for external or dynamic calls.
            if (targetA == null || targetB == null)
            {
                Debug.LogWarning(
                    "[MotorBehaviours] Warning: Interpose invoked " +
                    "with a null target."
                );

                return null;
            }

            Vector3 midpoint =
                (targetA.transform.position +
                 targetB.transform.position) * 0.5f;

            // Explicit delegation to Arrive.
            return SB_Arrive.GetDesiredVelocity(
                me,
                midpoint,
                toleranceRadius,
                slowdownRadius
            );
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos ||
                targetA == null ||
                targetB == null)
            {
                return;
            }

            Vector3 positionA = targetA.transform.position;
            Vector3 positionB = targetB.transform.position;
            Vector3 midpoint = (positionA + positionB) * 0.5f;

            // Draw the segment joining A and B.
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(positionA, positionB);

            // Draw a cross at the midpoint.
            Gizmos.color = Color.red;

            float crossSize = 2f;

            Gizmos.DrawLine(
                midpoint - Vector3.right * crossSize,
                midpoint + Vector3.right * crossSize
            );

            Gizmos.DrawLine(
                midpoint - Vector3.up * crossSize,
                midpoint + Vector3.up * crossSize
            );
        }
    }
}