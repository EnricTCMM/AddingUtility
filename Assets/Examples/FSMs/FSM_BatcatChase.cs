using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_BatcatChase", menuName = "Finite State Machines/FSM_BatcatChase", order = 1)]
public class FSM_BatcatChase : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private GameObject mouse, otherMouse;
    private PursuePlusOA pursue;
    private ArrivePlusOA arrive;
    private BATCAT_Blackboard blackboard;
    private float pursuingTime;
    private float restingTime;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        pursue = GetComponent<PursuePlusOA>();
        arrive = GetComponent<ArrivePlusOA>();
        blackboard = GetComponent<BATCAT_Blackboard>();
        pursuingTime = 0;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/
         
       
        State HIDING = new State("HIDING",
            () => { }, 
            () => { }, 
            () => { }  
        );

        State PURSUING = new State("PURSUING",
            () => { pursuingTime = 0; 
                    pursue.target = mouse; 
                    pursue.enabled = true; 
            }, 
            () => { pursuingTime += Time.deltaTime; },
            () => { pursue.enabled = false; }  
        );

        State RESTING = new State("RESTING",
            () => { restingTime = 0; },
            () => { restingTime += Time.deltaTime; }, 
            () => { } 
        );

        State TRANSPORTING = new State("TRANSPORTING",
            () => { mouse.transform.parent = transform; mouse.tag = "TRAPPED_MOUSE"; 
                    arrive.target = blackboard.jail; arrive.enabled = true; }, 
            () => { }, 
            () => { mouse.transform.parent = null; arrive.enabled = false; }  
        );

        State RETURNING = new State("RETURNING",
            () => { arrive.target = blackboard.hideout; arrive.enabled = true; }, 
            () => { },
            () => { arrive.enabled = false; }  
        );

        /* STAGE 2: create the transitions with their logic(s)
         * --------------------------------------------------- */

        Transition mouseDetected = new Transition("Mouse Detected",
            () => {
                mouse = SensingUtils.FindInstanceWithinRadius(gameObject, "MOUSE", blackboard.mouseDetectableRadius);
                return mouse != null;
            }
        );

        
        Transition freshAgaing = new Transition("Fresh Again",
            () => { return restingTime >= blackboard.maxRestingTime; }
        );

      
        Transition jailReached = new Transition("Jail reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.jail) <= blackboard.placeReachedRadius; }
        );

        Transition hideoutReached = new Transition("Hideout reached",
           () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.hideout) <= blackboard.placeReachedRadius; }
        );

        Transition pursueTooLong = new Transition("Pursue too long",
            () => { return pursuingTime > blackboard.maxPursuingTime; }
        );

        Transition mouseCloser = new Transition("Mouse Closer",
            () => {
                otherMouse = SensingUtils.FindInstanceWithinRadius(gameObject, "MOUSE", 
                                                                   blackboard.mouseDetectableRadius);
                return
                    otherMouse != null &&
                    SensingUtils.DistanceToTarget(gameObject, otherMouse) 
                    < SensingUtils.DistanceToTarget(gameObject, mouse);
            },
            () => { mouse = otherMouse; }
        );

        Transition mouseEscaped = new Transition("Mouse escaped",
            () => { return SensingUtils.DistanceToTarget(gameObject, mouse) 
                           >= blackboard.mouseHasVanishedRadius; 
                  }
        );

        Transition mouseReached = new Transition("Mouse Reached",
           () => { return SensingUtils.DistanceToTarget(gameObject, mouse) 
                          <= blackboard.mouseReachedRadius; 
           }
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(HIDING, PURSUING, RESTING, TRANSPORTING, RETURNING);

        AddTransition(HIDING, mouseDetected, PURSUING);

        AddTransition(PURSUING, mouseReached, TRANSPORTING);
        AddTransition(PURSUING, mouseEscaped, RETURNING);
        AddTransition(PURSUING, pursueTooLong, RESTING);
        AddTransition(PURSUING, mouseCloser, PURSUING);

        AddTransition(RESTING, freshAgaing, RETURNING);

        AddTransition(TRANSPORTING, jailReached, RETURNING);

        AddTransition(RETURNING, mouseDetected, PURSUING);
        AddTransition(RETURNING, hideoutReached, HIDING);


        /* STAGE 4: set the initial state */

        initialState = RETURNING;
    }
}
