using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_DAISY_ALONE", menuName = "Behaviour Trees/BT_DAISY_ALONE", order = 1)]
public class BT_DAISY_ALONE : BehaviourTree
{
    

     // construtor
    public BT_DAISY_ALONE()  { 
        /* Receive BT parameters and set them. Remember all are of type string */
    }
    
    public override void OnConstruction()
    {
        root = new RepeatForeverDecorator(
            new Sequence(
                new ACTION_ChooseRandomCorner("theCorner"),
                new ACTION_Arrive("theCorner")
           )
        );
    }
}
