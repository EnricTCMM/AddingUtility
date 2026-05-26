using FSMs;
using MotorBehaviours;
using UnityEngine;
using AICourse.Utilities;

[CreateAssetMenu(fileName = "FSM_BatcatFeed", 
                 menuName = "Finite State Machines/FSM_BatcatFeed", order = 1)]

public class FSM_BatcatFeed : FiniteStateMachine
{

    private BATCAT_Blackboard blackboard; // the blackboard (info depot)

    private GameObject trashcan; // the trashcan being approached or rummaged
    private GameObject sardine; // the sardine being transported or eaten
    private SB_Wander wanderAround; // steering
    private SB_Arrive arrive; // steering
    private float elapsedTime; // time elapsed in EATING or RUMMAGING states


    public override void OnEnter()
    {
        // get the blackboard
        blackboard = GetComponent<BATCAT_Blackboard>();

        // get the motor behaviours (they should be off -disabled-)
        wanderAround = GetComponent<SB_Wander>();
        arrive = GetComponent<SB_Arrive>();
        
        base.OnEnter();
    }

    public override void OnExit()
    {
        // turn off motor behaviours that this machine may have turned on
        wanderAround.enabled = false;
        arrive.enabled = false;
        base.OnExit();
    }

    public override void OnConstruction()
    {
        // STAGE 1: create the states with their logic(s)

        State WANDERING = new State("WANDERING",
            () => { wanderAround.Enable(); },
            () => { /* do nothing in particular */ },
            () => { wanderAround.Disable(); }
        );

        
        State REACHING_CAN = new State("REACHING TRASH CAN",
            () => { arrive.target = trashcan; arrive.Enable();  },
            () => {/* do nothing in particular */ },
            () => { arrive.Disable();  }
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
            () => { arrive.target = blackboard.hideout; arrive.Enable(); },
            () => {/* do nothing in particular */ },
            () => { arrive.Disable();  }
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
