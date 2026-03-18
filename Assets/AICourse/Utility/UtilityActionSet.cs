using UnityEngine;

//[CreateAssetMenu(fileName = "UtilityActionSet", menuName = "Scriptable Objects/UtilityActionSet")]
public abstract class UtilityActionSet : ScriptableObject
{
    // internally this class keps a list of pairs action/IConsideration
    // (IConsideration could be a Consideration or a Scorer)
    
    public virtual void OnConstruction()
    {
        
    }
}