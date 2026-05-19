using UnityEngine;
using MotorBehaviours;

public class BoidSpawnerAndRegistry : GroupRegistry
{
    public int numInstances = 20;
    public float delay = 0.5f;
    public GameObject prefab;
    public GroupRegistry registry;
    public GameObject attractor;

    // the following attributes are specifically created to help listeners of UI
    // components get the initial values for the UI elements they're attached to
    [HideInInspector]
    public float maxSpeed, maxForce, cohesionThreshold, repulsionThreshold, alignmentThreshold, coneOfVisionAngle,
        cohesionWeight, repulsionWeight, alignmentWeight, seekWeight;
    
    private int created = 0;
    private float elapsedTime = 0f;

    void Start()
    {
        GameObject dummy = Instantiate(prefab);
        MotorManager motorManager = dummy.GetComponent<MotorManager>();
        SB_Flocking flocking = dummy.GetComponent<SB_Flocking>();
        
        maxSpeed = motorManager.maxSpeed;
        maxForce = motorManager.maxForce;
        
        cohesionThreshold = flocking.cohesionThreshold;
        repulsionThreshold = flocking.separationThreshold;
        alignmentThreshold = flocking.alignmentThreshold;
        coneOfVisionAngle = flocking.coneOfVisionAngle;
        cohesionWeight = flocking.cohesionWeight;
        repulsionWeight = flocking.separationWeight;
        alignmentWeight = flocking.alignmentWeight;
        seekWeight = flocking.attractionWeight;
        Destroy(dummy);
    }
    
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
        clone.GetComponent<SB_Flocking>().registry = registry;
        if (attractor!=null)
        {
            clone.GetComponent<SB_Flocking>().attractor = attractor;
        }

        if (created==0)
        {
            // first one and only it
            /* ShowRadiiPro shr = clone.GetComponent<ShowRadiiPro>();
            shr.componentTypeName = "MotorBehaviours.SB_Flocking";
            shr.innerFieldName = "separationThreshold";
            shr.outerFieldName = "cohesionThreshold";
            shr.enabled = true;
            */

            clone.GetComponent<SB_Flocking>().showFlockingGizmos = true;
            //clone.GetComponent<SB_Wander>().showWanderGizmos = true;
            //clone.GetComponent<SB_ObstacleAvoidance>().showWhiskers = true;

            if (attractor!=null)
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
