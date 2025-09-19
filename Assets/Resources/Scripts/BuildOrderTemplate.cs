using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New building order template")]
public class BuildOrderTemplate : ScriptableObject
{
    public GameObject prefab;
    public List<Item> itemCost;
    public List<int> itemAmounts;
    public Sprite icon;
}
