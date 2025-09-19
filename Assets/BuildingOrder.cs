using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingOrder : Building
{
    Dictionary<Item, int> buildCost;
    Inventory inv;
    public List<Item> items = new List<Item>();
    public List<int> amounts = new List<int>();
    public GameObject building;
    SquashAndStretch squashAndStretch;
    void Awake()
    {
        inv = GetComponent<Inventory>();
        squashAndStretch = GetComponent<SquashAndStretch>();
    }
    void Start()
    {
        buildCost = ItemManager.ReturnInventoryArray();

        for (int i = 0; i < items.Count; i++)
        {
            int amount = amounts[i];
            buildCost[items[i]] = amount;
            if (buildCost[items[i]] != 0)
            {
                while (amount != 0)
                {
                    int requestAmount = FrogOrderManager.instance.workerFrogCarryCapacity;
                    if (amount < FrogOrderManager.instance.workerFrogCarryCapacity) requestAmount = amount % FrogOrderManager.instance.workerFrogCarryCapacity;

                    StockpileRequest request = new StockpileRequest(items[i], requestAmount);
                    requests.Add(request);

                    amount -= requestAmount;
                }
            }
        }
    }

    public override void StockpileInteract(Stockpile stockpile, FrogWorker worker)
    {
        base.StockpileInteract(stockpile, worker);
        inv.Combine(worker.inv);

        requests.Remove(stockpile.errands[worker].request);

        stockpile.ResetErrand(worker);

        worker.WalkToPoint(stockpile.transform.position);

        squashAndStretch.Play();
    }

    void Update()
    {
        if (requests.Count == 0)
        {
            Instantiate(building, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
