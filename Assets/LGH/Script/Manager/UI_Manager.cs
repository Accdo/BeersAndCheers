using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, Inventory_UI> inventoryUIByName = new Dictionary<string, Inventory_UI>();

    public GameObject inventoryPanel;

    public List<Inventory_UI> inventoryUIs;

    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;

    public GameObject RecipePanel;
    public GameObject CookingPanel;

    public Cooking_UI cookingUI;

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventoryUI();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            dragSingle = true;
        }
        else
        {
            dragSingle = false;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            // 레시피 시설 패널 생성
            ToggleRecipeUI();
        }
    }

    public void ToggleRecipeUI()
    {
        if (RecipePanel != null)
        {
            if (!RecipePanel.activeSelf)
            {
                RecipePanel.SetActive(true);
            }
            else
            {
                RecipePanel.SetActive(false);
            }
        }
    }

    public void ToggleCookingUI()
    {
        if (CookingPanel != null)
        {
            Debug.Log("요리 패널 토글");
            if (!CookingPanel.activeSelf)
            {
                CookingPanel.SetActive(true);
                // 레시피 선택했다는 사실을 저장

                // 요리 패널 Refresh
                cookingUI.Refresh();
            }
            else
            {
                CookingPanel.SetActive(false);
            }
        }
    }

    public void ToggleInventoryUI()
    {
        // 인벤토리인지 핫바인지 구분
        if (inventoryPanel != null)
        {
            if (!inventoryPanel.activeSelf)
            {
                inventoryPanel.SetActive(true);
                RefreshInventory("Backpack");
            }
            else
            {
                inventoryPanel.SetActive(false);
            }
        }
    }

    public void RefreshInventory(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            inventoryUIByName[inventoryName].Refresh();
        }
    }

    public void RefreshAll()
    {
        foreach (KeyValuePair<string, Inventory_UI> keyValuePair in inventoryUIByName)
        {
            keyValuePair.Value.Refresh();
        }
    }

    public Inventory_UI GetInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            return inventoryUIByName[inventoryName];
        }

        return null;
    }

    private void Initialize()
    {
        foreach (Inventory_UI ui in inventoryUIs)
        {
            if (!inventoryUIByName.ContainsKey(ui.inventoryName))
            {
                inventoryUIByName.Add(ui.inventoryName, ui);
            }
        }
    }
}