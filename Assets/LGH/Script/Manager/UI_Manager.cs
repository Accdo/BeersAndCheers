using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public Dictionary<string, Inventory_UI> inventoryUIByName = new Dictionary<string, Inventory_UI>();

    public GameObject inventoryPanel;
    public GameObject hotbarPanel;

    [Header("인벤토리 모음")]
    public List<Inventory_UI> inventoryUIs;

    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;


    [Header("레시피 패널")]
    public GameObject RecipePanel;
    [Header("쿠킹 패널")]
    public GameObject CookingPanel;
    public Cooking_UI cookingUI;

    [Header("상점 패널")]
    public GameObject ShopPanel;

    [Header("맥주 설치 패널")]
    public GameObject DeployPanel;
    //public GameObject DeployObjectPanel;

    [Header("상자 패널")]
    public GameObject storageboxPanel; // 상자
    public GameObject freezeboxPanel; // 냉동 상자

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

        if (Input.GetKeyDown(KeyCode.O))
        {
            // 상점 패널 생성
            ToggleShopUI();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            // 맥주 시설 패널 생성
            ToggleBeerUI();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            // 맥주 시설 패널 생성
            ToggleStorageBoxUI();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            // 냉동 상자 패널 생성
            ToggleFreezeBoxUI();
        }
    }

    public void ToggleBeerUI()
    {
        if (DeployPanel != null)
        {
            if (!DeployPanel.activeSelf)
            {
                GH_GameManager.instance.player.StartOtherWork();
                GH_GameManager.instance.player.MouseVisible(true);
                DeployPanel.SetActive(true);
            }
            else
            {
                GH_GameManager.instance.player.EndOtherWork();
                GH_GameManager.instance.player.MouseVisible(false);
                DeployPanel.SetActive(false);
                CookingPanel.SetActive(false);
               
            }
        }
    }


    public void ToggleShopUI()
    {
        if (ShopPanel != null)
        {
            if (!ShopPanel.activeSelf)
            {
                GH_GameManager.instance.player.MouseVisible(true);
                ShopPanel.SetActive(true);
            }
            else
            {
                GH_GameManager.instance.player.MouseVisible(false);
                ShopPanel.SetActive(false);
            }
        }
    }

    public void ToggleRecipeUI()
    {
        if (RecipePanel != null)
        {
            if (!RecipePanel.activeSelf)
            {
                GH_GameManager.instance.player.StartOtherWork();
                GH_GameManager.instance.player.MouseVisible(true);
                RecipePanel.SetActive(true);
            }
            else
            {
                GH_GameManager.instance.player.EndOtherWork();
                GH_GameManager.instance.player.MouseVisible(false);
                RecipePanel.SetActive(false);
                CookingPanel.SetActive(false);
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
    public void ToggleStorageBoxUI()
    {
        // 저장 박스인지 구분
        if (storageboxPanel != null)
        {
            if (!storageboxPanel.activeSelf)
            {
                GH_GameManager.instance.player.MouseVisible(true);
                storageboxPanel.SetActive(true);
                RefreshInventory("StorageBox");
            }
            else
            {
                GH_GameManager.instance.player.MouseVisible(false);
                storageboxPanel.SetActive(false);
            }
        }
    }
    public void ToggleFreezeBoxUI()
    {
        // 저장 박스인지 구분
        if (freezeboxPanel != null)
        {
            if (!freezeboxPanel.activeSelf)
            {
                GH_GameManager.instance.player.MouseVisible(true);
                freezeboxPanel.SetActive(true);
                RefreshInventory("FreezeBox");
            }
            else
            {
                GH_GameManager.instance.player.MouseVisible(false);
                freezeboxPanel.SetActive(false);
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
                GH_GameManager.instance.player.MouseVisible(true);
                inventoryPanel.SetActive(true);
                RefreshInventory("Backpack");
            }
            else
            {
                GH_GameManager.instance.player.MouseVisible(false);
                inventoryPanel.SetActive(false);
            }
        }
    }

    // 미니 게임 시작 시 핫바 UI 없애고 마우스도 안보이기
    // 미니 게임 끝나면 핫바 다시보이기
    public void ActiveHotbarUI(bool isActive = false)
    {
        // 인벤토리인지 핫바인지 구분
        if (hotbarPanel != null)
        {
            hotbarPanel.SetActive(isActive);
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