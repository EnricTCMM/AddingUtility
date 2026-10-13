using FSMs;
using MotorBehaviours;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Ghost", menuName = "Finite State Machines/FSM_Ghost", order = 1)]
public class FSM_Ghost : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    
    private MotorManager motor;
    private GHOST_Blackboard blackboard;
    private SB_Seek seek;
    private SB_Pursue pursue;
    private float normalSpeed;
    private float elapsedTime;
    private GameObject nerd;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        
        blackboard = GetComponent<GHOST_Blackboard>();  
        seek = GetComponent<SB_Seek>();
        pursue = GetComponent<SB_Pursue>(); 
        motor = GetComponent<MotorManager>();
        normalSpeed = motor.maxSpeed;
        
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        pursue.Disable();
        seek.Disable();
        motor.maxSpeed = normalSpeed;
        base.OnExit();
    }

    public override void OnConstruction()
    {
        
        State goCastle = new State("GO CASTLE",
            () =>
            {
                seek.target = blackboard.castle;
                motor.maxSpeed = 4*normalSpeed;
                seek.Enable();
                
            }, 
            () => { },
            () =>
            {
                seek.Disable();
                motor.maxSpeed = normalSpeed;
            } 
        );
        
        State hide = new State("HIDE",
            () => { elapsedTime = 0f;}, 
            () => { elapsedTime += Time.deltaTime;}, 
            () => { } 
        );
        
        State  selectTarget = new State("SELECT_TARGET",
            () => { }, 
            () => { nerd = SensingUtils.FindRandomInstanceWithinRadius(gameObject, "NERD", blackboard.nerdDetectionRadius); }, 
            () => { } 
        );
        
        State approach = new State("APPROACH",
            () =>
            {
                pursue.target = nerd;
                pursue.Enable();
            }, 
            () => { }, 
            () => { pursue.Disable(); } 
        );
        
        State cryBoo = new State("CRY BOO",
            () =>
            {
                elapsedTime = 0f;
                blackboard.booParticleSystem.SetActive(true);
                pursue.Enable();
            }, 
            () => { elapsedTime += Time.deltaTime;},
            () =>
            {
                blackboard.booParticleSystem.SetActive(false);
                pursue.Disable();
            } 
        );
        
       
        //---
        
        Transition castleReached = new Transition("TransitionName",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, blackboard.castle) < blackboard.castleReachedRadius;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition timeoutOne = new Transition("TransitionName",
            () =>
            {
                return elapsedTime >= blackboard.hideTime;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        
        Transition timeoutTwo = new Transition("TransitionName",
            () =>
            {
                return elapsedTime >= blackboard.booDuration;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        
        Transition targetSelected = new Transition("TransitionName",
            () =>
            {
                return nerd != null;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        
        Transition targetClose = new Transition("TransitionName",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, nerd) <= blackboard.closeEnoughToScare;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        
        AddStates(goCastle, hide, selectTarget, approach, cryBoo);
        AddTransition(goCastle, castleReached, hide);
        AddTransition(hide, timeoutOne, selectTarget);
        AddTransition(selectTarget, targetSelected, approach);
        AddTransition(approach, targetClose, cryBoo);
        AddTransition(cryBoo, timeoutTwo, goCastle);
         
        initialState = goCastle;
        
    }
}
