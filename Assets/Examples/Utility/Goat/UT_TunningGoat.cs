using UnityEngine;
using Utility;
using BTs;

[CreateAssetMenu(fileName = "UT_TunningGoat", menuName = "Utility/UT_TunningGoat", order = 1)]
public class UT_TunningGoat : UtilityActionSet
{
    public override void OnConstruction()
    {
    
       // ==========================================
        // 1. ACTION INSTANTIATION 
        // ==========================================
        Action eat = ScriptableObject.CreateInstance<BT_Eat>();
        Action sleep = new ACTION_Sleep();
        Action evade = new ACTION_Evade("predator");
        Action wander = ScriptableObject.CreateInstance<BT_GoatConstrainedWander>();
        eat.Name = "Eat";
        wander.Name = "Wander";

        // ==========================================
        // 2. CONSIDERATIONS & SCORERS
        // ==========================================

        //--- EATING (hunger, distance to cabbage)
        Scorer eatScorer = new Scorer("EatScorer", AggregationPolicy.MULTIPLY);
        eatScorer.AddConsideration(new Consideration("hunger", Curves.Linear, "hunger/eat consid."));
        eatScorer.AddConsideration(new Consideration("DistanceToCabbage", Curves.InverseLinear,
            "dist cabbage consid."));

        // --- SLEEPING (energy, panicLevel)
        Scorer sleepScorer = new Scorer("SleepScorer", AggregationPolicy.MULTIPLY);
        sleepScorer.AddConsideration(new Consideration("energy", Curves.InvertedSigmoid(24f, 0.2f), "energy consid."));
        sleepScorer.AddConsideration(new Consideration("panicLevel", Curves.INVERTED_AGGRESSIVE_LOGARITHMIC,
            "panic/sleep consid"));
        // uncomment the following line for "hungry goats don't sleep" (they rather find food)
        // sleepScorer.AddConsideration(new Consideration("hunger", Curves.InverseLinear, "hunger/sleep consid"));

        // --- FLEEING (panicLevel)
        Consideration fleeingConsideration =
            new Consideration("panicLevel", Curves.Linear, "panic/flee consid.");

        // --- WANDERING (fallback action)
        Consideration wanderingConsideration = new Consideration("WanderDrive", Curves.Linear, "wanderDrive consid.");


        // ============================================
        // 3. BINDING ACTIONS TO CONSIDERATIONS/SCORERS
        // ============================================
        Bind(eat, eatScorer);
        Bind(sleep, sleepScorer);
        Bind(evade, fleeingConsideration);
        Bind(wander, wanderingConsideration);

        // ==========================================
        // 4. INERTIA SETUP
        // ==========================================
        // SetInertia(eat, 0.10f); // commitment to eating is quite high 
           SetInertia(sleep, 0.2f); // sleep has a considerable inertia (inertia being higher than wander drive only hunger o wolf wake it up)
        // fleeing has no inertia
        // wandering has no inertia
        
    } // OnConstruction ends here
    
    // ACTION DEFINITION HERE
    // Actions defined here do not name-collide with actions defined elsewhere
    
} // UtilityActionSet ends here

// ACTION DEFINITION HERE 
// Actions defined here may name-collide with actions defined elsewhere 