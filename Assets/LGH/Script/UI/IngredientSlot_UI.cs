using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientSlot_UI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI quantityText;

    public void SetItem(IngredientData itemData, int playerItemCount, int requiredItemCount)
    {
        if (itemData != null)
        {
            itemIcon.sprite = itemData.icon;
            itemIcon.color = new Color(1, 1, 1, 1);
            quantityText.text = playerItemCount + " / " + requiredItemCount;
            // 현재 플레이어 아이템 갯수 + " / " + 요리에 필요한 아이템 개수
        }
    }

    public void SetEmpty()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        quantityText.text = "";
    }
}
