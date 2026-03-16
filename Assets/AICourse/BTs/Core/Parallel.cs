using System;
using UnityEngine;

namespace BTs
{

    // All code by ESN
    public abstract class Parallel : InternalNode
    {

        public Parallel() : base() { }
        public Parallel(string name) : base(name) { }

        public Parallel(params INode[] tasks) : base()
        {
            foreach (INode t in tasks)
            {
                this.AddChild(t);
            }
        }
        public Parallel(string name, params INode[] tasks) : base(name)
        {
            foreach (INode t in tasks)
            {
                this.AddChild(t);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            // initialize (Clear) all children. REASON: make stuff reusable
            foreach (var child in children)
                child.Clear();
        }

        
        protected  Status AndBased_OnTick()
        {
            // succeed when all children succeed (= fail if any fails)
            int succeeded, failed;
            TickAllActive(out succeeded, out failed);

            if (failed > 0) { status = Status.FAILED; }
            else if (succeeded == children.Count) { status = Status.SUCCEEDED;}
            else status = Status.RUNNING;

            return status;
        }

        
        protected Status OrBased_OnTick()
        {
            // succeed when any child succeeds (= fail if all fail)
            int succeeded, failed;
            TickAllActive(out succeeded, out failed);

            if (succeeded > 0) { status = Status.SUCCEEDED; }
            else if (failed == children.Count) { status = Status.FAILED; }
            else status = Status.RUNNING;

            return status;
        }


        private void TickAllActive (out int succeeded, out int failed)
        {
            succeeded = 0;
            failed = 0;
            // tick all children nor succeeeded neither failed and count how many of them 
            // succeed or fail...
            foreach (INode child in children)
            {
                if (child.GetStatus() == Status.SUCCEEDED) succeeded++;
                else if (child.GetStatus() == Status.FAILED) failed++;
                else
                {
                    Status st = child.Tick();
                    if (st == Status.SUCCEEDED) succeeded++;
                    else if (st == Status.FAILED) failed++;
                }
            }
        }
    }
}
