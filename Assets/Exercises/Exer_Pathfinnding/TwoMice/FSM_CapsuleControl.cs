using FSMs;
using UnityEngine;
using Steerings;

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
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        capsule = gameObject.transform.Find("ProtectiveCapsule").gameObject;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        capsule.SetActive(false);
        base.OnExit();
    }

    public override void OnConstruction()
    {
        
        State capsule_OFF = new State("CAPSULE OFF",
            () => { /* COMPLETE */ }, 
            () => { }, 
            () => { }  
        );

        /* COMPLETE */

        initialState = capsule_OFF;

    }
}
