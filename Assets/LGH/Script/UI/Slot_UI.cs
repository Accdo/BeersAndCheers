using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 슬롯 UI를 담은 클래스
public class Slot_UI : MonoBehaviour
{
    public int slotID;
    public Inventory inventory;
    
    public Image itemIcon;
    public TextMeshProUGUI quantityText;

    public Image freshGage;

    [SerializeField] private GameObject highlight;

    public void SetItem(Inventory.Slot slot)
    {
        if (slot != null)
        {
            itemIcon.sprite = slot.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            quantityText.text = slot.count.ToString();

            if (slot.itemData is FoodData || slot.itemData is IngredientData)
            {
                ShowFreshGage(slot.freshPoint / 100f);
            }
        }
    }

    public void ShowFreshGage(float freshPoint)
    {
        // 신선도 게이지가 0 이하일 때
        if (freshPoint <= 0)
        {
            itemIcon.color = new Color(0.6f, 0, 0, 1);
            return;
        }

        freshGage.fillAmount = freshPoint;

        if (freshPoint <= 0.3f)
            freshGage.color = new Color(0.6f, 0, 0, 1);
        else if (freshPoint <= 0.6f)
            freshGage.color = new Color(0.8f, 0.25f, 0, 1);
        else
            freshGage.color = new Color(0, 0.6f, 0, 1);
    }

    public void SetEmpty()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";

        freshGage.color = new Color(0, 0.6f, 0, 0);
    }

    public void SetHighlight(bool isOn)
    {
        highlight.SetActive(isOn);
    }
}
