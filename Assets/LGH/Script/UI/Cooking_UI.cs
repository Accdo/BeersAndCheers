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
    private int cookItemCount;


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
                    cookItemCount = foodData.ingredientCounts[i];

                    cookingSlots[i].SetItem(foodData.ingredients[i], playerItemCount, cookItemCount);
                }
                // 나머지는 빈 슬롯으로 설정
                else
                {
                    cookingSlots[i].SetEmpty();
                }
            }
            if (SelectItem.data is DeployData deployData)
            {
                // 요리 재료 개수 까지만 실행
                if (i < deployData.ingredients.Length)
                {
                    playerItemCount = GH_GameManager.instance.player.inventory.GetItemCount(SelectItem, i);
                    Debug.Log("playerItemCount : " + playerItemCount);
                    cookItemCount = deployData.ingredientCounts[i];
                    Debug.Log("cookItemCount : " + cookItemCount);

                    cookingSlots[i].SetItem(deployData.ingredients[i], playerItemCount, cookItemCount);
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
        for (int i = 0; i < cookingSlots.Count; i++)
        {
            // 선택된 아이템이 음식 아이템인지 확인
            if (SelectItem.data is FoodData foodData)
            {
                // 요리 재료 개수 까지만 실행
                if (i < foodData.ingredients.Length)
                {
                    playerItemCount = GH_GameManager.instance.player.inventory.GetItemCount(SelectItem, i);
                    cookItemCount = foodData.ingredientCounts[i];
                }
                else
                {
                    // 나머지 빈 슬롯에 대해 더 이상 실행하지 않음
                    Debug.Log("No more ingredients to cook.");
                    break;
                }

                // 플레이어가 가진 재료 아이템 갯수가 요리에 필요한 재료 아이템 갯수 이상일 때
                if (playerItemCount >= cookItemCount)
                {
                    // 인벤토리 음식재료 제거
                    for (int j = 0; j < cookItemCount; j++)
                    {
                        // 인벤토리에서 재료 아이템 제거
                        GH_GameManager.instance.player.inventory.RemoveItem(foodData.ingredients[i].itemName);
                    }

                    // 요리창 새로고침
                    Refresh();

                    Debug.Log($"Cooking {SelectItem.data.itemName} completed!");
                }
                else
                {
                    // 플레이어가 가진 재료 아이템 갯수가 요리에 필요한 재료 아이템 갯수 미만일 때
                    Debug.Log($"Not enough ingredients to cook {SelectItem.data.itemName}. Required: {cookItemCount}, Available: {playerItemCount}");
                    return;
                }
            }
            else
            {
                // 선택된 아이템이 음식 아이템이 아닐 때
                Debug.Log("Selected item is not a food item.");
                return;
            }

        }

         //요리 미니게임 시작
         //cooking.CookingSystem(SelectItem.data.icon);
         if (SelectItem.data is FoodData foodData2)
         {
                cooking.CookingSystem(foodData2.ingredients[0].icon, SelectItem.data.icon);
         }

        
        //실패시 리턴
        if (cookingMinigame.isCookingSuccess) return;

        // 별 문제 없었다면
        // 인벤토리 완성된 음식 추가
        GH_GameManager.instance.player.inventory.Add("Backpack", SelectItem);
        // 인벤 새로고침
        GH_GameManager.instance.uiManager.RefreshAll();
    }

    public void CreateItem()
    {
        for (int i = 0; i < cookingSlots.Count; i++)
        {
            // 선택된 아이템이 음식 아이템인지 확인
            if (SelectItem.data is DeployData deployData)
            {
                // 요리 재료 개수 까지만 실행
                if (i < deployData.ingredients.Length)
                {
                    playerItemCount = GH_GameManager.instance.player.inventory.GetItemCount(SelectItem, i);
                    cookItemCount = deployData.ingredientCounts[i];
                }
                else
                {
                    // 나머지 빈 슬롯에 대해 더 이상 실행하지 않음
                    Debug.Log("No more ingredients to create.");
                    break;
                }

                // 플레이어가 가진 재료 아이템 갯수가 요리에 필요한 재료 아이템 갯수 이상일 때
                if (playerItemCount >= cookItemCount)
                {
                    // 인벤토리 음식재료 제거
                    for (int j = 0; j < cookItemCount; j++)
                    {
                        // 인벤토리에서 재료 아이템 제거
                        GH_GameManager.instance.player.inventory.RemoveItem(deployData.ingredients[i].itemName);
                    }

                    // 요리창 새로고침
                    Refresh();

                    Debug.Log($"Creating {SelectItem.data.itemName} completed!");
                }
                else
                {
                    // 플레이어가 가진 재료 아이템 갯수가 요리에 필요한 재료 아이템 갯수 미만일 때
                    Debug.Log($"Not enough ingredients to cook {SelectItem.data.itemName}. Required: {cookItemCount}, Available: {playerItemCount}");
                    return;
                }
            }
            else
            {
                // 선택된 아이템이 음식 아이템이 아닐 때
                Debug.Log("Selected item is not a deployItem.");
                return;
            }

        }

        // 시설 배치 시작
        

        // 별 문제 없었다면
        // 인벤토리 완성된 맥주 추가
        GH_GameManager.instance.player.inventory.Add("Backpack", SelectItem);
        // 인벤 새로고침
        GH_GameManager.instance.uiManager.RefreshAll();
    }

}
