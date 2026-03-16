using System;

namespace BTs
{
    public class RepeatForeverDecorator : Decorator
    {
        public RepeatForeverDecorator() : base() { }
        public RepeatForeverDecorator(INode child) : base(child) { }

        public override Status OnTick()
        {
            if (status == Status.FAILED || status == Status.SUCCEEDED)
                throw new Exception("Repeat decorator ticked in " + status.ToString() + " status");

            Status st = children[currentChild].Tick();
            if (st == Status.SUCCEEDED || st == Status.FAILED)
            {
                children[currentChild].Clear();
            }

            return status; // status never changes...
        }
    }
}
