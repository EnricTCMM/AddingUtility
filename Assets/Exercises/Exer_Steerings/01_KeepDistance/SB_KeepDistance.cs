using AICourse.Utilities;
using UnityEngine;

namespace MotorBehaviours
{
    /// <summary>
    /// Makes the agent maintain a specified distance from a target.
    ///
    /// The desired position lies on the line joining the target
    /// and the agent, at the requested distance from the target.
    /// </summary>
    public class SB_KeepDistance : LinearMotorBehaviour
    {
        [Header("Target Settings")]
        public GameObject target;

        [Min(0f)]
        [Tooltip(
            "Distance that the agent tries to maintain " +
            "from the target."
        )]
        public float desiredDistance = 5f;

        [Header("Arrive Settings")]
        [Min(0f)]
        [Tooltip(
            "Distance from the desired position at which " +
            "the agent stops."
        )]
        public float toleranceRadius = 0.5f;

        [Min(0f)]
        [Tooltip(
            "Distance from the desired position at which " +
            "the agent starts slowing down."
        )]
        public float slowdownRadius = 5f;

        [Header("Debug Settings")]
        [Tooltip(
            "Draws the desired distance and the calculated " +
            "position around the target."
        )]
        public bool showGizmos = true;

        // The method called by MotorManager.
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Fail fast: a target is required.
            if (target == null)
            {
                Debug.LogError(
                    $"[MotorBehaviours] Critical Error: Missing target " +
                    $"on {GetType().Name} component of GameObject " +
                    $"'{gameObject.name}'."
                );

                Debug.Break();
                return null;
            }

            return SB_KeepDistance.GetDesiredVelocity(
                me,
                target,
                desiredDistance,
                toleranceRadius,
                slowdownRadius
            );
        }

        /// <summary>
        /// Calculates a position at the requested distance from the
        /// target, along the line joining the target and the agent,
        /// and delegates movement to SB_Arrive.
        /// </summary>
        public static Vector3? GetDesiredVelocity(
            MotorManager me,
            GameObject target,
            float desiredDistance,
            float toleranceRadius,
            float slowdownRadius)
        {
            // Safe exit for external or dynamic calls.
            if (target == null)
            {
                Debug.LogWarning(
                    "[MotorBehaviours] Warning: KeepDistance invoked " +
                    "with a null target."
                );

                return null;
            }

            Vector3 targetPosition =
                target.transform.position;

            Vector3 directionFromTarget =
                me.transform.position - targetPosition;

            // If the agent and the target occupy practically
            // the same position, there is no radial direction.
            if (directionFromTarget.sqrMagnitude < 0.0001f)
            {
                // Use the direction indicated by the target's
                // current orientation as a stable fallback.
                directionFromTarget =
                    Utils.OrientationToVector(
                        target.transform.eulerAngles.z
                    );
            }
            else
            {
                directionFromTarget.Normalize();
            }

            Vector3 desiredPosition =
                targetPosition +
                directionFromTarget * desiredDistance;

            // Explicit delegation to Arrive.
            return SB_Arrive.GetDesiredVelocity(
                me,
                desiredPosition,
                toleranceRadius,
                slowdownRadius
            );
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos || target == null)
            {
                return;
            }

            Vector3 targetPosition =
                target.transform.position;

            Vector3 directionFromTarget =
                transform.position - targetPosition;

            if (directionFromTarget.sqrMagnitude < 0.0001f)
            {
                directionFromTarget =
                    Utils.OrientationToVector(
                        target.transform.eulerAngles.z
                    );
            }
            else
            {
                directionFromTarget.Normalize();
            }

            Vector3 desiredPosition =
                targetPosition +
                directionFromTarget * desiredDistance;

            // Draw the desired distance around the target.
            Gizmos.color = Color.cyan;
            DrawCircle(
                targetPosition,
                desiredDistance,
                48
            );

            // Draw the radial line from the target
            // to the desired position.
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                targetPosition,
                desiredPosition
            );

            // Draw a cross at the desired position.
            Gizmos.color = Color.red;

            float crossSize = 2f;

            Gizmos.DrawLine(
                desiredPosition - Vector3.right * crossSize,
                desiredPosition + Vector3.right * crossSize
            );

            Gizmos.DrawLine(
                desiredPosition - Vector3.up * crossSize,
                desiredPosition + Vector3.up * crossSize
            );
        }

        /// <summary>
        /// Draws a circle on the XY plane for debugging purposes.
        /// </summary>
        private static void DrawCircle(
            Vector3 center,
            float radius,
            int segments)
        {
            if (radius <= 0f || segments < 3)
            {
                return;
            }

            float angleStep = 360f / segments;

            Vector3 previousPoint =
                center + Vector3.right * radius;

            for (int i = 1; i <= segments; i++)
            {
                float angle =
                    i * angleStep * Mathf.Deg2Rad;

                Vector3 nextPoint =
                    center +
                    new Vector3(
                        Mathf.Cos(angle),
                        Mathf.Sin(angle),
                        0f
                    ) * radius;

                Gizmos.DrawLine(
                    previousPoint,
                    nextPoint
                );

                previousPoint = nextPoint;
            }
        }
    }
}