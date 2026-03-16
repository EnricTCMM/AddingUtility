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

        return Status.FAILED;  // change when complete (you don't want this action to fail)
    }
}
