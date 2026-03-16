
using UnityEngine;

namespace Steerings {

    public class GoWhereYouLook : SteeringBehaviour
    {
        public override Vector3 GetLinearAcceleration()
        {
            return GoWhereYouLook.GetLinearAcceleration(Context);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me)
        {
            Vector3 myDirection = Utils.OrientationToVector(me.transform.eulerAngles.z);
            Vector3 inFrontOfme = me.transform.position + myDirection;
            
            SURROGATE_TARGET.transform.position = inFrontOfme;
            return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
        }
    }
}
