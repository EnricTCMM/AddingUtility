using System;

namespace BTs
{

    public abstract class Decorator : InternalNode
    {
        public Decorator() : base() { }
        public Decorator(String name) : base(name) { }
        public Decorator(INode child) : base()
        {
            AddChild(child);
        }
        public Decorator(String name, INode child) : base(name)
        {
            AddChild(child);
        }

        public override void AddChild(INode child)
        {
            if (this.children.Count == 1) throw new Exception("Adding a second child to a decorator is not allowed");
            base.AddChild(child);
        }


        public override void AddChildren(params INode[] children)
        {
            // or maybe allow it provided only one child is given...?
            throw new Exception("AddChildren not invokable on decorators since decorators only have one child");
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            // EXPERIMENTAL CODE. REASON: make stuff reusable
            children[currentChild].Clear();
        }
    }
}
