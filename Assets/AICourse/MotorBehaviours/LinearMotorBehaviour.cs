using UnityEngine;

namespace MotorBehaviours
{
    public abstract class LinearMotorBehaviour : MotorBehaviour
    {
        public abstract Vector3? GetDesiredVelocity(MotorManager me);
    }
}