using UnityEngine;
using Steerings; 

namespace MotorBehaviours
{
    public class SB_AdvancedObstacleAvoidance : LinearMotorBehaviour
    {
        [Header("Whisker Settings")]
        public float lookAheadLength = 3f;
        public float secondaryWhiskerAngle = 30f;
        [Range(0.1f, 1f)]
        public float secondaryWhiskerRatio = 0.7f;
        public float avoidDistance = 2f;

        [Header("Advanced Sensor Settings")]
        [Tooltip("The radius of the agent, used to give thickness to the central sweep (CircleCast)")]
        public float agentRadius = 0.5f;

        [Header("Perseverance")]
        [Tooltip("Time to maintain the evasion maneuver after losing sight of the obstacle")]
        public float perseveranceTime = 0.5f;

        [Header("Physics Settings")]
        [Tooltip("The physics layers that represent obstacles")]
        public LayerMask obstacleLayer;

        [Header("Debug Settings")]
        public bool showWhiskers = true;
       

        // Internal State
        private bool isPersevering = false;
        private bool isCurrentlyHitting = false;
        private float perseveranceElapsed = 0f;
        private Vector3 avoidanceVelocity; 
        private Vector3 currentEscapePoint; // Cached for Gizmos drawing
        
        // Tracks which whisker triggered the evasion: -1 (None), 0 (Main), 1 (Left), 2 (Right)
        private int activeWhiskerIndex = -1; 
        
        private float crossSize = 2.5f; // Size of the debug cross

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            isCurrentlyHitting = false;
            
            // 1. Detect and get the pure evasion intention
            Vector3? evasionIntent = Detection2D(me);

            // 2. If an obstacle is detected right now
            if (evasionIntent != null)
            {
                avoidanceVelocity = evasionIntent.Value; 
                perseveranceElapsed = 0f;
                isPersevering = false;
                isCurrentlyHitting = true;
                return avoidanceVelocity;
            }

            // 3. No obstacle detected THIS frame, check for perseverance
            if (activeWhiskerIndex != -1 && perseveranceElapsed < perseveranceTime)
            {
                isPersevering = true;
                perseveranceElapsed += Time.deltaTime;
                return avoidanceVelocity;
            }

            // 4. Reset state when no evasion is needed
            isPersevering = false;
            activeWhiskerIndex = -1;
            return null; 
        }

        private Vector3? Detection2D(MotorManager me)
        {
            // Calculate the main forward vector based on velocity (if moving) or rotation (if completely stopped)
            Vector3 forwardVector;
            if (me.currentVelocity.magnitude > 0.01f)
            {
                forwardVector = me.currentVelocity.normalized;
            }
            else
            {
                forwardVector = Utils.OrientationToVector(me.transform.eulerAngles.z);
            }

            // Calculate the whiskers' geometry
            Vector3 mainWhisker = forwardVector * lookAheadLength;
            
            float baseMovementAngleDeg = Utils.VectorToOrientation(forwardVector);

            float leftAngleDeg = baseMovementAngleDeg + secondaryWhiskerAngle;
            Vector3 leftWhisker = Utils.OrientationToVector(leftAngleDeg) * (lookAheadLength * secondaryWhiskerRatio);

            float rightAngleDeg = baseMovementAngleDeg - secondaryWhiskerAngle;
            Vector3 rightWhisker = Utils.OrientationToVector(rightAngleDeg) * (lookAheadLength * secondaryWhiskerRatio);

            bool isObstacleDetected = false;
            RaycastHit2D validHit = new RaycastHit2D();
            float minDistance = float.MaxValue;
            Vector2 pos2D = new Vector2(me.transform.position.x, me.transform.position.y);

            // -------------------------------------------------------------
            // SENSOR 0: CENTRAL SENSOR (Swept Collision for Clearance)
            // -------------------------------------------------------------
            Vector2 centralDir = new Vector2(mainWhisker.x, mainWhisker.y);
            // Using CircleCast to give the main ray physical volume
            RaycastHit2D centralHit = Physics2D.CircleCast(pos2D, agentRadius, centralDir.normalized, mainWhisker.magnitude, obstacleLayer);

            if (centralHit.collider != null)
            {
                minDistance = centralHit.distance;
                validHit = centralHit;
                isObstacleDetected = true;
                activeWhiskerIndex = 0; 
            }

            // -------------------------------------------------------------
            // SENSOR 1: LEFT SENSOR (Raycast for Peripheral Vision)
            // -------------------------------------------------------------
            Vector2 leftDir = new Vector2(leftWhisker.x, leftWhisker.y);
            RaycastHit2D leftHit = Physics2D.Raycast(pos2D, leftDir.normalized, leftWhisker.magnitude, obstacleLayer);

            // We must check if it hits AND if the threat is closer than the current minimum
            if (leftHit.collider != null && leftHit.distance < minDistance)
            {
                minDistance = leftHit.distance;
                validHit = leftHit;
                isObstacleDetected = true;
                activeWhiskerIndex = 1;
            }

            // -------------------------------------------------------------
            // SENSOR 2: RIGHT SENSOR (Raycast for Peripheral Vision)
            // -------------------------------------------------------------
            Vector2 rightDir = new Vector2(rightWhisker.x, rightWhisker.y);
            RaycastHit2D rightHit = Physics2D.Raycast(pos2D, rightDir.normalized, rightWhisker.magnitude, obstacleLayer);

            if (rightHit.collider != null && rightHit.distance < minDistance)
            {
                minDistance = rightHit.distance;
                validHit = rightHit;
                isObstacleDetected = true;
                activeWhiskerIndex = 2;
            }

            // -------------------------------------------------------------
            // DELEGATION
            // -------------------------------------------------------------
            if (isObstacleDetected)
            {
                // Calculate the surrogate target (escape point)
                currentEscapePoint = new Vector3(validHit.point.x, validHit.point.y, me.transform.position.z) 
                                      + new Vector3(validHit.normal.x, validHit.normal.y, 0f) * avoidDistance;
                
                // Calculate the velocity towards the escape point
                Vector3 desiredVelocity = (currentEscapePoint - me.transform.position).normalized * me.maxSpeed;
                return desiredVelocity;
            }

            return null;
        }

        private void OnDrawGizmos()
        {
            if (!showWhiskers || !Application.isPlaying) return;

            MotorManager me = GetComponent<MotorManager>();
            if (me == null) return;

            // Re-calculating whiskers for drawing purposes
            Vector3 forwardVector;
            if (me.currentVelocity.magnitude > 0.01f)
            {
                forwardVector = me.currentVelocity.normalized;
            }
            else
            {
                forwardVector = Utils.OrientationToVector(me.transform.eulerAngles.z);
            }

            Vector3 mainWhisker = forwardVector * lookAheadLength;
            float baseMovementAngleDeg = Utils.VectorToOrientation(forwardVector);

            float leftAngleDeg = baseMovementAngleDeg + secondaryWhiskerAngle;
            Vector3 leftWhisker = Utils.OrientationToVector(leftAngleDeg) * (lookAheadLength * secondaryWhiskerRatio);

            float rightAngleDeg = baseMovementAngleDeg - secondaryWhiskerAngle;
            Vector3 rightWhisker = Utils.OrientationToVector(rightAngleDeg) * (lookAheadLength * secondaryWhiskerRatio);

            Vector3[] whiskers = { mainWhisker, leftWhisker, rightWhisker };

            for (int i = 0; i < whiskers.Length; i++)
            {
                Color whiskerColor = Color.black;

                if (i == activeWhiskerIndex)
                {
                    if (isCurrentlyHitting) whiskerColor = Color.red;
                    else if (isPersevering) whiskerColor = new Color(1f, 0.5f, 0f); // Orange
                }

                Gizmos.color = whiskerColor;
                Gizmos.DrawRay(transform.position, whiskers[i]);
                
                // Optional: Draw a wire sphere at the end of the central whisker to visualize the sweep volume
                if (i == 0)
                {
                    Gizmos.DrawWireSphere(transform.position + whiskers[i], agentRadius);
                }
            }

            // Draw the black cross at the Surrogate Target if evading
            if (isCurrentlyHitting || isPersevering)
            {
                Gizmos.color = Color.black;
                // Horizontal line of the cross
                Gizmos.DrawLine(currentEscapePoint + Vector3.left * crossSize, currentEscapePoint + Vector3.right * crossSize);
                // Vertical line of the cross
                Gizmos.DrawLine(currentEscapePoint + Vector3.up * crossSize, currentEscapePoint + Vector3.down * crossSize);
            }
        }
    }
}