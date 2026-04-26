using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle2D : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("How many in-game minutes pass per real-life second.")]
    public float timeMultiplier = 60f; 
    
    [Tooltip("Current day of the week.")]
    public DayOfWeek currentDay = DayOfWeek.Monday;
    [Range(1, 7)]
    public int dayOfWeekNumber = 1;
    public bool showDayOfWeek = true;
    
    [Tooltip("Current hour (0-23).")]
    [Range(0, 23)] public int hours = 12;

    [Tooltip("Current minute (0-59).")]
    [Range(0, 59)] public float minutes = 0f;

    [Tooltip("Current time in decimal format (0-24).")]
    [Range(0, 24)] public float decimalTime;
    
    [Tooltip("Time deltas for calculations.")]
    public float deltaMinutes;  // how many minutes have passed since last update
    public float deltaHours; // how many hours have passed since last update

    [Header("Light Settings")]
    public Light2D globalLight;
    public Gradient lightColor;

    [Header("Camera Settings")]
    [Tooltip("If true, the script will also change the camera background color.")]
    public bool updateCameraBackground = true;
    public Camera targetCamera;
    
    [Header("UI Settings")]
    [Tooltip("If empty, it will search in children on Awake.")]
    public TextMeshProUGUI clockText;

    private void Awake()
    {
        // 1. Search for the Global Light 2D in children
        if (globalLight == null)
        {
            globalLight = GetComponentInChildren<Light2D>();
            if (globalLight == null)
            {
                Debug.LogWarning("DayNightCycle2D: No Global Light 2D found on " + gameObject.name + " or its children!");
            }
        }

        // 2. Search for the TextMeshProUGUI in children
        if (clockText == null)
        {
            clockText = GetComponentInChildren<TextMeshProUGUI>();
            if (clockText == null)
            {
                Debug.LogWarning("DayNightCycle2D: No TextMeshProUGUI found on " + gameObject.name + " or its children!");
            }
        }

        // 3. Find the main camera
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void Update()
    {
        AdvanceTime();
        UpdateLightingAndCamera();
        UpdateUI();
    }

    private void AdvanceTime()
    {
        minutes += Time.deltaTime * timeMultiplier;

        if (minutes >= 60f)
        {
            minutes -= 60f; 
            hours++;
        }

        if (hours >= 24)
        {
            hours = 0;
            AdvanceDay();
        }

        decimalTime = hours + (minutes / 60f);
        
        // deltas
        deltaMinutes = Time.deltaTime * timeMultiplier;
        deltaHours = deltaMinutes / 60f;
    }
    
    private void AdvanceDay()
    {
        // Cast the current day to an integer, add 1, and loop back if it exceeds Sunday (6)
        int nextDayIndex = (int)currentDay + 1;
        
        if (nextDayIndex > 6)
        {
            nextDayIndex = 0; // Back to Monday
        }
        
        dayOfWeekNumber = nextDayIndex+1;
        
        // Cast the integer back to the DayOfWeek enum
        currentDay = (DayOfWeek)nextDayIndex;
    }
    

    private void UpdateLightingAndCamera()
    {
        float timePercent = decimalTime / 24f;
        Color currentColor = lightColor.Evaluate(timePercent);

        // Update Global Light
        if (globalLight != null)
        {
            globalLight.color = currentColor;
        }

        // Update Camera Background
        if (updateCameraBackground && targetCamera != null)
        {
            targetCamera.backgroundColor = currentColor;
        }
    }
    
    private void UpdateUI()
    {
        if (clockText != null)
        {
            if (showDayOfWeek)
            {
                string timeString = hours.ToString("00") + ":" + Mathf.FloorToInt(minutes).ToString("00");
                clockText.text = currentDay.ToString() + " - " + timeString;
            }
            else
            {
                clockText.text = hours.ToString("00") + ":" + Mathf.FloorToInt(minutes).ToString("00");
            }
        }
    }
    
    //------
    
    public enum DayOfWeek 
    { 
        Monday, 
        Tuesday, 
        Wednesday, 
        Thursday, 
        Friday, 
        Saturday, 
        Sunday 
    }
}