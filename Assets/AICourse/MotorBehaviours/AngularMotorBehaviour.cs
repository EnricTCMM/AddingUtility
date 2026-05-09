namespace MotorBehaviours
{
    public abstract class AngularMotorBehaviour : MotorBehaviour
    {
        public abstract float GetTorque(MotorManager me);
    }
}