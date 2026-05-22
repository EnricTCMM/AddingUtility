using MotorBehaviours;
using Steerings;
using UnityEngine;

public class SoldierLines : MonoBehaviour
{
    public float length = 20;
    
    void OnDrawGizmos()
    {
        GameObject target = GetComponent<SB_KeepPosition>().target;
        MotorManager me = GetComponent<MotorManager>();
        Vector3 velocityDirection = me.currentVelocity.normalized;
        Vector3 myLookDir = Utils. OrientationToVector(me.transform.eulerAngles.z);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, target.transform.position);
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, velocityDirection * length);

        Gizmos.color = Color.limeGreen;
        Gizmos.DrawRay(transform.position, myLookDir * length);
    }
}
