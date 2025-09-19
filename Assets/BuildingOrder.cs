using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingOrder : Building
{
    public BuildOrderTemplate buildOrderTemplate;
    Inventory inv;
    SquashAndStretch squashAndStretch;
    void Awake()
    {
        inv = GetComponent<Inventory>();
        squashAndStretch = GetComponent<SquashAndStretch>();
    }
    void Start()
    {
        // Create all necessary stockpile requests
        for (int i = 0; i < buildOrderTemplate.itemCost.Count; i++)
        {
            int amount = buildOrderTemplate.itemAmounts[i];

            while (amount != 0)
            {
                int requestAmount = FrogOrderManager.instance.workerFrogCarryCapacity;
                if (amount < FrogOrderManager.instance.workerFrogCarryCapacity) requestAmount = amount % FrogOrderManager.instance.workerFrogCarryCapacity;

                StockpileRequest request = new StockpileRequest(buildOrderTemplate.itemCost[i], requestAmount);
                requests.Add(request);

                amount -= requestAmount;
            }
        }
    }

    public override void StockpileInteract(Stockpile stockpile, FrogWorker worker)
    {
        // Take items from frog worker
        base.StockpileInteract(stockpile, worker);
        inv.Combine(worker.inv);

        requests.Remove(stockpile.errands[worker].request);

        stockpile.ResetErrand(worker);

        worker.WalkToPoint(stockpile.transform.position);

        squashAndStretch.Play();
    }

    void Update()
    {
        // Check if all request are completed
        if (requests.Count == 0)
        {
            Instantiate(buildOrderTemplate.prefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            EventManager.newBuildingPlaced?.Invoke();
        }
    }
}
