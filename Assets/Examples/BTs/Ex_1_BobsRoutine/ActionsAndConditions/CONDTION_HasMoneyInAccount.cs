using UnityEngine;
using BTs;

public class CONDTION_HasMoneyInAccount : Condition
{

    public override bool Check ()
    {
        return ((BOB_Blackboard)blackboard).HasMoneyInAccount();
    }

}
