using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    [Header("기본 아이템")]
    public Item[] defaultItems;
    [Header("음식 아이템")]
    public Item[] foodItems;
    [Header("재료 아이템")]
    public Item[] ingredientItems;

    private Dictionary<string, Item> nameToItemDict = new Dictionary<string, Item>();

    private void Awake()
    {
        foreach (Item item in defaultItems)
        {
            AddItem(item);
        }
        foreach (Item item in foodItems)
        {
            AddItem(item);
        }
        foreach (Item item in ingredientItems)
        {
            AddItem(item);
        }
    }

    private void AddItem(Item item)
    {
        if (!nameToItemDict.ContainsKey(item.data.itemName))
        {
            nameToItemDict.Add(item.data.itemName, item);
        }
    }

    public Item GetItemByName(string key)
    {
        if (nameToItemDict.ContainsKey(key))
        {
            return nameToItemDict[key];
        }
        return null;
    }
}
