using UnityEngine;

namespace Steerings
{

    public class LeaderFollowingBlended : SteeringBehaviour
    {
        
        public GameObject target;
        public float requiredDistance;
        public float requiredAngle;

        public float wlr = 0.5f;

        public override GameObject GetTarget()
        {
            return target;
        }
      
        
        public override Vector3 GetLinearAcceleration()
        {
            /* COMPLETE */
            return Vector3.zero; // delete this line when exercise is complete
        }

        
        public static Vector3 GetLinearAcceleration (SteeringContext me /* COMPLETE */)
        {
            /*
             Compute both steerings
                lr = LinearRepulsion.GetLinearAcceleration(...)
                kp = KeepPosition...
             - if lr is zero return kp
             - else return the blending of lr and kp
             */

            /* COMPLETE */
            return Vector3.zero; // delete this line when exercise is complete
        }
    }
}