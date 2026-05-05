using System;
using UnityEngine;

public class SIM_Blackboard : DynamicBlackboard
{
    [Header("Needs")] 
    // needs set to values appropriate for Monday at 8 in the morning
    [Range(0, 100)] public float sleepiness = 4.1f;   //
    [Range(0, 100)] public float hunger = 55f;       //
    [Range(0, 100)] public float boredom = 2f;      //
    [Range(0f, 100f)] public float bladder = 99f;    //  

    [Header(" Environmental Factors ")] 
    [Range(0, 24)]  public float timeOfDay; // value comes from DayNightCycler
    [Range(1, 7)] public int dayOfWeek; // value comes from DayNightCycler

    // public GameObject dayNightCycler;
    public DayNightCycle2D dayNightCycler;  // in the inspector set it to the game object containing the DayNightCycle2D script

    [Header("Durations and effects")] 
    public float hoursToFullBladder = 6;
    public float hoursToFullBoredom = 48;
    public float hoursToFullSleepiness = 20;
    public float hoursToFullHunger = 8;
    public float hoursToZeroSleepiness = 8;  // it takes 8 hours to restore sleepiness to 0
    
    [Header("Thresholds")]
    [Tooltip("If hunger is below this value, the Sim will only eat a snack.")]
    public float snackThreshold = 50f;
    [Tooltip("If boredom is below this value, the Sim will choose short activities.")]
    public float boredomTolerance = 30f;
    
    [Header("Known Locations")] 
    public GameObject home;
    public GameObject office;
    public GameObject supermarket;
    public GameObject restaurant;
    public GameObject cinema;

    [Header("Current Location")] 
    public string currentLocationName = "HOME"; // externally set after going somewhere
    
    [Header("UI")]
    public OnScreenInfo onScreenInfo; // ACTION_Info uses this component to display info
    
    [Header("Other")]
    public bool sleeping = false;
    
    // simplify access to deltas 
    public float deltaHours
    {
        get { return dayNightCycler.deltaHours; }
    }
    public float deltaMinutes
    {
        get { return dayNightCycler.deltaMinutes; }
    }

    void Awake()
    {
        timeOfDay = dayNightCycler.decimalTime;
        dayOfWeek = dayNightCycler.dayOfWeekNumber;
    }
    
    void Update()
    {
        timeOfDay = dayNightCycler.decimalTime;
        dayOfWeek = dayNightCycler.dayOfWeekNumber;
        UpdateSleepiness();
        UpdateHunger();
        UpdateBoredom();
        UpdateBladder();
    }

    public void UpdateSleepiness()
    {
        // it takes hoursToFullSleepiness be fully sleepy
        // and it takes hoursToZeroSleepiness to restore sleepiness to 0

        if (sleeping) 
            sleepiness -= (deltaHours / hoursToZeroSleepiness) * 100;  
        else 
            sleepiness += (deltaHours / hoursToFullSleepiness)*100;
        
        sleepiness = Mathf.Clamp(sleepiness, 0f, 100f);
    }

    public void UpdateBoredom()
    {
        // hoursToFullBoredom to full boredom
        boredom += (deltaHours / hoursToFullBoredom)*100;
        boredom = Mathf.Clamp(boredom, 0f, 100f);
    }

    public void UpdateHunger()
    {
        // During daytime, it takes hoursToFullHunger to be fully hungry
        // During nighttime, it takes longer
        if(timeOfDay>7 && timeOfDay<22)
            hunger += (deltaHours / hoursToFullHunger)*100;
        else 
            hunger += (deltaHours / hoursToFullHunger)*40;
        
        hunger = Mathf.Clamp(hunger, 0f, 100f);
    }

    public void UpdateBladder()
    {
        // it takes hoursToFullBladder to have a full bladder
        bladder += (deltaHours / hoursToFullBladder)*100;
        bladder = Mathf.Clamp(bladder, 0f, 100f);
    }

    // ------------------- ACTION RELATED

    public void StartSleeping()
    {
        sleeping = true;
    }

    public void EndSleeping()
    {
        sleeping = false;
    }

    public void EatFullMealEffect()
    {
        hunger = 0;
    }

    public void EatSnackEffect()
    {
        hunger = hunger - 33;
        hunger = Mathf.Clamp(hunger, 0, 100);
    }

    public void ReadChapterEffect()
    {
      // decreases the equivalent of fifteen minutes of boredom (0.25 hours = 15 minutes)
      boredom -= (0.25f / hoursToFullBoredom) * 100;
      boredom = Mathf.Clamp(boredom, 0f, 100f);
    }
    public void PlayCasualGameEffect()
    {
        // decreases the equivalent of fifteen minutes of boredom
    boredom -= (0.25f / hoursToFullBoredom) * 100;
    boredom = Mathf.Clamp(boredom, 0f, 100f);
    }

    public void WatchFilmCinemaEffect()
    {
        // best against boredom
        boredom = 0;
    }

    public void WatchNetflixEffect()
    {
        // decreases the equivalent of one hour of boredom
        boredom -= (1f / hoursToFullBoredom) * 100;
        boredom = Mathf.Clamp(boredom, 0f, 100f);
    }
}