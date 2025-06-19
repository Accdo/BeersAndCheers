using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Item/FoodData", order = 2)]
public class FoodData : ItemData
{
    public int freshPoint = 100;
    public GameObject foodPrefab;
    public IngredientData[] ingredients;
    public int[] ingredientCounts; // 요리에 필요한 재료 개수
    public int price = 0;
}
