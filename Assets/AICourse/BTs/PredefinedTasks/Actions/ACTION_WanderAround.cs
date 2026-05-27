using UnityEngine;
using BTs;
using MotorBehaviours;

public class ACTION_WanderAround : Action
{
    public string keyAttractor;
    public string keyAttractionWeight;
    
    public ACTION_WanderAround(string keyAttractor, string keyAttractionWeight)
    {
        this.keyAttractor = keyAttractor;
        this.keyAttractionWeight = keyAttractionWeight;
    }
    
    private SB_Wander wander; 

    public override void OnInitialize()
    {
        // get the steering and initialize its parameters
        wander = GetComponent<SB_Wander>();
        
        // we should consider a fail fast approach here, if the component is not found
        
        // null values mean use values already set in the editor
        if (keyAttractor!=null)
            wander.attractor = blackboard.Get<GameObject>(keyAttractor);
        if  (keyAttractionWeight != null)
            wander.attractionWeight = blackboard.Get<float>(keyAttractionWeight);
        
        wander.Enable();
    }

    public override Status OnTick ()
    {
        // write here the code to be executed every time the actionName is ticked
        return Status.RUNNING;
    }

    public override void OnAbort()
    {
        // write here the code to be executed if the actionName is aborted while running
        wander.Disable();
    }

}
