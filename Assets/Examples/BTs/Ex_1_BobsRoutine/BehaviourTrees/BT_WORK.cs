using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_WORK", menuName = "Behaviour Trees/BT_WORK", order = 1)]
public class BT_WORK : BehaviourTree
{
   
    // Notice how this BT "uses" a simpler one (BT_TRANSPORT_BOX)
    public override void OnConstruction()
    {
        root = new Sequence(
            new RepeatTimesDecorator("3",
                                     new Sequence(
                                         new ACTION_Arrive("theBoxesArea"),
                                         new BT_TRANSPORT_BOX()
                                     )
            ),
            new ACTION_GetPaid()
        ); 
    }
}




