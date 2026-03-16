using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BTs
{
    // a BehaviourTree is a Tickable (INode) ScriptableObject built around a root Node.
    // most of the Tickable (INode) services are provided through the root

    public abstract class BehaviourTree : ScriptableObject, INode
    {

        protected Node root; // value must be set in onConstruction
        public string Name { get; set; }

        // NOTA: aquest constructor l'utilitza "el sistema". Quan es fa la creació 
        // encara no es coneix el gameObject associat. Per aquesta raó no es pot
        // donar el gameObject a root.
        public BehaviourTree () : base ()
        {
            Name = GetType().ToString();
        }
        

        // Aquí és on es materialitza la construcció de l'estructura arbòria.
        // No es fa al constructor, es fa aquí.
        // Aquest mètode és invocat en dos llocs:
        // - a l'executor (BehaviourTreeExecutor)
        // - a InternalNode. Quan un internal node rep el seu gameObject el propaga vers els
        // seus fills.Si el fill és un BT, la contextualització també significarà la construcció
        // de l'estructura arbòria.
        public void Contextualize (GameObject gameObject)
        {
            this.gameObject = gameObject;
            OnConstruction();
            // quan es fa la construcció -OnConstruction()- és quan root pren valor
            root.Contextualize(gameObject);
        }

        
        // -------------------------
        // this is the one and only method that subclasses must implement.
        // -------------------------
        public abstract void OnConstruction();


        // ------------------------------------------
        // implementation of the INode interface
        // ------------------------------------------
        // Notice how INode services are "delegated" to root (Node)

        public Status Tick()
        {
            return root.Tick();
        }

        public void Abort()
        {
            root.Abort();
        }

        public void Clear()
        {
            root.Clear();
        }


        public Status GetStatus()
        {
            // this method should not be invoked on non-constructed BTs
            return root.GetStatus();
        }

        public bool IsTerminated()
        {
            return root.IsTerminated();
        }

        // "lambdas" benefit from the existence of a public gameObject accessible in
        // OnConstruction
        public GameObject gameObject { get; private set; }

        public T GetComponent<T>()
        {
            return gameObject.GetComponent<T>();
        }

        public T AddComponent<T>() where T : Component
        {
            return gameObject.AddComponent<T>();
        }

        // BEWARE: gameobjects should only have one Blackboard object for if they have more
        // the one returned may be "random". 
        // As a result of this, SteeringContext cannot extend from Blackboard 
        // In future releases this issue could be addressed (maybe merging all blackboards in a single
        // object?)
        public DynamicBlackboard blackboard { get => GetComponent<DynamicBlackboard>(); }

    }

}
