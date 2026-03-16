using UnityEngine;

public class MoveToClick : MonoBehaviour {

	public bool rightClick = false;
	
    private int buttonNumber;

    void Start ()
    {
        if (rightClick) buttonNumber = 1;
        else buttonNumber = 0;
    }


	// Update is called once per frame
	void Update () {
		
        if (Input.GetMouseButtonDown(buttonNumber))
        {
            Vector3 click = Input.mousePosition;
            Vector3 wantedPosition = Camera.main.ScreenToWorldPoint(new Vector3(click.x, click.y, 1f));
            wantedPosition.z = transform.position.z;
            transform.position = wantedPosition;
        }
	}
}
