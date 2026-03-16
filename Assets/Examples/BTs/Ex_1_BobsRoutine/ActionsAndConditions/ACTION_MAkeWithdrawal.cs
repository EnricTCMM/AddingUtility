
using BTs;

public class ACTION_MAkeWithdrawal : Action
{
    
    public override Status OnTick ()
    {
        if (((BOB_Blackboard)blackboard).MakeWithdrawal())
            return Status.SUCCEEDED;
        else
            return Status.FAILED;
    }

}
