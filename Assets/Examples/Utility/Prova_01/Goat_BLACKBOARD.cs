using System;
using UnityEngine;

public class Goat_BLACKBOARD : DynamicBlackboard
{
    /* REMEMBER: values susceptible of usage in Utility Considerations should be "Ranged" with
     [Range(min, max)]
     Although 0-100 is quite common, other ranges are possible.
     */
    
    [Header("Internal Needs")]
    [Range(0f, 100f)] public float Energy = 100f;
    [Range(0f, 100f)] public float Hunger = 0f;

    [Header("Environmental Factors")]
    [Range(0f, 100f)] public float DistanceToCabbage = 100f; // 100 means no cabbage around
    [Range(0f, 100f)] public float DistanceToPredator = 100f;  // 100 means no predator around

    [Header("Base Drives")]
    [Range(0f, 1f)] public float WanderDrive = 0.15f;

    [Header("Other stuff")] 
    public float hungerDecrementPerCabbage = 33;
    public float energyIncrementPerSecond = 3;
    public GameObject sleepParticleSystem;
    public GameObject predator; 
    public GameObject cabbage;

    private void Start()
    {
        sleepParticleSystem = transform.Find("SleepParticleSystem").gameObject;
    }

    void Update()
    {
        // Internal needs change constantly
        Energy -= Time.deltaTime * 1.5f; 
        Hunger += Time.deltaTime * 2.0f;

        Energy = Mathf.Clamp(Energy, 0f, 100f);
        Hunger = Mathf.Clamp(Hunger, 0f, 100f);
        
        // Continous monitoring of the environment
        cabbage = SensingUtils.FindInstanceWithinRadius(gameObject, "CABBAGE", 99f);
        if (cabbage == null) DistanceToCabbage = 100f;
        else DistanceToCabbage = SensingUtils.DistanceToTarget(gameObject, cabbage);
        // no need to clamp since detection radius is 99
        
        predator = SensingUtils.FindInstanceWithinRadius(gameObject, "PREDATOR", 99f);
        if (predator == null) DistanceToPredator = 100f;
        else DistanceToPredator = SensingUtils.DistanceToTarget(gameObject, predator);
        // no need to clamp since detection radius is 99
    }

    public void EatCabbage()
    {
        Hunger = Mathf.Clamp(Hunger - hungerDecrementPerCabbage, 0f, 100f);
    }

    public void Sleep()
    {
        Energy = Mathf.Clamp(Energy + energyIncrementPerSecond*Time.deltaTime, 0f, 100f);
    }
}
