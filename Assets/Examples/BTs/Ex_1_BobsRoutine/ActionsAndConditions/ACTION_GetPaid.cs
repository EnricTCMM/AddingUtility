using UnityEngine;
using BTs;

public class ACTION_GetPaid : Action
{
    public ACTION_GetPaid() { }

    public override Status OnTick ()
    {
        // GetPaid is not intended to fail
        gameObject.GetComponent<BOB_Blackboard>().GetPaid();
        return Status.SUCCEEDED;
    }
}
