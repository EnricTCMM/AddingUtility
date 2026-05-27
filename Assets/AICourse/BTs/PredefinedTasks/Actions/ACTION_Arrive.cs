using UnityEngine;
using BTs;
using MotorBehaviours;

class ACTION_Arrive : Action
{
    public string keyTarget;
    public string keyTolerance;
    public string keySlowDownRadius;
    
    public ACTION_Arrive(string keyTarget, string keyTolerance="1.0", string keySlowDownRadius="10.0")
    {
        this.keyTarget=keyTarget;
        this.keyTolerance = keyTolerance;
        this.keySlowDownRadius = keySlowDownRadius;
    }

    private SB_Arrive arrive;

    public override void OnInitialize()
    {
        arrive = GetComponent<SB_Arrive>();
        
        arrive.target = blackboard.Get<GameObject>(keyTarget);
        arrive.toleranceRadius = 0.0f; // minimum tolerance.
                                       // OnTick will check if the target is within the tolerance
                                       // in keyTolerance
        arrive.slowdownRadius = blackboard.Get<float>(keySlowDownRadius);
        arrive.Enable();
    }

    public override Status OnTick()
    {
        if (SensingUtils.DistanceToTarget(gameObject, blackboard.Get<GameObject>(keyTarget)) 
                                           <= blackboard.Get<float>(keyTolerance))
        {
            arrive.Disable();
            return Status.SUCCEEDED;
        }
        else return Status.RUNNING;
    }

    public override void OnAbort()
    {
        arrive.Disable();
    }
}
