using System.ComponentModel.Design.Serialization;
using UnityEngine;
using Utility;
using BTs;

[CreateAssetMenu(fileName = "UT_Goat", menuName = "Utility/UT_Goat", order = 1)]
public class UT_Goat : UtilityActionSet
{
    public override void OnConstruction()
    {
       
        
        //----- EATING
        Scorer eatScorer = new Scorer("EatScorer", AggregationPolicy.MULTIPLY);
        eatScorer.AddConsideration(new Consideration("Hunger", Curves.Linear, "Hunger Level"));
        eatScorer.AddConsideration(new Consideration("DistanceToCabbage", Curves.InverseLinear, "Distance to Cabbage"));
        Bind(new ACTION_Eat(), eatScorer);
        
        // --- SLEEPING
        Scorer sleepScorer = new Scorer("SleepScorer", AggregationPolicy.MULTIPLY);
        sleepScorer.AddConsideration(new Consideration("Energy", Curves.InverseLinear, "Energy Level"));
        sleepScorer.AddConsideration(new Consideration("DistanceToPredator", Curves.InverseLinear, "Danger Level"));
        Bind(new ACTION_Sleep(), sleepScorer);
        
        // --- FLEEING
        Consideration fleeingConsideration = new Consideration("DistanceToPredator", Curves.MEDIUM_EXPONENTIAL, "Distance to Predator");
        Bind(new ACTION_Flee("predator"), fleeingConsideration);
        
        // --- WANDERING (fallback action)
        Consideration wanderingConsideration = new Consideration("WanderDrive", Curves.Linear, "Wander Drive");
        Bind(new ACTION_WanderAround(), wanderingConsideration);
    }
}

// local actions

class ACTION_Flee : Action
{
    public string keyTarget;

    public ACTION_Flee(string keyTarget)
    {
        this.keyTarget = keyTarget;
    }

    private Steerings.Flee flee;

    public override void OnInitialize()
    {
        flee = GetComponent<Steerings.Flee>();
        if (flee == null) flee = AddComponent<Steerings.Flee>();

        flee.target = blackboard.Get<GameObject>(keyTarget);
        flee.enabled = true;
    }

    public override Status OnTick()
    {
        return Status.RUNNING;
    }

    public override void OnAbort()
    {
        flee.enabled = false;
    }
}

class ACTION_Sleep : Action
{
    private GameObject parSystem;

    public override void OnInitialize()
    {
        parSystem = blackboard.Get<GameObject>("SleepParticleSystem");
        parSystem.GetComponent<ParticleSystem>().Play();
    }
    public override Status OnTick()
    {
        ((Goat_BLACKBOARD)blackboard).Sleep();
        return Status.RUNNING;
    }
    
    public override void OnAbort()
    {
        parSystem.GetComponent<ParticleSystem>().Stop();
    }
}

class BT_Eat : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new Sequence();
        root.AddChild(new ACTION_Arrive("cabbage"));
        root.AddChild(new ACTION_Deactivate("cabbage"));
        root.AddChild(new LambdaAction(() =>
        {
            ((Goat_BLACKBOARD)blackboard).EatCabbage();
            return Status.SUCCEEDED;
        }));
    }
}

// making an action out of a Behaviour tree
class ACTION_Eat : Action
{
    private BT_Eat eatBehaviorTree;
    
    public override void OnInitialize()
    {
        eatBehaviorTree = ScriptableObject.CreateInstance<BT_Eat>();
        eatBehaviorTree.Contextualize(gameObject);
    }
    public override Status OnTick()
    {
        return eatBehaviorTree.Tick();
    }
    public override void OnAbort()
    {
        eatBehaviorTree.Abort();
    }
}
