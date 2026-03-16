using UnityEngine;

namespace Steerings
{
	public class Utils
	{
		public static Vector3 OrientationToVector (float alpha) {
			// alpha is an angle in degrees (anticlockwise) 

			//alpha = 360 - alpha; // do this in clockwise systems

			// convert to radians
			alpha = alpha * Mathf.Deg2Rad;

			float cos = Mathf.Cos (alpha);
			float sin = Mathf.Sin (alpha);

			return new Vector3 (cos, sin, 0);
		}

		public static float VectorToOrientation (Vector3 vector) {

			Vector3 direction = vector.normalized;

			float sin = direction.y;
			float cos = direction.x;

			float tan = sin / cos;

			float orientation = Mathf.Atan (tan)*Mathf.Rad2Deg;

			// remeber atan returns in the interval [-pi/2, pi/2] [-90, 90]
			// cosine determines region 

			if (cos < 0)
				orientation = orientation + 180;

			// orientation = 360 - orientation; // do this in clockwise systems

			return orientation;
		}


		public static  float binomial () {
			return Random.value - Random.value;
		}

		public static float DotProduct (Vector3 a, Vector3 b) {
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		public static bool InCone (GameObject me, GameObject target, float totalAngle)
        {
			float dotProd = Utils.DotProduct(Utils.OrientationToVector(me.transform.eulerAngles.z),
										 (target.transform.position - me.transform.position).normalized
										);
			return dotProd > Mathf.Cos(totalAngle / 2 * Mathf.Deg2Rad);
		}

	}
}

