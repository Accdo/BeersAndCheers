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

    public void RemoveItem(string itemName)
    {
        if (backpack.CanRemove(itemName))
        {
            GH_GameManager.instance.uiManager.RefreshAll();
            return;
        }

        hotbar.Remove(itemName);
        GH_GameManager.instance.uiManager.RefreshAll();
    }

    public GameObject GetItemPrefab(string itemName)
    {
        GameObject itemPrefab = hotbar.GetItemPrefab(itemName);
        if (itemPrefab != null)
        {
            return itemPrefab;
        }

        itemPrefab = backpack.GetItemPrefab(itemName);
        if (itemPrefab != null)
        {
            return itemPrefab;
        }

        return null;
    }

    public int GetItemCount(Item item, int ingredientNum)
    {
        if (backpack.GetItemCount(item, ingredientNum) != -1)
            return backpack.GetItemCount(item, ingredientNum);
        if (hotbar.GetItemCount(item, ingredientNum) != -1)
            return hotbar.GetItemCount(item, ingredientNum);

        return 0;
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
