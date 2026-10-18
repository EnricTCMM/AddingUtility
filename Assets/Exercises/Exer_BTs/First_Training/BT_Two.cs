using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_Two", menuName = "Behaviour Trees/BT_Two", order = 1)]
public class BT_Two : BehaviourTree
{   
    public override void OnConstruction()
    {
        /* COMPLETE */
        root = new Sequence(
            new ACTION_Arrive("store"),
            new Selector(
                new Sequence(
                    new CONDITION_InstanceNear("beerDetectionRadius", "beerTag"),
                    new ACTION_Somersault(),
                    new ACTION_Speak("happyBurst")
                ),
                new Sequence(
                    new ACTION_Speak("outburst"),
                    new ACTION_WaitForSeconds("2"),
                    new ACTION_Arrive("home")
                )
            )
        );
    }
}
