
using UnityEngine;

public class GutsBehaviour : MonoBehaviour
{

    void Update()
    {
       
        transform.localScale *= 1.025f;
        if (transform.localScale.x >= 3)
        {
            gameObject.tag = "FREE_GUTS";
            this.enabled = false;
        }
    }
}
