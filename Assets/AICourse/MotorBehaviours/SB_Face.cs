using Steerings;
using UnityEngine;

namespace MotorBehaviours
{
    public class SB_Face : SB_Align
    {
        
        protected override float GetDesiredAngle(MotorManager me)
        {
            // this is the equivalent of calculating a surrogate target for Align
            // GetDesiredAngularSpeed will use this angle instead of target.transform.eulerAngles.z
            Vector3 directionToTarget = target.transform.position - me.transform.position;
            return Utils.VectorToOrientation(directionToTarget);
        }
        
    }
}