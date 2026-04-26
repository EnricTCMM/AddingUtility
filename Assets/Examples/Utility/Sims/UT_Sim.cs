using UnityEngine;
using Utility;
using BTs;

[CreateAssetMenu(fileName = "UT_Sim", menuName = "Utility/UT_Sim", order = 1)]
public class UT_Sim : UtilityActionSet
{
    public override void OnConstruction()
    {
        Action ACTION_Sleep = ScriptableObject.CreateInstance<BT_Sleep>();
        Scorer sleepScorer = new Scorer("SleepScorer", AggregationPolicy.MULTIPLY);
        // the more sleepy the Sim, the more urgent it is to sleep
        sleepScorer.AddConsideration(new Consideration("sleepiness", Curves.MILD_EXPONENTIAL, "sleepiness"));
        // better sleep at night, from 22 to 7
        Consideration conTimeOfDay = new Consideration("timeOfDay",
            (t) => { return t >= 22 || t <= 7 ? 1 : 0.5f; }, "timeOfDay");
        sleepScorer.AddConsideration(conTimeOfDay);
        Bind(ACTION_Sleep, sleepScorer);
        SetInertia(ACTION_Sleep, 0.3f);  // high inertia for sleeping
    }  
                                                       
        

        /* STAGE 2: Add Considerations to the Scorer
         * -----------------------------------------
         * Use blackboard property names and select an appropriate Curve from Curves.cs.
         * You can also define min/max if the property doesn't have a [Range] attribute.
         
        myActionScorer.AddConsideration(new Consideration("BlackboardKey", Curves.Linear, "ConsiderationName"));
        // myActionScorer.AddConsideration(new Consideration("AnotherKey", Curves.InvertedExponential(2f), 0f, 100f, "AnotherName"));
        */


        /* STAGE 3: Bind the Action to the Scorer
         * --------------------------------------
         * Make sure the action inherits from BTs.Action.
         
        Bind(new MyCustomAction(), myActionScorer);
        */
        
        
        /* Repeat STAGES 1 to 3 for all the actions this Utility AI needs to evaluate */
        
        // placing actions here (inside the UT class) avoids name clashes with other actions that might be used elsewhere
        // and have the same name.
    
        class BT_Sleep : BehaviourTree
        {
            override public void OnConstruction()
            {
                root = new Sequence();
            }
        }
    
        class BT_Eat : BehaviourTree
        {
            override public void OnConstruction()
            {
                root = new Sequence();
            }
        }
    
        class BT_Entertain : BehaviourTree
        {
            override public void OnConstruction()
            {
                root = new Sequence();
            }
        }
    
        class BT_UseBathroom : BehaviourTree
        {
            override public void OnConstruction()
            {
                root = new Sequence();
            }
        }
}
    
    
    
    



