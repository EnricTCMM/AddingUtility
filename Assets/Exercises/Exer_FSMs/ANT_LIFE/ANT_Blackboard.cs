
using UnityEngine;

public class ANT_Blackboard : MonoBehaviour
{
    [Header("Two point wandering")]
    public GameObject locationA;
    public GameObject locationB;

    public float intervalBetweenTimeOuts = 10f;
    public float initialAttractorWeight = 0.2f;
    public float attractorWeightIncrement = 0.2f;
    public float locationReachedRadius = 10f;

    [Header("Seed colecting")]
    public GameObject nest;
    public float seedDectionRadius = 100;
    public float seedReachedRadius = 5;
    public float nestReachedRadius = 20;

    [Header("Peril Fleeing")] 
    public float predatorDetectionRadius = 50;
    public float predatorFarEnough = 200;

    void Start()
    {
       
    }

   
}
