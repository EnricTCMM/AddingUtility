
using UnityEngine;


namespace Steerings
{
    public class Wander : SteeringBehaviour
    {
        public override Vector3 GetLinearAcceleration()
        {
            return Wander.GetLinearAcceleration(Context);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me)
        {
            // change target orientation (change location of surrogate target on unit circle)
            me.wanderTargetOrientation += me.wanderRate * Utils.binomial();

            // place surrogate target on circle of wanderRadius
            SURROGATE_TARGET.transform.position = Utils.OrientationToVector(me.wanderTargetOrientation) * me.wanderRadius;

            // place circle  "in front"
            // in front of me or in front of my velocity?
            // in fron of my velocity, definitely. Othewise, behaviour with policies different from lwyg is questionable
            if (me.velocity.magnitude>0.01f)
                SURROGATE_TARGET.transform.position +=
                    //me.transform.position + Utils.OrientationToVector(me.transform.eulerAngles.z) * me.wanderOffset;
                    me.transform.position + me.velocity.normalized * me.wanderOffset;
            else 
               SURROGATE_TARGET.transform.position += me.transform.position+ Utils.OrientationToVector(me.transform.eulerAngles.z) * me.wanderOffset;

            // show some gizmos before returning
            if (me.showWanderGizmos)
            {
                Debug.DrawLine(me.transform.position,
                           //me.transform.position + Utils.OrientationToVector(me.transform.eulerAngles.z) * me.wanderOffset,
                           me.transform.position + me.velocity.normalized * me.wanderOffset,
                           Color.black); 

                DebugExtension.DebugCircle(me.transform.position + 
                                                                 //Utils.OrientationToVector(me.transform.eulerAngles.z) * me.wanderOffset,
                                                                 me.velocity.normalized * me.wanderOffset,
                                           new Vector3(0, 0, 1),
                                           Color.red,
                                           me.wanderRadius);
                DebugExtension.DebugPoint(SURROGATE_TARGET.transform.position,
                                      Color.black,
                                      5f);
            }


            // Seek the surrogate target
            return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
        }
    }
}
