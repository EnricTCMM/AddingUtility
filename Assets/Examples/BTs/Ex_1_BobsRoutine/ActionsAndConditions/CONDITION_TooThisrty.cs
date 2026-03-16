using BTs;

public class CONDITION_TooThisrty : Condition
{
    public CONDITION_TooThisrty() { }

    public override bool Check ()
    {
        return ((BOB_Blackboard)blackboard).VeryThirsty();
    }
}
