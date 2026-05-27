using BTs;
using MotorBehaviours;

public class ACTION_Wander : Action
{
    private SB_Wander wander; 

    public override void OnInitialize()
    {
        wander = GetComponent<SB_Wander>();
        // we should consider a fail fast approach here, if the component is not found
        
        wander.Enable();
    }

    public override Status OnTick ()
    {
        
        return Status.RUNNING;
    }

    public override void OnAbort()
    {
        wander.Disable();
    }
}