using FSMs;
using UnityEngine;
using MotorBehaviours;

[CreateAssetMenu(fileName = "FSM_DriveAway", menuName = "Finite State Machines/FSM_DriveAway", order = 1)]
public class FSM_DriveAway : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private SB_Seek seek;
    private HEN_Blackboard blackboard;
    private AudioSource audioSource;
    private MotorManager motorManager;
    private GameObject theChick;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        seek = GetComponent<SB_Seek>();
        motorManager = GetComponent<MotorManager>();
        blackboard = GetComponent<HEN_Blackboard>();
        audioSource = GetComponent<AudioSource>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        if (currentState.Name.Equals("ANGRY"))
        {
            gameObject.transform.localScale /= 1.4f;
            motorManager.maxForce /= 2;
            motorManager.maxSpeed /= 2;
        }

        Debug.Log("FSM_DriveAway exiting");
        audioSource.Stop();
        seek.Disable();
        
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s) */

        FiniteStateMachine calm = ScriptableObject.CreateInstance<FSM_SearchWorms>();
        State angry = new State("ANGRY",
            () => {
                gameObject.transform.localScale *= 1.4f;
                motorManager.maxForce *= 2;
                motorManager.maxSpeed *= 2;
                seek.target = theChick;
                seek.enabled = true;
                audioSource.clip = blackboard.angrySound;
                audioSource.Play();
            }, 
            () => { }, 
            () => {
                gameObject.transform.localScale /= 1.4f;
                motorManager.maxForce /= 2;
                motorManager.maxSpeed /= 2;
                seek.enabled = false;
                audioSource.Stop();
            }    
        );

        /* STAGE 2: create the transitions with their logic(s) */
         

        Transition chickTooClose = new Transition("CHICK TOO CLOSE",
             () => { theChick = SensingUtils.FindInstanceWithinRadius(gameObject, "CHICK", blackboard.chickDetectionRadius);
                 return theChick != null;
             }
        );

        Transition chickFarAway = new Transition("CHICK FAR AWAY",
             () => { return SensingUtils.DistanceToTarget(gameObject, theChick) >= blackboard.chickFarEnoughRadius; }
        );

        /* STAGE 3: add states and transitions to the FSM */
      
            
        AddStates(calm, angry);
        AddTransition(calm, chickTooClose, angry);
        AddTransition(angry, chickFarAway, calm);


        /* STAGE 4: set the initial state */

        initialState = calm;
    }
}
