using UnityEngine;
using Utility;
using BTs;

[CreateAssetMenu(fileName = "Kitty_UT_ActionSet", menuName = "Utility/Kitty_UT_ActionSet", order = 1)]
public class Kitty_UT_ActionSet : UtilityActionSet
{
    public override void OnConstruction()
    {
        // ==========================================
        // 1. ACTION INSTANTIATION
        // ==========================================
        Action feedAction = ScriptableObject.CreateInstance<FeedAction>();
        Action sleepAction = ScriptableObject.CreateInstance<SleepAction>();
        Action playAction = ScriptableObject.CreateInstance<PlayAction>();

        // ==========================================
        // 2. CONSIDERATIONS & SCORERS
        // ==========================================
        
        // --- FEED (Hunger) ---
        // Urgent need: utility stays low until hunger is high, then spikes.
        Consideration hungerConsideration = new Consideration("Hunger", Curves.AGGRESSIVE_EXPONENTIAL, "Hunger Consid.");

        // --- SLEEP (Fatigue) ---
        // Smooth transition: kitty resists sleep until a midpoint, then gets very sleepy.
        Consideration fatigueConsideration = new Consideration("Fatigue", Curves.MEDIUM_SIGMOID, "Sleep Fatigue Consid.");
        

        // --- PLAY (Boredom + Fatigue Inhibitor) ---
        // 1st Consideration: Boredom drives the desire to play (triggers early).
        Consideration boredomConsideration = new Consideration("Boredom", Curves.MILD_LOGARITHMIC, "Boredom Consid.");
        
        // 2nd Consideration: Fatigue inhibits play. We use an INVERTED curve.
        // As Fatigue goes from 0 to 100, the output of this consideration goes from 1 to 0.
        Consideration playFatigueInhibitor = new Consideration("Fatigue", Curves.INVERTED_MEDIUM_SIGMOID, "Fatigue Inhibitor Consid.");
        
        // The Scorer multiplies both. If the kitty is too tired, the inhibitor approaches 0,
        // dragging the total play score down regardless of how bored the kitty is.
        Scorer playScorer = new Scorer("Play Scorer", AggregationPolicy.MULTIPLY, boredomConsideration, playFatigueInhibitor);

        // ==========================================
        // 3. BINDING ACTIONS TO SCORERS
        // ==========================================
        Bind(feedAction, hungerConsideration);
        Bind(sleepAction, fatigueConsideration);
        Bind(playAction, playScorer);

        // ==========================================
        // 4. INERTIA SETUP
        // ==========================================
        // Prevents state thrashing when scores are too close.
        SetInertia(feedAction, 0.10f);  // Keep eating once started
        SetInertia(sleepAction, 0.20f); // High inertia: do not wake up easily
        SetInertia(playAction, 0.05f);  // Easily distracted from playing
    }

    class FeedAction : BehaviourTree
    {
        public override void OnConstruction()
        {
            KittyBlackboard kbb = (KittyBlackboard)blackboard;
            this.name = "FEED";
            root = new Sequence(
                new ACTION_Arrive("Food"), // go where food is
                // let's say it takes some 3 seconds for Kitty to get satiated
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
            this.name = "SLEEP";
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
            this.name = "PLAY";
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
    
    
}