using System;
using UnityEngine;

public class SIM_Blackboard : DynamicBlackboard
{
    [Header("Needs")] 
    // needs set to values appropriate for Monday 8 in the morning
    [Range(0, 100)] public float sleepiness = 4.1f;   //
    [Range(0, 100)] public float hunger = 95f;       //
    [Range(0, 100)] public float boredom = 2f;      //
    [Range(0f, 100f)] public float bladder = 99f;    //  

    [Header(" Environmental Factors ")] 
    [Range(0, 24)]  public float timeOfDay; // value comes from DayNightCycler
    [Range(1, 7)] public int dayOfWeek; // value comes from DayNightCycler

    public GameObject dayNightCycler;
    private DayNightCycle2D dayNightCycle;

    [Header("Durations and effects")] 
    public float hoursToFullBladder = 6;
    public float hoursToFullBoredom = 48;
    public float hoursToFullSleepiness = 12;
    public float hoursToFullHunger = 8;
    public float hoursToZeroSleepiness = 8;  // it takes 8 hours to restore sleepiness to 0
    
    [Header("Locations")] 
    public GameObject home;
    public GameObject office;
    public GameObject supermarket;
    public GameObject restaurant;
    public GameObject cinema;

    [Header("Other")]
    public bool sleeping = false;
    
    private float deltaHours
    {
        get { return dayNightCycle.deltaHours; }
    }

    private float deltaMinutes
    {
        get { return dayNightCycle.deltaMinutes; }
    }

    void Awake()
    {
        // retrieve the cycler script from the cycler game object
        dayNightCycle = dayNightCycler.GetComponent<DayNightCycle2D>();
        timeOfDay = dayNightCycle.decimalTime;
        dayOfWeek = dayNightCycle.dayOfWeekNumber;
    }
    
    void Update()
    {
        timeOfDay = dayNightCycle.decimalTime;
        dayOfWeek = dayNightCycle.dayOfWeekNumber;
        UpdateSleepiness();
        UpdateHunger();
        UpdateBoredom();
        UpdateBladder();
    }

    public void UpdateSleepiness()
    {
        // it takes hoursToFullSleepiness be fully sleepy

        if (sleeping) sleepiness -= (deltaHours / hoursToZeroSleepiness) * 100;  
        else sleepiness += (deltaHours / hoursToFullSleepiness)*100;
        
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
        // it takes hoursToFullHunger to be fully hungry
        hunger += (deltaHours / hoursToFullHunger)*100;
        hunger = Mathf.Clamp(hunger, 0f, 100f);
    }

    public void UpdateBladder()
    {
        // it takes hoursToFullBladder have a full bladder
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

    /*
    public void Sleep () { 
        // sleeping decreases sleepiness 
        sleepiness -= deltaHours*sleepRestorePerHour;
    }
    */
}