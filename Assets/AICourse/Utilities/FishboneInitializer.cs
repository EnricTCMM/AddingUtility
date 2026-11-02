using System;
using MotorBehaviours;
using UnityEngine;
using Random = UnityEngine.Random;


public class FishboneInitializer : MonoBehaviour
{
    private void Awake()
    {
        // create a gameobject that serves as very-far-away target for seek.
        // then seek plus constant rotation will induce a spiral-like movement
        // this target is destroyed soon after the fishbone is destroyed.
        
        float x = 1000000f * (Random.value > 0.5f ? -1f : 1f);
        float y = 1000000f * (Random.value > 0.5f ? -1f : 1f);
        Vector3 distantPoint =  new Vector3(x,y,0);
        GameObject go = new GameObject();
        go.transform.position = distantPoint;
        TimeToLive timeToLive = go.AddComponent<TimeToLive>();
        timeToLive.secondsToLive=this.GetComponent<TimeToLive>().secondsToLive+1;
        
        SB_Seek seek = GetComponent<SB_Seek>();
        seek.target = go;
        seek.Enable();
    }
    
}
