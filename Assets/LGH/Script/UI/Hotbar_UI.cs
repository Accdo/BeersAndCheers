using UnityEngine;
using System.Collections.Generic;

public class Hotbar_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> hotbarSlots = new List<Slot_UI>();

    // 현재 선택한 슬롯
    private Slot_UI selectedSlot;

    private int selectedSlotNumber = -1;

    private void Start()
    {
        SelectSlot(0);
    }

    private void Update()
    {
        CheckAlphaNumbericKeys();
    }

    public void SelectSlot(Slot_UI slot)
    {
        SelectSlot(slot.slotID);
    }

    // 입력한 번호의 슬롯을 선택
    public void SelectSlot(int index)
    {
        if (hotbarSlots.Count == 10)
        {
            if (selectedSlot != null)
            {
                selectedSlot.SetHighlight(false);
            }
            selectedSlot = hotbarSlots[index];
            selectedSlot.SetHighlight(true);

            GH_GameManager.instance.player.inventory.hotbar.SelectSlot(index);

            // 선택한 슬롯이 기존 슬롯과 다를경우 아이템 생성 X
            //if (selectedSlotNumber != index)
                GH_GameManager.instance.player.EquipItem();

            //selectedSlotNumber = index;
        }
    }

    // 키 입력에 따른 슬롯 선택 함수 호출
    private void CheckAlphaNumbericKeys()
    {
        // 1 ~ 9 키
        for (int i = 0; i < 9; i++)
        {
            KeyCode key = (KeyCode)((int)KeyCode.Alpha1 + i);
            if (Input.GetKeyDown(key))
            {
                SelectSlot(i);
            }
        }
        // 0 키
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SelectSlot(9);
        }
    }
}
