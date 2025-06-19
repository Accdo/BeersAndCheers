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

    // 손님한테 아이템 건넬 때 호출
    // 인벤토리 및 핫바에 있는 아이템 제거
    // 아이템이 제거되었음으로 UI 갱신
    public void RemoveItem(string itemName)
    {
        // 인벤토리에서 아이템이 제거 되었다면 return
        if (backpack.CanRemove(itemName))
        {
            GH_GameManager.instance.uiManager.RefreshAll();
            return;
        }

        hotbar.Remove(itemName);
        GH_GameManager.instance.uiManager.RefreshAll();
    }

    // 인벤토리 및 핫바에 있는 아이템 프리팹 탐색 후 반환
    public GameObject GetItemPrefab(string itemName)
    {
        if (backpack.GetItemPrefab(itemName) != null)
            return backpack.GetItemPrefab(itemName);
        if (hotbar.GetItemPrefab(itemName) != null)
            return hotbar.GetItemPrefab(itemName);

        Debug.LogWarning($"Item with name {itemName} not found in any inventory.");
        return null;
    }

    // 인벤토리 및 핫바에 있는 아이템 탐색 후 아이템 반환
    public int GetItemCount(Item item, int ingredientNum)
    {
        if (backpack.GetItemCount(item , ingredientNum) != -1)
            return backpack.GetItemCount(item, ingredientNum);
        if (hotbar.GetItemCount(item, ingredientNum) != -1)
            return hotbar.GetItemCount(item, ingredientNum);

        Debug.LogWarning($"Item with name {item.data.itemName} not found in any inventory.");
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
