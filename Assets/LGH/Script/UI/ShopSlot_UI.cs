using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopSlot_UI : MonoBehaviour
{
    private Item item; // 아이템 데이터

    public Image itemImage;
    public TextMeshProUGUI itemNameText; // 아이템 이름 텍스트
    public TextMeshProUGUI itemPriceText; // 아이템 가격 텍스트

    void Start()
    {
        item = GetComponent<Item>();

        if (item != null)
        {
            // 아이템 이름과 가격을 UI에 설정
            itemImage.sprite = item.data.icon;
            itemNameText.text = item.data.name;
            itemPriceText.text = "$ " + item.data.price.ToString();
        }
    }

    void Update()
    {
        
    }
}
