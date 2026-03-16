using UnityEngine;
using BTs;

public class ACTION_DrinkBeer : Action
{
    public ACTION_DrinkBeer()  { }
    
    public override Status OnTick ()
    {
        ((BOB_Blackboard)blackboard).DrinkBeer();  // also gameObject.GetComponent<BOB_Blackboard>().DrinkBeer();
        return Status.SUCCEEDED;   
    }

}
