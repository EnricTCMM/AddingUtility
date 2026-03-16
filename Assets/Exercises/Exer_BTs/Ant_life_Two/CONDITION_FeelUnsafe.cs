using UnityEngine;
using BTs;

public class CONDITION_FeelUnsafe : Condition
{
    // parameters
    public string keySafeRadius;
    public string keyExtraSafeRadius;
    public string keyAttractor;

    // other (private) stuff used by the condition
    private bool lastTick = false;
    private GameObject attractor;
    private float safeRadius;
    private float extraSafeRadius;

    // Constructor
    public CONDITION_FeelUnsafe( string keyAttractor,
                                 string keySafeRadius,
                                 string keyExtraSafeRadius)  
    {
        this.keyAttractor = keyAttractor;
        this.keySafeRadius = keySafeRadius;
        this.keyExtraSafeRadius = keyExtraSafeRadius;
    }

    

    public override void OnInitialize()
    {
        attractor = blackboard.Get<GameObject>(keyAttractor);
        safeRadius = blackboard.Get<float>(keySafeRadius);
        extraSafeRadius = blackboard.Get<float>(keyExtraSafeRadius);
        lastTick = false;
    }

    public override bool Check ()
    {

       /* COMPLETE */

       return lastTick;
    }
}
