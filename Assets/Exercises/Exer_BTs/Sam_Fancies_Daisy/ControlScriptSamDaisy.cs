using UnityEngine;

public class ControlScriptSamDaisy : MonoBehaviour
{
    private Camera cam;
    private GameObject sambPrefab;
    private GameObject samcPrefab;

    
    void Start()
    {
        cam = Camera.main;
        sambPrefab = Resources.Load<GameObject>("SAM_B");
        samcPrefab = Resources.Load<GameObject>("SAM_C");
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (Input.GetKey("c"))
            {
                var position = cam.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0;

                GameObject sam = GameObject.Instantiate(samcPrefab);
                sam.transform.position = position;
            }
            else if (Input.GetKey("b")) {
                var position = cam.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0;

                GameObject sam = GameObject.Instantiate(sambPrefab);
                sam.transform.position = position;
            }
        }
       

    }
}
