using System;
using BTs;
using UnityEngine;
using Action = BTs.Action;

public class ACTION_UpdateKey<T> : Action
{
    private T value;
    private string key;

    // Notice second parameter is of type T not of type string
    public ACTION_UpdateKey(string key, T value )
    {
        this.key = key;
        this.value = value;
    }
    
    public override Status OnTick()
    {
        // ask the blackboard whether the key exists or not
        Type type = blackboard.GetCurrentType(key);
        if (type == null)
        {
            Debug.LogError("Blackboard key " + key + " does not exist");
            return Status.FAILED;
        }
        else if (type != typeof(T))
        {
            Debug.LogError("Blackboard key " + key + " is not of type "+typeof(T)+" current type is "+type);
            return Status.FAILED;
        }
        blackboard.Put(key, value);
        return Status.SUCCEEDED;
    }
}