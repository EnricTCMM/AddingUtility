using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Batcat_Daily", menuName = "Finite State Machines/FSM_Batcat_Daily", order = 1)]
public class FSM_Batcat_Daily : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private BATCAT_Blackboard blackboard;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<BATCAT_Blackboard>();

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
        // STAGE 1: create the states with their logic(s)
         
        FiniteStateMachine JAILING = ScriptableObject.CreateInstance<FSM_BatcatChase>();
        FiniteStateMachine FEEDING = ScriptableObject.CreateInstance<FSM_BatcatFeed>();
        JAILING.Name = "JAILING";
        FEEDING.Name = "FEEDING";


        // STAGE 2: create the transitions with their logic(s)

        Transition notHungry = new Transition("Not hungry",
            () => { return blackboard.hunger <= blackboard.hungerLowEnough; }
        );

        Transition hungryAndNotBusy = new Transition("Hungry and not busy",
            () => { return blackboard.hunger >= blackboard.hungerTooHigh!
                           && !JAILING.currentState.Name.Equals("TRANSPORTING"); }
        );

        // STAGE 3: add states and transitions to the FSM 
            
        AddStates(JAILING, FEEDING);

        AddTransition(FEEDING, notHungry, JAILING);
        AddTransition(JAILING, hungryAndNotBusy, FEEDING);


        // STAGE 4: set the initial state

        initialState = JAILING;
    }

}
