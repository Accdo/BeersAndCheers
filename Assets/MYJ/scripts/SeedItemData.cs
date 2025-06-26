using UnityEngine;

[CreateAssetMenu(fileName = "SeedItemData", menuName = "Item/SeedItemData", order = 2)]
public class SeedItemData : ItemData
{
    public GameObject cropPrefab;
    public float growTime;
    public int yieldAmount;
}