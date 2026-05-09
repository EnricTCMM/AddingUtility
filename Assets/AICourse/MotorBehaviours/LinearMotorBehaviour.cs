using UnityEngine;

namespace MotorBehaviours
{
    public abstract class LinearMotorBehaviour : MotorBehaviour
    {
        public abstract Vector3 GetForce(MotorManager me);
    }
}