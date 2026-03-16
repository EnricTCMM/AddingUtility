using System;
using System.Collections.Generic;
using UnityEngine;

namespace BTs
{
    class DynamicSelector : Selector
    {
        private List<Condition> conditions = new List<Condition>();

        public DynamicSelector() : base() {}
        public DynamicSelector(String name) : base(name) { }

        // dynamic selector overrides add since childs are not BTs but pairs (condition+BT)
        public override void AddChild(INode child)
        {
            throw new NotImplementedException("One-parameter AddChild in DynamicSelector cannot be used. Use two-parameter AddChild");
        }

        // same for AddChildren
        public override void AddChildren(params INode[] children)
        {
            throw new NotImplementedException("AddChildren in DynamicSelector cannot be used. Use two-parameter AddChild");
        }

        // this one is specific for DynamicSelectors (although first declared in Node)
        public override void AddChild(Condition condition, INode child)
        {
            conditions.Add(condition);
            base.AddChild(child);
        }
        


        public override Status OnTick()
        {
            if (status == Status.FAILED || status == Status.SUCCEEDED)
                throw new Exception("Dynamic Selector ticked in " + status.ToString() + " status");

            // remember that if OnTick is invoked the node has been initialized since only Tick() calls OnTick() 
            // and Tick initializes any non-initialized node

            // check conditions in an orderly way
            for (int child = 0; child<children.Count; child++)
            {
                if (conditions[child].Tick()==Status.SUCCEEDED) //if (conditions[child].Check())
                    // use Tick (instead) of check to guarantee proper initialization of conditions 
                {

                    // same child, do nothing special; diferent child, abort current 
               
                    if (child!=currentChild)
                    {
                        children[currentChild].Abort();
                        // in a sense, not only the child must be aborted. Also the condition
                        // since some conditions have memory. In this stage conditions should be marked
                        // not initialized (= Cleared)
                        conditions[currentChild].Clear();
                        currentChild = child;
                    }

                    // now tick the child. If it succeeds, succeed; if it fails, fail; if it runs, run
                    status = children[currentChild].Tick();
                    return status;
                }
            }

            // if this point is reached, no condition was found to be true. Selector fails...
            status = Status.FAILED;
            return status;
        }


        // when clearing a dynamic selector also clear the conditions...
        public override void Clear()
        {
            base.Clear(); 
            foreach (var conditions in conditions) conditions.Clear();
        }


        // when setting the gameObject for a priority selector, push it down the conditions also
        public override void Contextualize(GameObject gm)
        {
            base.Contextualize(gm);
            foreach (Condition con in conditions)
            {
                // Conditions always require a gameobject
                con.Contextualize(gm);
            }
        }

        // TODO TOTHINK: when aborting a Priority Selector, governing conditions should be "cleared" since they may have "permanent" 
        // information (e.g. one shot conditions...)
        // continous detection vs one shot detection...

    }
}
