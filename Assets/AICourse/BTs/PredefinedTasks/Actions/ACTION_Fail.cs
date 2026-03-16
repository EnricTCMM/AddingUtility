
namespace BTs
{
    public class ACTION_Fail : Action
    {
        public ACTION_Fail() { }

        public override Status OnTick()
        {
            return Status.FAILED;
        }
    }
}