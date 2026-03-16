using UnityEngine;

namespace Steerings
{

    public class KeepPosition : SteeringBehaviour
    {

        public GameObject target;
        public float requiredDistance;
        public float requiredAngle;

        /* COMPLETE */ 

        public override Vector3 GetLinearAcceleration()
        {
            /* COMPLETE */
            return Vector3.zero; // remove this line when exercise completed
        }

        
        public static Vector3 GetLinearAcceleration (SteeringContext me, GameObject target,
                                                     float distance, float angle)
        {
            /* COMPLETE */
            return Vector3.zero; // remove this line when exercise completed
        }

    }
}