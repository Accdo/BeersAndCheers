using UnityEngine;

public class Shop_UI : MonoBehaviour
{
    public void BuyItem(Item item)
    {
        // 아이템이 null이거나 데이터가 없는 경우
        if (!item || item.data == null)
            return;

        // 인벤토리가 꽉 찼는지 확인
        if (GH_GameManager.instance.player.inventory.IsFull("Backpack", item.data.itemName))
        {
            Debug.Log("인벤토리가 꽉 찼습니다 : IsFull");
            return;
        }

        // 플레이어가 아이템을 구매할 수 있는지 확인
        if (!GH_GameManager.instance.goldManager.RemoveMoney(item.data.price))
        {
            return;
        }

        GH_GameManager.instance.player.inventory.Add("Backpack", item);

        // 인벤 새로고침
        GH_GameManager.instance.uiManager.RefreshAll();
    }
}
