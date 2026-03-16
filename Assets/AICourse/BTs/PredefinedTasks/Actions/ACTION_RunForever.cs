using UnityEngine;
using BTs;

public class ACTION_RunForever : Action
{
    public ACTION_RunForever()  { }

    public override Status OnTick()
    {
        return Status.RUNNING;
    }
}
