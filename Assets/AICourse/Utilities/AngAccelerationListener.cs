using UnityEngine;
using MotorBehaviours;


public class AngAccelerationListener : MonoBehaviour
{
    private MotorManager ks;

    public void Awake()
    {
        ks = GetComponent<MotorManager>();
    }

    public void AcceptInput(float userInput)
    {
        ks.maxTorque  = userInput;
    }
}
