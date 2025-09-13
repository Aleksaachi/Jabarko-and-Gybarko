using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable
{
    public List<FrogWorker> workers = new List<FrogWorker>();
    [Header("Stockpile requests (Debug only)")]
    public int stockpileRequests;
    public int stockpileRequestsFulfilled;

    public virtual void RemoveWorker(FrogWorker workerToRemove)
    {
        workers.Remove(workerToRemove);
        workerToRemove.workingIn = null;
    }

    public virtual void AddWorker(FrogWorker workerToAdd)
    {
        workers.Add(workerToAdd);
        workerToAdd.workingIn = this;
    }

    public virtual void StockpileInteract(Stockpile stockpile, FrogWorker worker)
    {

    }
}
