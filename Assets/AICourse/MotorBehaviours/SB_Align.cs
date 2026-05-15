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
        
        
        public override float? GetDesiredAngularSpeed(MotorManager me)
        {
            // If there's no target FAIL-FAST
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical error: No target in {GetType().Name} in GameObject '{gameObject.name}'.");
                Debug.Break(); // immediate pause. 
                return 0;
            }

            // Delegation: We extract the Z angle of the target and pass it to the math function
            return SB_Align.GetDesiredAngularSpeed(me, target.transform.eulerAngles.z, toleranceRadius, slowdownRadius);
        }

        // 2. Static method (Pure math, open for delegation from other behaviors like LWYG)
        public static float GetDesiredAngularSpeed(MotorManager me, float targetRotation, float tolerance, float slowdown)
        {
            float currentRotation = me.transform.eulerAngles.z;
            
            // Calculates the shortest distance between the two angles (-180 to 180)
            float rotationDifference = Mathf.DeltaAngle(currentRotation, targetRotation);
            float rotationSize = Mathf.Abs(rotationDifference);

            // A. Within tolerance: we want to stop rotating
            if (rotationSize < tolerance)
            {
                return 0f; 
            }

            // B. Calculate ideal speed based on slowdown radius
            float targetSpeed;
            if (rotationSize > slowdown)
            {
                targetSpeed = me.maxAngularSpeed;
            }
            else
            {
                targetSpeed = me.maxAngularSpeed * (rotationSize / slowdown);
            }

            // C. Apply the correct sign (direction of rotation)
            targetSpeed *= Mathf.Sign(rotationDifference);
            
            return targetSpeed;
        }
    
    }
}