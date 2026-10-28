using FSMs;
using UnityEngine;
using MotorBehaviours;

[CreateAssetMenu(fileName = "FSM_ZombieWander", menuName = "Finite State Machines/FSM_ZombieWander", order = 1)]
public class FSM_ZombieWander : FiniteStateMachine
{
    private ZOMBIE_BLACKBOARD blackboard;
    private AstarPathFeeder pathFeeder;
    private SB_PathFollowing pathFollowing;
    private GameObject currentWanderpoint;

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
        

        State wandering = new State("WANDERING",
            () => {
                currentWanderpoint = blackboard.GetRandomWanderPoint();
                pathFeeder.target = currentWanderpoint;
                pathFeeder.Enable();
                pathFollowing.Enable();
            }, 
            () => { },
            () => { /* could stop the pathfeeder but no need */}
        );


        Transition waypointReached = new Transition("Waypoint Reached",
            () => {
                return SensingUtils.DistanceToTarget(gameObject, currentWanderpoint) <= blackboard.pointReachedRadius;
            }
        );

        AddStates(wandering);
        AddTransition(wandering, waypointReached, wandering);

        initialState = wandering;

    }

}
