using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_MakeFire", menuName = "Behaviour Trees/BT_MakeFire", order = 1)]
public class BT_MakeFire : BehaviourTree
{
    

    public override void OnConstruction()
    {
        // ------------------------------------------------
        //               "guarded" selectors
        // ------------------------------------------------
        // when condition is true the selector succeeds (and the item is not got
        // because the agent already possesses it

        Selector getWood = new Selector(
            new CONDITION_ParentsObjectWithTag("WOOD"),
            new BT_GET_ITEM("1000.0", "WOOD", "liftsound", "theWood")
        );

        Selector getMatches = new Selector(
            new CONDITION_ParentsObjectWithTag("MATCHES"),
            new BT_GET_ITEM("1000.0", "MATCHES", "liftsound", "theMatches")
        );

        Selector getNeswpaper = new Selector(
            new CONDITION_ParentsObjectWithTag("NEWSPAPER"),
            new BT_GET_ITEM("1000.0", "NEWSPAPER", "liftsound", "theNewspaper")
        );

        // ------------------------------------------------
        //               Random sequence
        // ------------------------------------------------
        // use a sequence to get the ingredients one after the other
        // use RandomSequence to get a less predictable behaviour
        Sequence getIngredients = new RandomSequence(
            getWood, getMatches, getNeswpaper
        );

        // construct the root combining the intermediate elements into a sequence

        root = new Sequence(
            getIngredients,
            new ACTION_Arrive("firepit", "25"),
            new ACTION_WaitForSeconds("1.5"),
            new ACTION_LeaveAtByTag("firepit", "WOOD"),
            new ACTION_LeaveAtByTag("firepit", "NEWSPAPER"),
            new ACTION_WaitForSeconds("2.5"),
            new ACTION_Activate("fire")
        );
 
    }
}
