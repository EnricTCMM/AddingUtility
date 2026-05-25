using AICourse.Utilities;
using UnityEngine;
using Steerings; // Required to access Utils.OrientationToVector

namespace MotorBehaviours
{
    public class SB_GoWhereYouLook : LinearMotorBehaviour
    {
        // The method that is called by the MotorManager
        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            return SB_GoWhereYouLook.CalculateDesiredVelocity(me);
        }

        // Math is done in this method. Also available for external calls (delegation)
        // NOTICE NOTICE NOTICE: this method cannot be called GetDesiredVelocity since its
        // signature clashes with that of the previous method. 
        // Take this into consideration in the rare occasions where you need to use it for delegation. 
        public static Vector3 CalculateDesiredVelocity(MotorManager me)
        {
            // 1. Obtain the current direction the agent is facing from its orientation
            Vector3 myDirection = Utils.OrientationToVector(me.transform.eulerAngles.z);
            
            // 2. Calculate a mathematical point just in front of the agent
            Vector3 inFrontOfMe = me.transform.position + myDirection;
            
            // 3. EXPLICIT DELEGATION TO SEEK
            // Instead of moving a SURROGATE_TARGET, we pass the coordinates directly to Seek
            return SB_Seek.GetDesiredVelocity(me, inFrontOfMe);
        }
    }
}