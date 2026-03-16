using UnityEngine;
using BTs;

public class CONDITION_ParentsObjectWithTag : Condition
{

    public string keyTag;

    public CONDITION_ParentsObjectWithTag (string keyTag)
    {
        this.keyTag = keyTag;
    }

    public override bool Check ()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.tag == blackboard.Get<string>(keyTag))
                return true;
        }
        return false;
    }
}
