
using UnityEngine;


public class NerdCreator : MonoBehaviour
{

    private GameObject nerdPrefab;
    private GameObject creationPoint;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        nerdPrefab = Resources.Load<GameObject>("RupertBoy");
        creationPoint = GameObject.Find("CREATION_POINT");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject nerd = GameObject.Instantiate(nerdPrefab);
            nerd.transform.position = creationPoint.transform.position;
        }
    }
}
