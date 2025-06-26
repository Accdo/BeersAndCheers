using UnityEngine;

public class FarmTilePlacer : MonoBehaviour
{
    public string inventoryName = "Hotbar"; // ��𿡼� �������� ��������
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GH_GameManager.instance.player.inventory;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                FarmTile tile = hit.collider.GetComponent<FarmTile>();
                if (tile != null)
                {
                    TryInteractWithTile(tile);
                }
            }
        }
    }
    void TryInteractWithTile(FarmTile tile)
    {
        if (tile.IsPlantable())
        {
            var SeedItemData = inventoryManager.hotbar.selectedSlot.itemData as SeedItemData;

            bool planted = tile.TryPlant(SeedItemData);
            if (planted)
            {
                inventoryManager.RemoveItem(SeedItemData.itemName);
                Debug.Log($"{SeedItemData.itemName} �ɾ����ϴ�!");
            }
        }
        else if (tile.state == FarmTile.TileState.Grown)
        {
            tile.Harvest();
        }
    }
}
