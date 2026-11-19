using UnityEngine;
using Utility;
using BTs;

[CreateAssetMenu(fileName = "UT_Landlady", menuName = "Utility/UT_Landlady", order = 1)]
public class UT_Landlady : UtilityActionSet
{
    /*
    [Range(0, 20)]  public int clientsWaitingBeer = 0;
    [Range(0, 5)] public int dirtyTables = 0;
    [Range(0, 4)] public int sleepingDrunkards = 0;
    [Range(0, 1)] public int activeQuarrels = 0;
    [Range(0, 100)] public int tankardsInBarrel = 0;
    [Range(0, 1)] public float baselineUtility = 0.1f;
    */
    
    public override void OnConstruction()
    {
        
        // ==========================================
        // 1. ACTION INSTANTIATION & NAMING
        // ==========================================
        Action serveBeer = ScriptableObject.CreateInstance<BT_ServeBeer>();
        serveBeer.Name = "SERVE BEER";
        Action cleanTables = ScriptableObject.CreateInstance<BT_CleanTable>();
        cleanTables.Name = "CLEAN TABLES";
        Action refill = ScriptableObject.CreateInstance<BT_Refill>();
        refill.Name = "REFILL";
        Action kickOutDrunkards = ScriptableObject.CreateInstance<BT_KickOutDrunkards>();
        kickOutDrunkards.Name = "KICK OUT DRUNKARDS";
        Action stopQuarrel = ScriptableObject.CreateInstance<BT_StopQuarrel>();
        stopQuarrel.Name = "STOP QUARREL";
        Action contemplateUniverse = ScriptableObject.CreateInstance<BT_Contemplate>();
        contemplateUniverse.Name = "CONTEMPLATE UNIVERSE";
        
        // ==========================================
        // 2. CONSIDERATIONS & SCORERS
        // ==========================================
        
        // --- SERVE BEER (clientsWaitingbeer, tankardsInBarrel, clientsQuarreling)
        Scorer serveBeerScorer = new Scorer("ServeBeerScorer", AggregationPolicy.MULTIPLY);
        // the more clients waiting for beer, the more urgent it is to serve them
        serveBeerScorer.AddConsideration(new Consideration("clientsWaitingBeer", Curves.AGGRESSIVE_LOGARITHMIC, "clients consid."));
        // zero tankards makes serving beer impossible (veto). Else go ahead and serve
        serveBeerScorer.AddConsideration(new Consideration("tankardsInBarrel",
            (x) => { return x == 0 ? 0 : 1;}, "tankardsInBarrel"));
        // A quarrel makes any other activity impossible (veto)
        serveBeerScorer.AddConsideration(new Consideration("activeQuarrels", Curves.InverseLinear, "quarrels/serve consid."));
        
        
        // --- CLEAN TABLE (dirtyTables, clientsQuarreling)
        Scorer cleanTablesScorer = new Scorer("CleanTablesScorer", AggregationPolicy.MULTIPLY);
        // the more dirty tables, the more urgent it is to clean them
        cleanTablesScorer.AddConsideration(new Consideration("dirtyTables", Curves.Linear, "dirtyTables"));
        // A quarrel makes any other activity almost impossible
        cleanTablesScorer.AddConsideration(new Consideration("activeQuarrels", Curves.InverseLinear, "clientsQuarreling"));
        
        
        // --- REFILL (tankardsInBarrel, clientsQuarreling)
        Scorer refillScorer = new Scorer("RefillScorer", AggregationPolicy.MULTIPLY);
        refillScorer.AddConsideration(new Consideration("tankardsInBarrel", Curves.INVERTED_AGGRESSIVE_LOGARITHMIC, "tankardsInBarrel"));
        // A quarrel makes any other activity impossible (veto)
        refillScorer.AddConsideration(new Consideration("activeQuarrels", Curves.InverseLinear, "clientsQuarreling"));
        
        
        // --- KICK OUT DRUNKARDS (sleepingDrunkards, clientsQuarreling) 
        Scorer kickOutDrunkardsScorer = new Scorer("KickOutDrunkardsScorer", AggregationPolicy.MULTIPLY);
        // the more drunkards sleeping, the more urgent it is to kick them out
        kickOutDrunkardsScorer.AddConsideration(new Consideration("sleepingDrunkards", Curves.MEDIUM_EXPONENTIAL, "sleepingDrunkards"));
        // A quarrel makes any other activity impossible (veto)
        kickOutDrunkardsScorer.AddConsideration(new Consideration("activeQuarrels", Curves.InverseLinear, "clientsQuarreling"));
        
        
        // --- STOP QUARREL (clientsQuarreling) 
        Consideration conClientsQuarreling = new Consideration("activeQuarrels", Curves.Linear, "activeQuarrels");
        // this action does not need inertia. Its utility is 0 or 1. If it starts, it will go uniterrupted to the end.

        
        // --- CONTEMPLATE THE UNIVERSE [FALLBACK ACTION]
        Consideration conBaseLine = new Consideration("baselineUtility", Curves.Linear, "baselineUtility");
        
        
        // =============================================
        // 3. BINDING ACTIONS TO CONSIDERATIONS/SCORERS
        // =============================================
        Bind(serveBeer, serveBeerScorer);
        Bind(cleanTables, cleanTablesScorer);
        Bind(refill, refillScorer);
        Bind(kickOutDrunkards, kickOutDrunkardsScorer);
        Bind(stopQuarrel, conClientsQuarreling);
        Bind(contemplateUniverse, conBaseLine);
        
        // ==========================================
        // 4. INERTIA SETUP
        // ==========================================
        SetInertia(serveBeer, 0.35f); // high inertia for serving a single beer
        SetInertia(cleanTables, 0.2f); // mid inertia for cleaning tables
        SetInertia(refill, 0.4f); // very high inertia for refilling the barrel
        SetInertia(kickOutDrunkards, 0.25f); // once started, end the job 
        // stopping a quarrel does not need inertia
        // contemplating the universe does not need inertia since it's just a fallback action
        
    }
}

class BT_Contemplate : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new Sequence(
                new ACTION_Quiet(),
                new ACTION_Speak("I'm contemplating the <color=blue> universe </color> and asking myself about the meaning of life..."),
                // this is a never-ending behaviour.
                new ACTION_RunForever()
        );
    }
}

class BT_StopQuarrel : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new Sequence(
            new ACTION_Quiet(),
            new ACTION_Speak("No quarreling in my place. I'm stopping this right now..."),
            new ACTION_WaitForSeconds("2"),
            new ACTION_Speak("Ready!"),
            new ACTION_WaitForSeconds("1"),
            new LambdaAction(() =>
            {
                ((LANDLADY_Blackboard)blackboard).decClientsQuarreling();
                return Status.SUCCEEDED;
            })
        );
    }
}

class BT_KickOutDrunkards : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new Sequence(
            new ACTION_Quiet(),
            new ACTION_Speak("Hey! this is not a sleeping place for drunkards!"),
            new ACTION_WaitForSeconds("2"),
            new RepeatUntilFailureDecorator(
                new Sequence(
                    new LambdaCondition(() =>
                    {
                        // fails (false) if all drunkards are gone. Then sequence fails and decorator succeeds
                        return ((LANDLADY_Blackboard)blackboard).sleepingDrunkards != 0;
                    }),
                    // new ACTION_Quiet(),
                    new ACTION_Speak("Out!"),
                    new ACTION_WaitForSeconds("0.5"),
                    new ACTION_Quiet(),
                    new LambdaAction(() =>
                    {
                        ((LANDLADY_Blackboard)blackboard).decSleepingDrunkards();
                        return Status.SUCCEEDED;
                    })
                ) // sequence in decorator ends here 
            ), // repeat until failure decorator ends here
            new ACTION_Speak("Ready! Got rid of all the drunkards!")
       ); // sequence ends here
    }
}

class BT_ServeBeer : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new Sequence(
            new ACTION_Quiet(),
            new ACTION_Speak("Coming with a beer to you..."),
            new ACTION_WaitForSeconds("1.5"),
            new ACTION_Speak("ready!"),
            new LambdaAction(() =>
            {
                ((LANDLADY_Blackboard)blackboard).decClientsWaitingBeer();
                ((LANDLADY_Blackboard)blackboard).decTankardsInBarrel();
                return Status.SUCCEEDED;
            })
        );
    }
}

class BT_CleanTable : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new Sequence(
            new ACTION_Quiet(),
            new ACTION_Speak("Let me clean a table..."),
            new ACTION_WaitForSeconds("2"),
            new ACTION_Speak("ready!"),
            new LambdaAction(() =>
            {
                ((LANDLADY_Blackboard)blackboard).decDirtyTables();
                return Status.SUCCEEDED;
            })
        );
    }
}

class BT_Refill : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new Sequence(
            new ACTION_Quiet(),
            new ACTION_Speak("Refilling the barrel..."),
            new RepeatUntilFailureDecorator(
                new Sequence(
                    new LambdaCondition(() =>
                    {
                        // fails (false) if the barrel is full. Then sequence fails and decorator succeeds
                        return ((LANDLADY_Blackboard)blackboard).tankardsInBarrel < 100;
                    }),
                    new LambdaAction(() =>
                    {
                        ((LANDLADY_Blackboard)blackboard).incTankardsInBarrel();
                        return Status.SUCCEEDED;
                    }),
                    new ACTION_WaitForSeconds("0.4") // refilling takes a while...
                ) // sequence in decorator ends here
            ) // repeat until failure decorator ends here
        ); // sequence ends here
    }
}
 
