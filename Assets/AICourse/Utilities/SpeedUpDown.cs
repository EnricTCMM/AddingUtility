using UnityEngine;
using Steerings;

public class SpeedUpDown : MonoBehaviour
{

    private SteeringContext sc;
    public float min = 5;
    public float max = 50;
    public float inc = 5;
    
    void Start()
    {
        // get the steering context
        sc = GetComponent<SteeringContext>();
        if (sc != null) sc.maxSpeed = min;
    }

    
    void Update()
    {
        if (sc != null)
        {
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) {
                if (sc.maxSpeed + inc <= max)
                    sc.maxSpeed += inc;
                else sc.maxSpeed = max;
            }
            else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (sc.maxSpeed - inc >= min)
                    sc.maxSpeed -= inc;
                else sc.maxSpeed = min;
            }
        }
    }
}
