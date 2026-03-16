using System.Collections.Generic;
using UnityEngine;

namespace Steerings
{
    public class Cohesion : SteeringBehaviour
    {
        public override Vector3 GetLinearAcceleration()
        {
            return Cohesion.GetLinearAcceleration(Context);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me)
        {

            Vector3 centreOfMasses = Vector3.zero;
            int mates = 0;

            // get all potential "targets" (all the cohesion targets) 
            // cache to improve efficiency...
            GameObject[] targets = GameObject.FindGameObjectsWithTag(me.idTag);
         
            // ICollection<GameObject> targets = me.groupContext.members;

            foreach (GameObject target in targets)
            {
                // do not take yourself into account
                if (target == me.gameObject) continue;

                // disregard distant targets
                if ((target.transform.position - me.transform.position).magnitude > me.cohesionThreshold) continue;

                // disregad targets outside cone of vision
                if (me.applyVision)
                    if (!Utils.InCone(me.gameObject, target, me.coneOfVisionAngle)) continue;

                centreOfMasses += target.transform.position;
                mates++;
            }

            if (mates == 0) return Vector3.zero;
            else
            {
                centreOfMasses /= mates;
                SURROGATE_TARGET.transform.position = centreOfMasses;
                return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
            }
        }
    }
}
