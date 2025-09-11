using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiberHarvester : Building
{
    public override void Select()
    {
        List<FrogWorker> newWorkers = FrogOrderManager.AssignWorkers();
    
        foreach (FrogWorker worker in newWorkers)
        {
            workers.Add(worker);
            worker.WalkToPoint(transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0));
            worker.workingIn = this;
        }
    }
}
