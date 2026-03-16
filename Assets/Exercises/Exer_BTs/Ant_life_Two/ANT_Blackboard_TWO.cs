using UnityEngine;

// renamed to ANT_Blackboard_TWO for there's another ANT_Blackboard 
// used in another exercise.

public class ANT_Blackboard_TWO : DynamicBlackboard
{
    public GameObject attractor;
    public GameObject nest;
    public float safeRadius = 40;
    public float extraSafeRadius = 20;
    public float lowSW = 0.2f;
    public float highSW = 0.8f;
    public float seedDetectionRadius = 60;
}
