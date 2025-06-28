using UnityEngine;

[CreateAssetMenu(fileName = "SeedItemData", menuName = "Item/SeedItemData", order = 2)]
public class SeedItemData : ItemData
{
    public GameObject plantedPrefab;
    public GameObject wateredPrefab;
    public GameObject grownPrefab;

    public float growTime;
    public int yieldAmount;
    public Item harvestItem;
}