using UnityEngine;
using BTs;

// Take parents the given object 
public class ACTION_Take : Action
{

    public string keyObjectToTake;    

    public ACTION_Take(string keyObjectToTake) {
        this.keyObjectToTake = keyObjectToTake;
    }
    
    public override Status OnTick()
    {
        blackboard.Get<GameObject>(keyObjectToTake).transform.parent = gameObject.transform;
        return Status.SUCCEEDED;
    }
}
