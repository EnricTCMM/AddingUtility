using UnityEngine;
using BTs;

public class CONDTION_HasMoneyToBuyBeer : Condition
{

    public CONDTION_HasMoneyToBuyBeer()  { }
   
    /* Add other constructors if necessary */

    // this is a BOB-specific condition. It relies on BOB's blackboard

    public override bool Check ()
    {
        return ((BOB_Blackboard)blackboard).HasMoneyToBuyBeer();
    }

}
