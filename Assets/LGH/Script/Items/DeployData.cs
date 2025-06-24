using UnityEngine;

[CreateAssetMenu(fileName = "DeployData", menuName = "Item/DeployData", order = 4)]
public class DeployData : ItemData
{
    public GameObject deployPrefab;
    public IngredientData[] ingredients;
    public int[] ingredientCounts; // 재료 개수
}
