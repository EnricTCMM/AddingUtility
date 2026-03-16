using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_DRINK_BEER_IN_BAR", menuName = "Behaviour Trees/BT_DRINK_BEER_IN_BAR", order = 1)]
public class BT_DRINK_BEER_IN_BAR : BehaviourTree
{
    // This behaviour fails if BuyBeer fails
    // It succeeds if the last action (DrinkBeer) Succeeds
    
    public override void OnConstruction()
    {
        // first create the sequence...
        root = new Sequence();

        // and then add the children one by one and in the right order
        root.AddChild(new ACTION_Arrive("theBar"));
        root.AddChild(new ACTION_BuyBeer());
        root.AddChild(new ACTION_Activate("theBubbles"));
        root.AddChild(new ACTION_PlaySound("theBurpingSound"));
        root.AddChild(new ACTION_WaitForSeconds("3"));
        root.AddChild(new ACTION_Deactivate("theBubbles"));
        root.AddChild(new ACTION_DrinkBeer());
    }
}
