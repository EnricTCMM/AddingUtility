using UnityEngine;
using BTs;

class ACTION_ChooseRandomCorner : Action
{
    public string keyoutCorner;

    public ACTION_ChooseRandomCorner(string keyoutCorner)
    {
        this.keyoutCorner = keyoutCorner;
    }

    public override Status OnTick()
    {
        DAISY_Blackboard bl = (DAISY_Blackboard)blackboard;

        /* COMPLETE */
        
        bl.Put(keyoutCorner, bl.GetRandomCorner());

        //return Status.FAILED;  // change when complete (you don't want this actionName to fail)
        return Status.SUCCEEDED;
    }
}
