using UnityEngine;
using BTs;

public class ACTION_DebugBlackboardEntry : Action
{
    public string keyName;

    public ACTION_DebugBlackboardEntry(string keyName) {
        this.keyName = keyName;
    }
    
    public override Status OnTick ()
    {
        Debug.Log(keyName+" ==> "+ blackboard.Get<object>(keyName));
        return Status.SUCCEEDED;
    }
}
