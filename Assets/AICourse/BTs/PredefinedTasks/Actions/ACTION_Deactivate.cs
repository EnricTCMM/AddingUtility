using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTs;

public class ACTION_Deactivate : Action
{

    public string keyTarget;

    public ACTION_Deactivate()  { }
   
    public ACTION_Deactivate(string keyTarget)
    {
        this.keyTarget = keyTarget;
    }    

    public override Status OnTick()
    {
        blackboard.Get<GameObject>(keyTarget).SetActive(false);
        return Status.SUCCEEDED;
    }
}
