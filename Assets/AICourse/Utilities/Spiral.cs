using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steerings { 
public class Spiral : SteeringBehaviour
    {
        public override float GetAngularAcceleration()
        {
            return Context.maxAcceleration;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return GoWhereYouLook.GetLinearAcceleration(Context);
        }
    }
}
