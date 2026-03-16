using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_SAM", menuName = "Behaviour Trees/BT_SAM", order = 1)]
public class BT_SAM : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new Sequence(
            new ACTION_FindByName("DAISY", "belovedDaisy"),
            new ACTION_Arrive("belovedDaisy", "15.0")
        ) ;
    }
}
