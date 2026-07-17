using UnityEngine;
using System;
using System.Reflection;
using MotorBehaviours;

public class DirectWithClick : MonoBehaviour
{

    public bool rightClick = false;

    private SB_Arrive arrive;
    private SB_Seek seek;
    private SB_Wander wanderAround;
    private Component xAndSpin;
    private int buttonNumber;
    private GameObject target;

    void Start()
    {
        arrive = GetComponent<SB_Arrive>();
        seek = GetComponent<SB_Seek>();
        wanderAround = GetComponent<SB_Wander>();
        xAndSpin = GetComponent("SeekAndSpin");
        if (xAndSpin == null || !((MonoBehaviour)xAndSpin).enabled) xAndSpin=  GetComponent("ArriveAndSpin");
        target = new GameObject(); target.SetActive(false);
    }

   
    void Update()
    {
        if (Input.GetMouseButtonDown(buttonNumber))
        {
            Vector3 click = Input.mousePosition;
            Vector3 wantedPosition = Camera.main.ScreenToWorldPoint(new Vector3(click.x, click.y, 1f));
            wantedPosition.z = 0f;
            target.transform.position = wantedPosition;

            DebugExtension.DebugPoint(wantedPosition, Color.black, 5f, 1.5f);

            if (arrive != null && arrive.enabled) arrive.target = target;
            else if (wanderAround!=null && wanderAround.enabled) wanderAround.attractor = target;
            else if (seek!=null && seek.enabled) seek.target = target;
            else if (xAndSpin!=null)
            {
                Type type = xAndSpin.GetType();
                FieldInfo field = type.GetField("target");
                if (field != null) field.SetValue(xAndSpin, target);
            }
        }
    }
}
