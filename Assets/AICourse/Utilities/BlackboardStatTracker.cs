
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// --- NEW: Enum to define the data type ---
public enum BlackboardDataType
{
    Float,
    Int,
    String,
    Bool
}

[System.Serializable]
public struct StatMapping
{
    [Tooltip("The exact variable name in the DynamicBlackboard (e.g., HUNGER, ENERGY)")]
    public string blackboardKey; 
    public StatBar uiBar;
}

[System.Serializable]
public class TextMapping
{
    [Tooltip("The exact variable name in the DynamicBlackboard (e.g., COINS, SCORE)")]
    public string blackboardKey; 

    [Tooltip("The text component (supports both UI Canvas and World Space TMP)")]
    public TMP_Text textComponent;

    // --- NEW: Dropdown in the Inspector to choose the type ---
    [Tooltip("What kind of data is stored in the blackboard for this key?")]
    public BlackboardDataType dataType = BlackboardDataType.Float;

    [Tooltip("Display format. Use {0} for the value. Examples: 'Gold: {0}', '{0}%', or just '{0}'")]
    public string textFormat = "{0}";
}

public class BlackboardStatTracker : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The blackboard to monitor. If null, it will search for one on this GameObject.")]
    public DynamicBlackboard blackboard;

    [Header("Stats to Display (UI Bars)")]
    [Tooltip("List of bars to be updated from the blackboard")]
    public List<StatMapping> statsToTrack = new List<StatMapping>();

    [Header("Stats to Display (Simple Texts)")]
    [Tooltip("List of text components to be updated from the blackboard")]
    public List<TextMapping> textsToTrack = new List<TextMapping>();

    void Start()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<DynamicBlackboard>();
        }

        // --- UI BARS INITIALIZATION ---
        foreach (var stat in statsToTrack)
        {
            if (stat.uiBar != null)
            {
                float min, max, currentValue;
                blackboard.TryGetRange(stat.blackboardKey, out min, out max);

                if (blackboard.Exists(stat.blackboardKey))
                {
                    currentValue = blackboard.Get<float>(stat.blackboardKey);
                }
                else
                {
                    currentValue = max;
                    Debug.LogWarning($"The key {stat.blackboardKey} does not exist in the Blackboard yet.");
                }

                stat.uiBar.InitializeBar(stat.blackboardKey, currentValue, max);
            }
        }

        // --- TEXT INITIALIZATION ---
        foreach (var textMap in textsToTrack)
        {
            if (textMap != null && string.IsNullOrEmpty(textMap.textFormat))
            {
                textMap.textFormat = "{0}"; 
            }
        }
    }

    void Update()
    {
        if (blackboard == null) return;

        // --- UI BARS UPDATE ---
        foreach (var stat in statsToTrack)
        {
            if (stat.uiBar != null && blackboard.Exists(stat.blackboardKey))
            {
                float min, max;
                blackboard.TryGetRange(stat.blackboardKey, out min, out max);
                float currentValue = blackboard.Get<float>(stat.blackboardKey);
                stat.uiBar.UpdateBar(currentValue, max);
            }
        }

        // --- TEXT UPDATE (NOW WITH TYPE CHECKING) ---
        foreach (var textMap in textsToTrack)
        {
            if (textMap.textComponent != null && blackboard.Exists(textMap.blackboardKey))
            {
                // We use a switch to ask the blackboard for the exact correct type
                switch (textMap.dataType)
                {
                    case BlackboardDataType.Float:
                        float fVal = blackboard.Get<float>(textMap.blackboardKey);
                        textMap.textComponent.text = string.Format(textMap.textFormat, fVal);
                        break;
                        
                    case BlackboardDataType.Int:
                        int iVal = blackboard.Get<int>(textMap.blackboardKey);
                        textMap.textComponent.text = string.Format(textMap.textFormat, iVal);
                        break;
                        
                    case BlackboardDataType.String:
                        string sVal = blackboard.Get<string>(textMap.blackboardKey);
                        textMap.textComponent.text = string.Format(textMap.textFormat, sVal);
                        break;

                    case BlackboardDataType.Bool:
                        bool bVal = blackboard.Get<bool>(textMap.blackboardKey);
                        textMap.textComponent.text = string.Format(textMap.textFormat, bVal);
                        break;
                }
            }
        }
    }
}