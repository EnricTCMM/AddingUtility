using UnityEngine;
using Utility;
using BTs;

// removing inertia shows what thrashing means in this context

[CreateAssetMenu(fileName = "UT_LinearKitty_ActionSet", menuName = "Utility/UT_LinearKitty_ActionSet", order = 1)]
public class UT_LinearKitty_ActionSet : UtilityActionSet
{
    public override void OnConstruction()
    {
        // ==========================================
        // 1. ACTION INSTANTIATION
        // ==========================================
        Action feedAction = ScriptableObject.CreateInstance<FeedAction>();
        Action sleepAction = ScriptableObject.CreateInstance<SleepAction>();
        Action playAction = ScriptableObject.CreateInstance<PlayAction>();
        feedAction.Name = "FEED";
        sleepAction.Name = "SLEEP";
        playAction.Name = "PLAY";

        // ==========================================
        // 2. CONSIDERATIONS & SCORERS
        // ==========================================
        
        // --- FEED (Hunger) ---
        Consideration hungerConsideration = new Consideration("Hunger", Curves.Linear, "Hunger Consid.");
        
        // --- SLEEP (Fatigue) ---
        Consideration fatigueConsideration = new Consideration("Fatigue", Curves.Linear, "Sleep Fatigue Consid.");
        
        // --- PLAY (Boredom + Fatigue Inhibitor) ---
        Consideration boredomConsideration = new Consideration("Boredom", Curves.Linear, "Boredom Consid.");
        
        // 2nd Consideration: Fatigue inhibits play. INVERTED CURVE
        // As Fatigue goes from 0 to 100, the output of this consideration goes from 1 to 0.
        Consideration playFatigueInhibitor = new Consideration("Fatigue", Curves.InverseLinear, "Fatigue Inhibitor Consid.");
        
        // The Scorer multiplies both. If the kitty is too tired, the inhibitor approaches 0,
        // dragging the total play score down regardless of how bored the kitty is.
        Scorer playScorer = new Scorer("Play Scorer", AggregationPolicy.MULTIPLY, boredomConsideration, playFatigueInhibitor);

        // =============================================
        // 3. BINDING ACTIONS TO CONSIDERATIONS/SCORERS
        // =============================================
        Bind(feedAction, hungerConsideration);
        Bind(sleepAction, fatigueConsideration);
        Bind(playAction, playScorer);

        // ==========================================
        // 4. INERTIA SETUP
        // ==========================================
        // Prevents state thrashing when scores are too close.
        SetInertia(feedAction, 0.40f);  // Keep eating once started
        SetInertia(sleepAction, 0.65f); // Vey high inertia: do not wake up easily (sleep soundly)
        SetInertia(playAction, 0.25f);  // Not easily distracted from playing
    }

    class FeedAction : BehaviourTree
    {
        public override void OnConstruction()
        {
            KittyBlackboard kbb = (KittyBlackboard)blackboard;
            
            root = new Sequence(
                new ACTION_Arrive("Food"), // go where food is
                // let's say it takes some 3 seconds for Kitty to get completely satiated
                new ACTION_WaitForSeconds("3"),
                new LambdaAction(() => { kbb.hunger = 0;
                    return Status.SUCCEEDED;
                })
            );
        }
    }

    class SleepAction : BehaviourTree
    {
        public override void OnConstruction()
        {
            KittyBlackboard kbb = (KittyBlackboard)blackboard;
            
            root = new Sequence(
                new ACTION_Arrive("Mat"),
                new LambdaAction(() => { 
                    kbb.RestABit(); 
                    if (kbb.fatigue == 0) return  Status.SUCCEEDED;
                    else return Status.RUNNING;
                })
            );
        }
    }
    
    class PlayAction : BehaviourTree
    {
        public override void OnConstruction()
        {
            KittyBlackboard kbb = (KittyBlackboard)blackboard;
            
            root = new Sequence(
                new ACTION_Arrive("Wool"),
                new LambdaAction(() =>
                {
                    kbb.PlayABit();
                    if (kbb.boredom == 0) return  Status.SUCCEEDED;
                    else return Status.RUNNING;
                })
            );
        }
    }
    
} // UtilityActionSet ends here

// ACTION DEFINITION HERE 
// Actions defined here may name-collide with actions defined elsewhere 