using NUnit.Framework.Internal;
using UnityEngine;
using static Inventory;

public class FarmTile : MonoBehaviour, IInteractable
{
    public enum TileState { Empty, Planted, WaterPrompt, Watered, Grown }
    public TileState state;

    public string GetCursorType() => "Farm";
    public string GetInteractionID() => "FarmTile";
    public InteractionType GetInteractionType() => InteractionType.Instant;

    private float growTimer;

    private SeedItemData currentSeed;

    private GameObject cropInstance;

    [SerializeField] private GameObject waterIcon;
    [SerializeField] private GameObject harvestIcon;

    private bool waterPromptShown = false;

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
        waterPromptShown = false;

        if (cropInstance != null)
            Destroy(cropInstance);

        cropInstance = Instantiate(currentSeed.plantedPrefab, transform.position + Vector3.up * 0.1f, currentSeed.plantedPrefab.transform.rotation);
        SoundManager.Instance.Play("FarmSFX");
        waterIcon?.SetActive(false);
        harvestIcon?.SetActive(false);
        return true;
    }

    public bool TryWater()
    {

        if(state == TileState.WaterPrompt)
        {
            state = TileState.Watered;
            growTimer = 0f;

            if (cropInstance != null)
                Destroy(cropInstance);

            cropInstance = Instantiate(currentSeed.wateredPrefab, transform.position + Vector3.up * 0.1f, currentSeed.wateredPrefab.transform.rotation);
            waterIcon?.SetActive(false);

            Debug.Log("���� �־����ϴ�");
            return true;
        }

        Debug.Log("���� �� �� ���� �����Դϴ�");
        return false;
    }

    public void Update()
    {
        growTimer += Time.deltaTime;

        if (state == TileState.Planted && !waterPromptShown && growTimer >= 5f)
        {
            state = TileState.WaterPrompt;
            waterPromptShown = true;
            waterIcon?.SetActive(true);
            Debug.Log("���� �ּ���!");
        }
        
        if (state == TileState.Watered && growTimer >= currentSeed.growTime)
        {
            if (growTimer >= currentSeed.growTime)
            {
                state = TileState.Grown;

                if (cropInstance != null)
                    Destroy(cropInstance);

                cropInstance = Instantiate(currentSeed.grownPrefab, transform.position + Vector3.up * 0.1f, currentSeed.grownPrefab.transform.rotation);
                harvestIcon?.SetActive(true);

                Debug.Log($"{currentSeed.itemName}��(��) �����߽��ϴ�!");
            }
        }
    }

    public void Harvest()
    {
        if (state == TileState.Grown)
        {
            Debug.Log($"{currentSeed.itemName} ��Ȯ �Ϸ�! ��Ȯ��: {currentSeed.yieldAmount}");
            
            if (cropInstance != null)
                Destroy(cropInstance);

            //����
            for (int i = 0; i< currentSeed.yieldAmount; i++)
            {
                GH_GameManager.instance.player.inventory.Add("Backpack", currentSeed.harvestItem);
                SoundManager.Instance.Play("SuccessSFX");
            }

            currentSeed = null;
            state = TileState.Empty;
            growTimer = 0f;
            waterPromptShown = false;

            waterIcon?.SetActive(false);
            harvestIcon?.SetActive(false);
        }
    }
    public void Interact()
    {
        FarmTilePlacer.Instance.TryInteractWithTile(this);
    }
}
