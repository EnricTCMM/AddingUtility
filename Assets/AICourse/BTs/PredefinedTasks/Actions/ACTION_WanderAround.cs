using UnityEngine;
using BTs;
using Steerings;

public class ACTION_WanderAround : Action
{
    public string keyAttractor;
    public string keySeekWeight;

    public ACTION_WanderAround() { }

    public ACTION_WanderAround(string keyAttractor, string keySeekWeight)
    {
        this.keyAttractor = keyAttractor;
        this.keySeekWeight = keySeekWeight;
    }

    private WanderAround wanderAround; 

    public override void OnInitialize()
    {
        // get the steering and initialize its parameters

        wanderAround = GetComponent<WanderAround>();
        if (wanderAround == null) wanderAround = AddComponent<WanderAround>();

        wanderAround.attractor = blackboard.Get<GameObject>(keyAttractor);
        wanderAround.Context.seekWeight = blackboard.Get<float>(keySeekWeight);
        wanderAround.enabled = true;
    }

    public override Status OnTick ()
    {
        // write here the code to be executed every time the action is ticked
        return Status.RUNNING;
    }

    public override void OnAbort()
    {
        // write here the code to be executed if the action is aborted while running
        wanderAround.enabled = false;
    }

}
