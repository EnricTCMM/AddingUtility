using UnityEngine;
using BTs;

public class ACTION_Activate : Action
{

    public string keyTarget;

    public ACTION_Activate(string keyTarget) {
        this.keyTarget = keyTarget;
    }
   
    public override Status OnTick()
    {
        blackboard.Get<GameObject>(keyTarget).SetActive(true);
        return Status.SUCCEEDED;   
    }
}
