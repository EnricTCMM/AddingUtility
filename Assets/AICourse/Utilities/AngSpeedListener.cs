using UnityEngine;
using Steerings;

public class AngSpeedListener : MonoBehaviour
{
    private SteeringContext ks;

    public void Awake()
    {
        ks = GetComponent<SteeringContext>();
       
    }

    public void AcceptInput(float userInput)
    {
        ks.maxAngularSpeed = userInput;
    }
}
