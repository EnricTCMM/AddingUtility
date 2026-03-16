using UnityEngine;

public class HEN_Blackboard : MonoBehaviour
{
    public float wormDetectableRadius = 60; // within this radius worms are detected
    public float wormReachedRadius = 12;    // at this distace worm is eatable
    public float timeToEatWorm = 1.5f;      // it takes this time to eat a worm

    public float chickDetectionRadius = 100;   // within this radius chicks are detected
    public float chickFarEnoughRadius = 250;   // from this distance on chicks stop being an annoyance

    public GameObject attractor;     // hen wanders around this point

    public AudioClip angrySound;
    public AudioClip eatingSound;
    public AudioClip cluckingSound;

    void Awake()
    {
        attractor = GameObject.Find("Attractor");
        angrySound = Resources.Load<AudioClip>("Sounds/AngryChicken");
        eatingSound = Resources.Load<AudioClip>("Sounds/Chew");
        cluckingSound = Resources.Load<AudioClip>("Sounds/ChickenClucking");
    }
}
