using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory_UI : MonoBehaviour
{
    public string inventoryName;
    public List<Slot_UI> slots = new List<Slot_UI>();
    [SerializeField] private Canvas canvas;
    private Inventory inventory;

    private void Start()
    {
        inventory = GH_GameManager.instance.player.inventory.GetInventoryByName(inventoryName);

        SetupSlots();
        Refresh();
    }

    // 새로고침
    public void Refresh()
    {
        // 슬롯 갯수가 같다면
        if (slots.Count == inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                // 플레이어가 가진 인벤토리 슬롯에 아이템이 존재 하는 경우
                if (inventory.slots[i].itemName != "")
                {
                    slots[i].SetItem(inventory.slots[i]);
                }
                else // 빈 슬롯
                {
                    slots[i].SetEmpty();
                }
            }
        }
    }

    // 아이템 제거하는데 돈이 들어오게 하자
    public void Remove()
    {
        Item itemToDrop = GH_GameManager.instance.itemManager.GetItemByName(
            inventory.slots[UI_Manager.draggedSlot.slotID].itemName);

        if (itemToDrop != null)
        {
            if (UI_Manager.dragSingle)
            {
                // 아이템 판매 금액 들어오기
                Debug.Log($"itemToDrop.data.price: {itemToDrop.data.price}");
                int sellMoney = itemToDrop.data.price / 2;
                Debug.Log($"아이템 판매: {itemToDrop.data.itemName}, 가격: {sellMoney}");
                GH_GameManager.instance.goldManager.AddMoney(sellMoney);

                inventory.Remove(UI_Manager.draggedSlot.slotID);
            }
            else
            {
                // 아이템 판매 금액 들어오기
                Debug.Log($"itemToDrop.data.price: {itemToDrop.data.price}");
                int sellMoney = itemToDrop.data.price * inventory.slots[UI_Manager.draggedSlot.slotID].count / 2;
                Debug.Log($"아이템 판매: {itemToDrop.data.itemName}, 가격: {sellMoney}");
                GH_GameManager.instance.goldManager.AddMoney(sellMoney);

                inventory.Remove(UI_Manager.draggedSlot.slotID, inventory.slots[UI_Manager.draggedSlot.slotID].count);
            }
            Refresh();
        }

        UI_Manager.draggedSlot = null;
    }

    public void SlotBeginDrag(Slot_UI slot)
    {
        UI_Manager.draggedSlot = slot;

        UI_Manager.draggedIcon = Instantiate(slot.itemIcon);
        UI_Manager.draggedIcon.transform.SetParent(canvas.transform);
        UI_Manager.draggedIcon.raycastTarget = false;
        UI_Manager.draggedIcon.rectTransform.sizeDelta = new Vector2(80, 80);

        MoveToMousePosition(UI_Manager.draggedIcon.gameObject);
    }

    public void SlotDrag()
    {
        MoveToMousePosition(UI_Manager.draggedIcon.gameObject);
    }

    public void SlotEndDrag()
    {
        Destroy(UI_Manager.draggedIcon.gameObject);
        UI_Manager.draggedIcon = null;
    }

    public void SlotDrop(Slot_UI slot)
    {
        if (UI_Manager.dragSingle)
        {
            // 드래그 선택한 슬롯에서 드롭 시킨 슬롯으로 이동
            UI_Manager.draggedSlot.inventory.MoveSlot(UI_Manager.draggedSlot.slotID, slot.slotID, slot.inventory);
        }
        else
        {
            UI_Manager.draggedSlot.inventory.MoveSlot(UI_Manager.draggedSlot.slotID, slot.slotID, slot.inventory,
                UI_Manager.draggedSlot.inventory.slots[UI_Manager.draggedSlot.slotID].count);
        }

        GH_GameManager.instance.uiManager.RefreshAll();
    }

    public void MoveToBox(Slot_UI slot)
    {
        if (Input.GetMouseButtonDown(1))
        {
            // 보관함 열려 있을 때
            if (GH_GameManager.instance.uiManager.storageboxPanel.activeSelf)
            {
                Debug.Log("storageboxPanel");

                
            }
            // 냉장 보관함 열려 있을 떄
            if (GH_GameManager.instance.uiManager.freezeboxPanel.activeSelf)
            {
                Debug.Log("freezeboxPanel");

            }
        }

        GH_GameManager.instance.uiManager.RefreshAll();
    }

    private void MoveToMousePosition(GameObject toMove)
    {
        if (canvas != null)
        {
            Vector2 position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                Input.mousePosition, null, out position);

            toMove.transform.position = canvas.transform.TransformPoint(position);
        }
    }

    // 슬롯 아이디 초기화 
    private void SetupSlots()
    {
        int counter = 0;

        foreach (Slot_UI slot in slots)
        {
            slot.slotID = counter;
            counter++;
            slot.inventory = inventory;
        }
    }
}