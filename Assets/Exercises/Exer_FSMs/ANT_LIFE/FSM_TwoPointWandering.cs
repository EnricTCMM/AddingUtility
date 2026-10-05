using FSMs;
using MotorBehaviours;
using UnityEngine;


[CreateAssetMenu(fileName = "FSM_TwoPointWandering", menuName = "Finite State Machines/FSM_TwoPointWandering", order = 1)]
public class FSM_TwoPointWandering : FiniteStateMachine
{


    private SB_Wander wanderAround;
    private ANT_Blackboard blackboard;

    private float elapsedTime = 0;


    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is executed every time the FSM is entered.
         * It's equivalent to the on enter actionName of any state 
         * Usually this code includes .GetComponent<...> invocations */

        blackboard = GetComponent<ANT_Blackboard>();
        wanderAround = GetComponent<SB_Wander>();
        wanderAround.attractionWeight = blackboard.initialAttractorWeight;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit actionName of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        wanderAround.Disable();

        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         */

        State goingA = new State("Going_A",
            () =>
            {
                wanderAround.attractor = blackboard.locationA;
                wanderAround.Enable();
                elapsedTime = 0;
            },
           () => { elapsedTime += Time.deltaTime;}, 
           () => {wanderAround.Disable();}
       );

        State goingB = new State("Going_B",
            () =>
            {
                wanderAround.attractor = blackboard.locationB;
                wanderAround.Enable();
                elapsedTime = 0;
            },
            () => { elapsedTime += Time.deltaTime;}, 
            () => {wanderAround.Disable();}
       );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------
        */

        
        
        Transition locationAReached = new Transition("Location A Reached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, blackboard.locationA) < blackboard.locationReachedRadius;
            }, // write the condition checkeing code in {}
            () =>
            {
                wanderAround.attractionWeight = blackboard.initialAttractorWeight;
            }  // write the on trigger code in {} if any. Remove line if no on trigger actionName needed
        );
        
        Transition locationBReached = new Transition("Location B Reached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, blackboard.locationB) < blackboard.locationReachedRadius;
            }, // write the condition checkeing code in {}
            () =>
            {
                wanderAround.attractionWeight = blackboard.initialAttractorWeight;
            }  // write the on trigger code in {} if any. Remove line if no on trigger actionName needed
        );

        Transition timeOut = new Transition("TimeOut",
            () =>
            {
                return elapsedTime >= blackboard.intervalBetweenTimeOuts;
            }, // write the condition checkeing code in {}
            () =>
            {
                elapsedTime = 0;
                wanderAround.attractionWeight += blackboard.attractorWeightIncrement;
            }  // write the on trigger code in {} if any. Remove line if no on trigger actionName needed
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
         */

        AddStates(goingA, goingB);
        
        AddTransition(goingA, locationAReached, goingB);
        AddTransition(goingA, timeOut, goingA);
        AddTransition(goingB, locationBReached, goingA);
        AddTransition(goingB, timeOut, goingB);
        

        initialState = goingA;
    }
}
