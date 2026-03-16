using UnityEngine;
using System.Collections;

public class TimeToLive : MonoBehaviour {

	public float secondsToLive = 5f;

	private float elapsedTime = 0f;

	public void Reset (float seconds)
	{
		secondsToLive = seconds;
		elapsedTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime >= secondsToLive)
			Object.Destroy (gameObject);
	}
}
