using UnityEngine;
using static Inventory;

public class Washer : MonoBehaviour,IInteractable
{
    #region ��ȣ�ۿ�
    public string GetCursorType() => "Washer";
    public string GetInteractionID() => "Washer";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    #endregion

    [SerializeField] private int useCount = 5; // ��� Ƚ��
    // �� ������
    [SerializeField] private GameObject washerWater;

    public WashingMinigame washingMinigame;
    //public Inventory inventory;
    //public InventoryManager inventoryManager;
    //public StuffData pail;

    private void Start()
    {
        useCount = 0;
    }

    public void Interact()
    {
        Slot slot = GH_GameManager.instance.player.inventory.hotbar.selectedSlot;

        if (useCount == 0)
        {
            if (slot.itemName == "WaterPail")
            {
                // �� ����
                SoundManager.Instance.Play("WaterSFX");
                washerWater.SetActive(true);
                useCount += 5;
                //GH_GameManager.instance.player.inventory.hotbar.CanRemove("WaterPail"); 
                GH_GameManager.instance.player.inventory.RemoveItem("WaterPail");
            }
        }
        else
        {
            if (PlateManager.instance.currentPlateCount > 0)
            {
                useCount--;
                washingMinigame.WashingMiniGameStart();
            }
           
            if(useCount == 0)
            {
                // �� ����
                washerWater.SetActive(false);
            }
        }
        
    }



    //public void StartWash()
    //{
    //    if (inventory.hotbar.selectedSlot.UseItem() != pail) return;

    //    count++;
    //    if (count >= 10)
    //        Destroy(gameObject);

    //}

    
}
