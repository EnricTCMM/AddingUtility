
using UnityEngine;

public class ShowRadii : MonoBehaviour {

   
    public float inner, outer;
    private Vector3 up = new Vector3(0, 0, 1);
	
	// Update is called once per frame
	void Update () {
        DebugExtension.DebugCircle(transform.position, up, Color.blue, inner);
        DebugExtension.DebugCircle(transform.position, up, Color.red, outer);
    }
}
