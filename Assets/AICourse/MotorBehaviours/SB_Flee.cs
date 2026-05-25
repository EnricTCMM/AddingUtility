using AICourse.Utilities;
using UnityEngine;
using Steerings; // Necessari per accedir a Utils.OrientationToVector

namespace MotorBehaviours
{
    public class SB_Flee : LinearMotorBehaviour
    {
        [Header("Flee Settings")]
        public GameObject target;
        [Tooltip("If >0, the agent will not flee if it is too far away from the target")]
        public float maxFleeDistance = 0f;
        [Tooltip("If true, it will log an error when no target is assigned.")]
        public bool signalErrorIfNoTarget = true;

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            if (target == null)
            {
                // no-target means no-menace, so the behaviour "has nothing to say" and returns null
                // which is different from returning Vector3.zero, which would mean "Stop".
                if (!signalErrorIfNoTarget) 
                {
                    return null;
                }
                else
                {
                    Debug.LogError($"[MotorBehaviours] Critical Error: No target in {nameof(SB_Flee)} in GameObject '{gameObject.name}'.");
                    return null;
                }
            }
            return SB_Flee.GetDesiredVelocity(me, target, maxFleeDistance);
        }

        public static Vector3? GetDesiredVelocity(MotorManager me, GameObject target, float maxFleeDistance)
        {
            // Safe exit to prevent NullReferenceException if called dynamically without a valid target
            if (target == null)
            {
                Debug.LogWarning($"[MotorBehaviours] Warning: fleeing from null target in {nameof(SB_Flee)}");
                return null; // abstention
            }
            return GetDesiredVelocity(me, target.transform.position, maxFleeDistance);
        }

        public static Vector3? GetDesiredVelocity(MotorManager me, Vector3 targetPosition, float maxFleeDistance)
        {
            float distanceFromTarget = (me.transform.position - targetPosition).magnitude;

            //We abstain if the target is too far away
            if (maxFleeDistance > 0 && distanceFromTarget > maxFleeDistance)
            {
                return null; // we are too far away
            }
            
            // Protection against exact overlap
            // We do this BEFORE delegating to Seek to prevent Seek from dealing with zero vectors
            if (distanceFromTarget < 0.001f)
            {
                // We pick a random angle in degrees and use Utils to get the vector
                float randomAngleDeg = Random.Range(0f, 360f);
                Vector3 randomDir = Utils.OrientationToVector(randomAngleDeg);
                return randomDir * me.maxSpeed;
            }

            // Flee is mathematically the exact opposite of Seek
            return -SB_Seek.GetDesiredVelocity(me, targetPosition);
        }
    }
}