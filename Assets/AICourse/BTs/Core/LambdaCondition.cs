
namespace BTs
{
    public class LambdaCondition : Condition
    {

        public delegate bool conditionDelegate();
        private conditionDelegate myCondition;

        public LambdaCondition(conditionDelegate myCond)
        {
            this.myCondition = myCond;
        }

        public override bool Check()
        {
            return myCondition();
        }
    }
}