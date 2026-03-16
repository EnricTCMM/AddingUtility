using UnityEngine;
using Pathfinding;

namespace Steerings
{
	public class PathFollowing : SteeringBehaviour
	{

		public Path path; // path being public can be "setted" from the outside... (e.g by pathFeeder)
		public float wayPointReachedRadius = 3f;
        
		public int currentWaypointIndex = 0;  // public just for debbuging purposes


        public override Vector3 GetLinearAcceleration()
        {
            return PathFollowing.GetLinearAcceleration(Context, path, ref currentWaypointIndex, wayPointReachedRadius);
        }


        public static Vector3 GetLinearAcceleration (SteeringContext me, Path path, 
			                                         ref int currentWaypointIndex,
                                                     float wayPointReachedRadius )
		{
            // path shouldn't be neither null nor erroneous
            if (path == null)
            {
                Debug.LogError("PathFollowing invoked with null path");
				return Vector3.zero;
            }
            if (path.error)
            {
                Debug.LogError("PathFollowing invoked with an \"erroneous\" path");
                return Vector3.zero;
            }

            // if currentWaypoint is not valid, end of path has been reached
            if (path.vectorPath.Count == currentWaypointIndex)
                return Vector3.zero;

            // if we're "close" to the current waypoint try going to the next one
            float distance = (me.transform.position - path.vectorPath[currentWaypointIndex]).magnitude;
            if (distance <= wayPointReachedRadius)
                currentWaypointIndex++;

			// check if the previous ++ operation has led to the end of path
            if (path.vectorPath.Count == currentWaypointIndex)
                return Vector3.zero;

            SURROGATE_TARGET.transform.position = path.vectorPath[currentWaypointIndex];

            if (currentWaypointIndex == path.vectorPath.Count - 1)
                // use arrive for the last waypoint
				// Notice that the retrocompatibility version of GetLinearAcceleration is used.
                return Arrive.GetLinearAccelerationForPathfinding(me, SURROGATE_TARGET, wayPointReachedRadius / 2, wayPointReachedRadius * 2);
            else
                return Seek.GetLinearAcceleration(me, SURROGATE_TARGET);
        }
		
	}
}
