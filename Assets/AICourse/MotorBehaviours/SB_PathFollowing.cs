using System.Collections.Generic;
using UnityEngine;

namespace MotorBehaviours
{
    public class SB_PathFollowing : LinearMotorBehaviour
    {
        [Header("Path Settings")]
        public float wayPointReachedRadius = 3f;
        
        [Header("Arrive Settings (Final Waypoint)")]
        public float arriveRadius = 1.5f;
        public float slowDownRadius = 6f;

        // the waypoints
        // regarding type IList<Vector3> it's important to note that both List<Vector3> and Vector3[] 
        // are "implementors" of this interface and can be used interchangeably
        public IList<Vector3> waypoints;
        
        // Public only to facilitate visual debugging in the Inspector
        public int currentWaypointIndex = 0;

        // Public method so external pathfinding wrappers can inject the path
        public void SetPath(IList<Vector3> newPath)
        {
            waypoints = newPath;
            currentWaypointIndex = 0;
        }

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // Fail Fast and data protection: The path must exist and have at least one point
            if (waypoints == null || waypoints.Count == 0)
            {
                // If there is no valid path, we abstain.
                return null;
            }

            // If the index has reached or exceeded the array's length, we have reached the end.
            if (currentWaypointIndex >= waypoints.Count)
            {
                // Clear the path to avoid processing it in future frames
                waypoints = null; 
                return null; // Pure abstention (make way for another behavior)
            }

            // Check the distance to the current waypoint
            Vector3 currentTargetPos = waypoints[currentWaypointIndex];
            float distanceToTarget = (me.transform.position - currentTargetPos).magnitude;

            // If we are close enough, consider the point reached and move to the next one
            if (distanceToTarget <= wayPointReachedRadius)
            {
                currentWaypointIndex++;
            }

            // Check again if this increment made us reach the end directly
            if (currentWaypointIndex >= waypoints.Count)
            {
                waypoints = null;
                return null;
            }

            // Update the target position (necessary in case the index just changed)
            currentTargetPos = waypoints[currentWaypointIndex];

            // EXPLICIT DELEGATION:
            // If it is the last point in the array, use Arrive to brake smoothly.
            if (currentWaypointIndex == waypoints.Count - 1)
            {
                // Note: Assuming SB_Arrive is adapted to the new MotorManager.
                return SB_Arrive.GetDesiredVelocity(me, currentTargetPos, arriveRadius, slowDownRadius);
            }
            else
            {
                // For intermediate waypoints, use pure Seek to maintain inertia 
                // and cross them quickly without braking.
                return SB_Seek.GetDesiredVelocity(me, currentTargetPos);
            }
        }
    }
}