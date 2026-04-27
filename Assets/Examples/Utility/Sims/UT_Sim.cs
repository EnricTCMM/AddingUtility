using UnityEngine;
using Utility;
using BTs;
using UnityEditor.ShaderGraph.Internal;

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
        SetInertia(ACTION_Sleep, 0.3f); // high inertia for sleeping

        Action ACTION_Work = ScriptableObject.CreateInstance<UT_Sim.BT_Work >();
        Consideration dayOfWeekConsideration = new Consideration("dayOfWeek",
            (t) => { return t >= 6 ? 0f : 1f;}, "dayOfWeek");
        Consideration timeOfDayConsideration = new Consideration("timeOfDay",
            (t) => { return t >= 9 && t <= 17 ? 0.8f : 0f; }, "timeOfDay");
        Scorer workScorer = new Scorer("WorkScorer", AggregationPolicy.MULTIPLY);
        /*
         // should we want a burnout veto...
         Consideration burnoutVeto = new Consideration("sleepiness",
                    (s) => { return s >= 90 ? 0f : 1f; }, "burnoutVeto");
         workScorer.AddConsideration(burnoutVeto)           
         */
        workScorer.AddConsideration(dayOfWeekConsideration);
        workScorer.AddConsideration(timeOfDayConsideration);
        Bind(ACTION_Work, workScorer);
        // SetInertia(ACTION_Work, 0.2f);  // low inertia for working. Working is an interruptible activity.  

        Action ACTION_UseBathroom = ScriptableObject.CreateInstance<UT_Sim.BT_UseBathroom>();
        Consideration bladderConsideration = new Consideration("bladder", Curves.AGGRESSIVE_EXPONENTIAL, "bladder");
        Bind(ACTION_UseBathroom, bladderConsideration);
        SetInertia(ACTION_UseBathroom, 0.15f); // mid o or low inertia. Using the bathroom does not take long.
        
        Action ACTION_Eat = ScriptableObject.CreateInstance<UT_Sim.BT_Eat>();
        Scorer eatScorer = new Scorer("EatScorer", AggregationPolicy.MULTIPLY);
        // Biological Need: Medium Exponential (x^2)
        eatScorer.AddConsideration(new Consideration("hunger", Curves.MEDIUM_EXPONENTIAL, "hunger"));
        // Cultural Routine: Soft Veto for snacking outside of meal times
        Consideration mealTimeConsideration = new Consideration("timeOfDay",
            (t) => { 
                bool isBreakfast = t >= 7f && t <= 9f;
                bool isLunch = t >= 13f && t <= 15f;
                bool isDinner = t >= 20f && t <= 22f;
                // Return 1.0 during meals, 0.7 during the rest of the day
                return (isBreakfast || isLunch || isDinner) ? 1.0f : 0.7f; 
            }, "mealTime");
        eatScorer.AddConsideration(mealTimeConsideration);
        Bind(ACTION_Eat, eatScorer);
        SetInertia(ACTION_Eat, 0.2f); // Moderate inertia: Eating takes a bit of time, we want them to finish the meal
        
        Action ACTION_Entertainment = ScriptableObject.CreateInstance<UT_Sim.BT_Entertain>();
        Scorer entertainmentScorer = new Scorer("EntertainmentScorer", AggregationPolicy.MULTIPLY);
        // 1. The Baseline Need: Mild Exponential allows it to act as a healthy filler
        entertainmentScorer.AddConsideration(new Consideration("boredom", Curves.MILD_EXPONENTIAL, "boredom"));
        // 2. The "Quiet Hours" Penalty (soft veto):
        // The Sim prefers to chill or stay in bed between 23:00 and 06:00
        Consideration quietHoursConsideration = new Consideration("timeOfDay",
            (t) => { 
                // If it's the middle of the night, multiply by 0.4. Otherwise, 1.0.
                return (t >= 23f || t <= 6f) ? 0.4f : 1.0f; }, "quietHours");
        entertainmentScorer.AddConsideration(quietHoursConsideration);
        Bind(ACTION_Entertainment, entertainmentScorer);
        // Moderate/High inertia: Entertainment is usually an extended activity 
        // (watching a movie, playing a game session), so we want them to stick to it.
        SetInertia(ACTION_Entertainment, 0.25f);
        
    }


    // placing actions here (inside the UT class) avoids name clashes with other actions that might be used elsewhere
    // and have the same name.

    class BT_Sleep : BehaviourTree
    {
        override public void OnConstruction()
        {
            root = new Sequence(
                // Sleeping should take place at home...
                new ACTION_Quiet(),
                new ACTION_Speak("...Zzz..."),
                new RepeatUntilSuccessDecorator(
                    new LambdaAction(() => {
                        SIM_Blackboard bl = (SIM_Blackboard)blackboard;
                        bl.Sleep();
                        if (bl.sleepiness <= 0) return Status.SUCCEEDED;
                        else return Status.FAILED;
                    })
                )
            );
        }
    }
    
    class BT_Work : BehaviourTree
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