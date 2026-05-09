using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MotorBehaviours
{
    public class MotorManager : MonoBehaviour
    {
        public enum RotationalPolicy { NONE, LWYG, LWYGI, FT, FTI }
        
        [Header("Linear Movement Limits")]
        public float maxForce = 40f;
        public float maxSpeed = 10f;
        
        [Header("Angular Movement Limits")]
        public float maxTorque = 360f;  // torque is the angular equivalent of force (angular force, so to speak)
        public float maxAngularSpeed = 90f;

        [Header("Rotational Policy")]
        public RotationalPolicy rotationalPolicy = RotationalPolicy.NONE;
        // Used only if the policy requires a target (like Face Target - FT)
        public GameObject angularTarget; 

        // Internal state tracking (public so behaviours can read it, but hidden from inspector)
        [HideInInspector] public Vector3 currentVelocity = Vector3.zero;
        
        // TODO ToASK realment ha de ser un Rigidbody2D o un Rigidbody?
        private Rigidbody2D rb;
        
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            // nothing wrong shoudl happen if we don't have a rigidbody
        }
        
        void FixedUpdate()
        {
            // 1. GATHER LINEAR FORCES: Check active components, arbitrate and blend
            Vector3 finalLinearForce = CalculateLinearForce();
            // 2. APPLY LINEAR FORCE: Move the character
            ApplyLinearForce(finalLinearForce);
            // 3. HANDLE ROTATION: Apply overrides (Policies) or gather Angular forces
            ApplyRotations();
        }
        
        private Vector3 CalculateLinearForce()
        {
            Vector3 finalForce = Vector3.zero;

            // 1. GATHER AND SORT
            // Find all active Linear behaviours on this GameObject and sort them by highest priority first
            List<LinearMotorBehaviour> activeBehaviours = GetComponents<LinearMotorBehaviour>()
                .Where(behaviour => behaviour.enabled)
                .OrderByDescending(behaviour => behaviour.arbitrationPriority)
                .ToList();

            // If there are no active linear behaviours, we have nothing to do
            if (activeBehaviours.Count == 0)
            {
                return finalForce;
            }

            // Keep track of the highest priority level currently being processed
            int currentPriorityLevel = activeBehaviours[0].arbitrationPriority;
            float accumulatedWeight = 0f;
            
            // 2. ARBITRATION AND BLENDING LOOP
            foreach (LinearMotorBehaviour behaviour in activeBehaviours)
            {
                // ARBITRATION: 
                // If we drop to a lower priority level, and we already have accumulated force
                // from a higher priority group, we STOP. We ignore lower priorities completely.
                if (behaviour.arbitrationPriority < currentPriorityLevel && finalForce.magnitude > 0.001f)
                {
                    break; 
                }

                // Get the force from the current behaviour
                Vector3 force = behaviour.GetForce(this);

                // BLENDING:
                // If the behaviour returned a valid force, we blend it using its weight
                if (force.magnitude > 0.001f)
                {
                    finalForce += force * behaviour.blendingWeight;
                    accumulatedWeight += behaviour.blendingWeight;
                }
        
                // Update the priority level (in case the next element is lower and the current force was zero)
                currentPriorityLevel = behaviour.arbitrationPriority;
            }
            
            // 3. NORMALIZE WEIGHTS (Optional but recommended for stability)
            // If the combined weights exceed 1 (e.g., 60% Wander + 60% Seek), we scale the force down 
            // so we don't accidentally create super-speed behaviours.
            // TODO toASk what if accumulatedWeight is < 1? Could it be 0?
            if (accumulatedWeight > 1f)
            {
                finalForce = finalForce / accumulatedWeight;
            }

            // Ensure we never exceed the physical limits of the agent
            return Vector3.ClampMagnitude(finalForce, maxForce);
            
        }
        
         
        private void ApplyLinearForce(Vector3 force)
        {
            if (rb != null)
            {
                // Unity's physics engine does the magic here: a = F / m
                rb.AddForce(force); 
                
                // Clamp to maximum speed
                // TODO toAsk aplicar la força ja modifica la velocitat. No espera un frame?
                if (rb.velocity.magnitude > maxSpeed)
                {
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                }
                
                // Update our tracker so LWYG and other behaviours can read the actual velocity
                currentVelocity = rb.velocity; 
            }
            else
            {
                // Fallback for purely kinematic objects (assumes mass = 1)
                // TODO toAsk relament són aquestes les equacions que hem d'aplicar? (plausibilitat física?)
                currentVelocity += force * Time.fixedDeltaTime;
                
                if (currentVelocity.magnitude > maxSpeed)
                {
                    currentVelocity = currentVelocity.normalized * maxSpeed;
                }

                transform.position += currentVelocity * Time.fixedDeltaTime;
            }
        }
        
        private void ApplyRotations()
        {
            // TODO: Here we will execute the Rotational Policies (LWYG, FTI...)
            // and if NONE is selected, we will gather AngularMotorBehaviours (Align, Face...)
            // TODO: també haurem de mirar que la cosa tingui sentit
        }
        
    }
}