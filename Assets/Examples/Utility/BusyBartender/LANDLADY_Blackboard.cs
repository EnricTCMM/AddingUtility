using UnityEngine;

public class LANDLADY_Blackboard : DynamicBlackboard
{

    [Range(0, 20)]  public int clientsWaitingBeer = 0;
    [Range(0, 5)] public int dirtyTables = 0;
    [Range(0, 4)] public int sleepingDrunkards = 0;
    [Range(0, 1)] public int clientsQuarreling = 0;
    [Range(0, 100)] public int  tankardsInBarrel = 0;
    
    [Range(0, 1)] public float  baselineUtility = 0.1f;
    
    public void incClientsWaitingBeer() { clientsWaitingBeer = Mathf.Min(clientsWaitingBeer + 1, 20); }
    public void decClientsWaitingBeer() { clientsWaitingBeer = Mathf.Max(clientsWaitingBeer - 1, 0); }

    public void incDirtyTables() { dirtyTables = Mathf.Min(dirtyTables + 1, 5); }
    public void decDirtyTables() { dirtyTables = Mathf.Max(dirtyTables - 1, 0); }
}
