
using UnityEngine;

namespace Steerings
{
    public class Seek : SteeringBehaviour
    {
        public GameObject target;

        public override GameObject GetTarget()
        {
            return target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return Seek.GetLinearAcceleration(Context, target);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, 
                                                    GameObject target)
        {
            Vector3 directionToTarget;
            Vector3 acceleration;

            // Compute direction to target
            directionToTarget = target.transform.position - me.transform.position;
            directionToTarget.Normalize();

            // give max acceleration towards the target
            acceleration = directionToTarget * me.maxAcceleration;

            return acceleration;
        }
    }
}
