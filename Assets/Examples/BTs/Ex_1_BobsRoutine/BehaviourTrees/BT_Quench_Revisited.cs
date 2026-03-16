using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_Quench_Revisited", menuName = "Behaviour Trees/BT_Quench_Revisited", order = 1)]
public class BT_Quench_Revisited : BehaviourTree
{
    
    public override void OnConstruction()
    {
        root = new Sequence(
            new Selector(
                new CONDTION_HasMoneyToBuyBeer(),
                new Sequence(
                    new Selector(
                        new CONDTION_HasMoneyInAccount(),
                        new BT_WORK()
                    ),
                    new BT_WITHDRAW_MONEY_FROM_BANK()
                )
            ),
            new BT_DRINK_BEER_IN_BAR()
        ); 
    }
}
