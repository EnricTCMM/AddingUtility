using System;
using System.Collections.Generic;
using UnityEngine;

// All code by ESN 

namespace FSMs
{
    // superclass for all FSMs 
    // The IState interface is implemented in order to allow FSMs to be states of other FSMs
    public abstract class FiniteStateMachine : ScriptableObject, IState
    {
        // All the states in this FSM
        private List<IState> states;

        // All transitions are kept in a dictionary that maps states with their outgoing transitions (+ incoming state)
        private Dictionary<IState, List<TranstionDestinationPair>> transitions;

        public IState currentState;
        protected IState initialState;  // this must be set at construction (OnConstruction) time

        // a FiniteStateMachine recives a gameObject from the executor or from its parent "fsm"
        // this is the gameObject whose behaviour this fsm controls.
        protected GameObject gameObject;

        // no constructor provided. Being a scriptable object, creation must be done through
        // the ScriptableObject.CreateInstance<>() method.

        // -------------------------

        protected void AddState(IState state) {
            if (states.Contains(state)) throw new Exception("Repeated state in AddState(s) of a FSM ("+Name+"). Repeated state: "+state.Name);

            states.Add(state);
            transitions.Add(state, new List<TranstionDestinationPair>());  // pair the state with an empty list of transitions
                                                                           // transitions will come later

            // Efectively construct the state
            state.Construct(gameObject);
        }

        // Add several states in a single operartion (notice variable number of arguments)
        protected void AddStates (params IState[] states)
        {
            foreach (IState s in states)
                AddState(s);
        }
        
        // add states before adding transitions.
        protected void AddTransition(IState from, Transition transition, IState to ) {
            if (!states.Contains(from)) throw new Exception("Unknown source state in AddTransition of a FSM (" + Name + "). State: " + from.Name);
            if (!states.Contains(to)) throw new Exception("Unknown destination state in AddTransition of a FSM (" + Name + "). State: " + to.Name);
            foreach (TranstionDestinationPair pair in transitions.GetValueOrDefault(from))
            {
                if (pair.transition.Equals(transition))
                    throw new Exception("Repeated transition in AddTransition of FSM (" + Name + "). Transition: "+transition.Name);
            }
            // NOTICE: repetition of same transition with different source state is not checked... 
            // ... HENCE: same transition can be (re) used with a different source state
            TranstionDestinationPair transtionDestinationPair = new TranstionDestinationPair(transition, to);
            transitions.GetValueOrDefault(from).Add(transtionDestinationPair);
        }

        public void Update()
        {
            // get all transitions from currentState
            List<TranstionDestinationPair> currentTransitions = this.transitions.GetValueOrDefault(currentState);

            // check if a transtion triggers
            if (currentTransitions.Count != 0)
            {
                foreach (TranstionDestinationPair pair in currentTransitions)
                {
                    if (pair.transition.Check())
                    {
                        // execute the ON TRIGGER logic
                        pair.transition.OnTrigger();
                        // Exit current state, change state, enter new state
                        currentState.OnExit();
                        currentState = pair.destination;
                        currentState.OnEnter();
                        break; // Exit the foreach loop. Do not process more transitions
                    }
                }
            }

            // every update a InState action is executed
            currentState.InState();
        }

        // this is the only method fsms must implement
        public abstract void OnConstruction();

        // ------------------------------
        // ... Implementation of the IState iterface
        // ... Implementing this interface any FSM can be a state of any other FSM
        // ------------------------------

        // FSMs must redefine this method if they have particular OnEnter logic
        // BEWARE redefinitions MUST call base.super
        public virtual void OnEnter() {
            currentState = initialState;
            currentState.OnEnter();
        }

        // FSMs must redefine this method if they have particular OnExit logic
        public virtual void OnExit() { }

        public virtual void InState()
        {
            this.Update();
        }


        public void Construct(GameObject gm)
        {
            this.gameObject = gm;
            states = new List<IState>();
            transitions = new Dictionary<IState, List<TranstionDestinationPair>>();
            transitions.Add(TERMINATED, new List<TranstionDestinationPair>());
            OnConstruction();
        }

        private string _name = null;
        public string Name
        {
            get => _name==null?this.GetType().Name + "." + currentState.Name:_name+"."+currentState.Name;
            set => _name = value;
        } 

        // --- implementation of IState ends here

        // ------------------------------------------
        // GameObject and Transform related stuff 
        // (makes things more Unity-like)
        // ------------------------------------------
        public T GetComponent<T>()
        {
            return gameObject.GetComponent<T>();
        }

        public T AddComponent<T>() where T : Component
        {
            return gameObject.AddComponent<T>();
        }
        protected Transform transform
        {
            get { return gameObject.transform; }
        }

        // ------------------------------------------
        // --- Other stuff
        // ------------------------------------------

        // Pairing of a transition with its destination state
        private struct TranstionDestinationPair
        {
            public Transition transition;
            public IState destination;

            public TranstionDestinationPair(Transition transition, IState destination)
            {
                this.destination = destination;
                this.transition = transition;
            }
        }

        // a convenience "pit" state shared by all FSMs
        protected static State TERMINATED;

        // static constructor (= static initializer) just used to initialize TERMINATED
        static FiniteStateMachine ()
        {
            TERMINATED = new State("TERMINATED", 
                                   () => { /* Debug.Log("terminated"); */ }, 
                                   () => { }, 
                                   () => { });
        }

        // another convenience method 
        protected void DisableAllSteerings ()
        {
            if (gameObject!=null)
            {
                Steerings.SteeringBehaviour[] allSteerings = gameObject.GetComponents<Steerings.SteeringBehaviour>();
                foreach (Steerings.SteeringBehaviour steering in allSteerings)
                    steering.enabled = false;
            }

        }
    }
}
