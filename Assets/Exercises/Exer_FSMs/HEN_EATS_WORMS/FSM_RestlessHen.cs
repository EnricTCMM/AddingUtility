using FSMs;
using UnityEngine;
using MotorBehaviours;

[CreateAssetMenu(fileName = "FSM_RestlessHen", menuName = "Finite State Machines/FSM_RestlessHen", order = 1)]
public class FSM_RestlessHen : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private HEN_Blackboard blackboard;
    private MotorManager motorManager;
    private SB_Wander wanderAround;
    private float normalWeight;
    private Color normalColor;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        blackboard = GetComponent<HEN_Blackboard>();   
        motorManager = GetComponent<MotorManager>();
        wanderAround =  GetComponent<SB_Wander>();
        wanderAround.attractor = blackboard.attractor;
        
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        base.OnExit();
    }

    public override void OnConstruction()
    {
       
        FiniteStateMachine eatAlone = ScriptableObject.CreateInstance<FSM_DriveAway>();

        State gettingCloser = new State("GETTING CLOSER",
            () => { 
                SpriteRenderer spr = GetComponent<SpriteRenderer>();
                normalColor = spr.color;
                spr.color = blackboard.restlessColor;
                normalWeight = wanderAround.attractionWeight; 
                wanderAround.attractionWeight = 0.7f;
                wanderAround.Enable();
            },
            () => { },
            () => { wanderAround.attractionWeight = normalWeight; 
                wanderAround.Disable();
                GetComponent<SpriteRenderer>().color = normalColor;
            }
        );

        Transition tooFarFromAttractor = new Transition("Too Far From Attractor",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.attractor) >= blackboard.tooFarFromAttractor; }
        );

        Transition closeEnoughToAttractor = new Transition("Close Enough to Attractor",
           () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.attractor) < blackboard.closeEnoughToAttractor; }
        );


        AddStates(eatAlone, gettingCloser);
        AddTransition(eatAlone, tooFarFromAttractor, gettingCloser);
        AddTransition(gettingCloser,closeEnoughToAttractor, eatAlone);
        
        initialState = eatAlone;

    }
}
