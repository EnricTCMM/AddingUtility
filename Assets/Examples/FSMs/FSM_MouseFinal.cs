using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_MouseFinal", menuName = "Finite State Machines/FSM_MouseFinal", order = 1)]
public class FSM_MouseFinal : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/


        FiniteStateMachine NORMAL = ScriptableObject.CreateInstance<FSM_MouseAware>();
        NORMAL.Name = "FREE";

        State TRAPPED = new State("TRAPPED",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );


        /* STAGE 2: create the transitions with their logic(s)
         * --------------------------------------------------- */

        Transition mouseTrapped = new Transition("Mouse trapped",
            () => { return gameObject.tag == "TRAPPED_MOUSE"; }
        );

        Transition mouseFreed = new Transition("Mouse freed",
            () => { return gameObject.tag == "MOUSE"; }
        );



        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(NORMAL, TRAPPED);

        AddTransition(NORMAL, mouseTrapped, TRAPPED);
        AddTransition(TRAPPED, mouseFreed, NORMAL);


        /* STAGE 4: set the initial state */

        initialState = NORMAL;
    }
}
