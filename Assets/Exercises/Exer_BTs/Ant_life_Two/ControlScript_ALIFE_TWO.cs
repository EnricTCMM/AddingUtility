
using UnityEngine;

public class ControlScript_ALIFE_TWO : MonoBehaviour
{
    private Camera cam;
    private GameObject seedPrefab;
  

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        seedPrefab = Resources.Load<GameObject>("SEED");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey("s"))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject seed = GameObject.Instantiate(seedPrefab);
            seed.transform.position = position;
            seed.transform.Rotate(0, 0, Random.value * 360);
            seed.transform.localScale /= 5;
        }   
    }
}
