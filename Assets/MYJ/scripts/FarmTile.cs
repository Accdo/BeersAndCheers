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
        if (!IsPlantable()) return false; //���� ���� �� �ִ���

        if (slot == null) return false; //�������� ���� ������

        if (!(slot.itemData is SeedItemData seedData)) //�������� ��������
        {
            Debug.Log("�� �������� ������ �ƴմϴ�.");
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
                Debug.Log($"{currentSeed.itemName}��(��) �����߽��ϴ�!");
            }
        }
    }

    public void Harvest()
    {
        if (state == TileState.Grown)
        {
            Debug.Log($"{currentSeed.itemName} ��Ȯ �Ϸ�! ��Ȯ��: {currentSeed.yieldAmount}");
            Destroy(cropPrefab);
            currentSeed = null;
            state = TileState.Empty;
            //����
        }
    }

}
