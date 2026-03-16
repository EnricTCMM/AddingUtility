using System;
using System.Collections.Generic;
using UnityEngine;

// All code by ESN

namespace BTs
{
    public abstract class InternalNode : Node
    {
        // Internal nodes are the nodes capable of having children. They are non-leave nodes
        protected List<INode> children = new List<INode>();
        protected int currentChild = 0;

        public InternalNode()  { }   
        public InternalNode(string name) : base(name) {}
        public InternalNode(string name, params INode[] tasks) : base (name)
        {
            foreach (INode t in tasks)
            {
                this.AddChild(t);
            }
        }

        // Experimental code
        public delegate void OnAbortDelegate(); // OnAbortDelegate is a function type corresponding to void parameterless method
        public OnAbortDelegate onAbortHandler = null;

        // override dummy implementation in Node
        public override void AddChild(INode child)
        {
            // overridable since decorators can only have one child
            children.Add(child);
        }

        // override dummy implemetation in Node
        public override void AddChildren(params INode[] children)
        {
            foreach (INode child in children)
                AddChild(child);
        }

        public override void OnAbort()
        {
            // what if an internal node is aborted? 
            // abort its currentChild
            children[currentChild].Abort();

            // Experimental code
            // and then invoke its abortion handler...
            onAbortHandler?.Invoke(); // Invoke but only if it's not null
        }

        public override void OnInitialize()
        {
            // not sealed since some decorators and subclasses need to do some specific set up...
            if (children.Count == 0)
                throw new Exception("Non-leaf node "+Name+" initialized with no children. Non-leaves are meant to have children");

            status = Status.RUNNING; // or should there be something like Status.READY?
            currentChild = 0;
            // also clear the first child.  REASON: make stuff reusable
            children[currentChild].Clear(); // next children will be cleared by subclasses
        }

        public override void Clear() {
            base.Clear();
            foreach (var child in children) child.Clear();
        }

        public override void Contextualize(GameObject gm)
        {
            base.Contextualize(gm);
            foreach (INode child in children)
            {
                child.Contextualize(gm);
                // if the child is a BT the contextualization will trigger the 
                // construction of the structure ("pushing" the gameObject down)
                // if it is not a BT i just pushes the gameObject down
            }
        }

        
    }

    

}
