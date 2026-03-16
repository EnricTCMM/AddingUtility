using BTs;

public class ACTION_BuyBeer : Action
{
    public ACTION_BuyBeer()  { }
    
    public override Status OnTick ()
    {
        BOB_Blackboard bl = (BOB_Blackboard)blackboard;

        if (bl.BuyBeer())
            return Status.SUCCEEDED;
        else
            return Status.FAILED;
    }
}
