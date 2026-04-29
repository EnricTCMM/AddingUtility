
using System;
using BTs;
using UnityEngine;

public class CONDITION_CheckKeyValue : Condition
{
    private String key;
    private object value;
    
    public CONDITION_CheckKeyValue(String key, object value)
    {
        this.key = key;
        this.value = value;
    }
    public override bool Check()
    {
        if (!blackboard.Exists(key))
        {
            Debug.LogError("Blackboard key " + key + " does not exist");
            return false;
        }
        else return blackboard.Get<object>(key).Equals(value);
    }
}