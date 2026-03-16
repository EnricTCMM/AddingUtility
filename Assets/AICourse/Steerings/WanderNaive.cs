
using UnityEngine;

namespace Steerings
{
    public class WanderNaive : SteeringBehaviour
    {
        public override Vector3 GetLinearAcceleration()
        {
            return WanderNaive.GetLinearAcceleration(Context);
        }

        public static Vector3 GetLinearAcceleration (SteeringContext me)
        {
            // slightly change the orientation
            float orientation = me.transform.rotation.eulerAngles.z;
            orientation += Utils.binomial() * me.wanderRate;
            me.transform.rotation = Quaternion.Euler(0, 0, orientation);

            // and go where you look
            return GoWhereYouLook.GetLinearAcceleration(me);
        }
    }
}
