using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_DAISY_WHEN_SAM", menuName = "Behaviour Trees/BT_DAISY_WHEN_SAM", order = 1)]
public class BT_DAISY_WHEN_SAM : BehaviourTree
{
    

     // construtor
    public BT_DAISY_WHEN_SAM()  { 
        /* Receive BT parameters and set them. Remember all are of type string */
    }
    
    public override void OnConstruction()
    {
        DynamicSelector dyn =  new DynamicSelector();

        dyn.AddChild(
            new CONDITION_InstanceNear("samDetectionRadius", "samTag", "true", "samKey"),
            new Sequence(
                new ACTION_Arrive("samKey", "25"),
                new Selector(
                    new Sequence(
                        new CONDITION_InstanceNear("chocoDetectionRadius", "chocoTag"),
                        new ACTION_Activate("fingerParticleSystem"),
                        new ACTION_Arrive("farAwayPoint")
                    ),
                    new ACTION_Activate("heartParticleSystem")
                )
            )
        );
        
        dyn.AddChild(
            new CONDITION_AlwaysTrue(),
            ScriptableObject.CreateInstance<BT_DAISY_ALONE>()
        );

        root = dyn;
    }
}
