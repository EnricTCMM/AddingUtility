using UnityEngine;
using Pathfinding;
using MotorBehaviours; // Required to access the generic SB_PathFollowing

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(SB_PathFollowing))]
public class AstarPathFeeder : MonoBehaviour 
{
    [Header("Target Settings")]
    public GameObject target;    
    public float repathTime = 1f; 

    private Seeker seeker;
    private SB_PathFollowing pathFollowingSteering;
    private float elapsedTime = 0f;

    void Start() 
    {
        seeker = GetComponent<Seeker>();
        pathFollowingSteering = GetComponent<SB_PathFollowing>();
        
        if (seeker == null || pathFollowingSteering == null)
        {
            Debug.LogError("[AstarPathFeeder] Missing required components!");
            return;
        }

        // Note: We don't disable the steering here. The GetDesiredVelocity 
        // inside SB_PathFollowing will automatically return null (abstain) 
        // if there's no path assigned yet.
        
        RequestPath();
    }
    
    void Update() 
    {
        if (target == null) return;

        elapsedTime += Time.deltaTime;
        
        if (elapsedTime >= repathTime) 
        {
            RequestPath();
            elapsedTime = 0f;
        }
    }

    private void RequestPath()
    {
        // Safety guard: Only request a new path if the Seeker is not busy calculating another one
        if (target != null && seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.transform.position, OnPathComplete);
        }
    }

    // Callback method executed by the A* system when the calculation finishes
    public void OnPathComplete(Path p) 
    {
        // Fail Fast: Ensure the path calculation didn't fail (e.g., unreachable target)
        if (p.error)
        {
            Debug.LogWarning($"[AstarPathFeeder] Path calculation failed: {p.errorLog}");
            return;
        }

        // Adapter Pattern: Extract the pure mathematical vector list and inject it 
        // into our generic path following behavior, maintaining zero-allocation.
        pathFollowingSteering.SetPath(p.vectorPath);
    }
}