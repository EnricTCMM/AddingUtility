using UnityEngine;

namespace Steerings
{

    public class SoldierControl : SteeringBehaviour
    {
        public GameObject target;
        public float dist;
        public float degs;

        public override GameObject GetTarget()
        {
            return target;
        }
        
        public override Vector3 GetLinearAcceleration()
        {
            return SoldierControl.GetLinearAcceleration(Context, target, dist, degs);
        }

        
        public static Vector3 GetLinearAcceleration (SteeringContext me, GameObject target, 
                                                     float d, float deg)
        {
            SURROGATE_TARGET.transform.position = target.transform.position + Utils.OrientationToVector(target.transform.rotation.eulerAngles.z + deg).normalized * d;


            Debug.DrawLine(me.transform.position, me.transform.position + me.velocity.normalized * 20, Color.red);
            Debug.DrawLine(me.transform.position, target.transform.position, Color.blue);


            //return Seek.GetSteering(me, SURROGATE_TARGET);
            return Arrive.GetLinearAccelerationForPathfinding(me, SURROGATE_TARGET, 0, 5);
        }


        // LEGACY
        public void None()
        {
            this.rotationalPolicy = RotationalPolicy.NONE;
        }

        public void LWYGI()
        {
            this.rotationalPolicy = RotationalPolicy.LWYGI;
        }

        public void LWYG()
        {
            this.rotationalPolicy = RotationalPolicy.LWYG;
        }

        public void FTI()
        {
            this.rotationalPolicy = RotationalPolicy.FTI;
        }

        public void FT()
        {
            this.rotationalPolicy = RotationalPolicy.FT;
        }

    }
}