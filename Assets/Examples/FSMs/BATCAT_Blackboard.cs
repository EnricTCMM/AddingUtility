using UnityEngine;

public class BATCAT_Blackboard : DynamicBlackboard {

	public float hunger = 0f;   // How hungry is BATCAT?

	// CONSTANTS
	public float maxPursuingTime = 15f;  // after this time, BATCAT has to rest for a while
	public float maxRestingTime = 3f;    // the time BATCAT will rest
	public float mouseDetectableRadius = 150f;  // at this distance, mice are detectable
	public float trashcanDetectableRadius = 75f; // id for trash cans
	public float rummageTime = 5f; // the time rummaging lasts
	public float eatingTime = 5f;  // the time eating lasts
	public float hungerTooHigh = 100;  // upper threshold for hunger
	public float hungerLowEnough = 10; // lower threshold for hunger 
	public float normalHungerIncrement = 0.5f; // speed of hunger increment
	public float sardineHungerDecrement = 50f; // amount of hunger decrement per sardine
	public float mouseReachedRadius = 10f; // at this distance, mice are caught
	public float mouseHasVanishedRadius = 200f; // has to be higher than mouse detectable radius
	public float placeReachedRadius = 15; // at this distance a place has been reached

	public GameObject hideout; // the place where BATCAT hides
	public GameObject jail; // the place where BATCAT jails mice
	public GameObject sardinePrefab;
	public GameObject fishbonePrefab;

	void Start () {

		if (hideout == null) {
			hideout = GameObject.Find ("HIDEOUT");
			if (hideout == null) {
				Debug.LogError ("no HIDEOUT object found in "+this);
			}
		}

		if (jail == null) {
			jail = GameObject.Find ("JAIL");
			if (jail == null) {
				Debug.LogError ("no JAIL object found in "+this);
			}
		}

		if (sardinePrefab == null) {
			sardinePrefab = Resources.Load<GameObject> ("SARDINE");
			if (sardinePrefab == null) {
				Debug.LogError ("no SARDINE PREFAB in Resources folder found in " + this);
			}
		}

		if (fishbonePrefab == null) {
			fishbonePrefab = Resources.Load<GameObject> ("FISHBONE");
			if (fishbonePrefab == null) {
				Debug.LogError ("no FISHBONE PREFAB in Resources folder found in " + this);
			}
		}
	}

    private void Update()
    {
		hunger += normalHungerIncrement * Time.deltaTime;
    }

}
