using UnityEngine;

namespace Steerings
{
    public class Face : SteeringBehaviour
    {

        public GameObject target;

        // there's no getter for target since rotational behaviours do not 
        // apply facing policies (facing policies make no sense for rotational
        // behaviours)

        public override float GetAngularAcceleration()
        {
            return Face.GetAngularAcceleration(Context, target);
        }

        public static float GetAngularAcceleration(SteeringContext me, GameObject target)
        {
            Vector3 directionToTarget = target.transform.position - me.transform.position;
            SURROGATE_TARGET.transform.rotation = Quaternion.Euler(0, 0, 
                                                    Utils.VectorToOrientation(directionToTarget));
            
            return Align.GetAngularAcceleration(me, SURROGATE_TARGET);
        }
    }
}