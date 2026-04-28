using UnityEngine;

public class SIM_Blackboard : DynamicBlackboard
{
    [Header("Needs")] 
    [Range(0, 100)] public float sleepiness = 0f;   //
    [Range(0, 100)] public float hunger = 0f;       //
    [Range(0, 100)] public float boredom = 0f;      //
    [Range(0f, 100f)] public float bladder = 0f;    //  

    [Header(" Environmental Factors ")] 
    [Range(0, 24)]  public float timeOfDay = 12f; // value comes from DayNightCycler
    [Range(1, 7)] public int dayOfWeek = 1; // value comes from DayNightCycler

    public GameObject dayNightCycler;
    private DayNightCycle2D dayNightCycle;

    [Header("Effects")]
    public float sleepRestorePerHour = 100 / 8;  // sleeping an hour restores 1/8 (full restoration takes 8 hours)


    [Header("Locations")] 
    public GameObject home;
    public GameObject office;
    public GameObject supermarket;
    public GameObject restaurant;
    public GameObject cinema;

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
        // it takes half day (12 hours) to be fully sleepy
        sleepiness += (deltaHours / 12)*100; 
    }

    public void UpdateBoredom()
    {
        // two days (48 hours) to full boredom
        boredom += (deltaHours / 48)*100;
    }

    public void UpdateHunger()
    {
        // it takes 12 hours to be fully hungry
        hunger += (deltaHours / 12)*100;
    }

    public void UpdateBladder()
    {
        // it takes 6 hours to have a full bladder
        bladder += (deltaHours / 6)*100;
    }

    // ------------------- ACTION RELATED

    public void Sleep () { 
        // sleeping decreases sleepiness 
        sleepiness -= deltaHours*sleepRestorePerHour;
    }
}