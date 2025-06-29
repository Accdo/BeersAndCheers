using UnityEngine;

public class FarmTilePlacer : MonoBehaviour
{
    public static FarmTilePlacer Instance;

    public string inventoryName = "Hotbar"; // ��𿡼� �������� ��������
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
                        Debug.Log($"{seedData.itemName}��(��) �ɾ����ϴ�!");
                    }
                }
                break;

            case FarmTile.TileState.Planted:
                break;

            case FarmTile.TileState.WaterPrompt:

                //���Ѹ��� �������� �ִ� ��츸 ���� �� �� �ְ�
                if (slot != null && slot.itemData.itemName == "WaterPail") // or WateringCanData Ÿ�� Ȯ��
                {
                    bool watered = tile.TryWater();
                    if (watered)
                    {
                        Debug.Log("�� �ֱ� ����!");
                        SoundManager.Instance.Play("FishingSFX");
                    }
                }
                else
                {
                    Debug.Log("���Ѹ����� �ʿ��մϴ�.");
                }
                break;

            case FarmTile.TileState.Grown:
                tile.Harvest();
                break;
        }
    }
}
