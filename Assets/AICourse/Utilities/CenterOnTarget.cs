using UnityEngine;

public class CenterOnTarget : MonoBehaviour
{
    public GameObject target;

    public void Center()
    {
        transform.position = new Vector3(target.transform.position.x,
                                         target.transform.position.y,
                                         transform.position.z);
    }
}
