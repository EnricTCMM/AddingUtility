using System;

namespace BTs
{
    public abstract class Condition : Node
    {
        public Condition()
        {
            Name = GetType().ToString();
        }
        public Condition(string name) : base(name) { }

        // "users" should override this method
        public virtual bool Check()
        {
            return false;
        }

        // "users" can override this method
        public override void OnInitialize()
        {
        }

        public override sealed void OnAbort() { }  // Conditions cannot be aborted

        public override sealed void Initialize() {
            base.Initialize();
        } // Conditions do not override this, they override OnInitialize

        public override sealed Status OnTick()  // for conditions, do not override OnTick(); override Check() instead
        {
            if (Check()) return Status.SUCCEEDED;
            else return Status.FAILED;
        }

        
    }
}
