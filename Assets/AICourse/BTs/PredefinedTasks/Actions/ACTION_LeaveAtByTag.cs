using UnityEngine;
using BTs;

public class ACTION_LeaveAtByTag : Action {

    public string keyLocation;
    public string keyTag;

    public ACTION_LeaveAtByTag(string keyLocation,
                               string keyTag)
    {
        this.keyLocation = keyLocation;
        this.keyTag = keyTag;
    }

    public override Status OnTick()
    {

        GameObject child = FindChildByTag(blackboard.Get<string>(keyTag));
        if (child == null)
        {
            Debug.Log("Action leave by tag fails because no child with that tag found");
            return Status.FAILED;
        }
        child.transform.parent = null;
        child.transform.position = blackboard.Get<GameObject>(keyLocation).transform.position; 
        return Status.SUCCEEDED;
    }

    private GameObject FindChildByTag(string tag)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.tag == tag)
                return gameObject.transform.GetChild(i).gameObject;
        }
        return null;
    }

}
