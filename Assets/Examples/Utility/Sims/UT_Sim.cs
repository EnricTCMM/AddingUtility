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
        
        // better sleep at night, from 22 to 7 (do not normalize
        Consideration conTimeOfDay = new Consideration("timeOfDay",
            (t) => { return t >= 22 || t <= 7 ? 1 : 0.5f; }, 
            "timeOfDay", false);
        
        sleepScorer.AddConsideration(conTimeOfDay);
        Bind(ACTION_Sleep, sleepScorer);
        SetInertia(ACTION_Sleep, 0.3f); // high inertia for sleeping

        Action ACTION_Work = ScriptableObject.CreateInstance<UT_Sim.BT_Work>();
        
        // work Monday to Friday (do not normalize)
        Consideration dayOfWeekConsideration = new Consideration("dayOfWeek",
            (t) => { return t >= 6 ? 0f : 1f; }, 
            "dayOfWeek", false);
        
        // work from 9 to 17 (do not normalize)
        Consideration timeOfDayConsideration = new Consideration("timeOfDay",
            (t) => { return t >= 9 && t <= 17 ? 0.8f : 0f; }, 
            "timeOfDay", false);
        
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
        
        // Cultural Routine: Soft Veto for snacking outside of meal times (do not normalize)
        Consideration mealTimeConsideration = new Consideration("timeOfDay",
            (t) =>
            {
                bool isBreakfast = t >= 7f && t <= 9f;
                bool isLunch = t >= 13f && t <= 15f;
                bool isDinner = t >= 20f && t <= 22f;
                // Return 1.0 during meals, 0.7 during the rest of the day
                return (isBreakfast || isLunch || isDinner) ? 1.0f : 0.7f;
            }, 
            "mealTime", false);
        
        eatScorer.AddConsideration(mealTimeConsideration);
        Bind(ACTION_Eat, eatScorer);
        SetInertia(ACTION_Eat, 0.2f); // Moderate inertia: Eating takes a bit of time, we want them to finish the meal

        Action ACTION_Entertainment = ScriptableObject.CreateInstance<UT_Sim.BT_Entertain>();
        Scorer entertainmentScorer = new Scorer("EntertainmentScorer", AggregationPolicy.MULTIPLY);
        // 1. The Baseline Need: Mild Exponential allows it to act as a healthy filler
        entertainmentScorer.AddConsideration(new Consideration("boredom", Curves.MILD_EXPONENTIAL, "boredom"));
        
        // 2. The "Quiet Hours" Penalty (soft veto):
        // From 23 to 6, the Sim is relectant to engage in entertainment.
        Consideration quietHoursConsideration = new Consideration("timeOfDay",
            (t) =>
            {
                // If it's the middle of the night, multiply by 0.4. Otherwise, 1.0.
                return (t >= 23f || t <= 6f) ? 0.4f : 1.0f;
            }, 
            "quietHours", false);
        
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
                new ACTION_DebugLog("Going home to sleep..."),
                new ACTION_Arrive("home"),
                new ACTION_DebugLog("Sleeping at home..."),
                // new ACTION_Quiet(),
                // new ACTION_Speak("...Zzz..."),
                new RepeatUntilSuccessDecorator(
                    new LambdaAction(() =>
                    {
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
            root = new Sequence(
                // this SIM always works in the office. 
                new ACTION_DebugLog("Going to the office..."),
                new ACTION_Arrive("office"),
                new ACTION_DebugLog("WORKING hard, very hard!"),
                // once there it proceeds non stop
                new ACTION_RunForever()
            );
        }
    }

    class BT_Eat : BehaviourTree
    {
        override public void OnConstruction()
        {
            //  simple eating. Always takes place at the restaurant.
            root = new Sequence(
                new ACTION_DebugLog("Going to the restaurant..."),
                new ACTION_Arrive("restaurant"),
                // for simplicity and accurate time control, "eating" should be blackboard driven (???)
                new ACTION_DebugLog("EATING..."),
                // it takes 20 minutes to finish the meal. 
                new ACTION_WaitRealTime("20f"),
                new LambdaAction(() =>
                {
                    ((SIM_Blackboard)blackboard).hunger = 0;
                    return Status.SUCCEEDED;
                })
            );
        }
    }

    class BT_Entertain : BehaviourTree
    {
        override public void OnConstruction()
        {
            root = new Sequence(
                new ACTION_DebugLog("Going to the cinema..."),
                new ACTION_Arrive("cinema"),
                new ACTION_DebugLog("Enjoying the movie..."),
                new ACTION_WaitRealTime("90f"),
                new LambdaAction(() => {          
                        ((SIM_Blackboard)blackboard).boredom = 0;
                        return Status.SUCCEEDED;})
            );
        }
    }

    class BT_UseBathroom : BehaviourTree
    {
        override public void OnConstruction()
        {
            root = new Sequence(
                new ACTION_DebugLog("Going home..."),
                new ACTION_Arrive("Home"),
                new ACTION_DebugLog("taking a leak..."),
                new ACTION_WaitRealTime("10f"),
                new LambdaAction(() =>
                {
                    ((SIM_Blackboard)blackboard).bladder = 0;
                    return Status.SUCCEEDED;
                }) 
            );
        }
    }

    private class ACTION_WaitRealTime : Action {

        private string keyTime;
        public ACTION_WaitRealTime(string keyTime)
        {
            this.keyTime = keyTime;
        }
        
        private SIM_Blackboard bl;
        private float time;
        private float minutesElapsed;
        
        public override void OnInitialize()
        {
            bl = (SIM_Blackboard)blackboard;
            time = bl.Get<float>(keyTime);
            minutesElapsed = 0;
        }
        
        public override Status OnTick()
        {
            minutesElapsed += bl.dayNightCycler.GetComponent<DayNightCycle2D>().deltaMinutes;
            if (minutesElapsed >= time) return Status.SUCCEEDED;
            else return Status.RUNNING;
        }
    }
}