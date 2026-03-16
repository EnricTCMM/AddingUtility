
using UnityEngine;

namespace Steerings
{
    public class WanderAroundPlusAvoid : SteeringBehaviour
    {

        public GameObject attractor;

        public override Vector3 GetLinearAcceleration()
        {
            return WanderAroundPlusAvoid.GetLinearAcceleration(Context, attractor);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject attractor)
        {
            Vector3 avoidAcc = ObstacleAvoidance.GetLinearAcceleration(me);

            if (avoidAcc.Equals(Vector3.zero))
                return WanderAround.GetLinearAcceleration(me, attractor);
            else
                return avoidAcc;
        }

    }
}