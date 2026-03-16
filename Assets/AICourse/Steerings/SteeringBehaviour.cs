using UnityEngine;

// All code by ESN

// All aspects regarding RIGIDBODIES are highly experimental and should
// be used with extreme care.

namespace Steerings
{
    [RequireComponent(typeof(SteeringContext))]
    public abstract class SteeringBehaviour : MonoBehaviour
    {

        public enum RotationalPolicy { LWYG, LWYGI, FT, FTI, NONE };
        // LWYG: look where you go
        // LWYGI: look where you go immediately
        // FT: face your target
        // FTI: face your target immediately
        // NONE: no rotational policy

        public RotationalPolicy rotationalPolicy = RotationalPolicy.NONE;

        public SteeringContext Context { get => GetComponent<SteeringContext>(); }

        new Rigidbody2D rigidbody;

        private bool hasRigidbody;

        protected static GameObject SURROGATE_TARGET = null;

        void Start() 
        {
            rigidbody = GetComponent<Rigidbody2D>();
            hasRigidbody = rigidbody != null;

            if (SURROGATE_TARGET == null)
            {
                SURROGATE_TARGET = new GameObject("Surrogate Target");
                SURROGATE_TARGET.AddComponent<SteeringContext>();
            }
        }

        void FixedUpdate() {

            if (hasRigidbody)
            {
                ApplyLinearAccelerationWithRigidbody();
                ApplyAngularAccelerationWithRigidBody();
            }
            else
            {
                ApplyLinearAccelerationWithoutRigidBody();
                ApplyAngularAccelerationWithoutRigidbody();
            }
        }

        private void ApplyLinearAccelerationWithoutRigidBody ()
        {
            Vector3 acceleration = GetLinearAcceleration();
            // zero acceleration implies stop...
            
            if (acceleration.Equals(Vector3.zero))
            {
                Context.velocity = Vector3.zero;
                return;
            }

            float dt = Time.fixedDeltaTime;
            Vector3 velIncrement = acceleration * dt;
            Context.velocity += velIncrement;
            
            if (Context.clipVelocity)
                if (Context.velocity.magnitude > Context.maxSpeed)
                    Context.velocity = Context.velocity.normalized * Context.maxSpeed;

            transform.position += Context.velocity * dt + 0.5f * acceleration * dt * dt;
        }

        private void ApplyAngularAccelerationWithoutRigidbody ()
        {
            float acceleration = GetAngularAcceleration();
            // zero acceleration implies stop
            
            if (acceleration==0)
            {
                Context.angularSpeed = 0;
                return;
            }
            
            float dt = Time.fixedDeltaTime;
            Context.angularSpeed += acceleration * dt;

            if (Context.clipAngularSpeed)
                if (Mathf.Abs(Context.angularSpeed) > Context.maxAngularSpeed)
                    Context.angularSpeed = Context.maxAngularSpeed * 
                                                 Mathf.Sign(Context.angularSpeed);

            float orientation = transform.rotation.eulerAngles.z + 
                                Context.angularSpeed * dt + 0.5f * acceleration * dt * dt;
            transform.rotation = Quaternion.Euler(0, 0, orientation);
        }

        private void ApplyLinearAccelerationWithRigidbody()
        {
            Vector3 acceleration = GetLinearAcceleration();

            // zero acceleration implies stop...
            if (acceleration.Equals(Vector3.zero))
            {
                rigidbody.linearVelocity = Vector3.zero;
                Context.velocity = Vector3.zero;
                return;
            }

            if (rigidbody.isKinematic)
            {
                // compute velocity increcment
                Vector3 velIncrement = acceleration * Time.fixedDeltaTime;

                // compute new velocity and clip it if necessary
                Context.velocity += velIncrement;
                if (Context.clipVelocity)
                    if (Context.velocity.magnitude > Context.maxSpeed)
                        Context.velocity = Context.velocity.normalized * Context.maxSpeed;

                rigidbody.linearVelocity = Context.velocity;
                // the previous line is equivalent to
                // rigidbody.MovePosition(transform.position + context.velocity*Time.fixedDeltaTime);
                // BUT MovePosition does not seem to update rigidbody.velocity accordingly
            }

            else  // if rigidbody is not kinematic then it is dynamic ("controlled" by forces)
            {
                rigidbody.AddForce(acceleration * rigidbody.mass, ForceMode2D.Force);

                if (Context.clipVelocity)
                {
                    float speed = rigidbody.linearVelocity.magnitude;
                    if (speed > Context.maxSpeed) rigidbody.linearVelocity = rigidbody.linearVelocity.normalized * Context.maxSpeed;
                }

                // cache velocity in the context
                Context.velocity = rigidbody.linearVelocity; // "caching" of velocity required since behaviours like arrive need to 
                                                       // know it without having to access the rigidbody
                                                       // BUT: this velocity is the one previous to applying the force...
            }
        }

        private void ApplyAngularAccelerationWithRigidBody ()
        {
            float acceleration = GetAngularAcceleration();
            if (acceleration == 0)
            {
                rigidbody.angularVelocity = 0;
                Context.angularSpeed = 0;
            } 

            if (rigidbody.isKinematic)
            {
                float speedIncrement = acceleration * Time.fixedDeltaTime;
                // compute new angular speed and clip if necessary
                Context.angularSpeed += speedIncrement;
                if (Context.clipAngularSpeed)
                    if (Mathf.Abs(Context.angularSpeed) > Context.maxAngularSpeed)
                        Context.angularSpeed = Context.maxAngularSpeed * Mathf.Sign(Context.angularSpeed);
                // apply to rigidbody
                rigidbody.angularVelocity = Context.angularSpeed;
            }
            else
            {
                rigidbody.AddTorque(acceleration * rigidbody.inertia * Mathf.Deg2Rad, ForceMode2D.Force);
                
                if (Context.clipAngularSpeed)
                    if (Mathf.Abs(rigidbody.angularVelocity)>Context.maxAngularSpeed)
                        rigidbody.angularVelocity = Context.maxAngularSpeed * Mathf.Sign(rigidbody.angularVelocity);

                // cache
                Context.angularSpeed = rigidbody.angularVelocity;
               
            }
        }

        public virtual Vector3 GetLinearAcceleration()
        {
            // subclasses defining linear steerings must redefine this method 
            return Vector3.zero;
        }
        
        public virtual float GetAngularAcceleration ()
        {
            // subclasses defining rotational steerings must redefine this method
            // the current implementation
            // just applies rotational policies for linear steerings
            return ApplyRotationalPolicy(rotationalPolicy);
        }

        public virtual GameObject GetTarget ()
        {
            // subclasses having a target should redefine this method
            // if FT o FTI rotational policies make sense for them 
            Debug.LogError("Invoking non-redefined version of " +
                           "SteeringBehaviour.GetTarget(). " +
                           "Subclasses (linear) having a target should " +
                           "redefine this method if FT o FTI rotational policies" +
                           " make sense for them");
            
            return null;
        }

        private float ApplyRotationalPolicy (RotationalPolicy policy)
        {
            float angAcceleration = 0;
            switch (policy)
            {
                case RotationalPolicy.LWYG:
                    angAcceleration = PolicyLWYG();
                    break;
                case RotationalPolicy.LWYGI:
                    PolicyLWYGI();
                    break;
                case RotationalPolicy.FT:
                    angAcceleration = PolicyFT(GetTarget());
                    break;
                case RotationalPolicy.FTI:
                    PolicyFTI(GetTarget());
                    break;
            }
            return angAcceleration;
        } 

        // Rotational (FACING) policies

        private float PolicyLWYG()
        {
            float angAcceleration;
            if (Context.velocity.magnitude < 0.001f) angAcceleration = 0;
            // if (context.velocity.Equals(Vector3.zero)) angAcceleration = 0;
            else
            {
                SURROGATE_TARGET.transform.rotation = Quaternion.Euler(0, 0, Utils.VectorToOrientation(Context.velocity));
                angAcceleration = Align.GetAngularAcceleration(Context, SURROGATE_TARGET);
            }
            return angAcceleration;
        }

        private void PolicyLWYGI()
        {
            if (Context.velocity.magnitude < 0.001f) return;
            // this policy does not generate an acceleration. It changes the orientation of the go directly!!!
            if (hasRigidbody)
            {
                rigidbody.rotation = Utils.VectorToOrientation(Context.velocity);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, Utils.VectorToOrientation(Context.velocity));
            }
        }

        private float PolicyFT (GameObject target)
        {
            float angAcceleration = 0;
            if (target == null)
                Debug.LogError("FT rotational policy applied with null target");
            else
                angAcceleration = Face.GetAngularAcceleration(Context, target);

            return angAcceleration;
        }

        private void PolicyFTI (GameObject target)
        {
            if (target == null)
            {
                Debug.LogError("FT rotational policy applied with null target");
                return;
            }

            Vector3 directionToTarget = target.transform.position - transform.position;
            float orientation = Utils.VectorToOrientation(directionToTarget);

            if (hasRigidbody)
            {
                rigidbody.rotation = orientation;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, orientation);
            }

        }


    }
}
