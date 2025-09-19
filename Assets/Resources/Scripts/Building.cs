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
    public Material defaultMaterial;
    public Material outlineMaterial;

    public virtual void RemoveWorker(FrogWorker workerToRemove)
    {
        workers.Remove(workerToRemove);
        workerToRemove.workingIn = null;

        SpriteRenderer renderer = workerToRemove.gameObject.GetComponent<SpriteRenderer>();
        renderer.material = defaultMaterial;

    }

    public virtual void AddWorker(FrogWorker workerToAdd)
    {
        workers.Add(workerToAdd);
        workerToAdd.workingIn = this;
        workerToAdd.idlePoint = transform.position;

        SpriteRenderer renderer = workerToAdd.gameObject.GetComponent<SpriteRenderer>();
        renderer.material = outlineMaterial;
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

    void OnMouseEnter()
    {
        foreach (FrogWorker worker in workers)
        {
            SpriteRenderer renderer = worker.gameObject.GetComponent<SpriteRenderer>();
            renderer.material = outlineMaterial;
        }
    }

    void OnMouseExit()
    {
         foreach (FrogWorker worker in workers)
        {
            SpriteRenderer renderer = worker.gameObject.GetComponent<SpriteRenderer>();
            renderer.material = defaultMaterial;
        }
    }
}
