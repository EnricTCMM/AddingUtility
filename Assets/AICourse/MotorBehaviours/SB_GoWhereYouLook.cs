using AICourse.Utilities;
using UnityEngine;

namespace MotorBehaviours
{
    public class SB_GoWhereYouLook : LinearMotorBehaviour
    {
        // The method that is called by the MotorManager
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            return SB_GoWhereYouLook.GetDesiredVelocity(me, 0); 
        }

        // Math is done in this method. Also available for external calls (delegation)
        // NOTICE NOTICE NOTICE: second parameter is mostly a dummy parameter. It serves a single purpose:
        // - avoids name clashing with the instance parameter
        // Take this into consideration in the rare occasions where you need to use it for delegation. 
        // (as in SB_WanderNaive)
        public static Vector3 GetDesiredVelocity(MotorManager me, float targetSpeed)
        {
            // 1. Obtain the current direction the agent is facing from its orientation
            Vector3 myDirection = Utils.OrientationToVector(me.transform.eulerAngles.z);
            
            // 2. Calculate a mathematical point just in front of the agent
            Vector3 inFrontOfMe = me.transform.position + myDirection;
            
            // 3. EXPLICIT DELEGATION TO SEEK
            // Instead of moving a SURROGATE_TARGET, we pass the coordinates directly to Seek
            Vector3 seekVelocity =  SB_Seek.GetDesiredVelocity(me, inFrontOfMe);
            
            // adjust to target speed
            if (targetSpeed==0) return seekVelocity;
            else return seekVelocity.normalized * targetSpeed;
        }
    }
}