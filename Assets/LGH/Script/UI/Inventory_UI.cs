using UnityEngine;
using System.Collections.Generic;

public class Inventory_UI : MonoBehaviour
{
    public GameObject inventoryPanel;

    public GH_Player player;

    public List<Slot_UI> slots = new List<Slot_UI>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            Refresh();
        }
        else
        {
            inventoryPanel.SetActive(false);
        }
    }

    // 새로고침
    private void Refresh()
    {
        // 슬롯 갯수가 같다면
        if (slots.Count == player.inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                // 플레이어가 가진 인벤토리 슬롯에 아이템이 존재 하는 경우
                if (player.inventory.slots[i].itemName != "")
                {
                    slots[i].SetItem(player.inventory.slots[i]);
                }
                else // 빈 슬롯
                {
                    slots[i].SetEmpty();
                }
            }
        }
    }

    public void Remove(int slotID)
    {
        Item itemToDrop = GH_GameManager.instance.itemManager.GetItemByType(
            player.inventory.slots[slotID].itemName);

        if (itemToDrop != null)
        {
            player.inventory.Remove(slotID);
            Refresh();
        }
    }
}
