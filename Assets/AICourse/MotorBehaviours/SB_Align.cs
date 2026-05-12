using UnityEngine;
namespace MotorBehaviours
{
    public class SB_Align : AngularMotorBehaviour
    {
        [Header("Align/Face Settings")]
        public GameObject target;
        
        // The angle threshold (tolerance) to completely stop (in degrees)
        public float toleranceRadius = 2f; 
        
        // The angle threshold to start slowing down (in degrees)
        public float slowdownRadius = 30f; 
        

        protected virtual float GetDesiredAngle(MotorManager me)
        {
            // parameter not used here. But used in subclasse Face. 
            return target.transform.eulerAngles.z;
        }
        
        public override float GetDesiredAngularSpeed(MotorManager me)
        {
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical error: No target in {GetType().Name} in GameObject '{gameObject.name}'.");
                Debug.Break(); // immediate pause. 
                return 0f;
            }

            // 1. Get current and target rotations (assuming 2D Z-axis rotation)
            float currentRotation = me.transform.eulerAngles.z;
            float targetRotation = GetDesiredAngle(me);

            // 2. Get the shortest distance to the target angle (-180 to 180)
            float rotationDifference = Mathf.DeltaAngle(currentRotation, targetRotation);
            float rotationSize = Mathf.Abs(rotationDifference);

            // 3. Check if we have arrived
            if (rotationSize < toleranceRadius)
            {
                // [Gemini proposal] Optional: You could return a torque that explicitly brakes the agent here
                // return -me.currentAngularSpeed / timeToDesiredSpeed;
                
                me.StopRotationsCompletely(); // [Gemini Pun] PULL THE ROTATIONAL PARACHUTE!
                return 0f; 
            }

            // 4. Calculate desired speed based on distance (angular distance, that is)
            float desiredSpeed;
            if (rotationSize > slowdownRadius)
            {
                desiredSpeed = me.maxAngularSpeed;
            }
            else
            {
                // Linear falloff inside the slow radius
                desiredSpeed = me.maxAngularSpeed * (rotationSize / slowdownRadius);
            }

            // Combine speed and direction (sign)
            desiredSpeed *= Mathf.Sign(rotationDifference);

            return desiredSpeed;
        }
    }
}