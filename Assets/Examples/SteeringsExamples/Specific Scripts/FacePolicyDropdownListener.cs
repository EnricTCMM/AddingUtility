using MotorBehaviours;
using UnityEngine;

public class FacePolicyDropdownListener : MonoBehaviour
{
    
    public void OnValueChanged(int value)
    {
        /*
         * Red line: soldier's velocity direction
Blue line: direction to target
<- and -> rotate car
click teleports car
Space (re)starts/stopss
         */
        
        // Debug.Log("Received value:"+value);
        switch (value)
        {
            case 0:
                GetComponent<MotorManager>().rotationalPolicy = MotorManager.RotationalPolicy.NONE;
                //Debug.Log("Set to NONE");
                break;
            case 1:
                GetComponent<MotorManager>().rotationalPolicy = MotorManager.RotationalPolicy.LWYG;
                //Debug.Log("Set to LWYG");
                break;
            case 2:
                GetComponent<MotorManager>().rotationalPolicy = MotorManager.RotationalPolicy.LWYGI;
                //Debug.Log("Set to LWYGI");
                break;
            case 3: 
                GetComponent<MotorManager>().rotationalPolicy = MotorManager.RotationalPolicy.FT;
                //Debug.Log("Set to FT");
                break;
            case 4:
                GetComponent<MotorManager>().rotationalPolicy = MotorManager.RotationalPolicy.FTI;
                //Debug.Log("Set to FTI");
                break;
        }    
    }
}
