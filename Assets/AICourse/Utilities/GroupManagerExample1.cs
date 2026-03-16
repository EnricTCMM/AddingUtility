
using UnityEngine;

public class GroupManagerExample1 : Steerings.GroupManager
{
    public int numInstances = 20;
    public float delay = 0.5f;
    public GameObject prefab;
    public bool around = false;
    public GameObject attractor;

    private int created = 0;
    private float elapsedTime = 0f;

    // the following attributes are specifically created to help listeners of UI
    // components get the initial values for the UI elements they're attached to
    [HideInInspector]
    public float maxSpeed, maxAcceleration, cohesionThreshold, repulsionThreshold, coneOfVisionAngle,
    cohesionWeight, repulsionWeight, alignmentWeight, seekWeight;
    
    void Start()
    {
        GameObject dummy = Instantiate(prefab);
        Steerings.SteeringContext context = dummy.GetComponent<Steerings.SteeringContext>();
        maxSpeed = context.maxSpeed;
        maxAcceleration = context.maxAcceleration;
        cohesionThreshold = context.cohesionThreshold;
        repulsionThreshold = context.repulsionThreshold;
        coneOfVisionAngle = context.coneOfVisionAngle;
        cohesionWeight = context.cohesionWeight;
        repulsionWeight = context.repulsionWeight;
        alignmentWeight = context.alignmentWeight;
        seekWeight = context.seekWeight;
        Destroy(dummy);
    }

    // Update is called once per frame
    void Update()
    {
        Spawn();
    }


    private void Spawn ()
    {
        if (created == numInstances) return;

        if (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            return;
        }

        // if this point is reached, it's time to spawn a new instance
        GameObject clone = Instantiate(prefab);
        clone.transform.position = transform.position;
        if (around)
        {
            clone.AddComponent<Steerings.FlockingAroundPlusAvoidance>();
            clone.GetComponent<Steerings.FlockingAroundPlusAvoidance>().attractor = attractor;
            clone.GetComponent<Steerings.FlockingAroundPlusAvoidance>().rotationalPolicy = Steerings.SteeringBehaviour.RotationalPolicy.LWYGI;
        }
        else
        {
            clone.AddComponent<Steerings.FlockingPlusAvoidance>();
            clone.GetComponent<Steerings.FlockingPlusAvoidance>().rotationalPolicy = Steerings.SteeringBehaviour.RotationalPolicy.LWYGI;
        }
        
        

        if (created==0)
        {
            // first one and only it
            ShowRadiiPro shr = clone.GetComponent<ShowRadiiPro>();
            shr.componentTypeName = "Steerings.SteeringContext";
            shr.innerFieldName = "repulsionThreshold";
            shr.outerFieldName = "cohesionThreshold";
            shr.enabled = true;

            if (around)
            {
                if (clone.GetComponent<TrailRenderer>() != null)
                {
                    clone.AddComponent<ToggleTrail>();
                    clone.GetComponent<TrailRenderer>().enabled = true;
                }
            }
        }

        AddBoid(clone);
        created++;
        elapsedTime = 0f;
    }
}
