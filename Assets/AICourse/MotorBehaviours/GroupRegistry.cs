/*
 * Component: GroupRegistry
 * Author: Dr. Enric Sesa i Nogueras
 * Notice: This code contains generative AI contributions.
 * Description: 
 * Centralized registry to keep track of a group of agents (boids).
 * Replaces expensive FindGameObjectsWithTag calls by providing a 
 * maintained list of active members.
 */

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
    }
}