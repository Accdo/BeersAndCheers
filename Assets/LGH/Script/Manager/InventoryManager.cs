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

    [Header("상자")]
    public Inventory storagebox;
    public int storageboxSlotCount;

    [Header("냉동 상자")]
    public Inventory freezeBox;
    public int freezeBoxSlotCount;


    [Header("신선도 감소 간격")]
    public float decreaseInterval = 60f; // 1분
    private float timer = 0f;

    private void Awake()
    {
        backpack = new Inventory(backpackSlotCount);
        hotbar = new Inventory(hotbarSlotCount);
        storagebox = new Inventory(storageboxSlotCount);
        freezeBox = new Inventory(freezeBoxSlotCount);

        inventoryByName.Add("Backpack", backpack);
        inventoryByName.Add("Hotbar", hotbar);
        inventoryByName.Add("StorageBox", storagebox);
        inventoryByName.Add("FreezeBox", freezeBox);
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= decreaseInterval)
        {
            DecreaseFreshness(backpack);
            DecreaseFreshness(hotbar);
            DecreaseFreshness(storagebox);
            GH_GameManager.instance.uiManager.RefreshAll(); // UI 갱신
            timer = 0f;
        }
    }

    void DecreaseFreshness(Inventory inventory)
    {
        foreach (var slot in inventory.slots)
        {
            if (slot.itemData is FoodData || slot.itemData is IngredientData)
            {
                slot.freshPoint = Mathf.Max(0, slot.freshPoint - 1);
            }
        }
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
