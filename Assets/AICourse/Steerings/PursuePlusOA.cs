
using UnityEngine;

namespace Steerings
{
    public class PursuePlusOA : SteeringBehaviour
    {
        public GameObject target;

        public override GameObject GetTarget()
        {
            return target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return PursuePlusOA.GetLinearAcceleration(Context, target);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target)
        {
            // give priority to obstacle avoidance
            Vector3 avoidanceAcceleration = ObstacleAvoidance.GetLinearAcceleration(me);
            if (avoidanceAcceleration.Equals(Vector3.zero))
                return Pursue.GetLinearAcceleration(me, target);
            else
                return avoidanceAcceleration;
        }
    }
}
