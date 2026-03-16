using System;

namespace BTs
{
    public class RepeatTimesDecorator : Decorator
    {
        public string keyTimes;

        public RepeatTimesDecorator(string times) : base()
        {
            keyTimes = times;
        }
        public RepeatTimesDecorator(string times, INode child) : base(child)
        {
            keyTimes = times;
        }

        private int done; // set to zero at initialization time.
        private int times;
        private DynamicBlackboard bl;

        public override void Initialize()
        {
            base.Initialize();
            done = 0;
            bl = GetComponent<DynamicBlackboard>();
            times = bl.Get<int>(keyTimes);

        }

        public override Status OnTick()
        {
            if (status == Status.FAILED || status == Status.SUCCEEDED)
                throw new Exception("Repeat decorator ticked in " + status.ToString() + " status");

            Status st;
            if (done < times)
            {
                st = children[currentChild].Tick();
                if (st == Status.FAILED) status = Status.FAILED;  // fail if child fails
                else if (st == Status.SUCCEEDED)
                {
                    done++; // count one successful termination
                    if (done == times) status = Status.SUCCEEDED;
                    else children[currentChild].Clear(); // get ready for the next iteration.
                }
            }
            else status = Status.SUCCEEDED; // succeed for repeat 0 or negative times

            return status;
        }
    }
}
