using UnityEngine;

public class LILBOB_Blackboard : DynamicBlackboard
{
    public GameObject gym;
    public GameObject home;
    public GameObject store;
    public GameObject bank;
    public GameObject dump;

    public AudioClip impactSound;

    public string beerTag = "BEER";
    public string moneyTag = "MONEY";
    public string trashTag = "TRASH";
    public float beerDetectionRadius = 10;
    public float moneyDetectionRadius = 8;
    public float trashDetectionRadius = 8;

    public string outburst = "Oh f*ck\nNo Beer!";
    public string happyburst = "I love beer.\nCould't live without";
}
