using UnityEngine;

namespace MotorBehaviours
{
    public class SB_Wander : LinearMotorBehaviour
    {
        [Header("Wander Settings")]
        public float wanderRadius = 10f;
        public float wanderOffset = 25f;
        // How much the target angle can change per frame (in degrees)
        public float wanderRate = 15f; 

        [Header("Attractor Settings (WanderAround)")]
        [Tooltip("Attraction point. If left null pure wander")]
        public GameObject attractor;
        [Range(0f, 1f)]
        [Tooltip("0 = Pure wander, 1 = Pure seek towards the attractor ")]
        public float attractionWeight = 0.2f;
        
        [Header("Debug Settings")]
        public bool showWanderGizmos = true;

        // Internal state
        private float wanderTargetOrientation = 0f;
        // Stored for Gizmo drawing
        private Vector3 circleCenter;
        private Vector3 surrogateTargetPosition;

        public override Vector3? GetDesiredVelocity(MotorManager me)
        {
            // 1. Change the target orientation using a Binomial distribution
            // (Random.value - Random.value) produces a number between -1 and 1, 
            // but values closer to 0 are more likely. This makes the wander less erratic.
            float binomial = Random.value - Random.value;
            wanderTargetOrientation += binomial * wanderRate;

            // 2. Calculate the "Forward" direction of the agent
            Vector3 agentPosition = me.transform.position;
            Vector3 forwardVector;

            // In front of my velocity. 
            if (me.currentVelocity.magnitude > 0.01f)
            {
                forwardVector = me.currentVelocity.normalized;
            }
            else
            {
                // If stopped, use current rotation (Z axis in 2D)
                float currentRotRad = me.transform.eulerAngles.z * Mathf.Deg2Rad;
                forwardVector = new Vector3(Mathf.Cos(currentRotRad), Mathf.Sin(currentRotRad), 0f);
            }

            // 3. Place the center of the circle in front of the agent
            circleCenter = agentPosition + forwardVector * wanderOffset;

            // 4. Calculate the position of the surrogate target on the circle
            float targetRotRad = wanderTargetOrientation * Mathf.Deg2Rad;
            Vector3 targetOffset = new Vector3(Mathf.Cos(targetRotRad), Mathf.Sin(targetRotRad), 0f) * wanderRadius;
            surrogateTargetPosition = circleCenter + targetOffset;

            // pure wander intention (just seek the surrogate target). Modulus of returned 
            // velocity is 0 or maxSpeed. 
            Vector3 wanderVelocity = SB_Seek.GetDesiredVelocity(me, surrogateTargetPosition);

            // SORT OF A FAIL FAST...
            if (attractionWeight > 0.0 && attractor == null)
            {
                Debug.LogWarning($"BEWARE: null attractor with non-zero weight in SB_Wander of {gameObject}");
            }
            
            if (attractor != null)
            {
                Vector3 attractorVelocity = SB_Seek.GetDesiredVelocity(me, attractor.transform.position);
                //return Vector3.Lerp(wanderVelocity, attractorVelocity, attractionWeight);
                // previous line equivalent to this:
                return attractorVelocity * attractionWeight + wanderVelocity * (1 - attractionWeight);
            }
            else return wanderVelocity;
        }

        // Native Unity method to draw debug shapes in the Scene view
        private void OnDrawGizmos()
        {
            if (!showWanderGizmos || !Application.isPlaying) return;

            // 1. Línia negra: de l'agent al centre del cercle
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, circleCenter);

            // 2. Cercle vermell pur (Dibuixat manualment punt per punt)
            Gizmos.color = Color.red;
            int segments = 24; 
            float angleStep = 360f / segments;
            
            Vector3 prevPoint = circleCenter + new Vector3(wanderRadius, 0, 0); 
            
            for (int i = 1; i <= segments; i++)
            {
                float angleRad = i * angleStep * Mathf.Deg2Rad;
                Vector3 nextPoint = circleCenter + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * wanderRadius;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
            
            // 3. Creueta negra: El Surrogate Target
            Gizmos.color = Color.black;
            float crossSize = 3f; // Mida dels braços
            
            // Línia horitzontal de la creu
            Gizmos.DrawLine(surrogateTargetPosition + new Vector3(-crossSize, 0, 0), 
                surrogateTargetPosition + new Vector3(crossSize, 0, 0));
            // Línia vertical de la creu
            Gizmos.DrawLine(surrogateTargetPosition + new Vector3(0, -crossSize, 0), 
                surrogateTargetPosition + new Vector3(0, crossSize, 0));
            /*
            if (attractor != null)
            {
                Gizmos.color = Color.blue; // Un color diferent per a l'atracció
                Gizmos.DrawLine(transform.position, attractor.transform.position);
            }
            */
        }
    }
}