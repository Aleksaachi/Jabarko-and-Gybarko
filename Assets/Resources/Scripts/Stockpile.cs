using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stockpile : Building
{   
    [Header("Stockpile")]
    [Header("Stockpile settings")]
    public float buildingSearchRange;
    public List<Building> buildingsInRange;
    public Dictionary<FrogWorker, StockpileWorkErrand> errands = new Dictionary<FrogWorker, StockpileWorkErrand>();
    public Sprite[] fullnessSprites;
    public int[] thresholds;
    public Inventory inv;
    SpriteRenderer spriteRenderer;
    SquashAndStretch squashAndStretch;
    void Awake()
    {
        inv = GetComponent<Inventory>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        squashAndStretch = GetComponent<SquashAndStretch>();   
    }
    void Update()
    {
        FrogControl();
    }
    public void RemoveItem(Item itemToRemove, int amountToRemove)
    {
        // This function tries to remove an item
        if (!inv.TryRemove(itemToRemove, amountToRemove)) return;
        inv.RemoveItemUnconstrained(itemToRemove, amountToRemove);

        if (inv.itemSum < thresholds[1]) spriteRenderer.sprite = fullnessSprites[0];
        else if (inv.itemSum < thresholds[2]) spriteRenderer.sprite = fullnessSprites[1];
    }
    public override void AddWorker(FrogWorker workerToAdd)
    {
        base.AddWorker(workerToAdd);
        errands.Add(workerToAdd, new StockpileWorkErrand());
    }

    public override void RemoveWorker(FrogWorker workerToRemove)
    {
        base.RemoveWorker(workerToRemove);
        StockpileWorkErrand errand = errands[workerToRemove];
        if (errand.targetBuilding != null && errand.targetBuilding != this) {
            errand.targetBuilding.stockpileRequestsFulfilled--;
        }
        
        errands.Remove(workerToRemove);
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

    void FrogControl()
    {
        if (buildingsInRange.Count == 0)
        {
            // Find all plants in range
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, buildingSearchRange, LayerMask.GetMask("Selectable"));
            for (int i = 0; i < hits.Length; i++)
            {
                Building building = hits[i].GetComponent<Building>();
                if (building != null)
                {
                    buildingsInRange.Add(building);
                }
            }
        }

        foreach (FrogWorker worker in workers)
        {
            StockpileWorkErrand errand = errands[worker];
            // Find an errand for this frog worker
            if (errand.errandType == StockpileWorkErrandType.None)
            {
                // If the frog has items return them
                if (worker.inv.itemSum != 0)
                {
                    errands[worker].errandType = StockpileWorkErrandType.Bring;
                    errands[worker].targetBuilding = this;

                    worker.WalkToPoint(transform.position);
                }
                // If not find a building that has requests
                else
                {
                    foreach (Building building in buildingsInRange)
                    {
                        if (building.stockpileRequests > building.stockpileRequestsFulfilled)
                        {
                            errands[worker].errandType = StockpileWorkErrandType.WalkToOrder;
                            errands[worker].targetBuilding = building;
                            building.stockpileRequestsFulfilled++;
                            worker.WalkToPoint(building.transform.position);
                            worker.idle = false;
                            break;
                        }
                    }
                }
            }
            // Checks if the worker reached the errand destination
            else if (errand.errandType == StockpileWorkErrandType.WalkToOrder)
            {
                if (!worker.walking)
                {
                    errand.targetBuilding.StockpileInteract(this, worker);
                    worker.WalkToPoint(transform.position);
                }
            }
            // Checks if the delivery is complete
            else if (errand.errandType == StockpileWorkErrandType.Bring)
            {
                if (!worker.walking)
                {
                    if (inv.TryCombine(worker.inv))
                    {
                        // Add items and reset worker
                        inv.Combine(worker.inv);
                        errands[worker] = new StockpileWorkErrand();
                        squashAndStretch.Play();
                        worker.idle = true;

                        if (inv.itemSum >= thresholds[2]) spriteRenderer.sprite = fullnessSprites[2];
                        else if (inv.itemSum >= thresholds[1]) spriteRenderer.sprite = fullnessSprites[1];
                    }
                }
            }
        }
    }

    public class StockpileWorkErrand
    {
        public StockpileWorkErrandType errandType;
        public Building targetBuilding;

        public StockpileWorkErrand()
        {
            errandType = StockpileWorkErrandType.None;
            targetBuilding = null;
        }
    }
    public enum StockpileWorkErrandType
    {
        None,
        WalkToOrder,
        Bring
    }
}
