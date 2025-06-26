using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Cooking_UI : MonoBehaviour
{
    public List<IngredientSlot_UI> cookingSlots = new List<IngredientSlot_UI>();

    public Item SelectItem;

    public Cooking cooking;
    public CookingMinigame cookingMinigame;

    // 플레이어가 가진 재료 아이템 갯수
    private int playerItemCount;
    // 요리에 필요한 재료 아이템 갯수
    private int[] cookItemCount = new int[6];
    // 현재 요리재료 슬롯의 개수
    private int currentCookSlotCount;

    // 요리 재료의 신선도 점수 합계
    private int ingredientFreshPointTotal = 0;
    // 요리 재료의 총 갯수
    private int ingredientCount = 0;


    public void Refresh()
    {
        for (int i = 0; i < cookingSlots.Count; i++)
        {
            if (SelectItem.data is FoodData foodData)
            {
                // 요리 재료 개수 까지만 실행
                if (i < foodData.ingredients.Length)
                {
                    playerItemCount = GH_GameManager.instance.player.inventory.GetItemCount(SelectItem, i);
                    cookItemCount[i] = foodData.ingredientCounts[i];

                    cookingSlots[i].SetItem(foodData.ingredients[i], playerItemCount, cookItemCount[i]);
                }
                // 나머지는 빈 슬롯으로 설정
                else
                {
                    cookingSlots[i].SetEmpty();
                }
            }
        }
    }

    public void Cook()
    {
        var foodData = SelectItem.data as FoodData;
        if (foodData == null)
        {
            Debug.Log("foodData 참조 실패");
            return;
        }
        // 요리 재료의 신선도 점수 합계와 총 갯수 초기화
        ingredientFreshPointTotal = 0;
        ingredientCount = 0;

        for (int i = 0; i < cookingSlots.Count; i++)
        {
            // 빈 슬롯인지 확인 후, 아이템 갯수 변수 초기화
            if (i < foodData.ingredients.Length)
            {
                playerItemCount = GH_GameManager.instance.player.inventory.GetItemCount(SelectItem, i);
                cookItemCount[i] = foodData.ingredientCounts[i];
            }
            else
            {
                currentCookSlotCount = i;
                Debug.Log("더 이상 요리 재료가 없습니다.");
                break;
            }

            // 보유한 요리 재료가 부족할 시 return
            if (playerItemCount < cookItemCount[i])
            {
                Debug.Log($"Not enough ingredients to cook {SelectItem.data.itemName}. Required: {cookItemCount}, Available: {playerItemCount}");
                return;
            }
        }
        for (int i = 0; i < currentCookSlotCount; i++)
        {
            // 인벤토리 음식재료 제거
            for (int j = 0; j < cookItemCount[i]; j++)
            {
                // 복잡한 설계 @ @
                ingredientFreshPointTotal += GH_GameManager.instance.player.inventory.GetItemFreshPoint(foodData.ingredients[i].itemName);
                ingredientCount++;

                GH_GameManager.instance.player.inventory.RemoveItem(foodData.ingredients[i].itemName);
            }
            // 요리창 새로고침
            Refresh();

            Debug.Log($"Cooking {SelectItem.data.itemName} completed!");
        }


        //요리 미니게임 시작
        //cooking.CookingSystem(SelectItem.data.icon);

        GH_GameManager.instance.uiManager.ActiveHotbarUI();
        cooking.CookingSystem(foodData.ingredients[0].icon, SelectItem.data.icon);

        //실패시 리턴
        if (cookingMinigame.isCookingSuccess) return;



        foodData.freshPoint = ingredientFreshPointTotal / ingredientCount;
        Debug.Log($"완성된 음식 신선도 평균 : {foodData.freshPoint}");
        // 인벤토리 완성된 음식 추가
        GH_GameManager.instance.player.inventory.Add("Backpack", SelectItem);
        // 인벤 새로고침
        GH_GameManager.instance.uiManager.RefreshAll();
    }


}
