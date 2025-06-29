using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    [SerializeField]private int money = 0;

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateGoldText();
    }
    public void RemoveMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateGoldText();
        }
    }
    public bool HasMoney(int amount)
    {
        return money >= amount;
    }
    private void UpdateGoldText()
    {
        if (moneyText != null)
        {
            moneyText.text = money.ToString() + " G";
        }
        else
        {
            Debug.LogWarning("돈 텍스트가 할당 되지 않았습니다.");
        }
    }
}
