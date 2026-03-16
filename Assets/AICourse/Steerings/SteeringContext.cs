
using UnityEngine;


namespace Steerings
{
    public class SteeringContext : MonoBehaviour
    {
        // Constraints of linear movement
        [Foldout("Linear Contraints", styled=true)]
        public float maxAcceleration = 40;
        public float maxSpeed = 10;
        public bool clipVelocity = true;

        // Constraints of angular movement
        [Foldout("Angular constraints", styled = true)]
        public float maxAngularAcceleration = 360;
        public float maxAngularSpeed = 90;
        public bool clipAngularSpeed = true;

        // Arrive related parameters
        [Foldout("Arrive related parameters", styled = true)]
        public float timeToDesiredSpeed = 0.1f;
        public float closeEnoughRadius = 1;
        public float slowdownRadius = 5;

        // Align related parameters
        [Foldout("Align related parameters", styled = true)]
        public float timeToDesiredAngularSpeed = 0.1f;
        public float closeEnoughAngle = 2f;
        public float slowDownAngle = 10f;

        // Pursue & Evade related parameters
        [Foldout ("Pursue & Evade related parameters", styled = true)]
        public float maxPredictionTime = 3f;
        public bool showFutureTargetGizmos = false;

        // Group & flocking related parameters
        [Foldout("Group & flocking related parameters", styled = true)]
        public string idTag; // this tag DOES NOT tag the owner 
        public float repulsionThreshold = 15f;
        public float cohesionThreshold = 30f;
        public float coneOfVisionAngle = 270; 
        public bool applyVision = false;
        public float cohesionWeight = 0.3f;
        public float repulsionWeight = 0.5f;
        public float alignmentWeight = 0.2f;
        public bool addWanderIfZero=true;

        // wander related parameters
        [Foldout("Wander related parameters", styled = true)]
        public float wanderRate = 30f;
        public float wanderRadius = 10f;
        public float wanderOffset = 20f;
        public bool showWanderGizmos = false;
        // [HideInInspector]
        public float wanderTargetOrientation = 0f;
       
        // seek weight for "around" steerings
        [Foldout("Seek weight for \"around\" sterings", styled = true)]
        public float seekWeight = 0.2f;

        // Obstacle avoidance related parameters
        [Foldout("Obstacle avoidance related parameters", styled = true)]
        public float lookAheadLength = 10f;
        public float avoidDistance = 12f;  // avoid distance ALWAYS > look ahead
        public float secondaryWhiskerAngle = 30f;
        public float secondaryWhiskerRatio = 0.7f;
        public float perseveranceTime = 0f;  // experimental
        public bool showAvoidanceGizmos = false;
        // Experimental "perseverance" [Inertia durig OA] parameters
        [HideInInspector]
        public float perseveranceElapsed = 0;
        [HideInInspector]
        public bool persevering = false;
        [HideInInspector]
        public Vector3 avoidanceAcceleration = Vector3.zero; // cached acceleration

        // velocity and speeds
        [Foldout("velocity & speeds", styled = true, readOnly =true)]
        public Vector3 velocity = Vector3.zero;
        public float speed;
        public float angularSpeed;

        // group manager for group related stuff
        [Foldout("group manager for group related stuff", styled = true)]
        public GroupManager groupManager;

        public void Awake()
        {
            if (groupManager != null) groupManager.members.Add(this.gameObject); 
            // if done in Start, it's too late man!
            // groupManager.members must ensure no duplicates allowed

            // notice that this is done at awake time. Hence, only gameobjects existing at start-up time
            // will be taken into account. Gameobjects created later (e.g. by a spawner) or without a groupManager at 
            // start-up time should be added to the members set "manually" (by the spawner, for instance)

            // it could be a good practice to make spawners and the like inherit from group managers
            // See how boids are spawned in flocking sceenes (spawner is also a GroupManager)
        }

        public void Update()
        {
            speed = velocity.magnitude;
        }

        // no getters/setters provided.
        // Use Reflection to access public fields. It's much more fun...
    }
}
