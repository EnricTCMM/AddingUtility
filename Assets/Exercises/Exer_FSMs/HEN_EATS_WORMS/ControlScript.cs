using UnityEngine;

public class ControlScript : MonoBehaviour
{
    private Camera cam;
    private GameObject wormPrefab;
    private GameObject chickPrefab;
    private GameObject dummy; 

    // Start is called before the first frame update
    void Start()
    {
        dummy = new GameObject("dummy");
        cam = Camera.main;
        wormPrefab = Resources.Load<GameObject>("WORM");
        chickPrefab = Resources.Load<GameObject>("CHICK");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject worm = GameObject.Instantiate(wormPrefab);
            worm.transform.position = position;
            worm.transform.Rotate(0, 0, Random.value * 360);
        }

        if (Input.GetMouseButtonDown(0))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            dummy.transform.position = position;
            GameObject worm = SensingUtils.FindInstanceWithinRadius(dummy, "WORM", 10);
            if (worm != null) Destroy(worm);
        }

        if (Input.GetMouseButtonDown(2))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject chick = GameObject.Instantiate(chickPrefab);
            chick.transform.position = position;
            chick.transform.Rotate(0, 0, Random.value * 360);
        }
    }
}
