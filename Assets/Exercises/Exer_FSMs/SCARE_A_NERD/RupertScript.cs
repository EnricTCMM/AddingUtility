using UnityEngine;
using Steerings;

public class RupertScript : MonoBehaviour
{
    private GameObject destination;
    private Evade evade;
    private Seek seek;
    private bool evading = false;
    private GameObject ghost;

    void Start()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("TARGETLOC");
        destination = targets[(new System.Random()).Next(targets.Length)];
        seek = GetComponent<Seek>();
        evade = GetComponent<Evade>();
        GetComponent<SteeringContext>().maxSpeed += GetComponent<SteeringContext>().maxSpeed * Random.Range(0.2f, 0.8f);
        seek.target = destination;
        seek.enabled = true;
        evade.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (evading) return;

        ghost = SensingUtils.FindInstanceWithinRadius(gameObject, "GHOST", 6);
        if (ghost!=null)
        {
            tag = "SCARED";
            evading = true;
            seek.enabled = false;
            evade.target = ghost;
            evade.enabled = true;
            GetComponent<SteeringContext>().maxSpeed *= 4f;
            GetComponent<TimeToLive>().Reset(15);
        }
    }
}
