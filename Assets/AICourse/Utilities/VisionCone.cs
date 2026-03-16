
using UnityEngine;
using Steerings;

public class VisionCone : MonoBehaviour
{


    public float totalAngle = 60;
    public float radius = 5;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float half = totalAngle / 2;
        float orientation = transform.eulerAngles.z;
        float positive = orientation + half;
        float negative = orientation - half;

        Vector3 ray1 = Utils.OrientationToVector(positive)*radius;
        Vector3 ray2 = Utils.OrientationToVector(negative)*radius;

        Debug.DrawLine(transform.position, transform.position + ray1, Color.black);
        Debug.DrawLine(transform.position, transform.position + ray2, Color.black);

        // ---------

        /*
        float dotProd = Utils.DotProduct(Utils.OrientationToVector(transform.eulerAngles.z), 
                                         (target.transform.position-transform.position).normalized
                                        );
        // Debug.Log("Dotprod " + dotProd);

        if (dotProd > Mathf.Cos(half * Mathf.Deg2Rad)) Debug.LogWarning("SPOTTED");
        else Debug.Log("OUT");
        */

        if (Utils.InCone(gameObject, target, totalAngle)) Debug.LogWarning("SPOTTED");
        else Debug.Log("OUT");
    }
}
