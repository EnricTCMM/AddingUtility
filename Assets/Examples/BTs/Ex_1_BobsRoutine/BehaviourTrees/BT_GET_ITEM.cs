using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_GET_ITEM", menuName = "Behaviour Trees/BT_GET_ITEM", order = 1)]
public class BT_GET_ITEM : BehaviourTree
{
    // parametrized BTs should not be placed in the executor 
    // since the instantiation mechanism uses a parameterless 
    // constructor.
    
    public string keyFindRadius ;
    public string keyTag ;
    public string keyLiftSound ;
    public string keyoutObjectTaken ; // this is an output parameter

    public BT_GET_ITEM(string keyFindRadius, 
                       string keyTag, 
                       string keyLiftSound,
                       string keyoutObjectTaken) 
    {
        this.keyFindRadius = keyFindRadius;
        this.keyTag = keyTag;
        this.keyLiftSound = keyLiftSound;
        this.keyoutObjectTaken = keyoutObjectTaken;
    }


    public override void OnConstruction()
    {

        root = new Sequence();

        root.AddChild(new ACTION_FindInstanceWithinRadius(keyFindRadius,
                                                          keyTag,
                                                          keyoutObjectTaken));
        // FindInstanceWithinRadius 
        // will save the instance into the blackboard under the key
        // specified in parameter keyoutObjectTaken
        root.AddChild(new ACTION_Arrive(keyoutObjectTaken));
        root.AddChild(new ACTION_PlaySound(keyLiftSound));
        root.AddChild(new ACTION_Take(keyoutObjectTaken));
    }
}
