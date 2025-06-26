using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI rewardText;

    public void Setup(QuestReward reward)
    {
        string text = "";
        Sprite icon = null;

        switch (reward.type)
        {
            case RewardType.Item:
                if (reward.rewardItem != null)
                {
                    text = $"{reward.rewardItem.itemName} x{reward.itemAmount}";
                    icon = reward.rewardItem.icon;
                }
                break;
            case RewardType.Money:
                text = $"골드 +{reward.moneyAmount}";
                break;
            case RewardType.Satisfaction:
                text = $"만족도 +{reward.satisfactionBonus}";
                break;
        }

        if (rewardText != null)
            rewardText.text = text;
        if (iconImage != null)
            iconImage.sprite = icon;
    }
} 