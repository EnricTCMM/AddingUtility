using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_BOB", menuName = "Behaviour Trees/BT_BOB", order = 1)]
public class BT_BOB : BehaviourTree
{
    public override void OnConstruction()
    {

        DynamicSelector singOrQuench = new DynamicSelector();
        // in Dynamic selectors, each child is a "pair" <condition, task>
        // Each pair must be added using AddChild

        singOrQuench.AddChild(
            new CONDITION_TooThisrty(),
            new BT_QuenchThirst()  // use BT_Quench_Revisited to prevent BOB from swearing when there's no 
                                   // money in the acccount
        );

        singOrQuench.AddChild(
            new CONDITION_AlwaysTrue(),
            new Sequence(
                new ACTION_Arrive("thePark"),
                new RepeatForeverDecorator( new ACTION_PlaySound("theSong", "1.0", "true"))
            )
        );

        // the root is just an endless repetion of singOrQuench
        root = new RepeatForeverDecorator(singOrQuench);
    }
}
