using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
A group context is like a steering context but intended for gameobjects belonging 
to the same group (boids). 
It encapsulates all the the flocking-related parameters (including repulsion and cohesion)
and keeps track of the actual members of the grup.

Notice that SteeringContext contains a reference to the GroupContext (so that
every boid "knows" it's group context) 

This script must be placed in a "dummy" (possibly invisible) gameobject that 
also contains a Group Manager.
 */

namespace Steerings
{
    public class GroupContext : MonoBehaviour
    {
        [Header("'Boids' belonging to the group")]
        public List<GameObject> members = new List<GameObject>();

        /*
        // Group related parameters (includes repulsion and cohesion) 
        [Header("Group & flocking related parameters")]
        public float coneOfVisionAngle = 270;
        public float repulsionThreshold = 15f;
        public float cohesionThreshold = 30f;
        public float cohesionWeight = 0.3f;
        public float repulsionWeight = 0.5f;
        public float alignmentWeight = 0.2f;
        public bool addWanderIfZero = true;
        public bool applyVisionToRepulsion = false;
        */
    }
}
