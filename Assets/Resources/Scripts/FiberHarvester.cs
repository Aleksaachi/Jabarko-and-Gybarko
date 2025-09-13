using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FiberHarvester : Building
{
    public float plantSearchRange;
    public float harvestTime;
    public int harvests = 0;
    Dictionary<FrogWorker, HarversterWorkErrand> errands = new Dictionary<FrogWorker, HarversterWorkErrand>();
    Dictionary<FrogWorker, float> errandTime = new Dictionary<FrogWorker, float>();
    Dictionary<FrogWorker, GameObject> errandHarvest = new Dictionary<FrogWorker, GameObject>();
    public List<GameObject> plantsInRange = new List<GameObject>();
    public List<GameObject> plantSelectedForHarvest = new List<GameObject>();
    SquashAndStretch squashAndStretch;
    void Awake()
    {
        squashAndStretch = GetComponent<SquashAndStretch>();
    }
    public override void Select()
    {
        List<FrogWorker> newWorkers = FrogOrderManager.AssignWorkers();

        foreach (FrogWorker worker in newWorkers)
        {
            AddWorker(worker);
            worker.WalkToPoint(transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0));
        }
    }
    public override void AddWorker(FrogWorker workerToAdd)
    {
        base.AddWorker(workerToAdd);
        errands.Add(workerToAdd, HarversterWorkErrand.None);
    }

    public override void RemoveWorker(FrogWorker workerToRemove)
    {
        base.RemoveWorker(workerToRemove);
        errands.Remove(workerToRemove);
        plantSelectedForHarvest.Remove(errandHarvest[workerToRemove]);
        errandHarvest.Remove(workerToRemove);
        errandTime.Remove(workerToRemove);
        
    }
    void Update() {
        FrogControl(); 
    }
    void FrogControl()
    {
        if (plantsInRange.Count == 0)
        {
            // Find all plants in range
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, plantSearchRange, LayerMask.GetMask("Plants"));
            for (int i = 0; i < hits.Length; i++)
            {
                // Exclude plants that are being harvested
                if (!plantSelectedForHarvest.Contains(hits[i].gameObject))
                {
                    plantsInRange.Add(hits[i].transform.gameObject);
                }
            }
        }

        foreach (FrogWorker worker in workers)
        {
            // Find a plant for each worker
            if (errands[worker] == HarversterWorkErrand.None)
            {
                if (plantsInRange.Count != 0)
                {
                    errands[worker] = HarversterWorkErrand.Move;

                    errandTime.Add(worker, 0);
                    errandHarvest.Add(worker, plantsInRange[0]);
                    worker.WalkToPoint(errandHarvest[worker].transform.position);
                    plantSelectedForHarvest.Add(errandHarvest[worker]);
                    plantsInRange.RemoveAt(0);
                    
                }
            }
            // Move to plant
            else if (errands[worker] == HarversterWorkErrand.Move)
            {
                if (!worker.walking)
                {
                    errands[worker] = HarversterWorkErrand.Harvest;
                }
                
            }
            // Count down until the plant should be harvested
            else if (errands[worker] == HarversterWorkErrand.Harvest)
            {
                errandTime[worker] += Time.deltaTime;

                if (errandTime[worker] >= harvestTime)
                {
                    plantsInRange.Remove(errandHarvest[worker]);
                    Destroy(errandHarvest[worker]);
                    errandHarvest.Remove(worker);
                    errandTime.Remove(worker);
                    errands[worker] = HarversterWorkErrand.Collect;

                    worker.WalkToPoint(transform.position);
                }
            }
            // Bring the frog back
            else if (errands[worker] == HarversterWorkErrand.Collect)
            {
                if (!worker.walking)
                {
                    errands[worker] = HarversterWorkErrand.None;
                    harvests++;
                    squashAndStretch.StartAnimation();
                }
            }
        }
    }

    enum HarversterWorkErrand
    {
        None,
        Move,
        Harvest,
        Collect
    }
}
