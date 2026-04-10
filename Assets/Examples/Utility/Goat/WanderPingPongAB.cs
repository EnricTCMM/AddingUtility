using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "WanderPingPongAB", menuName = "Finite State Machines/WanderPingPongAB", order = 1)]
public class WanderPingPongAB : FiniteStateMachine
{
    public string keyLocationA = "locationA";
    public string keyLocationB = "locationB";
    private GameObject locationA, locationB;
    public float intervalBetweenTimeouts = 10f;
    public float locationReachedRadius = 5f;
    public float initialSeekWeight = 0.2f;
    public float seekIncrement = 0.1f; 
    
    private WanderAround wanderAround;
    private SteeringContext steeringContext;
    private DynamicBlackboard blackboard;
    
    private float elapsedTime = 0;
    
    public override void OnEnter()
    {
        wanderAround = GetComponent<WanderAround>(); // should be off
        steeringContext = GetComponent<SteeringContext>();
        blackboard = GetComponent<DynamicBlackboard>();
        
        locationA = blackboard.Get<GameObject>(keyLocationA);
        locationB = blackboard.Get<GameObject>(keyLocationB);   
        
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        
        State goingA = new State("Going_A",
           () => { elapsedTime = 0f; 
               wanderAround.attractor = locationA;
               wanderAround.enabled = true;
           },
           () => { elapsedTime += Time.deltaTime;}, 
           () => { wanderAround.enabled = false;}
       );

        State goingB = new State("Going_B",
           () => {
               elapsedTime = 0f;
               wanderAround.attractor = locationB;
               wanderAround.enabled = true;
           },
           () => { elapsedTime += Time.deltaTime; },
           () => { wanderAround.enabled = false; }
       );
        
        Transition locationAReached = new Transition("Location A reached",
            () => {
                return SensingUtils.DistanceToTarget(gameObject, locationA) < locationReachedRadius;
            },
            () => { steeringContext.seekWeight = initialSeekWeight;}
        );

        Transition locationBReached = new Transition("Location B reached",
            () => {
                return SensingUtils.DistanceToTarget(gameObject, locationB) < locationReachedRadius;
            },
            () => { steeringContext.seekWeight = initialSeekWeight; }
        );

        Transition timeOut = new Transition("TimeOut",
            () => { 
                bool to = elapsedTime >= intervalBetweenTimeouts;
                return to; },
            () => {
                float sk = Mathf.Min(1, steeringContext.seekWeight + seekIncrement);
                steeringContext.seekWeight = sk;
                elapsedTime = 0.0f;
            }
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
         */

        AddStates(goingA, goingB);

        AddTransition(goingA, locationAReached, goingB);
        AddTransition(goingB, locationBReached, goingA);
        AddTransition(goingA, timeOut, goingA);
        AddTransition(goingB, timeOut, goingB);

        /* STAGE 4: set the initial state */

        initialState = goingA;
    }
}
