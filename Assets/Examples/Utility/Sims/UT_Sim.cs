
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
        // better sleep at night, from 22 to 7 (do not normalize)
        // this is a "cultural" soft veto
        Consideration conTimeOfDay = new Consideration("timeOfDay",
            (t) => { return t >= 22 || t <= 7 ? 1 : 0.5f; }, 
            "timeOfDay", false);
        Consideration sleepinessConsideration = new Consideration("sleepiness", 
            (t) =>
            {
                // at night sleepiness behaves different than at day.
                SIM_Blackboard bl = (SIM_Blackboard)blackboard;
                if (bl.timeOfDay >= 22 || bl.timeOfDay <= 7)
                    return Curves.MILD_EXPONENTIAL(t); // at night almost linear
                else return Curves.AGGRESSIVE_EXPONENTIAL(t); // at day exponential
            },
            "sleepiness"
        );
        
        sleepScorer.AddConsideration(conTimeOfDay);
        sleepScorer.AddConsideration(sleepinessConsideration);
        Bind(ACTION_Sleep, sleepScorer);
        SetInertia(ACTION_Sleep, 0.5f); // very high inertia for sleeping

        Action ACTION_Work = ScriptableObject.CreateInstance<UT_Sim.BT_Work>();
        // work Monday to Friday (do not normalize)
        Consideration dayOfWeekConsideration = new Consideration("dayOfWeek",
            (t) => { return t >= 6 ? 0f : 1f; }, 
            "dayOfWeek", false);
        // work from 9 to 17 (do not normalize)
        Consideration timeOfDayConsideration = new Consideration("timeOfDay",
            (t) => { return t >= 9 && t <= 17 ? 0.9f : 0f; }, 
            "timeOfDay", false);
        Scorer workScorer = new Scorer("WorkScorer", AggregationPolicy.MULTIPLY);
        workScorer.AddConsideration(dayOfWeekConsideration);
        workScorer.AddConsideration(timeOfDayConsideration);
        Bind(ACTION_Work, workScorer);
        // no inertia for working. It's 0 or 0.9. 0.9 makes it hard to interrupt.
        
        Action ACTION_UseBathroom = ScriptableObject.CreateInstance<UT_Sim.BT_UseBathroom>();
        Consideration bladderConsideration = new Consideration("bladder", 
            Curves.AGGRESSIVE_EXPONENTIAL, "bladder");
        Bind(ACTION_UseBathroom, bladderConsideration);
        SetInertia(ACTION_UseBathroom, 1); // this makes it uninterruptible.

        Action ACTION_Eat = ScriptableObject.CreateInstance<UT_Sim.BT_Eat>();
        Scorer eatScorer = new Scorer("EatScorer", AggregationPolicy.MULTIPLY);
        eatScorer.AddConsideration(new Consideration("hunger", 
            Curves.Sigmoid(12, 0.75f), "hunger") // shifted sigmoid. 
        );
        Bind(ACTION_Eat, eatScorer);
        SetInertia(ACTION_Eat, 1f); // max inertia. Nothing can interrupt eating.

        // this action is a fallback. That's the reason of the 0.2
        Action ACTION_Entertainment = ScriptableObject.CreateInstance<UT_Sim.BT_Entertain>();
        Consideration entertainmentConsideration = new Consideration( "boredom",
            (v) =>
            {
                return Mathf.Max(0.2f, Curves.MILD_EXPONENTIAL(v)); 
            }, 
            "boredom"
        );
        Bind(ACTION_Entertainment, entertainmentConsideration);
        SetInertia(ACTION_Entertainment, 0.25f); // moderate inertia. 
    }


    // placing actions here (inside the UT class) avoids name clashes with other actions
    // that might be used elsewhere and have the same name.

    class BT_Sleep : BehaviourTree
    {
        override public void OnConstruction()
        {
            root = new Sequence(
                // this SIM always sleeps at home. 
                // first decide if it's necessary to go home.
                new Selector(
                    new CONDITION_CheckKeyValue("currentLocationName", "HOME"),
                    new Sequence(
                        new ACTION_DebugLog("Going home to sleep..."),
                        new ACTION_ShowInfo("Going home to sleep..."),
                        new ACTION_Arrive("home"),
                        new ACTION_UpdateKey<string>("currentLocationName", "HOME")
                    ) 
                ),
                
                new ACTION_DebugLog("Sleeping..."),
                new ACTION_ShowInfo("Sleeping..."),
                new LambdaAction(() =>
                {
                    ((SIM_Blackboard)blackboard).StartSleeping();
                    return Status.SUCCEEDED;
                }),
                new RepeatUntilSuccessDecorator(
                    new LambdaCondition(() =>
                    {
                        return ((SIM_Blackboard)blackboard).sleepiness <= 0;
                    })
                ),
                new LambdaAction(() =>
                {
                    ((SIM_Blackboard)blackboard).EndSleeping();
                    return Status.SUCCEEDED;
                })
            );
        }

        public override void OnAbort()
        {
            // if the BT is aborted, sleeping ends.
            ((SIM_Blackboard)blackboard).EndSleeping();
        }
    }

    class BT_Work : BehaviourTree
    {
        override public void OnConstruction()
        {
            root = new Sequence(
                // this SIM always works in the office. 
                // first decide if it is necessary to go to the office.
                new Selector(
                    new CONDITION_CheckKeyValue("currentLocationName", "OFFICE"),
                    new Sequence(
                        new ACTION_DebugLog("Going to the office..."),
                        new ACTION_ShowInfo("Going to the office..."),
                        new ACTION_Arrive("office"),
                        new ACTION_UpdateKey<string>("currentLocationName", "OFFICE")
                    )
                ),
                // once there work very hard until ...  (until the executor says so)
                new ACTION_DebugLog("Working hard, very hard..."),
                new ACTION_ShowInfo("Working hard, very hard..."),
                new ACTION_RunForever()
            );
        }
    }


    class BT_Eat : BehaviourTree
    {
        public override void OnConstruction()
        {
            Sequence haveASnack = new Sequence(
                new ACTION_DebugLog("Having a snack..."),
                new ACTION_ShowInfo("Having a snack..."),
                new ACTION_WaitRealTime("10"),
                new LambdaAction(() =>
                {
                    ((SIM_Blackboard)blackboard).EatSnackEffect();
                    return Status.SUCCEEDED;
                })
            );
            Sequence fullMealAtRestaurant = new Sequence(
                new ACTION_DebugLog("Going to the restaurant..."),
                new ACTION_ShowInfo("Going to the restaurant..."),
                new ACTION_Arrive("restaurant"),
                new ACTION_UpdateKey<string>("currentLocationName", "RESTAURANT"),
                new ACTION_DebugLog("EATING at the restaurant..."),
                new ACTION_ShowInfo("EATING at the restaurant..."),
                new ACTION_WaitRealTime("60f"),
                new LambdaAction(() =>
                {
                    ((SIM_Blackboard)blackboard).EatFullMealEffect();
                    return Status.SUCCEEDED;
                })
            );
            Sequence fullMealAtHome = new Sequence(
                new Selector(
                    new CONDITION_CheckKeyValue("currentLocationName", "HOME"),
                    // if previous condition was false, sequence is executed.
                    new Sequence(
                        new ACTION_DebugLog("Going home..."),
                        new ACTION_ShowInfo("Going home..."),
                        new ACTION_Arrive("home"),
                        new ACTION_UpdateKey<string>("currentLocationName", "HOME")
                    )
                ),
                new ACTION_DebugLog("Preparing meal at home..."),
                new ACTION_ShowInfo("Preparing meal at home..."),
                new ACTION_WaitRealTime("30f"),
                new ACTION_DebugLog("EATING at home..."),
                new ACTION_ShowInfo("EATING at home..."),
                new ACTION_WaitRealTime("30f"),
                new LambdaAction(() =>
                {
                    ((SIM_Blackboard)blackboard).EatFullMealEffect();
                    return Status.SUCCEEDED;
                })
            ); // fullMealAtHome ends here
            
            root = new Selector(
                // first branch: SIM is in the office
                new Sequence(
                    new CONDITION_CheckKeyValue("currentLocationName", "OFFICE"),
                    // can't leave the office to eat somewhere else. Only a snack is allowed. 
                    haveASnack
                ),
                // second branch: SIM is elsewhere (fallback)
                new Selector(
                    // if hunger is not very high, a snack will do
                    new Sequence(
                        new LambdaCondition(() =>
                        {
                            SIM_Blackboard bl = (SIM_Blackboard)blackboard;
                            return bl.hunger <= bl.snackThreshold;
                        }),
                        haveASnack
                    ) ,
                    // hunger is quite high. Full meal is required. 
                    new Selector(
                        new Sequence(
                            // restaurant opening hours? Restaurant or home. 
                            new LambdaCondition(() =>
                            {
                                float time = blackboard.Get<float>("timeOfDay");
                                return (time >= 12f && time <= 15f) || (time >= 18f && time <= 21f);
                            }),
                            new RandomSelector(fullMealAtHome, fullMealAtRestaurant)
                        ),
                        // not restaurant opening hours. Only full meal at home is possible.
                        fullMealAtHome
                    )
                )
            ); // root ends here
        }
    }
    

    class BT_Entertain : BehaviourTree
    {
        public override void OnConstruction()
        {
           // low boredom asks for short, activities that can be safely interrupted...
           Sequence readChapter = new Sequence(
                new ACTION_DebugLog("Reading a chapter..."),
                new ACTION_ShowInfo("Reading a chapter..."),
                new ACTION_WaitRealTime("20f"),
                new LambdaAction(() =>
                {
                    ((SIM_Blackboard)blackboard).ReadChapterEffect();
                    return Status.SUCCEEDED;
                })
           );

           Sequence playCasualGame = new Sequence(
               new ACTION_DebugLog("Playing a casual game..."),
               new ACTION_ShowInfo("Playing a casual game..."),
               new ACTION_WaitRealTime("20f"),
               new LambdaAction(() =>
               {
                   ((SIM_Blackboard)blackboard).PlayCasualGameEffect();
                   return Status.SUCCEEDED;
               })
           );

           Sequence goCinema = new Sequence(
               new ACTION_DebugLog("Going to the cinema..."),
               new ACTION_ShowInfo("Going to the cinema..."),
               new ACTION_Arrive("cinema"),
               new ACTION_UpdateKey<string>("currentLocationName", "CINEMA"),
               new ACTION_DebugLog("Enjoying the movie..."),
               new ACTION_ShowInfo("Enjoying the movie..."),
               new ACTION_WaitRealTime("90f"),
               new LambdaAction(() =>
               {
                   ((SIM_Blackboard)blackboard).WatchFilmCinemaEffect();
                   return Status.SUCCEEDED;
               })
           );
           
           Sequence watchNetflix = new Sequence(
               new ACTION_DebugLog("Enjoying Netflix..."),
               new ACTION_ShowInfo("Enjoying Netflix..."),
               new ACTION_WaitRealTime("60f"),
               new LambdaAction(() =>
                   {
                       ((SIM_Blackboard)blackboard).WatchNetflixEffect();
                       return Status.SUCCEEDED;
                })
           );


           root = new Sequence(
               // entertainment always starts at home
               new Selector(
                   new CONDITION_CheckKeyValue("currentLocationName", "HOME"),
                   new Sequence(
                       new ACTION_DebugLog("Going home..."),
                       new ACTION_ShowInfo("Going home..."),
                       new ACTION_Arrive("home"),
                       new ACTION_UpdateKey<string>("currentLocationName", "HOME")
                   ) 
               ),
               // now decide what to do.
               new Selector(
                   new Sequence(
                       new LambdaCondition(() =>
                       {
                           SIM_Blackboard bl = (SIM_Blackboard)blackboard;
                           return bl.boredom <= bl.boredomTolerance;
                       }), 
                       new RandomSelector(readChapter, playCasualGame) // low boredom options
                   ),
                   // if here, boredom is quite high. 
                   // during cinema opening hours, cinema is an option but not the only one
                   new Selector(
                       new Sequence(new LambdaCondition(() =>
                           {
                               float time = blackboard.Get<float>("timeOfDay");
                               return (time >= 10 && time <= 23.99f); // cinema opens from 10 to 24.
                           }),
                           new RandomSelector(goCinema, goCinema, goCinema,watchNetflix, watchNetflix, readChapter, playCasualGame)
                       ),
                       new RandomSelector(watchNetflix, watchNetflix, readChapter, playCasualGame) // cinema is not an option.
                   )
               )
           );
        }
    }

    class BT_UseBathroom : BehaviourTree
    {
        override public void OnConstruction()
        {
            root = new Sequence(
                new ACTION_DebugLog("taking a leak..."),
                new ACTION_ShowInfo("taking a leak..."),
                new ACTION_WaitRealTime("10f"),
                new LambdaAction(() =>
                {
                    ((SIM_Blackboard)blackboard).bladder = 0;
                    return Status.SUCCEEDED;
                }) 
            );
        }
    }

    
    // --- Other actions --- 
    
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
            minutesElapsed += bl.deltaMinutes;
            if (minutesElapsed >= time) return Status.SUCCEEDED;
            else return Status.RUNNING;
        }
    }

    private class ACTION_ShowInfo : Action
    {
        private string message;
        public ACTION_ShowInfo(string message)
        {
            this.message = message;
        }
        
        public override Status OnTick()
        {
            SIM_Blackboard bl = (SIM_Blackboard)blackboard;
            OnScreenInfo panel = bl.onScreenInfo;
            // DayNightCycle2D dayNightCycle = bl.dayNightCycler.GetComponent<DayNightCycle2D>();
            DayNightCycle2D dayNightCycler = bl.dayNightCycler;
            int hours = dayNightCycler.hours;
            int minutes = (int)dayNightCycler.minutes;
            string day = dayNightCycler.currentDay.ToString();
            panel.InjectInfo("["+day+" "+hours.ToString("D2") + ":" + minutes.ToString("D2") + "] " + message + "\n", true);
            return Status.SUCCEEDED;
        }
    }
}