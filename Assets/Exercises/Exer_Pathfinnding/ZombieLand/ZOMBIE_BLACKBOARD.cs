using UnityEngine;

public class ZOMBIE_BLACKBOARD : MonoBehaviour
{
    public float gutDetectedRadius = 150;
    public float gutReachedRadius = 10;
    public float pointReachedRadius = 3;

    private GameObject[] wanderPoints;
    
    void Awake()
    {
        wanderPoints = GameObject.FindGameObjectsWithTag("WANDERPOINT");
        
    }

    public GameObject GetRandomWanderPoint ()
    {
        return wanderPoints[Random.Range(0, wanderPoints.Length)];
    }
}
