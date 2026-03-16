using UnityEngine;


public class MOUSE_Blackboard : MonoBehaviour {

	public float hunger = 0f;   // How hungry am I?

	// CONSTANTS
	public float hungerTooHigh = 100; // when this level is reachead I'M HUNGRY
	public float hungerLowEnough = 10; // when this level is reached I'M SATIATED
	public float normalHungerIncrement = 5.5f; // speed of hunger increment
	public float bitesPerSecond = 1f; // one bite each 1/bitesPerSecond seconds
	public float cheeseHungerDecrement = 10f; // decrement per bite
	public float cheeseDetectableRadius = 40f; // at this distance cheese is detectable
	public float cheeseReachedRadius = 10f; // at this distance, cheese is reachable (can be bitten)
	public float perilDetectableRadius = 100f; // at this distance enenmy is detectable
	public float perilSafetyRadius = 200f; // at this distance, enemy is no longer a peril

	public MICE_GLOBAL_Blackboard globalBlackboard; // the blackboard that all mice share

	// aux. methods
	public bool Hungry () { return hunger>=hungerTooHigh; }
	public bool Satited() { return hunger <= hungerLowEnough; }

}
