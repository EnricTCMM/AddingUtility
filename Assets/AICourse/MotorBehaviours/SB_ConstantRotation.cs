using UnityEngine;

namespace MotorBehaviours
{

    public class SB_ConstantRotation : AngularMotorBehaviour
    {

        [Header("Angular Settings")]
        public float constantAngularVelocity = 180;
        
        public override float? GetDesiredAngularSpeed(MotorManager me)
        {
            return constantAngularVelocity;
        }
    }
}