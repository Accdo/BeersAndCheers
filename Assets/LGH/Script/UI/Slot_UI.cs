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
                //Debug.Log("Fresh Point: " + slot.freshPoint / 100f);
                ShowFreshGage(slot.freshPoint / 100f);
            }
        }
    }

    public void ShowFreshGage(float freshPoint)
    {
        if (freshPoint <= 0)
        {
            itemIcon.color = new Color(0.6f, 0, 0, 1);
            return;
        }
        freshGage.fillAmount = freshPoint;
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
