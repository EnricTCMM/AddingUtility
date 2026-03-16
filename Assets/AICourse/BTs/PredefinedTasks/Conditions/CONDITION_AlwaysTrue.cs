using UnityEngine;
using BTs;

class CONDITION_AlwaysTrue : Condition
{
    public CONDITION_AlwaysTrue()  { }

    public override bool Check()
    {
        return true;
    }
}
