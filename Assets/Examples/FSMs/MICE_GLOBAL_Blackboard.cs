using UnityEngine;

public class MICE_GLOBAL_Blackboard : MonoBehaviour
{
	public GameObject announcedCheese;		// the cheese some mouse has found
	public float cheeseAnnounceTTL = 30f;	// the time the announce will last

	public float elapsedTime = 0;

	public void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime >= cheeseAnnounceTTL)
			announcedCheese = null; // 
	}

	public void AnnounceCheese (GameObject Cheese) {
		announcedCheese = Cheese;
		elapsedTime = 0f;
	}
}

