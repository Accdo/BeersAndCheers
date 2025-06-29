using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StartItem
{
    public string inventoryName; // 인벤토리 이름
    public string itemName;
    public int count;
}

public class Player_StartItem : MonoBehaviour
{
    public InventoryManager inventory;

    public List<StartItem> startItems;

    private void Awake()
    {
        inventory = GetComponent<InventoryManager>();
    }

    private void Start()
    {
        foreach (var item in startItems)
        {
            for (int i = 0; i < item.count; i++)
            {
                inventory.Add(item.inventoryName, GH_GameManager.instance.itemManager.GetItemByName(item.itemName));

                GH_GameManager.instance.uiManager.RefreshAll();
            }
        }

        // for (int i = 0; i < 10; i++)
        // {
        //     inventory.Add("Backpack", GH_GameManager.instance.itemManager.GetItemByName("Carrot"));
        //     inventory.Add("Backpack", GH_GameManager.instance.itemManager.GetItemByName("Potato"));
        //     inventory.Add("Backpack", GH_GameManager.instance.itemManager.GetItemByName("Onion"));
        // }
    }
}
