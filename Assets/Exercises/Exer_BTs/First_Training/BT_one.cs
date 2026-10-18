using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_one", menuName = "Behaviour Trees/BT_one", order = 1)]
public class BT_one : BehaviourTree
{
    
    public override void OnConstruction()
    {

       /* COMPLETE */
       root = new Sequence(
            new ACTION_Arrive("home"),
            new ACTION_Arrive("gym"),
            new ACTION_Somersault(),
            new ACTION_PlaySound("impactSound")
       );

    }
}
