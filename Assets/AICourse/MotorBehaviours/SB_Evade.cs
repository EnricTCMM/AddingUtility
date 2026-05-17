using System;
using UnityEngine;

namespace MotorBehaviours
{
    public class SB_Evade : LinearMotorBehaviour
    {
        [Header("Evade Settings")]
        public GameObject target;
        public float maxPredictionTime = 3f;
        
        [Tooltip("If >0, the agent will not evade if the current target position is too far away")]
        public float maxEvadeDistance = 0f;
        
        [Tooltip("If true, it will log an error when no target is assigned.")]
        public bool signalErrorIfNoTarget = true;
        
        [Header("Debug")]
        public bool showGizmos = false;
        
        // --- State variables for Gizmo drawing ---
        private Vector3 futurePositionForGizmo;
        private bool shouldDrawGizmo = false;

        // the method that is called by the MotorManager
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Reset gizmo state every frame
            shouldDrawGizmo = false;

            // Fail Fast & Abstention: Mimicking SB_Flee logic
            if (target == null)
            {
                // no-target means no-menace, so the behaviour abstains and returns null
                if (!signalErrorIfNoTarget) 
                {
                    return null; 
                }
                else
                {
                    Debug.LogError($"[MotorBehaviours] Critical Error: No target in {GetType().Name} in GameObject '{gameObject.name}'.");
                    return null; 
                }
            }
            
            // Fail Fast: Check if target lacks a MotorManager
            MotorManager targetManager = target.GetComponent<MotorManager>();
            if (targetManager == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: target '{target.name}' lacks a MotorManager in {GetType().Name} component of GameObject '{gameObject.name}'.");
                Debug.Break();
                return Vector3.zero; 
            }

            // We use the out parameter to extract the calculated future position
            Vector3? desiredVelocity = SB_Evade.GetDesiredVelocity(me, target, maxPredictionTime, maxEvadeDistance, out futurePositionForGizmo);
            
            // If the calculation was successful, authorize the Gizmo drawing
            if (desiredVelocity != null)
            {
                shouldDrawGizmo = true;
            }

            return desiredVelocity;
        }

        // Intermediate method. Retrieves target's info
        public static Vector3? GetDesiredVelocity(MotorManager me, GameObject target, float maxPredictionTime, float maxFleeDistance, out Vector3 futurePosition)
        {
            futurePosition = Vector3.zero;

            // Safe exit to prevent NullReferenceException if called dynamically without a valid target
            if (target == null)
            {
                Debug.LogWarning($"[MotorBehaviours] Warning: evading from null target in {nameof(SB_Evade)}");
                return null; // Abstention
            }
            
            MotorManager targetManager = target.GetComponent<MotorManager>();
            if (targetManager == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Target '{target.name}' in Evade contains no MotorManager.");
                Debug.Break();
                return Vector3.zero;
            }

            // Target's velocity is provided by the target's MotorManager.
            Vector3 targetVelocity = targetManager.currentVelocity;
            
            return SB_Evade.GetDesiredVelocity(me, target.transform.position, targetVelocity, maxPredictionTime, maxFleeDistance, out futurePosition);
        }

        // Math is done in this method. Also available for external calls (delegation)
        public static Vector3? GetDesiredVelocity(MotorManager me, Vector3 targetPosition, Vector3 targetVelocity, float maxPredictionTime, float maxFleeDistance, out Vector3 futurePosition)
        {
            Vector3 directionToTarget = targetPosition - me.transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            // --- CLASSIC PRESENT PANIC ---
            // We evaluate the distance against the CURRENT position of the threat.
            // If the threat is currently outside our panic radius, we abstain.
            if (maxFleeDistance > 0 && distanceToTarget > maxFleeDistance)
            {
                futurePosition = Vector3.zero;
                return null; 
            }

            // PREDICT TIME TO TARGET
            float predictedTimeToTarget = 0f;
            if (me.maxSpeed > 0.001f)
            {
                predictedTimeToTarget = distanceToTarget / me.maxSpeed;
            }

            // Cap the prediction time so the agent doesn't predict too far into the future
            if (predictedTimeToTarget > maxPredictionTime)
            {
                predictedTimeToTarget = maxPredictionTime;
            }

            // CALCULATE FUTURE POSITION
            futurePosition = targetPosition + (targetVelocity * predictedTimeToTarget);
            
            // EDGE CASE PREVENTION: "Fleeing from myself"
            if ((futurePosition - me.transform.position).sqrMagnitude < 0.01f)
            {
                // We pass 0f as maxEvadeDistance because we already handled the abstention logic above
                return SB_Flee.GetDesiredVelocity(me, targetPosition, 0f);
            }

            // EXPLICIT DELEGATION TO FLEE
            // We pass 0f as maxEvadeDistance because we already handled the abstention logic above
            return SB_Flee.GetDesiredVelocity(me, futurePosition, 0f);
        }

        // --- Gizmo Drawing ---
        private void OnDrawGizmos()
        {
            // Only draw while playing, if the inspector checkbox is true, and we have a valid calculated position
            if (showGizmos && Application.isPlaying && shouldDrawGizmo)
            {
                Gizmos.color = Color.blue; 
                
                // Adjust this value to make the cross bigger or smaller
                float crossSize = 2f; 

                // Draw a 3D cross at the future position (X, Y, and Z axes)
                Gizmos.DrawLine(futurePositionForGizmo - Vector3.right * crossSize, futurePositionForGizmo + Vector3.right * crossSize);
                Gizmos.DrawLine(futurePositionForGizmo - Vector3.up * crossSize, futurePositionForGizmo + Vector3.up * crossSize);
                Gizmos.DrawLine(futurePositionForGizmo - Vector3.forward * crossSize, futurePositionForGizmo + Vector3.forward * crossSize);
            }
        }
    }
}