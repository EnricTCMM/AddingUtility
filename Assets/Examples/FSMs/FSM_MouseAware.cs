using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_MouseAware", menuName = "Finite State Machines/FSM_MouseAware", order = 1)]
public class FSM_MouseAware : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    private FleePlusOA flee;
    private MOUSE_Blackboard blackboard;
    private SteeringContext steeringContext;
    private GameObject peril;
    private float normalSpeed, normalAcceleration;
    private Color normalColor;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        flee = GetComponent<FleePlusOA>();
        blackboard = GetComponent<MOUSE_Blackboard>();
        steeringContext = GetComponent<SteeringContext>();

        normalSpeed = steeringContext.maxSpeed;
        normalAcceleration = steeringContext.maxAcceleration;
        normalColor = GetComponent<SpriteRenderer>().color;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        steeringContext.maxSpeed = normalSpeed;
        steeringContext.maxAcceleration = normalAcceleration;
        GetComponent<SpriteRenderer>().color = normalColor;

        DisableAllSteerings();

        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/

        FiniteStateMachine FEED = ScriptableObject.CreateInstance<FSM_MouseFeed>();
        FEED.Name = "FEED";

        State FLEE_PERIL = new State("FLEEING",
            () => { 
                // sometimes scared mice get an "extra boost" of speed. But not always
                if (Random.value>0.6f)
                {
                    float boostFactor = 1.3f + Random.value;
                    steeringContext.maxSpeed *= boostFactor;
                    steeringContext.maxAcceleration *= boostFactor;
                }
                GetComponent<SpriteRenderer>().color = new Color(3f/256, 120f/256, 7f/256);
                flee.target = peril;
                flee.enabled = true;
            }, 
            () => {/* do nothing in particular, just flee */ }, 
            () => {
                steeringContext.maxSpeed = normalSpeed;
                steeringContext.maxAcceleration = normalAcceleration;
                GetComponent<SpriteRenderer>().color = normalColor;
                flee.enabled = false;
            }    
        );


        /* STAGE 2: create the transitions with their logic(s)
         * --------------------------------------------------- */

        Transition perilDetected = new Transition("Peril detected",
            () => {
                peril = SensingUtils.FindInstanceWithinRadius(gameObject, "BATCAT", blackboard.perilDetectableRadius);
                return peril != null;
            }, // write the condition checkeing code in {}
            () => { }
        );

        Transition perilVanished = new Transition("Peril vanished",
            () => {
                return SensingUtils.DistanceToTarget(gameObject, peril) >= blackboard.perilSafetyRadius;
            }, // write the condition checkeing code in {}
            () => { }
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ---------------------------------------------- */
        

        AddStates(FEED, FLEE_PERIL);
        AddTransition(FEED, perilDetected, FLEE_PERIL);
        AddTransition(FLEE_PERIL, perilVanished, FEED);


        /* STAGE 4: set the initial state */

        initialState = FEED;
    }
}
