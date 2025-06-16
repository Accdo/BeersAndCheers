using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    // 실제 슬롯 정보를 담은 클래스
    [System.Serializable]
    public class Slot
    {
        public string itemName;
        public int count;
        public int maxAllowed;

        public Sprite icon;

        public Slot()
        {
            itemName = "";
            count = 0;
            maxAllowed = 99;
        }

        public bool IsEmpty
        {
            get
            {
                if (itemName == "" && count == 0)
                {
                    return true;
                }
                return false;
            }
        }

        // 아이템을 추가 할 수 있는가 없는가
        public bool CanAddItem(string itemName)
        {
            if (this.itemName == itemName && count < maxAllowed)
            {
                return true;
            }
            return false;
        }

        // 인벤토리에 아이템 데이터를 받아서 추가
        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            count++;
        }
        public void AddItem(string itemName, Sprite icon, int maxAllowed)
        {
            this.itemName = itemName;
            this.icon = icon;
            count++;
            this.maxAllowed = maxAllowed;
        }

        // 슬롯 비우기
        public void RemoveItem()
        {
            if (count > 0)
            {
                count--;

                if (count == 0)
                {
                    icon = null;
                    itemName = "";
                }
            }
        }
    }

    public List<Slot> slots = new List<Slot>();

    // 인벤토리 생성자
    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    // 아이템 이름을 통해 슬롯 리스트를 탐색하고 찾아서 <아이템 추가>
    public void Add(Item item)
    {
        foreach (Slot slot in slots)
        {
            // 아이템 이름이 같고, 아이템을 추가 가능하면 => 아이템 추가!
            if (slot.itemName == item.data.itemName && slot.CanAddItem(item.data.itemName))
            {
                slot.AddItem(item);
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            // 슬롯이 비어 있다면 => 아이템 추가!
            if (slot.itemName == "")
            {
                slot.AddItem(item);
                return;
            }
        }
    }

    public void Remove(int index)
    {
        slots[index].RemoveItem();
    }

    public void Remove(int index, int numToRemove)
    {
        if (slots[index].count >= numToRemove)
        {
            for (int i = 0; i < numToRemove; i++)
            {
                Remove(index);
            }
        }
    }

    public void MoveSlot(int fromIndex, int toIndex, Inventory toInventory, int numToMove = 1)
    {
        Slot fromSlot = slots[fromIndex];
        Slot toSlot = toInventory.slots[toIndex];
 
        // 이동시켜 놓을 슬롯이 비어있는가, 
        if (toSlot.IsEmpty || toSlot.CanAddItem(fromSlot.itemName))
        {
            for(int i = 0; i < numToMove; i++)
            {
                // 아이템이 비어있지 않다면
                if (!fromSlot.IsEmpty)
                {
                    // 아이템을 추가
                    toSlot.AddItem(fromSlot.itemName, fromSlot.icon, fromSlot.maxAllowed);
                    fromSlot.RemoveItem();
                }
            }
        }
    }
}
