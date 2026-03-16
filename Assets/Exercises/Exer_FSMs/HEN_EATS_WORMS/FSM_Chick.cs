using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Chick", menuName = "Finite State Machines/FSM_Chick", order = 1)]
public class FSM_Chick : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private WanderAround wanderAround;
    private Flee flee;
    private SteeringContext steeringContext;
    private HEN_Blackboard blackboard;
    public GameObject theHen;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        wanderAround = GetComponent<WanderAround>();
        theHen = GameObject.FindGameObjectWithTag("HEN");
        flee = GetComponent<Flee>();
        steeringContext = GetComponent<SteeringContext>();
        blackboard = theHen.GetComponent<HEN_Blackboard>();

        wanderAround.attractor = theHen;
        steeringContext.seekWeight = 0.4f;
        flee.target = theHen;
        base.DisableAllSteerings();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s) */
         

        State wanderState = new State("WANDER",
            () => { wanderAround.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { wanderAround.enabled = false; }  // write on exit logic inisde {}  
        );

        State fleeState = new State("FLEE",
            () => {
                GetComponent<AudioSource>().Play();
                steeringContext.maxAcceleration *= 3;
                steeringContext.maxSpeed *= 7;
                flee.enabled = true;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => {
                steeringContext.maxAcceleration /= 3;
                steeringContext.maxSpeed /= 7;
                flee.enabled = false;
            }  // write on exit logic inisde {}  
        );


        /* STAGE 2: create the transitions with their logic(s) */
   
        Transition henTooClose = new Transition("HEN TOO CLOSE",
            () => { return SensingUtils.DistanceToTarget(gameObject, theHen) 
                                        <= blackboard.chickDetectionRadius / 3; }
        );

        Transition henFarAway = new Transition("HEN FAR AWAY",
            () => { return SensingUtils.DistanceToTarget(gameObject, theHen) >= blackboard.chickFarEnoughRadius * 1.5; } 
        );

        /* STAGE 3: add states and transitions to the FSM */

        AddStates(wanderState, fleeState);
        AddTransition(wanderState, henTooClose, fleeState);
        AddTransition(fleeState, henFarAway, wanderState);

        /* STAGE 4: set the initial state */
         
        initialState = wanderState;

    }
}
