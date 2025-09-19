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
    public bool Debug1;
    void Awake()
    {
        inv = GetComponent<Inventory>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        squashAndStretch = GetComponent<SquashAndStretch>();   
    }
    void Update()
    {
        FrogControl();

        if (Debug1)
        {
            Debug1 = !Debug1;
            Debug.Log(gameObject.name + ": ");
            foreach (KeyValuePair<Item, int> pair in inv.inventory)
            {
                Debug.Log(pair.Key + " - " + pair.Value);
            }
        }
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
            int index = errand.targetBuilding.requests.IndexOf(errand.request);
            errand.targetBuilding.requests[index].fulfilled = false;
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
                    errands[worker].errandType = StockpileWorkErrandType.Store;
                    errands[worker].targetBuilding = this;

                    worker.WalkToPoint(transform.position);
                }
                // If not find a building that has requests
                else
                {
                    foreach (Building building in buildingsInRange)
                    {
                        if (errands[worker].errandType != StockpileWorkErrandType.None) break;

                        for (int i = 0; i < building.requests.Count; i++)
                        {
                            StockpileRequest request = building.requests[i];
                            if (!request.fulfilled)
                            {
                                // Deliver
                                if (request.type == StockpileRequestType.Deliver && inv.TryRemove(request.item, request.amount))
                                {
                                    errands[worker].errandType = StockpileWorkErrandType.GetFromStorage;
                                    errands[worker].targetBuilding = this;
                                    errands[worker].deliverTarget = building;
                                    worker.WalkToPoint(transform.position);
                                }
                                // Take
                                else if (request.type == StockpileRequestType.Take)
                                {
                                    errands[worker].errandType = StockpileWorkErrandType.WalkToOrder;
                                    errands[worker].targetBuilding = building;

                                    worker.WalkToPoint(building.transform.position);

                                }
                                else continue;

                                building.requests[i].fulfilled = true;
                                errands[worker].request = building.requests[i];

                                worker.idle = false;
                                break;
                            }
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
            else if (errand.errandType == StockpileWorkErrandType.Store)
            {
                if (!worker.walking)
                {
                    if (inv.TryCombine(worker.inv))
                    {
                        // Add items and reset worker
                        inv.Combine(worker.inv);

                        ResetErrand(worker);

                        squashAndStretch.Play();
                        
                        if (inv.itemSum >= thresholds[2]) spriteRenderer.sprite = fullnessSprites[2];
                        else if (inv.itemSum >= thresholds[1]) spriteRenderer.sprite = fullnessSprites[1];
                    }
                }
            }
            // Checks if the worker has collected the request items
            else if (errand.errandType == StockpileWorkErrandType.GetFromStorage)
            {
                if (!worker.walking)
                {
                    if (inv.TryRemove(errands[worker].request.item, errands[worker].request.amount))
                    {
                        inv.RemoveItemUnconstrained(errands[worker].request.item, errands[worker].request.amount);
                        worker.inv.AddItemUnconstrained(errands[worker].request.item, errands[worker].request.amount);

                        errands[worker].targetBuilding = errands[worker].deliverTarget;
                        errands[worker].errandType = StockpileWorkErrandType.WalkToOrder;

                        worker.WalkToPoint(errands[worker].targetBuilding.transform.position);

                        squashAndStretch.Play();

                        if (inv.itemSum < thresholds[1]) spriteRenderer.sprite = fullnessSprites[0];
                        else if (inv.itemSum < thresholds[2]) spriteRenderer.sprite = fullnessSprites[1];
                    }
                    else
                    {
                        int index = errands[worker].targetBuilding.requests.IndexOf(errands[worker].request);
                        errands[worker].targetBuilding.requests[index].fulfilled = false;
                        ResetErrand(worker);
                    }
                }
            }
        }
    }

    public void ResetErrand(FrogWorker worker)
    {
        errands[worker] = new StockpileWorkErrand();
        worker.idle = true;
    }
    public class StockpileWorkErrand
    {
        public StockpileWorkErrandType errandType;
        public Building targetBuilding;
        public StockpileRequest request;
        public Building deliverTarget;

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
        Store,
        GetFromStorage
    }
}
