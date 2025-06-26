using UnityEngine;
using static Inventory;

public class FarmTile : MonoBehaviour
{
    public enum TileState { Empty, Planted, Watered, Grown }
    public TileState state = TileState.Empty;

    private float growTimer;
    private SeedItemData currentSeed;
    public GameObject cropPrefab;

    public bool IsPlantable() => state == TileState.Empty;

    public bool TryPlant(SeedItemData seedItemData)
    {
        Slot slot = GH_GameManager.instance.player.inventory.hotbar.selectedSlot;
        if (!IsPlantable()) return false; //현재 심을 수 있는지

        if (slot == null) return false; //아이템이 없진 않은지

        if (!(slot.itemData is SeedItemData seedData)) //아이템이 씨앗인지
        {
            Debug.Log("이 아이템은 씨앗이 아닙니다.");
            return false;
        }

        currentSeed = seedData;
        growTimer = 0f;
        state = TileState.Planted;

        cropPrefab = Instantiate(currentSeed.cropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity, transform);
        return true;
    }

    public void Update()
    {
        if (state == TileState.Planted && currentSeed != null)
        {
            growTimer += Time.deltaTime;
            if (growTimer >= currentSeed.growTime)
            {
                state = TileState.Grown;
                Debug.Log($"{currentSeed.itemName}이(가) 성장했습니다!");
            }
        }
    }

    public void Harvest()
    {
        if (state == TileState.Grown)
        {
            Debug.Log($"{currentSeed.itemName} 수확 완료! 수확량: {currentSeed.yieldAmount}");
            Destroy(cropPrefab);
            currentSeed = null;
            state = TileState.Empty;
            //보상
        }
    }

}
