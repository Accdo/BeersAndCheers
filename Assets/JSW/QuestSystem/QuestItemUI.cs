using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuestItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public Image questIcon;
    public Button selectButton;
    
    public event Action<QuestData, ActiveQuest> OnQuestSelected;
    
    private QuestData questData;
    private ActiveQuest activeQuest;
    
    void Start()
    {
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectClicked);
        }
    }
    
    public void Setup(QuestData quest, ActiveQuest active = null)
    {
        questData = quest;
        activeQuest = active;
        
        if (titleText != null) titleText.text = quest.questTitle;
        if (questIcon != null && quest.questIcon != null) questIcon.sprite = quest.questIcon;
    }
    
    private void OnSelectClicked()
    {
        OnQuestSelected?.Invoke(questData, activeQuest);
    }
} 