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
    [Range(0, 1)] public int clientsQuarreling = 0;
    [Range(0, 100)] public int tankardsInBarrel = 0;
    [Range(0, 1)] public float baselineUtility = 0.1f;
    */
    
    public override void OnConstruction()
    {
        Action serveBeer = new ACTION_Succeed();
        Scorer serveBeerScorer = new Scorer("ServeBeerScorer", AggregationPolicy.ADJUSTED_MULTIPLY);
        // the more clients waiting for beer, the more urgent it is to serve them
        serveBeerScorer.AddConsideration(new Consideration("clientsWaitingBeer", Curves.AGGRESSIVE_LOGARITHMIC, "clientsWaitingBeer"));
        // zero tankards makes serving beer impossible. Else go ahead and serve
        serveBeerScorer.AddConsideration(new Consideration("tankardsInBarrel",
            (x) => { return x == 0 ? 0 : 1;}, "tankardsInBarrel"));
        // A quarrel makes any other activity almost impossible
        serveBeerScorer.AddConsideration(new Consideration("clientsQuarreling", Curves.Linear, "clientsQuarreling"));
        Bind(serveBeer, serveBeerScorer);
        SetInertia(serveBeer, 0.5f); // high inertia for serving beer
        
        Action cleanTables = new ACTION_Succeed();
        Scorer cleanTablesScorer = new Scorer("CleanTablesScorer", AggregationPolicy.ADJUSTED_MULTIPLY);
        // the more dirty tables, the more urgent it is to clean them
        cleanTablesScorer.AddConsideration(new Consideration("dirtyTables", Curves.Linear, "dirtyTables"));
        // A quarrel makes any other activity almost impossible
        cleanTablesScorer.AddConsideration(new Consideration("clientsQuarreling", Curves.Linear, "clientsQuarreling"));
        Bind(cleanTables, cleanTablesScorer);
        SetInertia(cleanTables, 0.2f); // mid inertia for cleaning tables

        Action refill = new ACTION_Succeed();
        Scorer refillScorer = new Scorer("RefillScorer", AggregationPolicy.ADJUSTED_MULTIPLY);
        // having the barrel full is important to keep the bussiness going
        refillScorer.AddConsideration(new Consideration("tankardsInBarrel", Curves.INVERTED_AGGRESSIVE_EXPONENTIAL, "tankardsInBarrel"));
        // A quarrel makes any other activity almost impossible
        refillScorer.AddConsideration(new Consideration("clientsQuarreling", Curves.Linear, "clientsQuarreling"));
        Bind(refill, refillScorer);
        SetInertia(refill, 0.6f); // high inertia for refilling the barrel
        
        Action kickOutDrunkards = new ACTION_Succeed();
        Scorer kickOutDrunkardsScorer = new Scorer("KickOutDrunkardsScorer", AggregationPolicy.ADJUSTED_MULTIPLY);
        // the more drunkards sleeping, the more urgent it is to kick them out
        kickOutDrunkardsScorer.AddConsideration(new Consideration("sleepingDrunkards", Curves.MEDIUM_EXPONENTIAL, "sleepingDrunkards"));
        // A quarrel makes any other activity almost impossible
        kickOutDrunkardsScorer.AddConsideration(new Consideration("clientsQuarreling", Curves.Linear, "clientsQuarreling"));
        Bind(kickOutDrunkards, kickOutDrunkardsScorer);
        SetInertia(kickOutDrunkards, 0.4f); // once started, end the job 
        
        Action stopQuarreling = new ACTION_Succeed();
        Scorer stopQuarrelingScorer = new Scorer("StopQuarrelingScorer", AggregationPolicy.ADJUSTED_MULTIPLY);
        stopQuarrelingScorer.AddConsideration(new Consideration("clientsQuarreling", Curves.Linear, "clientsQuarreling"));
        Bind(stopQuarreling, stopQuarrelingScorer);
        // this action does not need inertia. It's utility is 0 or 1. If it starts it will go uniterrupted to the end.
        
        Action contemplateUniverse = new ACTION_Succeed();
        Consideration con = new Consideration("baselineUtility", Curves.Linear, "baselineUtility");
        Bind(contemplateUniverse, con);
        // this action does not need inertia. It's just a fallback.
        
    }
}