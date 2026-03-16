using UnityEngine;
using BTs;

public class ACTION_Somersault : Action
{

    public string keySpeed;

    // construtor
    public ACTION_Somersault(string keySpeed = "180")
    {
        this.keySpeed = keySpeed;
    }

    private float speed;
    private float degs;
    private float original;

    public override void OnInitialize()
    {
        speed = blackboard.Get<float>(keySpeed);
        degs = 0;
        original = gameObject.transform.eulerAngles.z;
    }

    public override Status OnTick ()
    {
        degs += speed * Time.deltaTime;
        if (degs >= 360)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, original);
            return Status.SUCCEEDED;
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, original+degs);
            return Status.RUNNING;
        }
    }

    public override void OnAbort()
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, original);
    }

}
