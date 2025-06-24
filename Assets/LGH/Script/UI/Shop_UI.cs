using UnityEngine;

public class Shop_UI : MonoBehaviour
{
    public void BuyItem(Item item)
    {
        // 아이템 구매 로직을 여기에 구현합니다.

        if(!item || item.data == null)
            return;

        // 플레이어가 아이템을 구매할 수 있는지 확인
        if (!GH_GameManager.instance.goldManager.RemoveMoney(item.data.price))
        {
            Debug.Log("Not enough gold to buy this item.");
            return;
        }

        GH_GameManager.instance.player.inventory.Add("Backpack", item);

        // 인벤 새로고침
        GH_GameManager.instance.uiManager.RefreshAll();
    }
}
