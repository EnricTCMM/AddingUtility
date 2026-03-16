using UnityEngine;


public class CheeseBehaviour : MonoBehaviour {

	public int numberOfBites = 100;
    private Vector3 originalSize;
    private int initialNumberOfBites;

    void Start ()
    {
        originalSize = transform.localScale;
        initialNumberOfBites = numberOfBites;
    }

	public void BeBitten () {
		numberOfBites--;
        transform.localScale = transform.localScale - originalSize / initialNumberOfBites;
		if (numberOfBites <= 0)
			Destroy (gameObject);
	}
}
