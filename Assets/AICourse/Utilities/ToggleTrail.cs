using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTrail : MonoBehaviour
{
    TrailRenderer tr;
    float cooldown = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (cooldown <= 0)
        {
            if (tr != null)
            {
                if (Input.GetKey(KeyCode.T))
                {
                    tr.enabled = !tr.enabled;
                    tr.Clear();
                    cooldown = 0.2f;
                }
            }
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
    }
}
