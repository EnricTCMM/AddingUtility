using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steerings
{
    public class Align : SteeringBehaviour
    {
        public GameObject target;

		// there's no getter for target since rotational behaviours do not 
		// apply facing policies (facing policies make no sense for rotational
		// behaviours)

		public override float GetAngularAcceleration()
        {
            return Align.GetAngularAcceleration(Context, target);
        }

        public static float GetAngularAcceleration(SteeringContext me, GameObject target)
        {

			float result;
			float requiredAngularSpeed;
			float targetOrientation = target.transform.eulerAngles.z; // BEWARE...

			float requiredRotation = targetOrientation - me.transform.eulerAngles.z;  // how many degs do we have to rotate?

			if (requiredRotation < 0)
				requiredRotation = 360 + requiredRotation; // map to positive angles

			if (requiredRotation > 180)
				requiredRotation = -(360 - requiredRotation); // don't rotate more than 180 degs. just reverse rotation sense

			// when here, required rotation is in [-180, +180]

			float rotationSize = Mathf.Abs(requiredRotation);

			if (rotationSize <= me.closeEnoughAngle) // if we're "there", no steering needed
				return 0f;


			if (rotationSize > me.slowDownAngle)
				requiredAngularSpeed = me.maxAngularSpeed;
			else
				requiredAngularSpeed = me.maxAngularSpeed * (rotationSize / me.slowDownAngle);

			// restablish sign
			requiredAngularSpeed = requiredAngularSpeed * Mathf.Sign(requiredRotation);

			// compute acceleration
			result = (requiredAngularSpeed - me.angularSpeed) / me.timeToDesiredAngularSpeed;

			// clip acceleration if necessary
			if (Mathf.Abs(result) > me.maxAngularAcceleration)
				result= me.maxAngularAcceleration * Mathf.Sign(result);

			return result;
		}
    }
}
