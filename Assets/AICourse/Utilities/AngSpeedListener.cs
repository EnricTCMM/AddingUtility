using UnityEngine;
using MotorBehaviours;

public class AngSpeedListener : MonoBehaviour
{
    private MotorManager ks;

    public void Awake()
    {
        ks = GetComponent<MotorManager>();
       
    }

    public void AcceptInput(float userInput)
    {
        ks.maxAngularSpeed = userInput;
    }
}
