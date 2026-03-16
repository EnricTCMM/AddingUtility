using UnityEngine;

// all code by ESN
namespace FSMs
{
    // superclass for all "simple" States
    // this class is not meant to be subclassed. See comment above Construct method
    public sealed class State : IState
    {
        private static int id;  // used to give a number to anonymous states 

        public string Name { get; set; }

        public delegate void StateActionDelegate(); // StateActionDelegate is the type of the parameterless void functions

        // actions in a state are parameterless void functions
        private StateActionDelegate onEnter, inState, onExit; // three lambdas

        // Constructor (1). Takes three actions (lambdas)...
        public State( StateActionDelegate onEnter,
                      StateActionDelegate inState,
                      StateActionDelegate onExit)
        {
            id++;
            Name = "State_" + id;
            this.onEnter = onEnter;
            this.inState = inState;
            this.onExit = onExit;
        }

        // Constructor (2). Takes a name and  three actions (lambdas)...
        public State( string name,
                      StateActionDelegate onEnter,
                      StateActionDelegate inState,
                      StateActionDelegate onExit)
        {
            this.Name = name;
            this.onEnter = onEnter;
            this.inState = inState;
            this.onExit = onExit;
        }

        // ----------------------------------------------
        // --- implementation of the IState interface
        // ----------------------------------------------

        public void OnEnter()
        {
            // just invoke the lambda provided at constrution time
            // (OnEnter -uppercase O- invokes onEnter -lowercase o-)
            onEnter();  
        }

        public void InState()
        {
            // just invoke the lambda provided at constrution time
            inState();
        }

        public void OnExit()
        {
            // just invoke the lambda provided at constrution time
            onExit();
        }

        // dummy implementation. We do not keep the gameObject since 
        // actual instances will use the gameObject "enclosed" in the lambdas.
        // Nevertheless if there comes a time when this class needs to be subclassed with subclasses
        // providing alternative implementations for OnXXX and InXXX methods, then the gameObject (gm)
        // must be kept
        public void Construct(GameObject gm) { }
    }
}
