using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
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

        public bool CanAddItem()
        {
            if (count < maxAllowed)
            {
                return true;
            }
            return false;
        }

        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            count++;
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

    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    public void Add(Item item)
    {
        foreach (Slot slot in slots)
        {
            // 아이템 이름이 같고, 아이템을 추가 가능하면 => 아이템 추가!
            if (slot.itemName == item.data.itemName && slot.CanAddItem())
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

    public void Remove(int slotID)
    {
        slots[slotID].RemoveItem();
    }
}
