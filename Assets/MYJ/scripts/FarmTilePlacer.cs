using UnityEngine;

public class FarmTilePlacer : MonoBehaviour
{
    public static FarmTilePlacer Instance;

    public string inventoryName = "Hotbar"; // 어디에서 아이템을 가져올지
    private InventoryManager inventoryManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inventoryManager = GH_GameManager.instance.player.inventory;
    }

    public void TryInteractWithTile(FarmTile tile)
    {
        var slot = inventoryManager.hotbar.selectedSlot;

        switch (tile.state)
        {
            case FarmTile.TileState.Empty:
                if (slot != null && slot.itemData is SeedItemData seedData)
                {
                    bool planted = tile.TryPlant(seedData);
                    if (planted)
                    {
                        inventoryManager.RemoveItem(seedData.itemName);
                        Debug.Log($"{seedData.itemName}을(를) 심었습니다!");
                    }
                }
                break;

            case FarmTile.TileState.Planted:
                break;

            case FarmTile.TileState.WaterPrompt:

                //물뿌리개 아이템이 있는 경우만 물을 줄 수 있게
                if (slot != null && slot.itemData.itemName == "WaterPail") // or WateringCanData 타입 확인
                {
                    bool watered = tile.TryWater();
                    if (watered)
                    {
                        Debug.Log("물 주기 성공!");
                        SoundManager.Instance.Play("FishingSFX");
                    }
                }
                else
                {
                    Debug.Log("물뿌리개가 필요합니다.");
                }
                break;

            case FarmTile.TileState.Grown:
                tile.Harvest();
                break;
        }
    }
}
