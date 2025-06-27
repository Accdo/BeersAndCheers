using NUnit.Framework.Internal;
using UnityEngine;
using static Inventory;

public class FarmTile : MonoBehaviour, IInteractable
{
    public enum TileState { Empty, Planted, Watered, Grown }
    public TileState state;

    public string GetCursorType() => "Farm";
    public string GetInteractionID() => "FarmTile";
    public InteractionType GetInteractionType() => InteractionType.Instant;

    private float growTimer;
    private SeedItemData currentSeed;
    public GameObject cropPrefab;
    public Item Tomato;

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

        cropPrefab = Instantiate(currentSeed.plantedPrefab, transform.position + Vector3.up * 0.1f, Quaternion.identity, transform);
        return true;
    }

    public bool TryWater()
    {
        if(state == TileState.Planted)
        {
            state = TileState.Watered;
            Debug.Log("���� �־����ϴ�");
            return true;
        }

        Debug.Log("���� �� �� ���� �����Դϴ�");
        return false;
    }

    public void Update()
    {
        if (state == TileState.Watered && currentSeed != null)
        {
            growTimer += Time.deltaTime;
            if (growTimer >= currentSeed.growTime)
            {
                state = TileState.Grown;
                Destroy(cropPrefab);

                cropPrefab = Instantiate(currentSeed.grownPrefab, transform.position + Vector3.up * 0.1f, Quaternion.identity, transform);
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
            int randomCount = Random.Range(1, 4);

            for (int i = 0; i < randomCount; i++)
            {
                GH_GameManager.instance.player.inventory.Add("Backpack", Tomato);
            }
        }
    }
    public void Interact()
    {
        FarmTilePlacer.Instance.TryInteractWithTile(this);
    }
}
