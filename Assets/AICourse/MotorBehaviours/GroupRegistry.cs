using System.Collections.Generic;
using UnityEngine;

namespace MotorBehaviours
{
    public class GroupRegistry : MonoBehaviour
    {
        [Header("Registry Roster")]
        [Tooltip("The list of boids registered in this group.")]
        public List<GameObject> members = new List<GameObject>();

        public void Awake()
        {
            AddInitialBoids();
        }

        // Subclasses should consider the possibility of overriding
        // this method, especially if the group must not or should not
        // add its children as members automatically.
        public virtual void AddInitialBoids()
        {
            // Take all the objects parented by this GroupRegistry
            // (i.e. the gameObject containing this component) 
            // and make them members of the group
            foreach (Transform child in transform)
            {
                AddBoid(child.gameObject);
            }
        }

        public void AddBoid(GameObject boid)
        {
            if (!members.Contains(boid))
            {
                members.Add(boid);
                
                // Let the registry parent the boid to keep the hierarchy clean
                boid.transform.parent = this.transform;
            }
        }

        public void RemoveBoid(GameObject boid)
        {
            if (members.Contains(boid))
            {
                members.Remove(boid);
                
                // Unparent if it was parented to us
                if (boid.transform.parent == this.transform)
                {
                    boid.transform.parent = null;
                }
            }
        }
        
        // Convenience method for Steering Behaviours to retrieve the roster
        public List<GameObject> GetMembers()
        {
            return members;
        }
        
        // ------------ GENERAL SETTERS
        // propagate to all members

        public void SetRepulsionThreshold(float value)
        {
            foreach (GameObject member in members)
            {
                SB_Separation separation = member.GetComponent<SB_Separation>();
                if (separation!=null) separation.repulsionThreshold=value;
                SB_Flocking flocking = member.GetComponent<SB_Flocking>();
                if (flocking!=null) flocking.separationThreshold=value;
            }
        }
        
        public void SetCohesionThreshold(float value)
        {
            foreach (GameObject member in members)
            {
                SB_Cohesion cohesion = member.GetComponent<SB_Cohesion>();
                if (cohesion!=null) cohesion.cohesionThreshold=value;
                SB_Flocking flocking = member.GetComponent<SB_Flocking>();
                if (flocking!=null) flocking.cohesionThreshold=value;
            }
        }

        public void SetMaxSpeed(float value)
        {
            foreach (GameObject member in members)
            {
                MotorManager mb = member.GetComponent<MotorManager>();
                if (mb != null) mb.maxSpeed = value;
            }
        }
        
        public void SetMaxForce(float value)
        {
            foreach (GameObject member in members)
            {
                MotorManager mb = member.GetComponent<MotorManager>();
                if (mb != null) mb.maxForce = value;
            }
        }

        public void SetConeOfVisionAngle(float value)
        {
            foreach (GameObject member in members)
            {
                SB_Flocking sb = member.GetComponent<SB_Flocking>();
                if (sb != null) sb.coneOfVisionAngle = value;
            } 
        }
        
        public void SetAlignmentThreshold(float value)
        {
            foreach (GameObject member in members)
            {
                SB_Flocking sb = member.GetComponent<SB_Flocking>();
                sb.alignmentThreshold = value;
            }
        }

        public void SetAttractionWeight(float val)
        {
            foreach (GameObject member in members )
            {
                SB_Flocking sb = member.GetComponent<SB_Flocking>();
                sb.attractionWeight = val;  
            }
        }
    }
}