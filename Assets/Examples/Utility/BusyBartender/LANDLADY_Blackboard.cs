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
    
    public void incSleepingDrunkards() { sleepingDrunkards = Mathf.Min(sleepingDrunkards + 1, 4); }
    public void decSleepingDrunkards() { sleepingDrunkards = Mathf.Max(sleepingDrunkards - 1, 0); }
    
    public void incClientsQuarreling() { clientsQuarreling = Mathf.Min(clientsQuarreling + 1, 1); }
    public void decClientsQuarreling() { clientsQuarreling = Mathf.Max(clientsQuarreling - 1, 0); }
    
    public void incTankardsInBarrel() { tankardsInBarrel = Mathf.Min(tankardsInBarrel + 1, 100); }
    public void decTankardsInBarrel() { tankardsInBarrel = Mathf.Max(tankardsInBarrel - 1, 0); }
    
    public void emptyBarrel() { tankardsInBarrel = 0; }
}
