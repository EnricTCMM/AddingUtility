using System;


namespace BTs
{
    class Selector : InternalNode
    {

        private static int instanceCounter = 0;

        public Selector() : base("SELECTOR-" + instanceCounter++) { }
        public Selector(string name) : base(name) { }
        public Selector(params INode[] tasks) : base("SELECTOR-" + instanceCounter++, tasks){}
        public Selector(string name, params INode[] tasks) : base(name, tasks) { }
       

        public override Status OnTick()
        {
            if (status == Status.FAILED || status == Status.SUCCEEDED)
                throw new Exception("Selector ticked in " + status.ToString() + " status");

            Status st = children[currentChild].Tick();

            switch (st)
            {
                case Status.RUNNING:
                    // status continues to be RUNNING
                    break;
                case Status.SUCCEEDED:
                    status = Status.SUCCEEDED;
                    break;
                case Status.FAILED:
                    if (currentChild == children.Count - 1)
                        status = Status.FAILED;  // when last child fails, selector fails
                    else
                    {
                        // move to the next child
                        currentChild++;
                        children[currentChild].Clear(); // clear to force initialization REASON: make stuff reusable
                        // status continues to be Running
                    }
                    break;
            }

            return status;
        }
    }
}
