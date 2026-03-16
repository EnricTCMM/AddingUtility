
using UnityEngine;
using Steerings;
using System.Collections.Generic;

public class ChickSpawner : MonoBehaviour
{
    public bool arbitrated = true;
    
    public int numInstances = 4;
    public float interval = 1f; // one every interval seconds

    private GameObject prefab;
    private int generated;
    private float elapsedTime;

    [HideInInspector]
    public float repulsionThreshold = 15f;
    [HideInInspector]
    public float lrWeight = 0.5f;
    [HideInInspector]
    List<GameObject> chicks = new List<GameObject>();


    // Use this for initialization
    void Start()
    {
        if (arbitrated)
            prefab = Resources.Load<GameObject>("CHICK_ARB");
        else
            prefab = Resources.Load<GameObject>("CHICK_BLEND");
    }

    // Update is called once per frame
    void Update()
    {
        if (generated == numInstances)
            return;

        if (elapsedTime >= interval)
        {
            // spawn creating an instance...
            GameObject clone = Instantiate(prefab);
            clone.transform.position = this.transform.position;

            if (arbitrated)
                clone.GetComponent<LeaderFollowingArbitrated>().target = this.gameObject;
            else
                clone.GetComponent<LeaderFollowingBlended>().target = this.gameObject;

            chicks.Add(clone);

            if (generated==0)
            {
                clone.GetComponent<ShowRadiiPro>().enabled = true;
            }

            generated++;
            elapsedTime = 0;
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void SetRepulsionThreshold (float value)
    {
        foreach (GameObject chick in chicks)
        {
            chick.GetComponent<SteeringContext>().repulsionThreshold = value;
        }
    }

    public void SetLrWeight(float value)
    {
        if (arbitrated) return; // makes no sense for arbitrated chicks

        foreach (GameObject chick in chicks)
        {
            chick.GetComponent<LeaderFollowingBlended>().wlr = value;
        }
    }
}
