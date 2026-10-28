using FSMs;
using UnityEngine;
using MotorBehaviours;

[CreateAssetMenu(fileName = "FSM_Prince", menuName = "Finite State Machines/FSM_Prince", order = 1)]
public class FSM_Prince : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private ROYAL_Blackboard blackboard;
    private AstarPathFeeder pathFeeder;
    private SB_PathFollowing pathFollowing;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter actionName of any state 
         * Usually this code includes .GetComponent<...> invocations */

        blackboard = GetComponent<ROYAL_Blackboard>();
        pathFeeder = GetComponent<AstarPathFeeder>();
        pathFollowing = GetComponent<SB_PathFollowing>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit actionName of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        pathFollowing.Disable();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        //++++++-----------------------
        // això és el que cal eliminar de cara als estudiants. La resta no cal
        //+++++------------------------
        
        State goingToPrincess = new State("GOING TO PRINCESS",
            () => {
                pathFeeder.target = blackboard.partner;
                pathFeeder.Enable();
                pathFollowing.Enable();
            }, 
            () => { }, 
            () => {
                pathFeeder.Disable();
                pathFollowing.Disable();
            }    
        );

        State goingToExit = new State("GOING TO EXIT",
            () => {
                pathFeeder.target = blackboard.exit;
                pathFeeder.Enable();
                pathFollowing.Enable();
            }, 
            () => { }, 
            () => {
                pathFeeder.Disable();
                pathFollowing.Disable();
            }   
        );


        Transition princessReached = new Transition("Princess Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.partner) < 20; }
        );

           
        AddStates(goingToPrincess, goingToExit);

        AddTransition(goingToPrincess, princessReached, goingToExit);

        initialState = goingToPrincess;
        
        
        // Això és el que es deixa... 
        /* COMPLETE */

        /* STAGE 1: create the states with their logic(s)
          *-----------------------------------------------

         State varName = new State("StateName",
             () => { }, // write on enter logic inside {}
             () => { }, // write in state logic inside {}
             () => { }  // write on exit logic inisde {}
         );

          */


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger actionName needed
        );

        */


        /* STAGE 3: add states and transitions to the FSM
         * ----------------------------------------------

        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */


        /* STAGE 4: set the initial state

        initialState = ...

         */
        
        
    }
}
