using Utility;
using System;

namespace BTs
{
    // May 2026. NOT FINISHED YET. Implications of atomicity have to be carefully considered.
    // Especially in the context of dynamic selectors (and maybe other types of nodes)
    // remove the abstract qualifier when ready
    // The problem of atomicity has arisen in the context of Utility-based action selection
    // where it just involves high-level abortion denial. 
    
    public abstract class AtomicDecorator : Decorator
    {
        private UtilityExecutor executor;
        
        public override Status OnTick()
        {
            if (status == Status.FAILED || status == Status.SUCCEEDED)
                throw new Exception("Atomic decorator ticked in " + status.ToString() + " status");

            Status st = children[currentChild].Tick();
            if (st == Status.SUCCEEDED || st == Status.FAILED)
            {
                if (executor != null)
                {
                    // assert that atomicity is no longer required since the child has terminated
                }
            }

            return status; 
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            // get the UtilityExecutor, if any
            executor = GetComponent<UtilityExecutor>();
            if (executor != null)
            {
                // assert that an atomic operation is being performed
            }
        }

        public override void OnAbort()
        {
            // notice that atomic does not mean non-abortable. At this level.
            // It is for the executor to determine the precise meaning of atomic. 
            base.OnAbort();
            if (executor != null)
            {
                // assert that atomicity is no longer required
            }
        }
    }
}