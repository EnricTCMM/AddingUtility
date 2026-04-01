
using System.Collections.Generic;
using UnityEngine;

// This struct allows the Inspector to show pairs of "Blackboard Key" and "UI Bar"
[System.Serializable]
public struct StatMapping
{
    [Tooltip("The exact variable name in the DynamicBlackboard (e.g., HUNGER, ENERGY, HEALTH)")]
    public string blackboardKey; 
    public StatBar uiBar;
}

public class BlackboardStatTracker : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Set it if you want to use a different blackboard than the one on the same GameObject")]
    public DynamicBlackboard blackboard;

    [Header("Stats to Display")]
    [Tooltip("Add as many bars as you want to track from the blackboard")]
    public List<StatMapping> statsToTrack = new List<StatMapping>();

    void Start()
    {
        // If the blackboard is not assigned, look for it on the same GameObject
        if (blackboard == null)
        {
            blackboard = GetComponent<DynamicBlackboard>();
        }

        // Initialize all the bars configured in the list
        foreach (var stat in statsToTrack)
        {
            if (stat.uiBar != null)
            {
                float min, max, currentValue;

                // Get the range (the DynamicBlackboard returns 0 and 100 if no [Range] attribute is found)
                blackboard.TryGetRange(stat.blackboardKey, out min, out max);

                // Check if the key exists before attempting to read it
                if (blackboard.Exists(stat.blackboardKey))
                {
                    currentValue = blackboard.Get<float>(stat.blackboardKey);
                }
                else
                {
                    currentValue = max; // If it hasn't been created yet, assume it is at maximum
                    Debug.LogWarning($"The key {stat.blackboardKey} does not exist in the Blackboard yet.");
                }

                // Configure the UI bar with the blackboard key as the label name
                stat.uiBar.InitializeBar(stat.blackboardKey, currentValue, max);
            }
        }
    }

    void Update()
    {
        if (blackboard == null) return;

        // Update the bars every frame by reading the DynamicBlackboard
        foreach (var stat in statsToTrack)
        {
            // Only update if we have an assigned bar and the value exists in the blackboard
            if (stat.uiBar != null && blackboard.Exists(stat.blackboardKey))
            {
                float min, max;
                blackboard.TryGetRange(stat.blackboardKey, out min, out max);
                
                float currentValue = blackboard.Get<float>(stat.blackboardKey);
                
                stat.uiBar.UpdateBar(currentValue, max);
            }
        }
    }
}