using UnityEngine;
using static Inventory;

public class Washer : MonoBehaviour,IInteractable
{
    #region 상호작용
    public string GetCursorType() => "Washer";
    public string GetInteractionID() => "Washer";
    public InteractionType GetInteractionType() => InteractionType.MiniGame;
    #endregion

    [SerializeField] private int useCount = 5; // 사용 횟수
    // 물 프리팹
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
                // 물 생성
                SoundManager.Instance.Play("WaterSFX");
                washerWater.SetActive(true);
                useCount += 5;
                GH_GameManager.instance.player.inventory.hotbar.CanRemove("WaterPail"); 
            }
        }
        else
        {
            useCount--;
            washingMinigame.WashingMiniGameStart();
            if(useCount == 0)
            {
                // 물 삭제
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
