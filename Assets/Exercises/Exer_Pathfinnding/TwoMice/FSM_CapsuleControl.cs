using FSMs;
using UnityEngine;

[CreateAssetMenu(fileName = "FSM_CapsuleControl", menuName = "Finite State Machines/FSM_CapsuleControl", order = 1)]
public class FSM_CapsuleControl : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private GameObject capsule;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter actionName of any state 
         * Usually this code includes .GetComponent<...> invocations */

        capsule = gameObject.transform.Find("ProtectiveCapsule").gameObject;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit actionName of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        capsule.SetActive(false);
        base.OnExit();
    }

    public override void OnConstruction()
    {
        
        
        State capsule_ON = new State("CAPSULE ON",
            () => { capsule.SetActive(true); }, 
            () => { }, 
            () => { }   
        );

        State capsule_OFF = new State("CAPSULE OFF",
            () => { capsule.SetActive(false); }, 
            () => { }, 
            () => { }  
        );

        Transition fireIsClose = new Transition("Fire is Close",
            () => { return SensingUtils.FindInstanceWithinRadius(gameObject, "FIRE", 20) != null; } 
        );

        Transition fireIsNotClose = new Transition("Fire is Not Close",
            () => { return SensingUtils.FindInstanceWithinRadius(gameObject, "FIRE", 20) == null; }
        );

        AddStates(capsule_ON, capsule_OFF);
        AddTransition(capsule_OFF, fireIsClose, capsule_ON);
        AddTransition(capsule_ON, fireIsNotClose, capsule_OFF);

        initialState = capsule_OFF;

        
        /* Això és el que es dona als estudiants 
        State capsule_OFF = new State("CAPSULE OFF",
            () => { // COMPLETE }, 
            () => { }, 
            () => { }  
        );

        // COMPLETE

        initialState = capsule_OFF;
        
        /* Final del que es dona als estudiants */

    }
}
