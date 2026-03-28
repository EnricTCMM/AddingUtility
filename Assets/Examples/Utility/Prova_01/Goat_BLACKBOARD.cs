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
    [Range(0f, 100f)] public float DangerLevel = 0f;         // 0 is safe, 100 means a predator is near

    [Header("Base Drives")]
    [Range(0f, 1f)] public float WanderDrive = 0.15f; 

    void Update()
    {
        // Internal needs change constantly
        Energy -= Time.deltaTime * 1.5f; 
        Hunger += Time.deltaTime * 2.0f;

        // Simulating environmental changes for testing purposes
        // In a real game, sensors or colliders would update these values
        DistanceToCabbage = Mathf.PingPong(Time.time * 10f, 100f); // Moves between 0 and 100
        
        // Danger spikes every 15 seconds to simulate a predator passing by
        if (Time.time % 15f < 2f) DangerLevel = 80f; 
        else DangerLevel = Mathf.Max(0, DangerLevel - Time.deltaTime * 10f); // Cools down slowly

        Energy = Mathf.Clamp(Energy, 0f, 100f);
        Hunger = Mathf.Clamp(Hunger, 0f, 100f);
    }
}
