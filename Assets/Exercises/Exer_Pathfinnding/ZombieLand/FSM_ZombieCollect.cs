using FSMs;
using UnityEngine;
using MotorBehaviours;

[CreateAssetMenu(fileName = "FSM_ZombieCollect", menuName = "Finite State Machines/FSM_ZombieCollect", order = 1)]
public class FSM_ZombieCollect : FiniteStateMachine
{
     /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private ZOMBIE_BLACKBOARD blackboard;
    private AstarPathFeeder pathFeeder;
    private SB_PathFollowing pathFollowing;
    private GameObject theGuts;
    private GameObject depositPoint;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<ZOMBIE_BLACKBOARD>();
        pathFeeder = GetComponent<AstarPathFeeder>();
        pathFollowing = GetComponent<SB_PathFollowing>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        pathFeeder.Disable();
        pathFollowing.Disable();
        base.OnExit();
    }

    public override void OnConstruction()
    {

        IState wandering = ScriptableObject.CreateInstance<FSM_ZombieWander>();

        State reaching = new State("REACHING",
            () => {
                pathFeeder.target = theGuts;
                pathFeeder.Enable();
                pathFollowing.Enable();
            },
            () => { },
            () => {
                pathFeeder.Disable();
                pathFollowing.Disable();
            }  // write on exit logic inisde {}  
        );

        State transporting = new State("TRANSPORTING",
            () => {
                theGuts.transform.parent = gameObject.transform;
                depositPoint = blackboard.GetRandomDepositPoint();
                pathFeeder.target = depositPoint;
                pathFeeder.Enable();
                pathFollowing.Enable();
            }, 
            () => { }, 
            () => {
                theGuts.transform.parent = null;
                theGuts.tag = "Untagged";
                pathFeeder.Disable();
                pathFollowing.Disable();
            }   
        );

        Transition gutsSmelt = new Transition("Guts Smelt",
            () => {
                theGuts = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "FREE_GUTS", blackboard.gutDetectedRadius);
                return theGuts != null;
            }, 
            () => { }
        );

        Transition gutsReached = new Transition("Guts Reached",
            () => {
                return SensingUtils.DistanceToTarget(gameObject, theGuts) <= blackboard.gutReachedRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition gutsInPlace = new Transition("Guts in Place",
            () => {
                return SensingUtils.DistanceToTarget(gameObject, depositPoint) <= blackboard.pointReachedRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        AddStates(wandering,reaching, transporting);

        AddTransition(wandering, gutsSmelt, reaching);
        AddTransition(reaching, gutsReached, transporting);
        AddTransition(transporting, gutsInPlace, wandering);

        initialState = wandering;

    }

}
