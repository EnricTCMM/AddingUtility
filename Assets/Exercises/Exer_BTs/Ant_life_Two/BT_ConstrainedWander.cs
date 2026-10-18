using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_ConstrainedWander", menuName = "Behaviour Trees/BT_ConstrainedWander", order = 1)]

public class BT_ConstrainedWander : BehaviourTree
{
    public BT_ConstrainedWander() {
    }
    public override void OnConstruction()
    {
        DynamicSelector dyn = new DynamicSelector();
        dyn.AddChild(
            new CONDITION_FeelUnsafe("nest", "safeRadius", "extraSafeRadius"),
            new ACTION_WanderAround("nest", "highSW")
        );
        dyn.AddChild(
            new CONDITION_AlwaysTrue(),
            new ACTION_WanderAround("nest", "lowSW")
        );
        root = dyn;
    }
}