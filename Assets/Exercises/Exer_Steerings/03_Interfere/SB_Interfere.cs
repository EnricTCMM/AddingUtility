using AICourse.Utilities;
using UnityEngine;

namespace MotorBehaviours
{
    /// <summary>
    /// Places the agent in front of a target, on its current line
    /// of movement and at a specified distance.
    ///
    /// If the target is practically stopped, its orientation is used
    /// instead of its current velocity.
    /// </summary>
    public class SB_Interfere : LinearMotorBehaviour
    {
        [Header("Interference Target")]
        public GameObject target;

        [Min(0f)]
        [Tooltip(
            "Distance from the target at which the agent tries " +
            "to position itself."
        )]
        public float interferenceDistance = 5f;

        [Header("Arrive Settings")]
        [Tooltip(
            "Distance from the interference point at which " +
            "the agent stops."
        )]
        public float toleranceRadius = 0.5f;

        [Tooltip(
            "Distance from the interference point at which " +
            "the agent starts slowing down."
        )]
        public float slowdownRadius = 5f;

        [Header("Movement Detection")]
        [Min(0f)]
        [Tooltip(
            "The target is considered practically stopped when " +
            "its speed is below this value."
        )]
        public float minimumTargetSpeed = 0.05f;

        [Header("Debug Settings")]
        [Tooltip(
            "Draws the direction used and the calculated " +
            "interference point."
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

            // Fail fast: the target must expose its current velocity.
            MotorManager targetManager =
                target.GetComponent<MotorManager>();

            if (targetManager == null)
            {
                Debug.LogError(
                    $"[MotorBehaviours] Critical Error: Target " +
                    $"'{target.name}' lacks a MotorManager in " +
                    $"{GetType().Name} component of GameObject " +
                    $"'{gameObject.name}'."
                );

                Debug.Break();
                return null;
            }

            return SB_Interfere.GetDesiredVelocity(
                me,
                target,
                interferenceDistance,
                toleranceRadius,
                slowdownRadius,
                minimumTargetSpeed
            );
        }

        /// <summary>
        /// Calculates an interference point in front of the target
        /// and delegates movement to SB_Arrive.
        ///
        /// The target's velocity determines the direction while it is
        /// moving. If it is practically stopped, its orientation is used.
        /// </summary>
        public static Vector3? GetDesiredVelocity(
            MotorManager me,
            GameObject target,
            float interferenceDistance,
            float toleranceRadius,
            float slowdownRadius,
            float minimumTargetSpeed)
        {
            // Safe exit for external or dynamic calls.
            if (target == null)
            {
                Debug.LogWarning(
                    "[MotorBehaviours] Warning: Interfere invoked " +
                    "with a null target."
                );

                return null;
            }

            MotorManager targetManager =
                target.GetComponent<MotorManager>();

            if (targetManager == null)
            {
                Debug.LogWarning(
                    $"[MotorBehaviours] Warning: Target " +
                    $"'{target.name}' lacks a MotorManager in " +
                    $"{nameof(SB_Interfere)}."
                );

                return null;
            }

            Vector3 targetVelocity =
                targetManager.currentVelocity;

            Vector3 targetDirection;

            // Use the actual movement direction when the target
            // is moving at a significant speed.
            if (targetVelocity.sqrMagnitude >=
                minimumTargetSpeed * minimumTargetSpeed)
            {
                targetDirection = targetVelocity.normalized;
            }
            else
            {
                // If the target is practically stopped, use
                // the direction indicated by its orientation.
                targetDirection = Utils.OrientationToVector(
                    target.transform.eulerAngles.z
                );
            }

            Vector3 interferencePosition =
                target.transform.position +
                targetDirection * interferenceDistance;

            // Explicit delegation to Arrive.
            return SB_Arrive.GetDesiredVelocity(
                me,
                interferencePosition,
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

            MotorManager targetManager =
                target.GetComponent<MotorManager>();

            if (targetManager == null)
            {
                return;
            }

            Vector3 targetVelocity =
                targetManager.currentVelocity;

            Vector3 targetDirection;

            if (targetVelocity.sqrMagnitude >=
                minimumTargetSpeed * minimumTargetSpeed)
            {
                targetDirection = targetVelocity.normalized;
            }
            else
            {
                targetDirection = Utils.OrientationToVector(
                    target.transform.eulerAngles.z
                );
            }

            Vector3 targetPosition =
                target.transform.position;

            Vector3 interferencePosition =
                targetPosition +
                targetDirection * interferenceDistance;

            // Draw the direction used to calculate the
            // interference point.
            Gizmos.color = Color.blue;

            Gizmos.DrawLine(
                targetPosition,
                interferencePosition
            );

            // Draw a cross at the interference point.
            Gizmos.color = Color.red;

            float crossSize = 2f;

            Gizmos.DrawLine(
                interferencePosition - Vector3.right * crossSize,
                interferencePosition + Vector3.right * crossSize
            );

            Gizmos.DrawLine(
                interferencePosition - Vector3.up * crossSize,
                interferencePosition + Vector3.up * crossSize
            );
        }
    }
}