using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MotorBehaviours
{
    public class MotorManager : MonoBehaviour
    {
        public enum RotationalPolicy { NONE, LWYG, LWYGI, FT, FTI }
        public enum LinearForcePolicy { PROPORTIONAL, BANG_BANG}
        public enum TorquePolicy {PROPORTIONAL, BANG_BANG}
        
        [Header("Linear Movement Limits")]
        public float maxForce = 40f;
        public float maxSpeed = 10f;
        
        [Header("Angular Movement Limits (in degrees)")]
        public float maxTorque = 360f;  // torque is the angular equivalent of force (angular force, so to speak)
        public float maxAngularSpeed = 90f;

        [Header("Rotational Policy Settings")]
        public RotationalPolicy rotationalPolicy = RotationalPolicy.NONE;
        // Used only if the policy requires a target (like Face Target - FT)
        public GameObject rotationalPolicyTarget; 
        public float policyToleranceRadius = 2f;
        public float policySlowdownRadius = 30f;
        // public float policyTimeToDesiredSpeed = 0.1f;
        
        [Foldout("Advanced Linear Settings")]
        public LinearForcePolicy forcePolicy = LinearForcePolicy.PROPORTIONAL;
        [Tooltip("Only used under PROPORTIONAL POLICY. 1.0 is Reynolds implementation")]
        public float timeToDesiredSpeed = 0.1f;
        
        [Foldout("Advanced Angular Settings")]
        public TorquePolicy torquePolicy = TorquePolicy.PROPORTIONAL;
        [Tooltip("Only used under PROPORTIONAL POLICY. 1.0 is Reynolds implementation")]
        public float timeToDesiredAngularSpeed = 0.1f;
        
        // Internal state tracking (public so behaviours can read it, but hidden from inspector)
        [HideInInspector] public Vector3 currentVelocity = Vector3.zero;
        [HideInInspector] public float currentAngularSpeed = 0f;
        
        private bool anyLinearBehaviourSpoke = false;
        
        // Ens preparem tant per a escenaris 2D com 3D. Lamentablement Unity no
        // considera que Rigidbody2D sigui un cas especial de Rigidbody (3D)
        // (Els motors físics són diferents en cada cas)
        private Rigidbody2D rb2D;
        private Rigidbody rb3D;
        
        void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            rb3D = GetComponent<Rigidbody>();
            // no passa res si no tenim Rigidbody. És quelcom que ja contemplem
        }
        
        /// <summary>
        /// Immediate stop of the agent. Kills any inertia
        /// </summary>
        public void StopCompletely()
        {
            currentVelocity = Vector3.zero;
            
            if (rb3D != null && !rb3D.isKinematic) rb3D.linearVelocity = Vector3.zero;
            else if (rb2D != null && rb2D.bodyType==RigidbodyType2D.Dynamic) rb2D.linearVelocity = Vector3.zero;
        }
        
        /// <summary>
        /// Instantly stops the agent from rotating, killing any angular inertia.
        /// </summary>
        public void StopRotationsCompletely()
        {
            currentAngularSpeed = 0f;
            
            if (rb3D != null && !rb3D.isKinematic) rb3D.angularVelocity = Vector3.zero;
            else if (rb2D != null && rb2D.bodyType == RigidbodyType2D.Dynamic) rb2D.angularVelocity = 0f; // Note: In 2D, angularVelocity is a float!
        }
        
        /// <summary>
        /// Converts a desired velocity into the final application force 
        /// based on the Manager's advanced linear policies.
        /// </summary>
        public Vector3 GetAdjustedForceFromDesiredVelocity(Vector3 desiredVelocity)
        {
            // avoid jittering: if we want to stop and we almost are, we stop immediately. 
            if (desiredVelocity.sqrMagnitude == 0f && currentVelocity.magnitude < 0.05f)
            {
                StopCompletely(); // <--- sets current velocity to zero 
                return Vector3.zero; // <--- no force applied
				// zero velocity & no force ==> no movement. 
            }
            
            Vector3 velocityDifference = desiredVelocity - currentVelocity;
            
            // If we already have the perfect velocity, apply no force (inertia will work ...)
            if (velocityDifference.sqrMagnitude < 0.001f) return Vector3.zero;

            if (forcePolicy == LinearForcePolicy.BANG_BANG)
            {
                // Calculate the exact force needed to reach the desired velocity in a single frame
                Vector3 idealForce = velocityDifference / Time.fixedDeltaTime;
                
                // If the ideal force is within our limits, apply it to nail the speed without overshooting
                if (idealForce.magnitude < maxForce)
                {
                    return idealForce;
                }
                
                // Otherwise, apply maximum possible effort (Pure Bang-Bang aggressiveness)
                return velocityDifference.normalized * maxForce;
            }
            else
            {
                // Proportional effort 
                // (If timeToDesiredSpeed is 1, it behaves as the classic Reynolds algorithm)
                return velocityDifference / timeToDesiredSpeed;
                // should result have a magnitude higher than maxForce, subsequent clamping will fix it. 
            }
        }
        
        /// <summary>
        /// Converts a desired angular velocity into the final application torque 
        /// based on the Manager's advanced angular policies.
        /// </summary>
        public float GetAdjustedTorqueFromDesiredAngularSpeed(float desiredAngularSpeed)
        {
            if (desiredAngularSpeed == 0f && Mathf.Abs(currentAngularSpeed) < 0.5f)
            {
                StopRotationsCompletely();
                return 0f;
            }
            
            float speedDifference = desiredAngularSpeed - currentAngularSpeed;

            // If we already have the perfect angular velocity, apply no torque
            if (Mathf.Abs(speedDifference) < 0.001f) return 0f;

            if (torquePolicy == TorquePolicy.BANG_BANG)
            {
                // Calculate the exact torque needed to reach the desired angular velocity in a single frame
                float idealTorque = speedDifference / Time.fixedDeltaTime;
                
                // If the ideal torque is within our limits, apply it to nail the rotation without overshooting
                if (Mathf.Abs(idealTorque) < maxTorque)
                {
                    return idealTorque;
                }
                
                // Otherwise, apply maximum rotational effort
                return Mathf.Sign(speedDifference) * maxTorque;
            }
            else
            {
                // Proportional effort
                return speedDifference / timeToDesiredAngularSpeed;
            }
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
            List<LinearMotorBehaviour> activeBehaviours = GetComponents<LinearMotorBehaviour>()
                .Where(behaviour => behaviour.enabled)
                .OrderBy(behaviour => behaviour.arbitrationPriority)
                .ToList();

            // An empty list is equivalent to total abstention. We must brake. ---
            if (activeBehaviours.Count == 0)
            {
                anyLinearBehaviourSpoke = false;
                // brake != immediate stop. brake = request zero velocity.
                return GetAdjustedForceFromDesiredVelocity(Vector3.zero);
            }

            int currentPriorityLevel = activeBehaviours[0].arbitrationPriority;
            float accumulatedWeight = 0f;
            
            // --- FLAG: Track if any behaviour returned a valid velocity ---
            anyLinearBehaviourSpoke = false; // Reset before checking this frame
            
            // 2. ARBITRATION AND BLENDING LOOP
            foreach (LinearMotorBehaviour behaviour in activeBehaviours)
            {
                if (behaviour.arbitrationPriority > currentPriorityLevel && finalForce.magnitude > 0.001f)
                {
                    break; 
                }
                
                Vector3? desiredVelocity = behaviour.GetDesiredVelocity(this);
                if (desiredVelocity == null)
                {
                    continue;
                }
                
                // A behaviour has requested a velocity!
                anyLinearBehaviourSpoke = true; // At least one behaviour is active!
                
                Vector3 force = GetAdjustedForceFromDesiredVelocity(desiredVelocity.Value);

                if (force.magnitude > 0.001f)
                {
                    finalForce += force * behaviour.blendingWeight;
                    accumulatedWeight += behaviour.blendingWeight;
                }
        
                currentPriorityLevel = behaviour.arbitrationPriority;
            } 
            
            // --- BRAKING LOGIC ---
            // If ALL behaviours abstained (returned null), the manager must actively brake
            if (!anyLinearBehaviourSpoke)
            {
                // brake != immediate stop. brake = request zero velocity.
                return GetAdjustedForceFromDesiredVelocity(Vector3.zero);
            }

            // 3. NORMALIZE WEIGHTS
            if (accumulatedWeight > 0.001f)
            {
                finalForce = finalForce / accumulatedWeight;
            }

            return Vector3.ClampMagnitude(finalForce, maxForce);
        }// end CalculateLinearForce()

        private float CalculateTorque()
        {
            // en implementar aquest mètode deixo la mateixa aproximació d'arbitri i blending que
            // en el cas de les forces lineals bo i que en el moment d'escriure aquestes línies 
            // no tinc exemples en què això realment sigui necessari (no hi ha comportaments motors
            // de naturalesa angular obtinguts per combinació -ni arbitrada ni ponderada-)
            float finalTorque = 0f;

            // 1. GATHER AND SORT
            List<AngularMotorBehaviour> activeBehaviours = GetComponents<AngularMotorBehaviour>()
                .Where(behaviour => behaviour.enabled)
                .OrderBy(behaviour => behaviour.arbitrationPriority)
                .ToList();

            // --- FIXED GUARD CONDITION ---
            // If the list is empty and policy is NONE, there are absolutely no angular inputs.
            // We must exit immediately to prevent IndexOutOfRangeException and brake smoothly.
            if (activeBehaviours.Count == 0)
            {
                return GetAdjustedTorqueFromDesiredAngularSpeed(0f);
            }

            int currentPriorityLevel = activeBehaviours[0].arbitrationPriority;
            float accumulatedWeight = 0f;
            
            // --- FLAG: Track if any behaviour returned a valid angular speed ---
            bool anyAngularBehaviourSpoke = false;

            // 2. ARBITRATION AND BLENDING LOOP
            foreach (AngularMotorBehaviour behaviour in activeBehaviours)
            {
                // ARBITRATION
                // Note: we use Mathf.Abs because torque can be negative (turning right/left)
                if (behaviour.arbitrationPriority > currentPriorityLevel && Mathf.Abs(finalTorque) > 0.001f)
                {
                    break;
                }

                float? desiredSpeed = behaviour.GetDesiredAngularSpeed(this);
                if (desiredSpeed == null)
                {
                    continue;
                }
                
                // A behaviour has requested an angular speed!
                anyAngularBehaviourSpoke = true;
                
                float torque = GetAdjustedTorqueFromDesiredAngularSpeed(desiredSpeed.Value);

                // BLENDING
                if (Mathf.Abs(torque) > 0.001f)
                {
                    finalTorque += torque * behaviour.blendingWeight;
                    accumulatedWeight += behaviour.blendingWeight;
                }

                currentPriorityLevel = behaviour.arbitrationPriority;
            }

            // --- BRAKING LOGIC ---
            // If ALL active angular behaviours abstained (returned null), we brake smoothly
            if (!anyAngularBehaviourSpoke)
            {
                return GetAdjustedTorqueFromDesiredAngularSpeed(0f);
            }

            // 3. NORMALIZE WEIGHTS
            // same policy as for linear forces
            if (accumulatedWeight > 0.001f)
            {
                finalTorque = finalTorque / accumulatedWeight;
            }

            return finalTorque;
            
        }   // end CalculateTorque()
        
         
        private void ApplyLinearForce(Vector3 force)
        {
            // 3D PHYSICS
            if (rb3D != null && !rb3D.isKinematic)
            {
                rb3D.AddForce(force); 
                rb3D.linearVelocity = Vector3.ClampMagnitude(rb3D.linearVelocity, maxSpeed);
                /* // ClampMagnitude is equivalent to... 
                if (rb3D.linearVelocity.magnitude > maxSpeed)
                {
                    rb3D.linearVelocity = rb3D.linearVelocity.normalized * maxSpeed;
                }*/
                
                currentVelocity = rb3D.linearVelocity;
            }
            // 2D PHYSICS
            else if (rb2D != null && rb2D.bodyType == RigidbodyType2D.Dynamic)
            {
                rb2D.AddForce(force); 
                rb2D.linearVelocity = Vector3.ClampMagnitude(rb2D.linearVelocity, maxSpeed);
                currentVelocity = rb2D.linearVelocity;
            }
            // NO PHYSICS (Kinematic fallback)
            else
            {
                currentVelocity += force * Time.fixedDeltaTime;  // we assume that mass is 1
                currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
                // actualització de la posició. No fem servir
                // x = x0 + v0·dt + 1/2a·dt^2 sinó que ens estalviem 1/2a·dt^2 (molt petit)
                // substituint v0t per vt (velocitat actualitzada) 
                // (Semi-implicit Euler o Symplectic Euler)
                
                if (rb3D != null && rb3D.isKinematic)
                {
                    rb3D.MovePosition(rb3D.position + currentVelocity * Time.fixedDeltaTime);
                }
                // Si és 2D Kinematic
                else if (rb2D != null && rb2D.bodyType == RigidbodyType2D.Kinematic)
                {
                    rb2D.MovePosition(rb2D.position + (Vector2)currentVelocity * Time.fixedDeltaTime);
                }
                // Si no hi ha cap Rigidbody en absolut
                else
                {
                    transform.position += currentVelocity * Time.fixedDeltaTime;
                }
            }
        } // end ApplyLinearForce()
        
        private void ApplyRotations()
        {
            // 0. ABSTENTION DEPENDENCY
            // If there is an active automatic policy but the agent is abstaining from 
            // linear movement, the rotational policies must be suspended.
            if (rotationalPolicy != RotationalPolicy.NONE && !anyLinearBehaviourSpoke)
            {
                if (rotationalPolicy == RotationalPolicy.LWYG || rotationalPolicy == RotationalPolicy.FT)
                {
                    ApplyTorque(GetAdjustedTorqueFromDesiredAngularSpeed(0f));
                }
                return; 
            }
            
            // If this point is reached, it means the rotation is authorized:
            // Either the agent is actively moving (so automatic policies are allowed to execute),
            // or the policy is set to NONE (so we evaluate independent angular behaviours).

            // 1. IMMEDIATE POLICIES (Bypass physics completely)
            if (rotationalPolicy == RotationalPolicy.LWYGI)
            {
                ApplyLWYGI();
                return;
            }
            if (rotationalPolicy == RotationalPolicy.FTI)
            {
                ApplyFTI();
                return;
            }

            // 2. SMOOTH POLICIES OR BEHAVIOURS
            float finalTorque = 0f;

            if (rotationalPolicy == RotationalPolicy.LWYG)
            {
                finalTorque = CalculateLWYGTorque();
            }
            else if (rotationalPolicy == RotationalPolicy.FT)
            {
                finalTorque = CalculateFTTorque();
            }
            else if (rotationalPolicy == RotationalPolicy.NONE)
            {
                // CalculateTorque acts as the arbitrator for angular behaviours
                finalTorque = CalculateTorque();
            }

            // 3. THE ULTIMATE PHYSICAL CLAMP
            // Ensure that no matter the source (Policy or Behaviour), 
            // we NEVER exceed the physical limits of the agent.
            finalTorque = Mathf.Clamp(finalTorque, -maxTorque, maxTorque);

            // 4. APPLY THE TORQUE
            ApplyTorque(finalTorque);
        }

        private void ApplyTorque(float torque)
        {
            // 3D PHYSICS
            if (rb3D != null && !rb3D.isKinematic)
            {
                // In a 3D context playing a 2D game, rotation happens around the Z axis
                rb3D.AddTorque(new Vector3(0, 0, torque)); 
                // In 3D Unity uses radians for angular velocity, so we convert to degrees
                rb3D.angularVelocity = Vector3.ClampMagnitude(rb3D.angularVelocity, maxAngularSpeed*Mathf.Deg2Rad);
                currentAngularSpeed = rb3D.angularVelocity.z * Mathf.Rad2Deg;
            }
            // 2D PHYSICS
            else if (rb2D != null && rb2D.bodyType == RigidbodyType2D.Dynamic)
            {
                rb2D.AddTorque(torque);
                // In 2D, angular velocity is a simple float, so we clamp it normally
                // In 2D Unity uses degrees. No conversion needed.
                rb2D.angularVelocity = Mathf.Clamp(rb2D.angularVelocity, -maxAngularSpeed, maxAngularSpeed);
                currentAngularSpeed = rb2D.angularVelocity;
            }
            // NO PHYSICS (Kinematic fallback)
            else
            {
                currentAngularSpeed += torque * Time.fixedDeltaTime;
                currentAngularSpeed = Mathf.Clamp(currentAngularSpeed, -maxAngularSpeed, maxAngularSpeed);
                // Rotate around the Z axis
                // angles already in degrees.
                // Si és 3D Kinematic
                if (rb3D != null && rb3D.isKinematic)
                {
                    // Al 3D hem de multiplicar la rotació actual per una nova rotació a l'eix Z
                    Quaternion deltaRotation = Quaternion.Euler(0, 0, currentAngularSpeed * Time.fixedDeltaTime);
                    rb3D.MoveRotation(rb3D.rotation * deltaRotation);
                }
                
                // Si és 2D Kinematic
                else if (rb2D != null && rb2D.bodyType == RigidbodyType2D.Kinematic)
                {
                    rb2D.MoveRotation(rb2D.rotation + currentAngularSpeed * Time.fixedDeltaTime);
                }
                // Si no hi ha cap Rigidbody en absolut
                else
                {
                    transform.Rotate(0, 0, currentAngularSpeed * Time.fixedDeltaTime);
                }
            }
        }

        private void ApplyLWYGI()
        {
            if (currentVelocity.magnitude < 0.001f) return;
            
            // Standard Math to get the angle from a 2D vector
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            
            if (rb2D != null)
            {
                rb2D.rotation = angle;
            }
            else
            {
                // Fallback for 3D or Kinematic
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void ApplyFTI()
        {
            // Fail Fast if target is missing
            if (rotationalPolicyTarget == null)
            {
                Debug.LogError($"[MotorManager] Critical Error: In MotorManager 'Rotational Policy Target' is missing on GameObject '{gameObject.name}' but policy is set to FTI.");
                Debug.Break();
                return;
            }

            Vector3 direction = rotationalPolicyTarget.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (rb2D != null)
            {
                rb2D.rotation = angle;
            }
            else
            {
                // Fallback for 3D or Kinematic
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        
        private float CalculateLWYGTorque()
        {
            // If we are not moving, we don't change our rotation
            if (currentVelocity.magnitude < 0.001f) return 0f;

            // Target angle is where we are currently heading
            float targetAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            return InternalSmoothAlign(targetAngle);
        }
        
        private float CalculateFTTorque()
        {
            // Fail Fast if target is missing
            if (rotationalPolicyTarget == null)
            {
                Debug.LogError($"[MotorManager] Critical Error: In MotorManager 'Rotational Policy Target' is missing on GameObject '{gameObject.name}' but policy is set to FT.");
                Debug.Break();
                return 0f;
            }

            Vector3 direction = rotationalPolicyTarget.transform.position - transform.position;
            if (direction.sqrMagnitude == 0f) return 0f;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return InternalSmoothAlign(targetAngle);
        }

        // Shared logic for smooth rotation policies
        private float InternalSmoothAlign(float targetAngle)
        {
            // this is an "internal" implementation of the ALIGN behaviour
            float currentRotation = transform.eulerAngles.z;
            float rotationDifference = Mathf.DeltaAngle(currentRotation, targetAngle);
            float rotationSize = Mathf.Abs(rotationDifference);

            if (rotationSize < policyToleranceRadius)
            {
                StopRotationsCompletely(); // [Gemini Pun!!!] PULL THE ROTATIONAL PARACHUTE!
                return 0f;
            }

            float desiredSpeed;
            if (rotationSize > policySlowdownRadius)
            {
                desiredSpeed = maxAngularSpeed;
            }
            else
            {
                desiredSpeed = maxAngularSpeed * (rotationSize / policySlowdownRadius);
            }

            desiredSpeed *= Mathf.Sign(rotationDifference);

            return GetAdjustedTorqueFromDesiredAngularSpeed(desiredSpeed);
        }
    }
}