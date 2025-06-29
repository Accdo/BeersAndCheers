using UnityEngine;

public class Shop_UI : MonoBehaviour
{
    public void BuyItem(Item item)
    {
        // 아이템이 null이거나 데이터가 없는 경우
        if (!item || item.data == null)
            return;

        // 플레이어가 아이템의 가격만큼 돈을 가지고 있는지 확인
        if (GH_GameManager.instance.goldManager.HasMoney(item.data.price))
        {
            GH_GameManager.instance.player.inventory.Add("Backpack", item);
            
            if (GH_GameManager.instance.player.inventory.IsFull("Backpack", item.data.itemName))
                return;

            // 인벤 새로고침
            GH_GameManager.instance.uiManager.RefreshAll();

            GH_GameManager.instance.goldManager.RemoveMoney(item.data.price);
        }
        else
        {
            Debug.Log("아이템 구매 실패: 돈이 부족합니다.");
            // 돈 부족 UI 띄우기
            // GH_GameManager.instance.uiManager.ShowNotEnoughMoneyUI();
        }
    }
}
