using Steerings;
using FSMs;
using UnityEngine;

[CreateAssetMenu(fileName = "FSM_BatcatFeed", 
                 menuName = "Finite State Machines/FSM_BatcatFeed", order = 1)]

public class FSM_BatcatFeed : FiniteStateMachine
{

    private BATCAT_Blackboard blackboard; // the blackboard (info depot)

    private GameObject trashcan; // the trashcan being approached or rummaged
    private GameObject sardine; // the sardine being transported or eaten
    private WanderAroundPlusAvoid wanderAround; // steering
    private ArrivePlusOA arrive; // steering
    private float elapsedTime; // time elapsed in EATING or RUMMAGING states


    public override void OnEnter()
    {
        // get the blackboard
        blackboard = GetComponent<BATCAT_Blackboard>();

        // Get the steerings (they should be off at enter time)
        wanderAround = GetComponent<WanderAroundPlusAvoid>();
        arrive = GetComponent<ArrivePlusOA>();

        base.OnEnter();
    }

    public override void OnExit()
    {
        // Turn off all steerings. That's all.
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        // STAGE 1: create the states with their logic(s)

        State WANDERING = new State("WANDERING",
            () => { wanderAround.enabled = true; },
            () => { /* do nothing in particular */ },
            () => { wanderAround.enabled = false; }
        );

        State REACHING_CAN = new State("REACHING TRASH CAN",
            () => { arrive.target = trashcan; arrive.enabled = true; },
            () => {/* do nothing in particular */ },
            () => { arrive.enabled = false; }
        );

        State RUMMAGING = new State("RUMMAGING",
            () => { elapsedTime = 0; },
            () => { elapsedTime += Time.deltaTime; },
            () => {
                // when exiting rummaging create a sardine and "hold" it
                sardine = Instantiate(blackboard.sardinePrefab);
                sardine.transform.parent = gameObject.transform;
                sardine.transform.position = gameObject.transform.position;
                sardine.transform.localRotation = Quaternion.Euler(0, 0, 
                                      gameObject.transform.rotation.z + 90);
            }
        );

        State REACHING_HIDEOUT = new State("REACHING HIDEOUT",
            () => { arrive.target = blackboard.hideout; arrive.enabled = true; },
            () => {/* do nothing in particular */ },
            () => { arrive.enabled = false; }
        );

        State EATING = new State("EATING",
            () => { elapsedTime = 0; },
            () => { elapsedTime += Time.deltaTime; },
            () => {
                // after eating, hunger decreases
                blackboard.hunger -= blackboard.sardineHungerDecrement;
                // Destroy the sardine
                Destroy(sardine);
                // create the fishbone
                GameObject fishbone = Instantiate(blackboard.fishbonePrefab);
                fishbone.transform.position = gameObject.transform.position;
                fishbone.transform.rotation = Quaternion.Euler(0, 0, 
                                                     180 * Utils.binomial());
            }
        );

        // STAGE 2: create the transitions with their logic(s)

        Transition trashcanDetected = new Transition( "Trashcan Detected",
            () => { trashcan = SensingUtils.FindInstanceWithinRadius(gameObject,"TRASH_CAN", 
                                                       blackboard.trashcanDetectableRadius); 
                    return trashcan!=null; },
            () => { }
        );

        Transition trashcanReached = new Transition( "Trashacan Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, trashcan) 
                                        < blackboard.placeReachedRadius; },
            () => { }
        );

        Transition foodFound = new Transition( "Food Found",
            () => { return elapsedTime >= blackboard.rummageTime; },
            () => { }
        );

        Transition hideoutReached = new Transition( "Hideout Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.hideout) 
                                        < blackboard.placeReachedRadius; },
            () => { }
        );

        Transition foodEaten = new Transition( "Food Eaten",
            () => { return elapsedTime >= blackboard.eatingTime; },
            () => { }
        );

        // STAGE 3: add states and transition to the FSM

        AddStates(WANDERING, REACHING_CAN, RUMMAGING, EATING, REACHING_HIDEOUT);

        AddTransition(WANDERING, trashcanDetected, REACHING_CAN);
        AddTransition(REACHING_CAN, trashcanReached, RUMMAGING);
        AddTransition(RUMMAGING, foodFound, REACHING_HIDEOUT);
        AddTransition(REACHING_HIDEOUT, hideoutReached, EATING);
        AddTransition(EATING, foodEaten, WANDERING);

        // STAGE 4: set the initial state

        initialState = WANDERING;
    }

}
