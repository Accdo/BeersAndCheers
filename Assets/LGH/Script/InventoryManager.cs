using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Dictionary<string, Inventory> inventoryByName = new Dictionary<string, Inventory>();

    [Header("인벤토리")]
    public Inventory backpack;
    public int backpackSlotCount;

    [Header("상시 인벤토리")]
    public Inventory hotbar;
    public int hotbarSlotCount;

    private void Awake()
    {
        backpack = new Inventory(backpackSlotCount);
        hotbar = new Inventory(hotbarSlotCount);

        inventoryByName.Add("Backpack", backpack);
        inventoryByName.Add("Hotbar", hotbar);
    }

    public void Add(string inventoryName, Item item)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            inventoryByName[inventoryName].Add(item);
        }
    }

    public Inventory GetInventoryByName(string inventoryName)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            return inventoryByName[inventoryName];
        }
        return null;
    }
}
