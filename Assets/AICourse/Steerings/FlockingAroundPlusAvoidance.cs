
using UnityEngine;

namespace Steerings
{
    public class FlockingAroundPlusAvoidance : FlockingAround
    {
        // public GameObject attractor;

        public override Vector3 GetLinearAcceleration()
        {
            return FlockingAroundPlusAvoidance.GetLinearAcceleration(Context, attractor);
        }

        public new static Vector3 GetLinearAcceleration(SteeringContext me, GameObject attractor)
        {
            Vector3 avoidAcc = ObstacleAvoidance.GetLinearAcceleration(me);

            if (avoidAcc.Equals(Vector3.zero))
                return FlockingAround.GetLinearAcceleration(me, attractor);
            else
                return avoidAcc;
        }
    }
}
