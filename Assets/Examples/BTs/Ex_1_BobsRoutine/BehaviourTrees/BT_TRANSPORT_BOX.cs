using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_TRANSPORT_BOX", menuName = "Behaviour Trees/BT_TRANSPORT_BOX", order = 1)]
public class BT_TRANSPORT_BOX : BehaviourTree
{
   
    // notice how this BT "uses" a simpler one (BT_GET_ITEM)
    // Also notice how BT_GET_ITEM has four parameters
    // At creation time, the names of the keys are provided
    public override void OnConstruction()
    {
     
        root = new Sequence();

        BT_GET_ITEM getItem = new BT_GET_ITEM(
            "findRadius",  // key. Blackboard must have an entry with key findRadius
            "BoxTag",         // key. Blackboard must have an entry with key BOX
            "liftsound",   // key. Blackboard must have an entry with key liftsound
            "instanceFound" // keyout. Blackboard will have an entry with key instanceFound
                            // this entry will contain the instance detected by BT_GET_ITEM if any...
        );

        Action arrive = new ACTION_Arrive("theWarehouse");
        Action drop = new ACTION_Drop("instanceFound");
        Action playSound = new ACTION_PlaySound("dropSound");
        Action setTag = new ACTION_SetTag("instanceFound", "DROPPED");

        root.AddChildren(getItem, arrive, drop, playSound, setTag);
    }
}
