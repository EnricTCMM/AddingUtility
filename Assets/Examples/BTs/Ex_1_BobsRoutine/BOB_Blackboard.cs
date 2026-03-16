using UnityEngine;

public class BOB_Blackboard : DynamicBlackboard {

    public float findRadius = 200;
    public GameObject theBar;
    public GameObject theBank;
    public float thirst = 0.0f;
    public string boxTag = "BOX";

    //...

    public int moneyInPocket = 0;
    public int moneyInAccount = 0;
    public int withdrawalAmmount = 20;
    public int salary = 20;
    public int priceOfBeer = 10;
    public float thirstReductionPerBeer = 75f;
    
    public float thirstIncrementPerSecond = 1;
    public float thirstThreshold = 100;
    public int flowers = 0;

    
    public GameObject theWarehouse;
    public GameObject theBoxesArea;
    public GameObject thePark;
    public GameObject theBubbles;
    public GameObject theDollars;
    public AudioClip theBurpingSound;
    public AudioClip theCurseSound;
    public AudioClip theSong;
    public AudioClip liftSound;
    public AudioClip dropSound;
    public AudioClip moneySound;

    private TextMesh pocketLine;
    private TextMesh accountLine;
    private TextMesh thirstLine;

    // Use this for initialization
    void  Start () {
       
        pocketLine = GameObject.Find("PocketLine").GetComponent<TextMesh>();
        if (pocketLine != null) pocketLine.text = "Pocket: " + moneyInPocket;

        accountLine = GameObject.Find("AccountLine").GetComponent<TextMesh>();
        if (accountLine != null) accountLine.text = "Account: " + moneyInAccount;

        thirstLine = GameObject.Find("ThirstLine").GetComponent<TextMesh>();
        if (thirstLine != null) thirstLine.text = "Thirst: " + Mathf.RoundToInt(thirst);
    }
	
	// Update is called once per frame
	void Update () {
        thirst += thirstIncrementPerSecond * Time.deltaTime;
        if (thirstLine != null) thirstLine.text = "Thirst: " + Mathf.RoundToInt(thirst);
    }


    public bool VeryThirsty ()
    {
        return thirst >= thirstThreshold;
    }

    public bool HasMoneyToBuyBeer ()
    {
        return moneyInPocket >= priceOfBeer;
    }

    public bool HasMoneyInAccount ()
    {
        return moneyInAccount >= withdrawalAmmount;
    }

    public bool BuyBeer()
    {
        if (!HasMoneyToBuyBeer())
            return false;
        else
        {
            moneyInPocket -= priceOfBeer;
            if (pocketLine != null) pocketLine.text = "Pocket: " + moneyInPocket;
            return true;
        }
    }

    public void DrinkBeer()
    {
        thirst -= thirstReductionPerBeer;
        if (thirstLine != null) thirstLine.text = "Thirst: " + Mathf.RoundToInt(thirst);
    }

    public void GetPaid () {
        moneyInAccount += salary;
        if (accountLine != null) accountLine.text = "Account: " + moneyInAccount;
    }

    public bool MakeWithdrawal ()
    {
        if (!HasMoneyInAccount()) 
            return false;
        else 
        {
            moneyInAccount -= withdrawalAmmount;
            moneyInPocket += withdrawalAmmount;
            if (pocketLine != null) pocketLine.text = "Pocket: " + moneyInPocket;
            if (accountLine != null) accountLine.text = "Account: " + moneyInAccount;
            return true;
        }
    }

    
}
