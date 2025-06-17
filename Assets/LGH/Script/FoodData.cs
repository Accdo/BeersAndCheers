using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Item/FoodData", order = 2)]
public class FoodData : ItemData
{
    public int freshPoint = 100;
    public GameObject foodPrefab;
    public int price = 0;
}
