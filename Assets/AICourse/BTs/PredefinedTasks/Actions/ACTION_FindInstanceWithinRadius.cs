using UnityEngine;
using BTs;

public class ACTION_FindInstanceWithinRadius : Action
{

    public string keyRadius;
    public string keyTag;
    public string keyoutInstanceFound;

    public ACTION_FindInstanceWithinRadius() { }

    public ACTION_FindInstanceWithinRadius(string keyRadius, string keyTag, string keyoutInstanceFound)
    {
        this.keyRadius = keyRadius;
        this.keyTag = keyTag;
        this.keyoutInstanceFound = keyoutInstanceFound;
    }

    public override Status OnTick ()
    {
        GameObject instance = SensingUtils.FindInstanceWithinRadius(gameObject, 
                                                                    blackboard.Get<string>(keyTag), 
                                                                    blackboard.Get<float>(keyRadius));
        if (instance != null)
        {
            blackboard.Put(keyoutInstanceFound, instance);
            return Status.SUCCEEDED;
        }
        else
            return Status.FAILED;
    }
}
