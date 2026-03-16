
using UnityEngine;

namespace Steerings {

    public class Flee : SteeringBehaviour
    {
        public GameObject target;

        public override GameObject GetTarget()
        {
            return target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return Flee.GetLinearAcceleration(Context, target);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target)
        {
            return -Seek.GetLinearAcceleration(me, target);
        }
    } 
}

