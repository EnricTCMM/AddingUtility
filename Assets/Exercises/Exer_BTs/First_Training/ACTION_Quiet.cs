using UnityEngine;
using TMPro;
using BTs;

public class ACTION_Quiet : Action
{
   
    public override Status OnTick ()
    {
        GameObject bubble = FindChildWithTag(gameObject, "BUBBLE");
        if (bubble != null) bubble.SetActive(false);
        return Status.SUCCEEDED;
    }

    private GameObject FindChildWithTag(GameObject go, string tag)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            if (go.transform.GetChild(i).tag == tag)
                return go.transform.GetChild(i).gameObject;
        }
        return null;
    }

}
