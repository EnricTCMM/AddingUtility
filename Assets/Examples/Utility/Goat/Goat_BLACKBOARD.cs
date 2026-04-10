using System;
using UnityEngine;

public class Goat_BLACKBOARD : DynamicBlackboard
{
    /* REMEMBER: values susceptible of usage in Utility Considerations should be "Ranged" with
     [Range(min, max)]
     Although 0-100 is quite common, other ranges are possible.
     */
    
    public const float cabbageDetectionRadius = 99f; // beware: used in a range 
    
    [Header("Internal Needs and states")]
    [Range(0f, 100f)] public float energy = 100f;
    [Range(0f, 100f)] public float hunger = 0f;
    [Range(0f, 100f)] public float panicLevel = 0f;

    [Header("Environmental Factors")]
    [Range(0f, cabbageDetectionRadius+1)] public float DistanceToCabbage = 100f; // 100 means no cabbage around
    [Range(0f, 100f)] public float DistanceToPredator = 100f;  // 100 means no predator around

    [Header("Base Drives")]
    [Range(0f, 1f)] public float WanderDrive = 0.15f;

    [Header("Other stuff")] 
    public float relaxRadius = 80;
    public float nonRelaxRadius = 140;
    public float hungerDecrementPerCabbage = 33;
    public float energyIncrementPerSecond = 6; // energy increases 6 units per second when sleeping
    public float panicDecreaseFactor = 20; // panic level decreases 20 units per second
    public GameObject sleepParticleSystem;
    public GameObject centre;
    public GameObject predator; 
    public GameObject cabbage;

    private GameObject visiblePredator;
    private bool lastTime = false;
    
    private void Start()
    {
        sleepParticleSystem = transform.Find("SleepParticleSystem").gameObject;
    }

    void Update()
    {
        // Internal needs change constantly
        energy -= Time.deltaTime * 1.5f; 
        hunger += Time.deltaTime * 2.0f;

        energy = Mathf.Clamp(energy, 0f, 100f);
        hunger = Mathf.Clamp(hunger, 0f, 100f);
        
        // Continous monitoring of the environment
        cabbage = SensingUtils.FindInstanceWithinRadius(gameObject, "CABBAGE", 99f);
        if (cabbage == null) DistanceToCabbage = cabbageDetectionRadius+1;
        else DistanceToCabbage = SensingUtils.DistanceToTarget(gameObject, cabbage);
        // no need to clamp since detection radius is 99
        
        visiblePredator = SensingUtils.FindInstanceWithinRadius(gameObject, "PREDATOR", 99f);
        if (visiblePredator != null)
        {
            // predator is visible. Maximum PANIC!!!
            predator = visiblePredator;
            DistanceToPredator = SensingUtils.DistanceToTarget(gameObject, predator);
            panicLevel=100f;
        }
        else if (predator != null)
        {
            // predator is not visible. Reduce PANIC
            DistanceToPredator = 100f; // equivalent to no predator around
            panicLevel -= Time.deltaTime * panicDecreaseFactor;
            panicLevel = Mathf.Clamp(panicLevel, 0f, 100f);
            if (panicLevel<=0) 
                predator = null;
        }
    }

    public void EatCabbage()
    {
        hunger = Mathf.Clamp(hunger - hungerDecrementPerCabbage, 0f, 100f);
    }

    public void Sleep()
    {
        energy = Mathf.Clamp(energy + energyIncrementPerSecond*Time.deltaTime, 0f, 100f);
    }

    public bool CentreAnxious()
    {
        if (SensingUtils.DistanceToTarget(gameObject, centre) > nonRelaxRadius)
        {
            lastTime = true;
        }
        if (SensingUtils.DistanceToTarget(gameObject, centre) < relaxRadius)
        {
            lastTime = false;
        }
        return lastTime;
    }
}
