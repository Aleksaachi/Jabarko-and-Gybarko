using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable
{
    public List<FrogWorker> workers = new List<FrogWorker>();

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
}
