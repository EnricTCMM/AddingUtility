using System;

namespace BTs
{
    public abstract class Action : Node
    {
        
        public Action() {
            Name = GetType().ToString();
        }
        public Action(string name) : base(name) { }

    }
}
