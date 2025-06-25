using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjectiveItemUI : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public Image checkMark;

    public void Setup(QuestObjective objective, int currentAmount)
    {
        if (descriptionText != null)
            descriptionText.text = objective.description;

        int required = 1;
        switch (objective.type)
        {
            case ObjectiveType.CollectItem: required = objective.requiredAmount; break;
            case ObjectiveType.KillMonster: required = objective.killCount; break;
            case ObjectiveType.CookFood:    required = objective.foodCount; break;
        }

        if (progressText != null)
            progressText.text = $"{currentAmount}/{required}";

        if (checkMark != null)
            checkMark.enabled = (currentAmount >= required);
    }
} 