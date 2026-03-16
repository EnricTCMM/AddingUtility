
using UnityEngine;

public class RecallPosition : MonoBehaviour
{
    public Vector3 position;
    public bool backToInitial;
    private Vector3 initialPosition;


    void Start()
    {
        if (backToInitial) initialPosition = transform.position;
        else initialPosition = position;
    }

    
    void Recall()
    {
        transform.position = initialPosition;
    }
}
