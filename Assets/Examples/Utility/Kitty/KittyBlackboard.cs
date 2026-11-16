using UnityEngine;

using UnityEngine;

// This blackboard inherits from DynamicBlackboard to expose its fields 
// to the Utility AI system via reflection.
public class KittyBlackboard : DynamicBlackboard
{
    // The Range attribute is strictly required here so the base class 
    // can extract the min/max values during Initialize() and provide them 
    // to the Utility Considerations for normalization.
    [Range(0f, 100f)]
    public float hunger = 0f;

    [Range(0f, 100f)]
    public float fatigue = 0f;

    [Range(0f, 100f)]
    public float boredom = 0f;

    [Header("Degradation Rates (Units per second)")]
    public float hungerRate = 2.5f;
    public float fatigueRate = 1.0f;
    public float boredomRate = 4.0f;

    [Header("Places")] 
    public GameObject food;
    public GameObject mat;
    public GameObject wool;
    

    private void Update()
    {
        // Simulate the passage of time by modifying the kitty's needs.
        // We use Clamp to ensure they never exceed the maximum range defined (100f).
        hunger = Mathf.Clamp(hunger + hungerRate * Time.deltaTime, 0f, 100f);
        fatigue = Mathf.Clamp(fatigue + fatigueRate * Time.deltaTime, 0f, 100f);
        boredom = Mathf.Clamp(boredom + boredomRate * Time.deltaTime, 0f, 100f);
    }

    public void RestABit()
    {
        fatigue = Mathf.Clamp(fatigue - 4*fatigueRate * Time.deltaTime, 0f, 100f);
    }
    
    public void PlayABit()
    {
        boredom = Mathf.Clamp(boredom - 6*boredomRate * Time.deltaTime, 0f, 100f);
    }
}