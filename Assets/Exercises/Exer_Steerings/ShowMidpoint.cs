
using UnityEngine;

public class ShowMidpoint : MonoBehaviour
{

    public GameObject objA;
    public GameObject objB;

    // Update is called once per frame
    void Update()
    {
        if (objA != null && objB != null)
        {
            Vector3 midPoint = (objA.transform.position + objB.transform.position) / 2;
            // show midPoint 
            DebugExtension.DebugPoint(midPoint, Color.red, 3);
            // show AB line
            Debug.DrawLine(objA.transform.position, objB.transform.position, Color.blue);
        }
    }
}
