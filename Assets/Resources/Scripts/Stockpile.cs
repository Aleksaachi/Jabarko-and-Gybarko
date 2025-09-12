using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stockpile : Building
{
    public int capacity;

    [SerializeField]
    public Item[] items;
    Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    int itemSum;
    public Sprite[] fullnessSprites;
    public int[] thresholds;
    SpriteRenderer spriteRenderer;
    public bool DEBUG1;
    public bool DEBUG2;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        for (int i = 0; i < items.Length; i++)
        {
            inventory.Add(items[i], 0);
        }
    }
    void Update()
    {
        if (DEBUG1)
        {
            AddItem(items[0], 5);
            DEBUG1 = false;
        }
        if (DEBUG2)
        {
            RemoveItem(items[0], 4);
            DEBUG2 = false;
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

    public override void Select()
    {
        List<FrogWorker> newWorkers = FrogOrderManager.AssignWorkers();
    
        foreach (FrogWorker worker in newWorkers)
        {
            AddWorker(worker);
            worker.WalkToPoint(transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0));
        }
    }
}
