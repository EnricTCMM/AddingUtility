using UnityEngine;
using BTs;
using Steerings;

class ACTION_Arrive : Action
{
    public string keyTarget;
    public string keyCloseEnough;


    public ACTION_Arrive(string keyTarget, string keyCloseEnough="1.0")
    {
        this.keyTarget=keyTarget;
        this.keyCloseEnough = keyCloseEnough;
    }

    private Arrive arrive;

    public override void OnInitialize()
    {
        arrive = GetComponent<Arrive>();
        
        arrive.target = blackboard.Get<GameObject>(keyTarget);
        arrive.Context.closeEnoughRadius = 0.0f; // OnTick will make the decisions ragarding stopping
        arrive.enabled = true;
    }

    public override Status OnTick()
    {
        if (SensingUtils.DistanceToTarget(gameObject, blackboard.Get<GameObject>(keyTarget)) 
                                           <= blackboard.Get<float>(keyCloseEnough))
        {
            arrive.enabled = false;
            return Status.SUCCEEDED;
        }
        else return Status.RUNNING;
    }

    public override void OnAbort()
    {
        arrive.enabled = false;
    }
}
