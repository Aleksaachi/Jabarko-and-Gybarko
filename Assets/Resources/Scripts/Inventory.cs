using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<Item, int> inventory;
    public int capacity;
    public int itemSum;

    void Start()
    {
        inventory = ItemManager.ReturnInventoryArray();
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
    }
    public bool TryCombine(Inventory secondInventory) {
        // Checks if secondInventory and this inventory can be combined
        if (secondInventory.itemSum + itemSum > capacity) return false;
        return true;
    }
    public void Combine(Inventory secondInventory)
    {
        // Takes all items from secondInventory and places them in this one
        foreach (KeyValuePair<Item, int> pair in secondInventory.inventory)
        {
            inventory[pair.Key] += pair.Value;
        }
        itemSum += secondInventory.itemSum;

        secondInventory.inventory = ItemManager.ReturnInventoryArray();
        secondInventory.itemSum = 0;
    }
}
