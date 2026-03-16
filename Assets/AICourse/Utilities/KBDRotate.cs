using UnityEngine;


public class KBDRotate : MonoBehaviour {

	public float angularSpeed = 90;


	// Update is called once per frame
	void Update () {


		// RIGHT ARROW (KEY) => rotate clockwise (add)
		// LEFT ARROW (KEY) => rotate anticlockwise (substract)


		float move = Input.GetAxis("Horizontal");

		float orientation = transform.eulerAngles.z;
		orientation = orientation - move * angularSpeed * Time.deltaTime;
		transform.rotation = Quaternion.Euler(0, 0, orientation);
	}
}
