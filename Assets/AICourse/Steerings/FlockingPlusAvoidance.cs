
using UnityEngine;

namespace Steerings
{
    public class FlockingPlusAvoidance : SteeringBehaviour
    {
        public override Vector3 GetLinearAcceleration()
        {
            return FlockingPlusAvoidance.GetLinearAcceleration(Context);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me)
        {
            Vector3 avoidAcc = ObstacleAvoidance.GetLinearAcceleration(me);

            if (avoidAcc.Equals(Vector3.zero))
                return Flocking.GetLinearAcceleration(me);
            else
                return avoidAcc;
        }
    }
}
