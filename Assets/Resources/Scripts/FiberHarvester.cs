using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FiberHarvester : Building
{
    [Header("Harvester")]
    public float plantSearchRange;
    public float harvestTime;
    public int fiber = 0;
    Dictionary<FrogWorker, HarvestErrand> errands = new Dictionary<FrogWorker, HarvestErrand>();
    public List<Plant> plantsInRange = new List<Plant>();
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
        errands.Add(workerToAdd, new HarvestErrand());
    }

    public override void RemoveWorker(FrogWorker workerToRemove)
    {
        base.RemoveWorker(workerToRemove);
        if (errands[workerToRemove].plant != null) errands[workerToRemove].plant.targetedForHarvest = false;
        errands.Remove(workerToRemove);
    }

    public override void StockpileInteract(Stockpile stockpile, FrogWorker worker)
    {
        // Replace the work errand in the stockpile with a inventory errand
        base.StockpileInteract(stockpile, worker);

        // Change the errand order in the stockpile
        stockpile.errands[worker].errandType = Stockpile.StockpileWorkErrandType.Bring;
        stockpile.errands[worker].targetBuilding = stockpile;

        // Calculate the amount that will be taken
        int amountToGrab;
        if (fiber >= FrogOrderManager.instance.workerFrogCarryCapacity) amountToGrab = FrogOrderManager.instance.workerFrogCarryCapacity;
        else amountToGrab = fiber%FrogOrderManager.instance.workerFrogCarryCapacity;
        
        // Fix request amounts
        fiber -= amountToGrab;
        stockpileRequests--;
        stockpileRequestsFulfilled--;

        // Give items to the worker
        worker.inv.AddItem(ItemManager.GetItem("Fiber"), amountToGrab);

        squashAndStretch.StartAnimation();
    }
    void Update()
    {
        PlantSearch();
        FrogControl();
        stockpileRequests = Mathf.FloorToInt((float)fiber/FrogOrderManager.instance.workerFrogCarryCapacity);
    }
    void PlantSearch()
    {
        int workablePlants = 0;
        foreach (Plant plant in plantsInRange)
        {
            if (!plant.targetedForHarvest && plant.harvestable)
            {
                workablePlants++;
            }
        }

        if (workablePlants == 0)
        {
            // Find all plants in range
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, plantSearchRange, LayerMask.GetMask("Plants"));
            for (int i = 0; i < hits.Length; i++)
            {
                Plant plant = hits[i].transform.GetComponent<Plant>();
                // Exclude plants that are being harvested
                if (!plant.targetedForHarvest && plant.harvestable)
                {
                    plantsInRange.Add(plant);
                }
            }
        }
    }
    void FrogControl()
    {
        foreach (FrogWorker worker in workers)
        {
            HarvestErrand errand = errands[worker];
            // Find a plant for each worker
            if (errand.errandType == HarversterErrandType.None)
            {
                for (int i = 0; i < plantsInRange.Count; i++)
                {
                    Plant plant = plantsInRange[i];
                    if (plant.targetedForHarvest) continue;

                    errands[worker].errandType = HarversterErrandType.Move;
                    errands[worker].time = 0;
                    errands[worker].plant = plant;
                    worker.WalkToPoint(plant.transform.position);

                    plant.targetedForHarvest = true;
                    plantsInRange.RemoveAt(i);

                    worker.idle = false;
                    break;
                }
            }
            // Move to plant
            else if (errand.errandType == HarversterErrandType.Move)
            {
                if (!worker.walking)
                {
                    errands[worker].errandType = HarversterErrandType.Harvest;
                }

            }
            // Count down until the plant should be harvested
            else if (errand.errandType == HarversterErrandType.Harvest)
            {
                errand.time += Time.deltaTime;

                if (errand.time >= harvestTime)
                {
                    plantsInRange.Remove(errand.plant);
                    errand.plant.Harvest();

                    errand.yield = errand.plant.yield;
                    errand.plant = null;
                    errand.time = 0;
                    errands[worker].errandType = HarversterErrandType.Collect;

                    worker.WalkToPoint(transform.position);
                }
            }
            // Bring the frog back
            else if (errand.errandType == HarversterErrandType.Collect)
            {
                if (!worker.walking)
                {
                    errands[worker].errandType = HarversterErrandType.None;
                    fiber += errand.yield;

                    squashAndStretch.StartAnimation();
                    worker.idle = true;
                }
            }
        }
    }

    enum HarversterErrandType
    {
        None,
        Move,
        Harvest,
        Collect
    }

    class HarvestErrand
    {
        public HarversterErrandType errandType;
        public float time;
        public Plant plant;
        public int yield;

        public HarvestErrand()
        {
            errandType = HarversterErrandType.None;
            time = 0;
            plant = null;
        }
    }
}
