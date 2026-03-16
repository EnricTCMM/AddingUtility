using System;
using UnityEngine;

namespace BTs
{
    // superclass of all nodes in a BT that are not BTs themselves
    // (BTs and Nodes share the same interface: INode)
    public abstract class Node : INode
    {
        protected Status status = Status.LIMBO;
        protected bool initialized = false;

        public string Name { get; protected set; }

        public Node() { }
        public Node(string name) { Name = name; }

        public Status GetStatus() { return this.status; }


        // ---- The Tick, OnTick pair
        public Status Tick()
        {
            if (!initialized)
                Initialize();

            status = OnTick();
            return status;
        }

        //  subclasses redefine this method in order to effectively decide what 
        // must be done when ticked
        public abstract Status OnTick();
        //-------------------------------------------------------

        // ---- the Abort, Clear, OnAbort triad 
        public void Abort()
        {
            if (initialized)
            {
                // if is necessary because non initialized tasks may have non-initialized fields 
                // that OnAbort may try to access...
                OnAbort();
                Clear();
            }
        }

        // clear makes Nodes non-initialized again
        public virtual void Clear()
        {
            status = Status.LIMBO;
            initialized = false;
        }

        // subclasses redefine this method in order to effectively decide
        // what must be done on abortion
        public virtual void OnAbort() { }
        // ---------------------------------------------------------


        //---- The Initialize, OnInitialize pair
        public virtual void Initialize()
        {
            status = Status.RUNNING;
            OnInitialize();
            initialized = true;
        }

        // subclasses redefine this method in order to effectively decide
        // what must be done on initialization
        public virtual void OnInitialize() { }

        

        public bool IsTerminated()
        {
            return status == Status.SUCCEEDED || status == Status.FAILED;
        }



        public bool IsInitialized()
        {
            return initialized;
        }

        //-----------------------------------------------------------------------
        // some useful virtual non-implemented methods
        //-----------------------------------------------------------------------
        // this virtual non-implemented methods exist for the sole benefit of attribute
        // root in class BehaviourTree. With them "idioms" like root.AddChild are possible
        // Actually the "child" concept does not make sense for all nodes. Actions and
        // Conditions are also nodes and they're always childless.
        public virtual void AddChild(INode child) { throw new NotImplementedException(); }
        public virtual void AddChildren(params INode[] children) { throw new NotImplementedException(); }
        // Adds a condition/ITickable pair (only for dynamic selectors)
        public virtual void AddChild(Condition condition, INode child) { throw new NotImplementedException(); }


        //-----------------------------------------------------------------------------
        // Unity related section (all other code should be Unity-independent)
        // ----------------------------------------------------------------------------


        // a node gets contextualized when it has received the gameObject it is going to work upon
        public virtual void Contextualize(GameObject gm)
        {
            gameObject = gm;
        }

        public GameObject gameObject { get; private set; }

        public T GetComponent<T>()
        {
            return gameObject.GetComponent<T>();
        }

        public T AddComponent<T>() where T:Component
        {
            return gameObject.AddComponent<T>();
        }

        // the blackboard that materializes the context of the gameobject 
        // and hence the context where the Node is ticked
        public DynamicBlackboard blackboard
        {
            get { return gameObject.GetComponent<DynamicBlackboard>(); }
        }

    }
}
