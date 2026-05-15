using Steerings;
using UnityEngine;

namespace MotorBehaviours
{
    public class SB_Face : AngularMotorBehaviour
    {
        [Header("Face Settings")]
        public GameObject target;
        public float toleranceRadius = 2f;
        public float slowdownRadius = 30f;
        
        public override float? GetDesiredAngularSpeed(MotorManager me)
        {
            // If there's no target FAIL-FAST
            if (target == null)
            {
                Debug.LogError($"[MotorBehaviours] Critical error: No target in {GetType().Name} in GameObject '{gameObject.name}'.");
                Debug.Break(); // immediate pause. 
                return 0;
            }
            
            // calculate direction to target
            Vector3 direction = target.transform.position - me.transform.position;
            if (direction.sqrMagnitude == 0f) return 0f;

            // from vector to angle
            float targetAngle = Utils.VectorToOrientation(direction);

            // 3. DELEGATION!!!
            return SB_Align.GetDesiredAngularSpeed(me, targetAngle, toleranceRadius, slowdownRadius);
        }
        
    }
}