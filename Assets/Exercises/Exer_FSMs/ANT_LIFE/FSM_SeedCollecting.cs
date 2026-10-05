using FSMs;
using UnityEngine;
using MotorBehaviours;


[CreateAssetMenu(fileName = "FSM_SeedCollecting", menuName = "Finite State Machines/FSM_SeedCollecting", order = 1)]
public class FSM_SeedCollecting : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private ANT_Blackboard blackboard;
    private SB_Arrive arrive;
    private GameObject theSeed;
    private bool transportingSeed = false;
    
    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        
        blackboard = GetComponent<ANT_Blackboard>();   
        arrive = GetComponent<SB_Arrive>(); 
        
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        
        arrive.Disable();
        if (transportingSeed)
        {
            transportingSeed = false;
            theSeed.tag = "SEED";
            theSeed.transform.parent = null;
        }
        
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

         */

        FiniteStateMachine wandering = ScriptableObject.CreateInstance<FSM_TwoPointWandering>();

        State goingToSeed = new State("GOING TO SEED",
            () =>
            {
                arrive.target = theSeed;
                arrive.Enable();
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { arrive.Disable();}  // write on exit logic inisde {}  
        );
        
        State transportingSeed = new State("TRANSPORTING SEED",
            () =>
            {
                theSeed.transform.parent = this.transform;
                theSeed.tag = "NO_SEED"; // retag so that other ants know this seed is mine
                this.transportingSeed = true;
                arrive.target = blackboard.nest;
                arrive.Enable();
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () =>
            {
                arrive.Disable();
                theSeed.transform.parent = null;
                this.transportingSeed = false;
                //theSeed.tag = "Untagged";  // no need since the seed tagged NO_SEED is no longer detectable
            }  // write on exit logic inisde {}  
        );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        
        Transition nearbySeedDetected = new Transition("Nearby seed detected",
            () =>
            {
                theSeed = SensingUtils.FindInstanceWithinRadius(gameObject, "SEED", blackboard.seedDectionRadius);
                return theSeed != null;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        
        Transition seedReached = new Transition("seed Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, theSeed) < blackboard.seedReachedRadius;}, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        
        Transition seedTaken = new Transition("seed Taken",
            () => { return theSeed.tag != "SEED";}, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        
        Transition nestReached = new Transition("nest Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.nest) < blackboard.nestReachedRadius;}, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM
         * ----------------------------------------------

        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        
        AddStates(wandering,goingToSeed, transportingSeed);
        AddTransition(wandering, nearbySeedDetected, goingToSeed);
        AddTransition(goingToSeed, seedTaken, wandering);
        AddTransition(goingToSeed, seedReached, transportingSeed);
        AddTransition(transportingSeed, nestReached, wandering);


        /* STAGE 4: set the initial state

        initialState = ...

         */
        
        initialState = wandering;

    }
}
