using UnityEngine;

namespace Steerings
{
    public class WanderAround : SteeringBehaviour
    {
        public GameObject attractor;

        public override Vector3 GetLinearAcceleration()
        {
            return WanderAround.GetLinearAcceleration(Context, attractor);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject attractor)
        {
            Vector3 seekAcc = Seek.GetLinearAcceleration(me, attractor);
            Vector3 wanderAcc = Wander.GetLinearAcceleration(me);

            return seekAcc*me.seekWeight + wanderAcc*(1-me.seekWeight);
        }
    }
}
