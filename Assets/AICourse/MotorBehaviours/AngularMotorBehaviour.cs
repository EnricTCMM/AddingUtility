namespace MotorBehaviours
{
    public abstract class AngularMotorBehaviour : MotorBehaviour
    {
        public abstract float? GetDesiredAngularSpeed(MotorManager me);
    }
}