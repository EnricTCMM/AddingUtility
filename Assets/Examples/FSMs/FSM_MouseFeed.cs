using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_MouseFeed", menuName = "Finite State Machines/FSM_MouseFeed", order = 1)]
public class FSM_MouseFeed : FiniteStateMachine
{
    
    private MOUSE_Blackboard blackboard;
    private WanderPlusAvoid wanderPlusAvoid;
    private ArrivePlusOA arrive;
    private float timeSinceLastBite;
    private GameObject cheese;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<MOUSE_Blackboard>();
        wanderPlusAvoid = GetComponent<WanderPlusAvoid>();
        arrive = GetComponent<ArrivePlusOA>();
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


        /* STAGE 1: create the states with their logic(s) */

        State WANDERING = new State("WANDERING",
            () => { wanderPlusAvoid.enabled = true; }, 
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; }, 
            () => { wanderPlusAvoid.enabled = false; }  
        );

        State REACHING = new State("REACHING CHEESE",
            () => { arrive.target = cheese; arrive.enabled = true; },
            () => { blackboard.hunger += blackboard.normalHungerIncrement * Time.deltaTime; },
            () => { arrive.enabled = false;
            }
        );

        State EATING = new State("EATING",
            () => { timeSinceLastBite = 100; },
            () => { if (timeSinceLastBite >= 1 / blackboard.bitesPerSecond) {
                        cheese.SendMessage ("BeBitten");
                        blackboard.hunger -= blackboard.cheeseHungerDecrement;
                        timeSinceLastBite = 0;
                    }
                    else
                    {
                        timeSinceLastBite += Time.deltaTime;
                    }
            },
            () => { /* do nothing in particular when exiting*/ }
        );



        /* STAGE 2: create the transitions with their logic(s)
         * --------------------------------------------------- */

        Transition hungryAndCheeseDetected = new Transition("Cheese Detected",
           () => { if (!blackboard.Hungry()) return false;  
                   cheese = SensingUtils.FindInstanceWithinRadius(gameObject, 
                                        "CHEESE", blackboard.cheeseDetectableRadius);
                   return cheese != null;
                 },
           () => { blackboard.globalBlackboard.AnnounceCheese(cheese); }
        );

        Transition hungryAndCheeseAnnounced = new Transition("Cheese Announced",
           () => { return  blackboard.Hungry() 
                           &&  blackboard.globalBlackboard.announcedCheese != null; 
                 },
           () => { cheese = blackboard.globalBlackboard.announcedCheese; }
        );

        Transition cheeseVanished = new Transition("Cheese vanished",
            () => { return cheese == null || cheese.Equals(null); }
        );

        Transition cheeseReached = new Transition("Cheese reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, cheese) < blackboard.cheeseReachedRadius; } // write the condition checkeing code in {}
        );

        Transition satiated = new Transition("satiated",
            () => { return blackboard.Satited(); } 
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ---------------------------------------------- */
            

        AddStates(WANDERING, REACHING, EATING);

        AddTransition(WANDERING, hungryAndCheeseDetected, REACHING);
        AddTransition(WANDERING, hungryAndCheeseAnnounced, REACHING);
        AddTransition(REACHING, cheeseVanished, WANDERING);
        AddTransition(REACHING, cheeseReached, EATING);
        AddTransition(EATING, cheeseVanished, WANDERING);
        AddTransition(EATING, satiated, WANDERING);

        /* STAGE 4: set the initial state */

        initialState = WANDERING;
    }
}
