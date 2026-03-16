using System;
using UnityEngine;

namespace BTs
{
    // Succeeds if all children succeed
    // fails if any children fails

    public class ParallelAnd : Parallel
    {
        public ParallelAnd() : base() { }
        public ParallelAnd(string name) : base(name) { }

        public ParallelAnd(params INode[] tasks) : base()
        {
            foreach (INode t in tasks)
            {
                this.AddChild(t);
            }
        }
        public ParallelAnd(string name, params INode[] tasks) : base(name)
        {
            foreach (INode t in tasks)
            {
                this.AddChild(t);
            }
        }

        
        public override Status OnTick()
        {
            if (status == Status.FAILED || status == Status.SUCCEEDED)
                throw new Exception("ParallelAnd ticked in " + status.ToString() + " status");

            // Different base methods implement different policies
            return base.AndBased_OnTick();
        }
       
    }
}
