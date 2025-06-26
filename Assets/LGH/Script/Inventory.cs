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

        public ItemData itemData;
        public int freshPoint;

        public GameObject UseItem()
        {
            if (itemData is WeaponData weaponData)
            {
                return weaponData.weaponPrefab;
            }
            else if (itemData is FoodData foodData)
            {
                return foodData.foodPrefab;
            }
            else if (itemData is DeployData deployData)
            {
                return deployData.deployPrefab;
            }

            return null;
        }

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

            itemData = item.data;
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
                    itemData = null;
                    freshPoint = 0; // 신선도 초기화
                }
            }
        }
    }

    public List<Slot> slots = new List<Slot>();
    public Slot selectedSlot = null;

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
            // ================================================================================================?
            if (slot.itemName == item.data.itemName && slot.CanAddItem(item.data.itemName))
            {
                slot.AddItem(item);

                int AverageFreshPoint = slot.freshPoint * (slot.count - 1); // 현재 신선도 * 현재 아이템 개수
                AverageFreshPoint += GetDefaultFreshPoint(item.data); // 새로 추가되는 아이템
                AverageFreshPoint /= slot.count; // 평균 신선도 계산

                slot.freshPoint = AverageFreshPoint; // 신선도 평균으로 변경

                return;
            }
        }

        foreach (Slot slot in slots)
        {
            // 슬롯이 비어 있다면 => 아이템 추가!
            if (slot.itemName == "")
            {
                slot.AddItem(item);

                slot.freshPoint = GetDefaultFreshPoint(item.data);
                return;
            }
        }

        //Debug.LogWarning("인벤토리가 꽉찼습니다 : " + item.data.itemName);
    }

    public bool IsFull(string itemName)
    {
        foreach (Slot slot in slots)
        {
            // 찾는 아이템이 있고, 아이템 최대 개수가 아니면
            if (slot.itemName == itemName && slot.CanAddItem(itemName))
                return false;
            else if (slot.itemName == "") // 슬롯이 비어있다면
                return false;
        }
        return true;
    }

    // 아이템 신선도의 기본값을 가져오는 함수
    private int GetDefaultFreshPoint(ItemData data)
    {
        if (data is FoodData foodData)
            return foodData.freshPoint;
        if (data is IngredientData ingredientData)
            return ingredientData.freshPoint;
        return 0;
    }

    // 아이템 삭제
    public void Remove(int index)
    {
        slots[index].RemoveItem();
    }

    // 아이템 여러개 삭제
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

    // 아이템 이름을 통해 슬롯 리스트를 탐색하고 찾아서 <아이템 삭제>
    public void Remove(string itemName)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemName == itemName)
            {
                slots[i].RemoveItem();
                return;
            }
        }
    }
    public bool CanRemove(string itemName)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemName == itemName)
            {
                slots[i].RemoveItem();
                return true;
            }
        }
        return false;
    }

    // 아이템 이름을 통해 슬롯 리스트를 탐색 후 아이템 프리팹 반환
    public GameObject GetItemPrefab(string itemName)
    {
        foreach (Slot slot in slots)
        {
            // 아이템 이름이 같고
            if (slot.itemName == itemName)
            {
                return slot.UseItem();
            }
        }
        return null;
    }

    // 아이템을 통해 슬롯 리스트를 탐색 후 아이템 반환
    public int GetItemCount(Item item, int ingredientNum)
    {
        if (item.data is FoodData fodData && ingredientNum >= fodData.ingredients.Length)
        {
            Debug.LogWarning($"Ingredient number {ingredientNum} is out of range for item {item.data.itemName}.");
            return -1;
        }
        if (item.data is DeployData deploData && ingredientNum >= deploData.ingredients.Length)
        {
            Debug.LogWarning($"Ingredient number {ingredientNum} is out of range for item {item.data.itemName}.");
            return -1;
        }

        foreach (Slot slot in slots)
        {
            // 아이템 이름이 같고
            if (item.data is FoodData foodData)
            {
                if (slot.itemName == foodData.ingredients[ingredientNum].itemName)
                {
                    return slot.count;
                }
            }
            if (item.data is DeployData deployData)
            {
                if (slot.itemName == deployData.ingredients[ingredientNum].itemName)
                {
                    return slot.count;
                }
            }
        }
        return -1;
    }

    public int GetItemFreshPoint(string itemName)
    {
        foreach (Slot slot in slots)
        {
            Debug.Log($"인벤 슬롯에 아이템 이름 : {slot.itemName}, 필요한 재료 아이템 이름 : {itemName}");
            // 아이템 이름이 같고
            if (slot.itemName == itemName)
            {
                return slot.freshPoint;
            }
        }
        return -1;
    }

    // 아이템 이동
    public void MoveSlot(int fromIndex, int toIndex, Inventory toInventory, int numToMove = 1)
    {
        Slot fromSlot = slots[fromIndex];
        Slot toSlot = toInventory.slots[toIndex];

        // 이동시켜 놓을 슬롯이 비어있는가, 
        if (toSlot.IsEmpty || toSlot.CanAddItem(fromSlot.itemName))
        {
            for (int i = 0; i < numToMove; i++)
            {
                // 아이템이 비어있지 않다면
                if (!fromSlot.IsEmpty)
                {
                    // 아이템을 추가
                    toSlot.AddItem(fromSlot.itemName, fromSlot.icon, fromSlot.maxAllowed);
                    // 도착 슬롯의 아이템 데이터를 추가
                    toInventory.slots[toIndex].itemData = fromSlot.itemData;

                    int AverageFreshPoint = toInventory.slots[toIndex].freshPoint * (toInventory.slots[toIndex].count - 1); // 현재 신선도 * 현재 아이템 개수
                    AverageFreshPoint += fromSlot.freshPoint; // 새로 추가되는 아이템
                    AverageFreshPoint /= toInventory.slots[toIndex].count; // 평균 신선도 계산

                    toInventory.slots[toIndex].freshPoint = AverageFreshPoint; // 신선도 평균으로 변경

                    // 시작 슬롯의 아이템 데이터를 비우기
                    //slots[fromIndex].itemData = null;

                    fromSlot.RemoveItem();
                }
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (slots != null && slots.Count > 0)
        {
            selectedSlot = slots[index];
        }
    }
}
