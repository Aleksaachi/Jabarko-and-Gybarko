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
    [Header("Inventory settings")]
    public int capacity;

    [SerializeField]
    public Item[] items;
    Dictionary<Item, int> inventory;
    int itemSum;
    public Sprite[] fullnessSprites;
    public int[] thresholds;
    SpriteRenderer spriteRenderer;
    SquashAndStretch squashAndStretch;
    public bool DEBUG1;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        squashAndStretch = GetComponent<SquashAndStretch>();
        inventory = ItemManager.ReturnInventoryArray();
    }
    void Update()
    {
        FrogControl();
        if (DEBUG1)
        {
            Debug.Log(itemSum);
            DEBUG1 = false;
        }
    }
    public bool TryAdd(int amountToAdd)
    {
        // Used to check if you can add new items to the stockpile, meant to be used before attempting to start an action that moves items into this
        if (itemSum + amountToAdd > capacity) return false;
        return true;
    }
    public void AddItem(Item itemToAdd, int amountToAdd)
    {
        // This function tries to add an item
        if (!TryAdd(amountToAdd)) return;

        AddItemUnconstrained(itemToAdd, amountToAdd);
    }
    public void AddItemUnconstrained(Item itemToAdd, int amountToAdd)
    {
        // This function is supposed to be used with TryAdd().
        inventory[itemToAdd] += amountToAdd;
        itemSum += amountToAdd;

        if (itemSum >= thresholds[2]) spriteRenderer.sprite = fullnessSprites[2];
        else if (itemSum >= thresholds[1]) spriteRenderer.sprite = fullnessSprites[1];
    }
    public bool TryRemove(Item itemToRemove, int amountToRemove)
    {
        // Used to check if you can remove a certain number of items from the stockpile, meant to be used before attempting to take items from this
        if (inventory[itemToRemove] < amountToRemove) return false;
        return true;
    }
    public void RemoveItem(Item itemToRemove, int amountToRemove)
    {
        // This function tries to remove an item
        if (!TryRemove(itemToRemove, amountToRemove)) return;

        RemoveItemUnconstrained(itemToRemove, amountToRemove);
    }
    public void RemoveItemUnconstrained(Item itemToRemove, int amountToRemove)
    {
        // This function is supposed to be used with TryRemove().
        inventory[itemToRemove] -= amountToRemove;
        itemSum -= amountToRemove;

        if (itemSum < thresholds[1]) spriteRenderer.sprite = fullnessSprites[0];
        else if (itemSum < thresholds[2]) spriteRenderer.sprite = fullnessSprites[1];
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
                foreach (Building building in buildingsInRange)
                {   
                    if (building.stockpileRequests > building.stockpileRequestsFulfilled)
                    {
                        errands[worker].errandType = StockpileWorkErrandType.WalkToOrder;
                        errands[worker].targetBuilding = building;
                        building.stockpileRequestsFulfilled++;
                        worker.WalkToPoint(building.transform.position);
                        break;
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
                    StockPileInventoryWorkErrand inventoryErrand = errand.GetInventoryErrand();
                    if (TryAdd(inventoryErrand.amount))
                    {
                        // Add items and reset worker
                        AddItemUnconstrained(inventoryErrand.item, inventoryErrand.amount);
                        errands[worker] = new StockpileWorkErrand();
                        squashAndStretch.StartAnimation();
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
        public virtual StockPileInventoryWorkErrand GetInventoryErrand()
        {
            return null;
        }
    }
    public class StockPileInventoryWorkErrand : StockpileWorkErrand
    {
        public Item item;
        public int amount;

        public StockPileInventoryWorkErrand(StockpileWorkErrand errandToCopy, Item itemToDeliver, int amountToDeliver)
        {
            errandType = errandToCopy.errandType;
            targetBuilding = errandToCopy.targetBuilding;

            item = itemToDeliver;
            amount = amountToDeliver;
        }

        public override StockPileInventoryWorkErrand GetInventoryErrand()
        {
            return this;
        }
    }
    public enum StockpileWorkErrandType
    {
        None,
        WalkToOrder,
        Bring
    }
}
