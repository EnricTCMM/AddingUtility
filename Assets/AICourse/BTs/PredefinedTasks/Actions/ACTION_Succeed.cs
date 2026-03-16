using UnityEngine;
using BTs;

public class ACTION_Succeed : Action
{
    public ACTION_Succeed() { }
    
    public override Status OnTick ()
    {
        return Status.SUCCEEDED;
    }
}
