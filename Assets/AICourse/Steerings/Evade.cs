
using UnityEngine;

namespace Steerings
{
    public class Evade : SteeringBehaviour
    {

        public GameObject target;

        public override GameObject GetTarget()
        {
            return target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return Evade.GetLinearAcceleration(Context, target);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target)
        {
            // velocity of target required. Let's get if from its Steering context
            SteeringContext targetContext = target.GetComponent<SteeringContext>();
            if (targetContext == null)
            {
                Debug.LogWarning("Evade invoked with a target that has no context attached. Resorting to Flee");
                return Flee.GetLinearAcceleration(me, target);
            }

            Vector3 directionFromTarget = me.transform.position - target.transform.position;
            float distanceToMe = directionFromTarget.magnitude;
            float currentSpeed = targetContext.velocity.magnitude;

            // determine the time it will take the target to reach me
            float predictedTimeToMe = distanceToMe / currentSpeed;
            if (predictedTimeToMe > me.maxPredictionTime)
                predictedTimeToMe = me.maxPredictionTime;

            // now determine future (at predicted time) location of target
            Vector3 futurePositionOfTarget = target.transform.position + targetContext.velocity * predictedTimeToMe;

            if (me.showFutureTargetGizmos)
                DebugExtension.DebugPoint(futurePositionOfTarget, Color.red, 2f);


            // There's a problem when future target position is me since this implies "fleeing from myself" In
            // this case just "forget about the future" and flee the target itself
            if ((futurePositionOfTarget - me.transform.position).magnitude < 0.1f) 
                return Flee.GetLinearAcceleration(me, target);
        
          

            // create surrogate target and place it at future location
            SURROGATE_TARGET.transform.position = futurePositionOfTarget;
            // Delegate to flee 
            return Flee.GetLinearAcceleration(me, SURROGATE_TARGET);

        }
    }
}