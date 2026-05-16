using UnityEngine;

namespace MotorBehaviours
{
    public class SB_Pursue : LinearMotorBehaviour
    {
        [Header("Pursue Settings")]
        public GameObject target;
        public float maxPredictionTime = 3f;

        // the method that is called by the MotorManager
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Fail Fast: Check if target is missing
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Missing Target on {GetType().Name} component of GameObject '{gameObject.name}'.");
                Debug.Break();
                return Vector3.zero;
            }
            // Fail Fast: Check if target lacks a  MotorManager
            MotorManager targetManager = target.GetComponent<MotorManager>();
            if (targetManager == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: target '{target.name}' lacks a MotorManager in {GetType().Name} component of GameObject '{gameObject.name}'.");
                Debug.Break();
                return Vector3.zero;
            }

            return SB_Pursue.GetDesiredVelocity(me, target, maxPredictionTime);
        }

        // Intermediate method. Retrives target's info
        public static Vector3? GetDesiredVelocity(MotorManager me, GameObject target, float maxPredictionTime)
        {
            // Fail Fast: Check if target is missing
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Missing Target in Pursue");
                Debug.Break();
                return Vector3.zero;
            }
            // Fail Fast: Check if target lacks a  MotorManager
            MotorManager targetManager = target.GetComponent<MotorManager>();
            if (targetManager == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical Error: Target '{target.name}' in Pursue contains no MotorManager.");
                Debug.Break();
                return Vector3.zero;
            }

            // target's velocity is provided by the target's MotorManager.
            Vector3 targetVelocity = targetManager.currentVelocity;
            
            return SB_Pursue.GetDesiredVelocity(me, target.transform.position, targetVelocity, maxPredictionTime);
        }

        // Math is done in this method. Also available for external calls (delegation)
        public static Vector3? GetDesiredVelocity(MotorManager me, Vector3 targetPosition, Vector3 targetVelocity, float maxPredictionTime)
        {
            Vector3 directionToTarget = targetPosition - me.transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            // PREDICT TIME TO TARGET
            // We use me.maxSpeed instead of current velocity to avoid division by zero
            // and to ensure a stable prediction even if the agent is currently stopped.
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
            Vector3 futurePositionOfTarget = targetPosition + (targetVelocity * predictedTimeToTarget);
            
            // EXPLICIT DELEGATION TO SEEK
            // We mathematically delegate the actual steering calculation to SB_Seek
            return SB_Seek.GetDesiredVelocity(me, futurePositionOfTarget);
        }
    }
}