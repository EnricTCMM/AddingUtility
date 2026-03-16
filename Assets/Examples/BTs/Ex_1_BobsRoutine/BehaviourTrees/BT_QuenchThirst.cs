using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_QuenchThirst", menuName = "Behaviour Trees/BT_QuenchThirst", order = 1)]
public class BT_QuenchThirst : BehaviourTree
{
    
    
    public override void OnConstruction()
    {
        root = new Selector();

        // first child
        root.AddChild(new BT_DRINK_BEER_IN_BAR());
        
        // second child is itself a sequence
        root.AddChild(new Sequence(
                new BT_WITHDRAW_MONEY_FROM_BANK(),
                new BT_DRINK_BEER_IN_BAR()
            )
        );

        // third child, another sequence
        root.AddChild(new Sequence(
                new BT_WORK(),
                new BT_WITHDRAW_MONEY_FROM_BANK(),
                new BT_DRINK_BEER_IN_BAR()
            )
        );


    }
}
