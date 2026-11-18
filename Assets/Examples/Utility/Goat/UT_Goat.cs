
using UnityEngine;
using Utility;
using BTs;
using MotorBehaviours;


[CreateAssetMenu(fileName = "UT_Goat", menuName = "Utility/UT_Goat", order = 1)]
public class UT_Goat : UtilityActionSet
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
        eatScorer.AddConsideration(new Consideration("hunger", Curves.MILD_EXPONENTIAL, "hunger/eat consid."));
        eatScorer.AddConsideration(new Consideration("DistanceToCabbage", Curves.INVERTED_AGGRESSIVE_EXPONENTIAL, "dist cabbage consid."));
        
        // --- SLEEPING (energy, panicLevel)
        Scorer sleepScorer = new Scorer("SleepScorer", AggregationPolicy.ADJUSTED_MULTIPLY);
        sleepScorer.AddConsideration(new Consideration("energy", Curves.InvertedSigmoid(20, 0.3f), "energy consid."));
        sleepScorer.AddConsideration(new Consideration("panicLevel", Curves.INVERTED_AGGRESSIVE_LOGARITHMIC, "panic/sleep consid"));
        // uncomment the following line for "hungry goats don't sleep" (they rather find food)
        // sleepScorer.AddConsideration(new Consideration("hunger", Curves.InverseLinear, "hunger/sleep consid"));
        
        // --- FLEEING (panicLevel)
        Consideration fleeingConsideration = new Consideration("panicLevel", Curves.AGGRESSIVE_LOGARITHMIC, "panic/flee consid.");
        
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
        SetInertia(eat, 0.10f); // commitment to eating is quite high 
        SetInertia(sleep, 0.2f); // sleep has a considerable inertia
        // fleeing has no inertia
        // wandering has no inertia
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

    private SB_Evade evade;
    private MotorManager motorManager;

    public override void OnInitialize()
    {
        evade = GetComponent<SB_Evade>();
        if (evade == null) evade = AddComponent<SB_Evade>();

        motorManager = GetComponent<MotorManager>();
        motorManager.maxSpeed *= 3;
        motorManager.maxForce *= 9;
        
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
        motorManager.maxSpeed /= 3;
        motorManager.maxForce /= 9;
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


class BT_GoatConstrainedWander : BehaviourTree
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
                    GetComponent<SB_Wander>().attractionWeight = 0.5f;
                    return Status.SUCCEEDED;
                }),
                new ACTION_Wander()
           )
        ); // first child ends here
        
        root.AddChild( new CONDITION_AlwaysTrue(), 
            new Sequence(
                new LambdaAction(() =>
                {
                    GetComponent<SB_Wander>().attractionWeight = 0.05f;
                    return Status.SUCCEEDED;
                }),
                new ACTION_Wander()
            )
        ); // second child ends here
    }
}