using Pathfinding;
using UnityEngine;

public class ControlScript_ZombieLand : MonoBehaviour
{
    private Camera cam;
    private GameObject brainPrefab;
    

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        brainPrefab = Resources.Load<GameObject>("GUT(ROOM)");
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;

            // change position to that of the closest walkable node
            if (AstarPath.active != null)
            {
                GraphNode node = AstarPath.active.GetNearest(position, NNConstraint.Default).node;
                position = (Vector3)node.position;
            }

            GameObject guts = GameObject.Instantiate(brainPrefab);

            guts.transform.position = position;
            guts.transform.Rotate(0, 0, Random.value * 360);
        }
    }
}
