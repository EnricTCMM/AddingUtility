using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steerings
{
    public class Arrive : SteeringBehaviour
    {

        public GameObject target;

        public override GameObject GetTarget()
        {
            return target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return Arrive.GetLinearAcceleration(Context, target);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target)
        {
            Vector3 directionToTarget = target.transform.position - me.transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget < me.closeEnoughRadius) return Vector3.zero;

            if (distanceToTarget > me.slowdownRadius) return Seek.GetLinearAcceleration(me, target);

            float desiredSpeed = me.maxSpeed * (distanceToTarget / me.slowdownRadius);
            Vector3 desiredVelocity = directionToTarget.normalized * desiredSpeed;
            Vector3 requiredAcceleration = (desiredVelocity - me.velocity) / me.timeToDesiredSpeed;

            if (requiredAcceleration.magnitude > me.maxAcceleration)
                requiredAcceleration = requiredAcceleration.normalized * me.maxAcceleration;

            return requiredAcceleration;
        }


        // the following method exists for retrocompatibility with the pathfollowing steering.
        // only PathFollowing should invoke it.
        // It gets the radiuses from its parameters instead of getting them from the context
        public static Vector3 GetLinearAccelerationForPathfinding( SteeringContext me, GameObject target,
                                                                  float closeEnoughRadius, float slowdownRadius)
        {
            Vector3 directionToTarget = target.transform.position - me.transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget < closeEnoughRadius) return Vector3.zero;

            if (distanceToTarget > slowdownRadius) return Seek.GetLinearAcceleration(me, target);

            float desiredSpeed = me.maxSpeed * (distanceToTarget / slowdownRadius);
            Vector3 desiredVelocity = directionToTarget.normalized * desiredSpeed;
            Vector3 requiredAcceleration = (desiredVelocity - me.velocity) / me.timeToDesiredSpeed;

            if (requiredAcceleration.magnitude > me.maxAcceleration)
                requiredAcceleration = requiredAcceleration.normalized * me.maxAcceleration;

            return requiredAcceleration;
        }

    }
}
