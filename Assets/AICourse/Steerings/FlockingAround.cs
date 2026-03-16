using UnityEngine;

namespace Steerings
{
    public class FlockingAround : SteeringBehaviour
    {

        public GameObject attractor;

        public override Vector3 GetLinearAcceleration()
        {
            return FlockingAround.GetLinearAcceleration(Context, attractor);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject attractor)
        {
            Vector3 seekAcc = Seek.GetLinearAcceleration(me, attractor);
            Vector3 flockingAcc = Flocking.GetLinearAcceleration(me);

            return seekAcc * me.seekWeight + flockingAcc * (1 - me.seekWeight);
        }
    }
}
