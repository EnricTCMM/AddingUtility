using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_WanderAndCollectSeeds", menuName = "Behaviour Trees/BT_WanderAndCollectSeeds", order = 1)]
public class BT_WanderAndCollectSeeds : BehaviourTree
{
    
     // construtor
    public BT_WanderAndCollectSeeds()  { 
        /* Receive BT parameters and set them. Remember all are of type string */
    }
    
    public override void OnConstruction()
    {
        DynamicSelector dynSel = new DynamicSelector();
        BT_ConstrainedWander cw = CreateInstance<BT_ConstrainedWander>();
        
        // first branch
        dynSel.AddChild(
            new CONDITION_InstanceNear("seedDetectionRadius", "SEED", "true", "theSeed"),
            new Sequence(
                new ACTION_Arrive("theSeed", "2"), 
                new ACTION_Take("theSeed"),
                new ACTION_Arrive("nest", "5"),
                new ACTION_Drop("theSeed"),
                new ACTION_SetTag("theSeed", "DROPPED")
            )
        );
        
        // second branch
        dynSel.AddChild(
            new CONDITION_AlwaysTrue(),
            cw // <------ any BT is a task
        );
        
        // for part B, root would be dynSel (root = dynSel)
        // for part C, root is the endless repetition of dynSel

        root = new RepeatForeverDecorator(dynSel);
    }
}
