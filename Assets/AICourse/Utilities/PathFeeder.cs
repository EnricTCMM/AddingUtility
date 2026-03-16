using UnityEngine;
using Pathfinding;
using Steerings;


// this behaviour "feeds" a path to PathFollowing until a target has been reached

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(PathFollowing))]
public class PathFeeder : MonoBehaviour {

	public GameObject target;    // the required destination
	public float repathTime = 1; // "recalculate" path every repathTime seconds

	private Seeker seeker;
	private PathFollowing pathFollowingSteering;
	private Path currentPath;

	private float elapsedTime = 0f;  // time elapsed since last repathing op.

	// Use this for initialization
	void Start () {
		seeker = GetComponent<Seeker> ();
		if (seeker == null)
			Debug.LogError ("No seeker attached in PathFeeder");

		pathFollowingSteering = GetComponent<PathFollowing> ();
		if (pathFollowingSteering == null)
			Debug.LogError ("No PathFollowing steering attached. No steering to feed");

		pathFollowingSteering.enabled = false; // can't be enabled until a path is available

		// start the path computation process
		seeker.StartPath(this.transform.position, target.transform.position, OnPathComplete);
	}
	
	// Update is called once per frame
	void Update () {
		if (elapsedTime >= repathTime) {
			// stop following the current path: the target may have moved...
			pathFollowingSteering.enabled = false;
			// ask seeker to calculate a new path
			seeker.StartPath (this.transform.position, target.transform.position, OnPathComplete);
			elapsedTime = 0f;
		} else {
			elapsedTime += Time.deltaTime;
		}
	}

	public void OnPathComplete (Path p) {
		// this is a "callback" method. if this method is called, a path has been computed and "stored" in p
		currentPath = p;

		// feed the path to the path-following steering
		pathFollowingSteering.path = currentPath;
		pathFollowingSteering.currentWaypointIndex = 0;
		pathFollowingSteering.enabled = true;
	}
}
