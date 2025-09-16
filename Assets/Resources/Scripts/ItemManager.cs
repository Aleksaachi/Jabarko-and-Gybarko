using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public Dictionary<string, Item> namesToItems = new Dictionary<string, Item>();

    void Awake()
    {
        instance = this;

        Item[] allItems = Resources.LoadAll<Item>("ScriptableObjects/Items");
        foreach (Item item in allItems)
        {
            namesToItems.Add(item.name, item);
        }
    }

    public static Item GetItem(string name)
    {
        return instance.namesToItems[name];
    }

    public static Dictionary<Item, int> ReturnInventoryArray()
    {
        Dictionary<Item, int> newInventory = new Dictionary<Item, int>();
        for (int i = 0; i < instance.namesToItems.Count; i++)
        {
            KeyValuePair<string, Item> pair = instance.namesToItems.ElementAt(i);
            newInventory.Add(pair.Value, 0);
        }

        return newInventory;
    }
}
