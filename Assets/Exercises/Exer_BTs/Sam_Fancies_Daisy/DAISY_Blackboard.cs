using UnityEngine;

public class DAISY_Blackboard : DynamicBlackboard {

    public GameObject fingerParticleSystem;
    public GameObject heartParticleSystem;
    public GameObject farAwayPoint;

    public float samDetectionRadius = 130;
    public float chocoDetectionRadius = 35;

    public string samTag = "SAM";
    public string chocoTag = "CHOCOLATES";
    
    private GameObject[] corners;

	void Start () {
        corners = GameObject.FindGameObjectsWithTag("CORNER");
       
        fingerParticleSystem = GameObject.Find("MiddleFingerParticleSystem");
        heartParticleSystem = GameObject.Find("HeartsParticleSystem");
        farAwayPoint = GameObject.Find("FAR POINT");

        fingerParticleSystem.SetActive(false);
        heartParticleSystem.SetActive(false);
	}
	
    public GameObject GetRandomCorner ()
    {
        // get a random corner
        return corners[Random.Range(0, corners.Length)];
    }
	
}
