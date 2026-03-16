using UnityEngine;
using BTs;

public class CONDITION_InstanceNear : Condition
{
    public string keyRadius;
    public string keyTag;
    public string keyHasMemory;
    public string keyoutInstanceFound; // output

    public CONDITION_InstanceNear( string keyRadius, string keyTag,
                                   string keyHasMemory="false", 
                                   string keyoutInstanceFound="dummy") {
        this.keyRadius = keyRadius;
        this.keyTag = keyTag;
        this.keyHasMemory = keyHasMemory;
        this.keyoutInstanceFound = keyoutInstanceFound;
    }
   
    // this guarantees a one-shot behaviour in case parameterHasMemory is true
    private bool previous;
    private bool memory;

    public override void OnInitialize() 
    {
        previous = false;
        memory = blackboard.Get<bool>(keyHasMemory);
    }

    public override bool Check()
    {
        bool result;
        GameObject instance;

        if (previous && memory) return true;
        instance = SensingUtils.FindInstanceWithinRadius(gameObject, 
                                                         blackboard.Get<string>(keyTag), 
                                                         blackboard.Get<float>(keyRadius));
        if (instance != null)
        {
            blackboard.Put(keyoutInstanceFound, instance);
            previous = true;
            result = true;
        }
        else
            result = false;

        return result;
    }

}
