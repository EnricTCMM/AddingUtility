

namespace BTs
{
    public class LambdaAction : Action
    {

        public delegate Status OnTickDelegate();
        private OnTickDelegate lambdaOnTick;

        // parameter must be a lambda with no parameters 
        // returing a Status 
        // ()=> {... return Status.SUCCEEDED;}
        // or a method doing so (casting required)
        // (LambdaAction.OnTickDelegate)Method
        public LambdaAction(OnTickDelegate lambda)
        {
            lambdaOnTick = lambda;
        }

        public override Status OnTick()
        {
            // retornar el que retorni el delegate...
            return lambdaOnTick();
        }
    }
}
