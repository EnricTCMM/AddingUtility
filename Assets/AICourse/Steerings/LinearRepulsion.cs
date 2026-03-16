using UnityEngine;

namespace Steerings
{
    public class LinearRepulsion : SteeringBehaviour
    {

        public override Vector3 GetLinearAcceleration()
        {
            return LinearRepulsion.GetLinearAcceleration(Context);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me)
        {
            Vector3 directionToTarget;
            float distanceToTarget;
            float repulsionStrength;
            Vector3 totalAcceleration = Vector3.zero;

            // get all potential "targets" (all the repulsive targets) 
            GameObject[] targets = GameObject.FindGameObjectsWithTag(me.idTag);
            //ICollection<GameObject> targets = me.groupContext.members;

            foreach (GameObject target in targets)
            {
                // do not take yourself into account
                if (target == me.gameObject)
                    continue;

                // disregard distant targets
                if ((target.transform.position - me.transform.position).magnitude > me.repulsionThreshold) continue;

                // Experimental...
                // disregad targets outside cone of vision if necessary
                if (me.applyVision)
                    if (!Utils.InCone(me.gameObject, target, me.coneOfVisionAngle)) continue;

                directionToTarget = target.transform.position - me.transform.position;
                distanceToTarget = directionToTarget.magnitude;

                // "repulsive" object close. Compute acceleration to avoid it and accumulate
                repulsionStrength = me.maxAcceleration * (me.repulsionThreshold - distanceToTarget) / me.repulsionThreshold;
                totalAcceleration -= directionToTarget.normalized * repulsionStrength;
                
            } // end of iteration over all repulsive targets 

            // clip if necessary
            if (totalAcceleration.magnitude > me.maxAcceleration)
                totalAcceleration = totalAcceleration.normalized * me.maxAcceleration;

            return totalAcceleration;
        }
    }
}