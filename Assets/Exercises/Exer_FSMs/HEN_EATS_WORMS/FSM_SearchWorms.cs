using FSMs;
using UnityEngine;
using MotorBehaviours;

[CreateAssetMenu(fileName = "FSM_SearchWorms", menuName = "Finite State Machines/FSM_SearchWorms", order = 1)]
public class FSM_SearchWorms : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private HEN_Blackboard blackboard;
    private SB_Wander wanderAround;
    private SB_Arrive arrive;
    private AudioSource audioSource;
    private GameObject theWorm;
    private float elapsedTime;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter actionName of any state 
         * Usually this code includes .GetComponent<...> invocations */

        /* COMPLETE */
        blackboard = GetComponent<HEN_Blackboard>();
        wanderAround = GetComponent<SB_Wander>();
        arrive = GetComponent<SB_Arrive>();
        audioSource = GetComponent<AudioSource>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit actionName of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
       
        /* COMPLETE */
        Debug.Log("FSM_SearchWorms exiting !!!");
        audioSource.Stop();
        wanderAround.Disable();
        arrive.Disable();
        
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* COMPLETE */
        
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

         */


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger actionName needed
        );

        */


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        
        /* STAGE 1: create the states with their logic(s) */
         
        State wander = new State("Wander",
            () => {
                audioSource.clip = blackboard.cluckingSound;
                audioSource.Play();
                wanderAround.enabled = true;
            }, 
            () => { }, 
            () => {
                audioSource.Stop();
                wanderAround.enabled = false; }
        );

        State reachWorm = new State("Reach Worm",
            () => { arrive.target = theWorm; arrive.enabled = true; }, 
            () => { }, 
            () => { arrive.enabled = false; }  
        );

        State eating = new State("Eating",
            () => {
                audioSource.clip = blackboard.eatingSound;
                audioSource.Play();
                elapsedTime = 0f; 
            }, // write on enter logic inside {}
            () => { elapsedTime += Time.deltaTime; }, // write in state logic inside {}
            () => {
                audioSource.Stop();
                Destroy(theWorm); 
            }  
        );

        /* STAGE 2: create the transitions with their logic(s) */

        Transition wormDetected = new Transition("Worm Detected",
            () => { 
                theWorm = SensingUtils.FindInstanceWithinRadius(gameObject, "WORM", blackboard.wormDetectableRadius);
                return theWorm != null;
            }
        );

        Transition wormVanished = new Transition("Worm Vanished",
            () => { return theWorm == null || theWorm.Equals(null); }
        );

        Transition wormReached = new Transition("Worm Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, theWorm) < blackboard.wormReachedRadius; }
        );

        Transition timeout = new Transition("Timeout",
            () => { return elapsedTime >= blackboard.timeToEatWorm; }
        );

        /* STAGE 3: add states and transitions to the FSM  */
         
        AddStates(wander, reachWorm, eating);
        AddTransition(wander, wormDetected, reachWorm);
        AddTransition(reachWorm, wormVanished, wander);
        AddTransition(reachWorm, wormReached, eating);
        AddTransition(eating, timeout, wander);

        /* STAGE 4: set the initial state */
        initialState = wander;
        
        
    }
}
