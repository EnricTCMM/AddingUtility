using UnityEngine;
namespace MotorBehaviours
{
    [RequireComponent(typeof(MotorManager))]
    public class SB_Align : AngularMotorBehaviour
    {
        [Header("Align/Face Settings")]
        public GameObject target;
        
        // The angle threshold (tolerance) to completely stop (in degrees)
        public float toleranceRadius = 2f; 
        
        // The angle threshold to start slowing down (in degrees)
        public float slowdownRadius = 30f; 
        
        // How fast we want to reach the target (desired) speed
        public float timeToDesiredSpeed = 0.1f;

        protected virtual float GetDesiredAngle(MotorManager me)
        {
            // parameter not used here. But used in subclasse Face. 
            return target.transform.eulerAngles.z;
        }
        
        public override float GetTorque(MotorManager me)
        {
            if (target == null) return 0f;

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
                // return -me.currentAngularVelocity / timeToDesiredSpeed;
                return 0f; 
            }

            // 4. Calculate desired speed based on distance
            float targetSpeed;
            if (rotationSize > slowdownRadius)
            {
                targetSpeed = me.maxAngularSpeed;
            }
            else
            {
                // Linear falloff inside the slow radius
                targetSpeed = me.maxAngularSpeed * (rotationSize / slowdownRadius);
            }

            // Combine speed and direction (sign)
            targetSpeed *= Mathf.Sign(rotationDifference);

            // 5. Calculate Torque needed to reach that target speed
            float torque = (targetSpeed - me.currentAngularVelocity)/timeToDesiredSpeed;

            return torque;
        }
    }
}