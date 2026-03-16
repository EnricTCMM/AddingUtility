using UnityEngine;
using BTs;

public class ACTION_Drop : Action
{

    // the (blackboard name of the) gameobject to drop
    public string keyTarget;

    public ACTION_Drop() { }

    public ACTION_Drop(string keyTarget) {
        this.keyTarget= keyTarget;
    }
  
    public override Status OnTick()
    {
        blackboard.Get<GameObject>(keyTarget).transform.parent = null;
        return Status.SUCCEEDED;
    }
}
