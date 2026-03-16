using UnityEngine;
using BTs;

public class ACTION_SetTag : Action
{

    public string keyTarget;
    public string keyNewTag;

    public ACTION_SetTag() { }
   
    public ACTION_SetTag(string keyTarget, string keyNewTag)  {
        this.keyTarget = keyTarget;
        this.keyNewTag = keyNewTag; 
    }
  
    public override Status OnTick()
    {
        if (blackboard.Get<GameObject>(keyTarget)==null)
        {
            Debug.Log("null target object in ACTION_SetTag");
            return Status.FAILED;
        }
        blackboard.Get<GameObject>(keyTarget).tag = blackboard.Get<string>(keyNewTag);
        return Status.SUCCEEDED;
    }
}
