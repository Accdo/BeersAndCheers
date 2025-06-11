using UnityEngine;
using System.Collections.Generic;

public class Hotbar_UI : MonoBehaviour
{
    [SerializeField] private List<Slot_UI> hotbarSlots = new List<Slot_UI>();

    private Slot_UI selectedSlot;

    private void Start()
    {
        SelectSlot(0);
    }

    private void Update()
    {
        CheckAlphaNumbericKeys();
    }

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
        }
    }

    private void CheckAlphaNumbericKeys()
    {
        for (int i = 0; i < 9; i++)
        {
            KeyCode key = (KeyCode)((int)KeyCode.Alpha1 + i);
            if (Input.GetKeyDown(key))
            {
                SelectSlot(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SelectSlot(9);
        }
    }
}
