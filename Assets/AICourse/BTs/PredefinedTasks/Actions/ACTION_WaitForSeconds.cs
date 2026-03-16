using UnityEngine;
using BTs;

public class ACTION_WaitForSeconds : Action
{
    public string keySeconds;
    
    public ACTION_WaitForSeconds(string keySeconds)
    {
        this.keySeconds = keySeconds;
    }

    private float elapsed;
    private float theSeconds; 

    public override void OnInitialize()
    {

        theSeconds = blackboard.Get<float>(keySeconds);
        elapsed = 0.0f;
    }

    public override Status OnTick()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= theSeconds)
            return Status.SUCCEEDED;
        else
            return Status.RUNNING;
    }
}
