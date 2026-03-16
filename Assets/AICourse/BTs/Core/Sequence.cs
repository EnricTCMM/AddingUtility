using System;

// All code by ESN

namespace BTs
{
    class Sequence : InternalNode
    {

        private static int instanceCounter = 0;

        public Sequence() : base("SEQUENCE-"+instanceCounter++) { }
        public Sequence(string name) : base(name) { } 

        public Sequence(params INode [] tasks) : base("SEQUENCE-" + instanceCounter++, tasks) {}
        public Sequence(string name, params INode[] tasks) : base(name, tasks) {}

        public override Status OnTick()
        {
            if (status == Status.FAILED || status == Status.SUCCEEDED)
            {
                throw new Exception("Sequence " + Name + " ticked in " + status.ToString() + " status");
                //Clear();
            }

            Status st = children[currentChild].Tick();

            switch (st)
            {
                case Status.RUNNING:
                    // status continues to be RUNNING
                    break;
                case Status.SUCCEEDED:
                    // child succeeded. If it was the last succeeed, else move to the next one
                    if (currentChild==children.Count-1)
                    {
                        status = Status.SUCCEEDED;
                    }
                    else
                    {
                        // move to the next child
                        currentChild++;
                        children[currentChild].Clear(); // clear to force initialization REASON: make stuff reusable
                        // status continues to be RUNNING
                    }
                    break;
                case Status.FAILED:
                   status = Status.FAILED;
                   break;
            }

            return status;
        }
    }
}
