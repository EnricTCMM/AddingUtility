
using UnityEngine;

public class GHOST_Blackboard : MonoBehaviour
{

    public GameObject booParticleSystem;
    public GameObject castle;
    public float hideTime = 5;
    public float booDuration = 3;
    public float closeEnoughToScare = 8;
    public float castleReachedRadius = 2;
    public float nerdDetectionRadius = 42;
    public string victimLabel = "NERD";

    void Awake()
    {
        booParticleSystem = GameObject.Find("BooParticleSystem");
        booParticleSystem.SetActive(false);
        castle = GameObject.Find("CastleInside");
    }

    public void CryBoo(bool on)
        // use this method to turn boo system on/off
    {
        booParticleSystem.SetActive(on);
    }
}
