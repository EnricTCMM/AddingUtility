
using UnityEngine;

namespace MotorBehaviours
{
    public class SB_ObstacleAvoidance : LinearMotorBehaviour
    {
        [Header("Whisker Settings")]
        public float lookAheadLength = 3f;
        public float secondaryWhiskerAngle = 30f;
        [Range(0.1f, 1f)]
        public float secondaryWhiskerRatio = 0.7f;
        public float avoidDistance = 2f;

        [Header("Perseverance")]
        [Tooltip("Time to maintain the evasion maneuver after losing 'sight' of the obstacle")]
        public float perseveranceTime = 0.5f;

        [Header("Physics Settings")]
        [Tooltip("The physics layers that represent obstacles")]
        public LayerMask obstacleLayer;

        [Header("Debug Settings")]
        public bool showWhiskers = true;

        // Internal State
        private bool persevering = false;
        private float perseveranceElapsed = 0f;
        private Vector3 avoidanceVelocity; // Cached intention

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // 1. Detect and get the pure evasion intention
            Vector3 evasionIntent = Detection2D(me);

            // 2. If an obstacle is detected right now, we react and reset perseverance
            if (evasionIntent != Vector3.zero)
            {
                avoidanceVelocity = evasionIntent; // Cache the velocity for later
                perseveranceElapsed = 0f;
                persevering = true;
                
                return evasionIntent;
            }

            // 3. If no obstacle is detected, should we persevere? (ESN Inertia)
            if (persevering)
            {
                perseveranceElapsed += Time.fixedDeltaTime;
                
                if (perseveranceElapsed < perseveranceTime)
                {
                    // Continue returning the cached velocity to clear the corner
                    return avoidanceVelocity; 
                }
                else
                {
                    // Perseverance time exhausted, go back to normal behaviors
                    persevering = false;
                }
            }

            // 4. No obstacle, no perseverance: no evasion needed
            // return Vector3.zero;
            return null;
        }

        private Vector3 Detection2D(MotorManager me)
        {
            // Calculate the current forward direction
            Vector3 forwardVector;
            if (me.currentVelocity.magnitude > 0.01f)
            {
                forwardVector = me.currentVelocity.normalized;
            }
            else
            {
                float currentRotRad = me.transform.eulerAngles.z * Mathf.Deg2Rad;
                forwardVector = new Vector3(Mathf.Cos(currentRotRad), Mathf.Sin(currentRotRad), 0f);
            }

            // Calculate the three whiskers 
            Vector3 mainWhisker = forwardVector * lookAheadLength;
            
            float leftAngleRad = (me.transform.eulerAngles.z + secondaryWhiskerAngle) * Mathf.Deg2Rad;
            Vector3 leftWhisker = new Vector3(Mathf.Cos(leftAngleRad), Mathf.Sin(leftAngleRad), 0f) * (lookAheadLength * secondaryWhiskerRatio);

            float rightAngleRad = (me.transform.eulerAngles.z - secondaryWhiskerAngle) * Mathf.Deg2Rad;
            Vector3 rightWhisker = new Vector3(Mathf.Cos(rightAngleRad), Mathf.Sin(rightAngleRad), 0f) * (lookAheadLength * secondaryWhiskerRatio);

            // Array to hold the rays for iteration
            Vector3[] whiskers = { mainWhisker, leftWhisker, rightWhisker };
            
            // Variables to track the closest hit
            bool hitFound = false;
            RaycastHit2D closestHit = new RaycastHit2D();
            float minDistance = float.MaxValue;

            // Cast the rays
            Vector2 pos2D = new Vector2(me.transform.position.x, me.transform.position.y);
            foreach (Vector3 whisker in whiskers)
            {
                Vector2 dir2D = new Vector2(whisker.x, whisker.y);
                float length = whisker.magnitude;

                // We use LayerMask instead of disabling the agent's collider (optimization)
                RaycastHit2D hit = Physics2D.Raycast(pos2D, dir2D.normalized, length, obstacleLayer);

                if (hit.collider != null)
                {
                    if (hit.distance < minDistance)
                    {
                        minDistance = hit.distance;
                        closestHit = hit;
                        hitFound = true;
						break; // new !!! 
                    }
                }
            }

            // If an obstacle was hit, calculate the escape point and delegate to Seek
            if (hitFound)
            {
                Vector3 escapePoint = new Vector3(closestHit.point.x, closestHit.point.y, me.transform.position.z) 
                                      + new Vector3(closestHit.normal.x, closestHit.normal.y, 0f) * avoidDistance;
                
                // DELEGATION: We want to Seek the escape point!
                return SB_Seek.GetDesiredVelocity(me, escapePoint);
            }

            return Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            if (!showWhiskers || !Application.isPlaying) return;

            MotorManager me = GetComponent<MotorManager>();
            if (me == null) return;

            Vector3 forwardVector;
            if (me.currentVelocity.magnitude > 0.01f)
            {
                forwardVector = me.currentVelocity.normalized;
            }
            else
            {
                float currentRotRad = me.transform.eulerAngles.z * Mathf.Deg2Rad;
                forwardVector = new Vector3(Mathf.Cos(currentRotRad), Mathf.Sin(currentRotRad), 0f);
            }

            Vector3 mainWhisker = forwardVector * lookAheadLength;
            
            float leftAngleRad = (me.transform.eulerAngles.z + secondaryWhiskerAngle) * Mathf.Deg2Rad;
            Vector3 leftWhisker = new Vector3(Mathf.Cos(leftAngleRad), Mathf.Sin(leftAngleRad), 0f) * (lookAheadLength * secondaryWhiskerRatio);

            float rightAngleRad = (me.transform.eulerAngles.z - secondaryWhiskerAngle) * Mathf.Deg2Rad;
            Vector3 rightWhisker = new Vector3(Mathf.Cos(rightAngleRad), Mathf.Sin(rightAngleRad), 0f) * (lookAheadLength * secondaryWhiskerRatio);

            Gizmos.color = persevering ? Color.red : Color.yellow;
            Gizmos.DrawRay(transform.position, mainWhisker);
            Gizmos.DrawRay(transform.position, leftWhisker);
            Gizmos.DrawRay(transform.position, rightWhisker);
        }
    }
}