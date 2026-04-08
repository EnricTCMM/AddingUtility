using System.ComponentModel.Design.Serialization;
using UnityEngine;
using Utility;
using BTs;
using Steerings;

[CreateAssetMenu(fileName = "UT_Goat", menuName = "Utility/UT_Goat", order = 1)]
public class UT_Goat : UtilityActionSet
{
    public override void OnConstruction()
    {
        //----- EATING
        Action eat = new ACTION_Eat();
        Scorer eatScorer = new Scorer("EatScorer", AggregationPolicy.MULTIPLY);
        eatScorer.AddConsideration(new Consideration("hunger", Curves.MILD_EXPONENTIAL, "hunger Level"));
        eatScorer.AddConsideration(new Consideration("DistanceToCabbage", Curves.INVERTED_AGGRESSIVE_EXPONENTIAL, "Distance to Cabbage"));
        Bind(eat, eatScorer);
        SetInertia(eat, 0.10f); // commitment to eating is quite high 
    
        
        // --- SLEEPING
        Action sleep = new ACTION_Sleep();
        Scorer sleepScorer = new Scorer("SleepScorer", AggregationPolicy.ADJUSTED_MULTIPLY);
        sleepScorer.AddConsideration(new Consideration("energy", Curves.InvertedSigmoid(20, 0.3f), "energy Level"));
        sleepScorer.AddConsideration(new Consideration("panicLevel", Curves.INVERTED_AGGRESSIVE_LOGARITHMIC, "panic Level"));
        // uncomment the following line for "hungry goats don't sleep" (they rather find food)
        // sleepScorer.AddConsideration(new Consideration("hunger", Curves.InverseLinear, "hunger Level"));
        Bind(sleep, sleepScorer);
        SetInertia(sleep, 0.2f); // sleep has a considerable inertia 
        
        
        // --- FLEEING
        Consideration fleeingConsideration = new Consideration("panicLevel", Curves.AGGRESSIVE_LOGARITHMIC, "Distance to Predator");
        Bind(new ACTION_Evade("predator"), fleeingConsideration);
        
        // --- WANDERING (fallback action)
        Action wander = new ACTION_ConstrainedWander();
        Consideration wanderingConsideration = new Consideration("WanderDrive", Curves.Linear, "Wander Drive");
        Bind(wander, wanderingConsideration);
        SetInertia(wander, 0); // wander has no inertia at all. It's a fallback hence interrupting it is not an issue.
    }
}

// local actions

class ACTION_Evade : Action
{
    public string keyTarget;

    public ACTION_Evade(string keyTarget)
    {
        this.keyTarget = keyTarget;
    }

    private Steerings.Evade evade;
    private SteeringContext context;

    public override void OnInitialize()
    {
        evade = GetComponent<Steerings.Evade>();
        if (evade == null) evade = AddComponent<Steerings.Evade>();

        context = GetComponent<SteeringContext>();
        context.maxSpeed *= 3;
        context.maxAcceleration *= 9;
        
        evade.target = blackboard.Get<GameObject>(keyTarget);
        evade.enabled = true;
        
        
    }

    public override Status OnTick()
    {
        return Status.RUNNING;
    }

    public override void OnAbort()
    {
        evade.enabled = false;
        context.maxSpeed /= 3;
        context.maxAcceleration /= 9;
    }
}

class ACTION_Sleep : Action
{
    private GameObject parSystemContainer;
    private ParticleSystem parSys;

    public override void OnInitialize()
    {
        parSystemContainer = blackboard.Get<GameObject>("SleepParticleSystem");
        parSys = parSystemContainer.GetComponent<ParticleSystem>();
        parSys.Play();
        var em = parSys.emission;
        em.enabled = true;
        parSystemContainer.SetActive(true);
    }
    public override Status OnTick()
    {
        ((Goat_BLACKBOARD)blackboard).Sleep();
        if (((Goat_BLACKBOARD)blackboard).energy > 99)
        {
            parSys.GetComponent<ParticleSystem>().Stop();
            parSystemContainer.SetActive(false);
            return Status.SUCCEEDED;
        }
        else 
            return Status.RUNNING;
    }
    
    public override void OnAbort()
    {
        parSys.GetComponent<ParticleSystem>().Stop();
        parSystemContainer.SetActive(false);
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

class ACTION_ConstrainedWander : Action
{
    private BT_ConstrainedWander constrainedWanderBehaviorTree;

    public override void OnInitialize()
    {
        constrainedWanderBehaviorTree = ScriptableObject.CreateInstance<BT_ConstrainedWander>();
        constrainedWanderBehaviorTree.Contextualize(gameObject);
    }
    public override Status OnTick()
    {
        return constrainedWanderBehaviorTree.Tick();
    }
    public override void OnAbort()
    {
        constrainedWanderBehaviorTree.Abort();
    }
}

class BT_ConstrainedWander : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new DynamicSelector();
        
        root.AddChild(
            new LambdaCondition(()=>
            {
                return ((Goat_BLACKBOARD)blackboard).CentreAnxious();
            }),
            new Sequence(
                new LambdaAction(() =>
                {
                    GetComponent<SteeringContext>().seekWeight = 0.8f;
                    return Status.SUCCEEDED;
                }),
                new ACTION_WanderAround()
           )
        ); // first child ends here
        
        root.AddChild( new CONDITION_AlwaysTrue(), 
            new Sequence(
                new LambdaAction(() =>
                {
                    GetComponent<SteeringContext>().seekWeight = 0.2f;
                    return Status.SUCCEEDED;
                }),
                new ACTION_WanderAround()
            )
        ); // second child ends here
    }
}