using System.Collections.Generic;
using UnityEngine;

/*
 A group manager "centralizes" group-related  operations, such as
 - adding and removing boids from the group
 - setting weights, thresholds and cone of vision (flocking parameters)

For specific purposes, create subclasses.
 */

namespace Steerings
{
    public class GroupManager : MonoBehaviour
    {

        [Header("'Boids' belonging to the group")]
        public List<GameObject> members = new List<GameObject>();

        public void Awake()
        {
            AddInitialBoids();
        }

        // subclasses should consider the possibility of overriding
        // this method, especially if group must not or should not
        // add its children as members.
        public virtual void AddInitialBoids ()
        {
            // take all the objects parented by this GroupManager
            // (i.e. the gameObject containing this component) 
            // and make them members of the group

            foreach (Transform ts in transform)
                AddBoid(ts.gameObject);
            // notice that no filterning is done
        }

        public void AddBoid(GameObject boid)
        {
            members.Add(boid);
            boid.GetComponent<SteeringContext>().groupManager = this;
            // let the owner of the manager parent all boids (Experimental) 
            boid.transform.parent = transform;
           
        }

        public void RemoveBoid(GameObject boid)
        {
            members.Remove(boid);
            boid.transform.parent = null;
            boid.GetComponent<SteeringContext>().groupManager = null;
        }

        //---------------
        // CONVENIENCE SETTERS
        //---------------
        // In my examples (ESN) this setters are used by UI components (sliders, text fields) 
        // to change scene parameters
        // They were added for listeners and with listeners in mind. 
        // Subclasses should take into account how to set UI components initial values since
        // listeners try to get them from actual attributes (fields) using reflection
        // See how GroupManagerExample1 declares hidden attributes to get initial values
        // from a particular boid instance

        public void SetCohesionThreshold(float value)
        {
            foreach (GameObject go in members)
            {
                go.GetComponent<SteeringContext>().cohesionThreshold = value;
            }
        }

        public void SetRepulsionThreshold(float value)
        {
            foreach (GameObject go in members)
            {
                go.GetComponent<SteeringContext>().repulsionThreshold = value;
            }
        }

        public void SetConeOfVisionAngle(float value)
        {
            foreach (GameObject go in members)
            {
                go.GetComponent<SteeringContext>().coneOfVisionAngle = value;
            }
        }

        public void SetCohesionWeight (float value)
        {
            foreach (GameObject go in members)
            {
                go.GetComponent<SteeringContext>().cohesionWeight = value;
            }
        }

        public void SetRepulsionWeight(float value)
        {
            Debug.Log("SetRepulsion invoked");
            foreach (GameObject go in members)
            {
                go.GetComponent<SteeringContext>().repulsionWeight = value;
            }
        }

        public void SetAlignmentWeight(float value)
        {
            foreach (GameObject go in members)
            {
                go.GetComponent<SteeringContext>().alignmentWeight = value;
            }
        }

        public void SetMaxSpeed (float value)
        {
            foreach (GameObject go in members)
            {
                go.GetComponent<SteeringContext>().maxSpeed = value;
            }
        }

        public void SetMaxAcceleration(float value)
        {
            foreach (GameObject go in members)
            {
                go.GetComponent<SteeringContext>().maxAcceleration = value;
            }
        }

        public void SetSeekWeight(float value)
        {
            foreach (GameObject go in members)
            {
                go.GetComponent<SteeringContext>().seekWeight = value;
            }
        }
    }
}
