using UnityEngine;
using Steerings;


public class AngAccelerationListener : MonoBehaviour
{
    private SteeringContext ks;

    public void Awake()
    {
        ks = GetComponent<SteeringContext>();
    }

    public void AcceptInput(float userInput)
    {
        ks.maxAngularAcceleration  = userInput;
    }
}
