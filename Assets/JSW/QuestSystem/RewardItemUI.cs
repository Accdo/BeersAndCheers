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
            case RewardType.UnlockFood:
                if (reward.unlockFood != null)
                {
                    text = $"<color=#FFD700><b>{reward.unlockFood.itemName}\n레시피 해금!</b></color>";
                    icon = reward.unlockFood.icon;
                }
                break;
        }

        if (rewardText != null)
            rewardText.text = text;
        if (iconImage != null)
            iconImage.sprite = icon;
    }
} 