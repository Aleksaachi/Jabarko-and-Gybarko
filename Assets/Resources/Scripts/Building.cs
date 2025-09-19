using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable
{
    public List<FrogWorker> workers = new List<FrogWorker>();
    [Header("Stockpile requests (Debug only)")]
    [SerializeField]
    public List<StockpileRequest> requests = new List<StockpileRequest>();

    public virtual void RemoveWorker(FrogWorker workerToRemove)
    {
        workers.Remove(workerToRemove);
        workerToRemove.workingIn = null;
    }

    public virtual void AddWorker(FrogWorker workerToAdd)
    {
        workers.Add(workerToAdd);
        workerToAdd.workingIn = this;
        workerToAdd.idlePoint = transform.position;
    }

    public virtual void StockpileInteract(Stockpile stockpile, FrogWorker worker)
    {

    }

    [Serializable]
    public class StockpileRequest
    {
        public StockpileRequestType type;
        public Item item;
        public int amount;
        public bool fulfilled;

        public StockpileRequest()
        {
            type = StockpileRequestType.Take;
        }
        public StockpileRequest(Item _item, int _amount)
        {
            type = StockpileRequestType.Deliver;
            item = _item;
            amount = _amount;
        }
    }

    public enum StockpileRequestType
    {
        Take,
        Deliver
    }
}
