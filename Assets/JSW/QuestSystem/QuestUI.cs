using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject questPanel;
    public Transform questListContent;
    public GameObject questItemPrefab;
    public GameObject questDetailPanel;
    
    [Header("퀘스트 상세 정보")]
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescriptionText;
    public Transform objectiveListContent;
    public GameObject objectiveItemPrefab;
    public Transform rewardListContent;
    public GameObject rewardItemPrefab;
    public Button acceptButton;
    public Button declineButton; // 거절 버튼
    public Button completeButton;
    public Button cancelButton;
    
    [Header("퀘스트 알림")]
    public GameObject questNotification;
    public TextMeshProUGUI notificationText;
    
    private QuestManager questManager;
    private QuestData selectedQuest;
    private CustomerAI currentQuestGiver; // 퀘스트를 제안한 손님
    private List<GameObject> questItems = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        questManager = QuestManager.Instance;
        if (questManager != null)
        {
            questManager.OnQuestAdded += UpdateQuestList;
            questManager.OnQuestCompleted += UpdateQuestList;
            questManager.OnQuestFailed += UpdateQuestList;
        }
        
        // 버튼 이벤트 연결
        acceptButton?.onClick.AddListener(AcceptQuest);
        declineButton?.onClick.AddListener(DeclineQuest);
        completeButton?.onClick.AddListener(CompleteQuest);
        cancelButton?.onClick.AddListener(CancelQuest);
        
        // 초기 상태 설정
        questPanel?.SetActive(false);
        questDetailPanel?.SetActive(false);
        questNotification?.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleQuestPanel();
        }
    }
    
    public void ToggleQuestPanel()
    {
        if (questPanel == null) return;
        
        bool isActive = !questPanel.activeSelf;
        questPanel.SetActive(isActive);
        
        // 마우스 커서 표시/숨김과 함께 플레이어의 다른 작업 상태도 제어
        if (isActive)
        {
            GH_GameManager.instance.player.MouseVisible(true);
            GH_GameManager.instance.player.StartOtherWork(); // 카메라 회전 비활성화
            UpdateQuestList();
            questDetailPanel.SetActive(false); // 퀘스트창 열 때 항상 상세정보는 닫음
        }
        else
        {
            GH_GameManager.instance.player.MouseVisible(false);
            GH_GameManager.instance.player.EndOtherWork(); // 카메라 회전 다시 활성화
        }
    }
    
    public void UpdateQuestList()
    {
        if (questListContent == null || questItemPrefab == null) return;
        
        foreach (var item in questItems)
        {
            if (item != null) Destroy(item);
        }
        questItems.Clear();
        
        foreach (var activeQuest in questManager.GetActiveQuests())
        {
            GameObject questItem = Instantiate(questItemPrefab, questListContent);
            QuestItemUI itemUI = questItem.GetComponent<QuestItemUI>();
            
            if (itemUI != null)
            {
                itemUI.Setup(activeQuest.questData, activeQuest);
                itemUI.OnQuestSelected += (questData, quest) => ShowQuestDetail(questData, quest, quest.customer);
            }
            
            questItems.Add(questItem);
        }
    }
    
    public void ShowQuestDetail(QuestData questData, ActiveQuest activeQuest, CustomerAI questGiver)
    {
        selectedQuest = questData;
        currentQuestGiver = questGiver;
        
        if (questDetailPanel == null) return;
        questDetailPanel.SetActive(true);
        
        questTitleText.text = questData.questTitle;
        questDescriptionText.text = questData.questDescription;
        
        UpdateObjectiveList(questData, activeQuest);
        UpdateRewardList(questData);
        
        bool isCompleted = activeQuest != null && questManager.IsQuestComplete(activeQuest);
        UpdateButtonStates(activeQuest, isCompleted);
    }
    
    private void UpdateObjectiveList(QuestData questData, ActiveQuest activeQuest)
    {
        if (objectiveListContent == null || objectiveItemPrefab == null) return;
        
        foreach (Transform child in objectiveListContent)
        {
            Destroy(child.gameObject);
        }

        if (questData.objectives.Count == 0)
        {
            Debug.LogWarning($"[QuestUI] 퀘스트 '{questData.questTitle}'에 목표(Objectives)가 설정되지 않았습니다.");
        }
        
        for (int i = 0; i < questData.objectives.Count; i++)
        {
            var objective = questData.objectives[i];
            GameObject objItem = Instantiate(objectiveItemPrefab, objectiveListContent);
            ObjectiveItemUI objUI = objItem.GetComponent<ObjectiveItemUI>();
            
            if (objUI != null)
            {
                int currentAmount = (activeQuest != null && i < activeQuest.objectives.Count) ? activeQuest.objectives[i].currentAmount : 0;
                objUI.Setup(objective, currentAmount);
            }
        }
    }
    
    private void UpdateRewardList(QuestData questData)
    {
        if (rewardListContent == null || rewardItemPrefab == null) return;
        
        foreach (Transform child in rewardListContent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var reward in questData.rewards)
        {
            GameObject rewardItem = Instantiate(rewardItemPrefab, rewardListContent);
            RewardItemUI rewardUI = rewardItem.GetComponent<RewardItemUI>();
            rewardUI?.Setup(reward);
        }
    }
    
    private void UpdateButtonStates(ActiveQuest activeQuest, bool isCompleted)
    {
        bool isAccepting = activeQuest == null;

        acceptButton?.gameObject.SetActive(isAccepting);
        declineButton?.gameObject.SetActive(isAccepting);

        // 완료 버튼은 항상 보이게, 조건 충족 시에만 활성화
        completeButton?.gameObject.SetActive(!isAccepting);
        if (completeButton != null)
            completeButton.interactable = isCompleted;

        // 포기 버튼은 진행 중이면 항상 보이게
        cancelButton?.gameObject.SetActive(!isAccepting);
    }
    
    private void AcceptQuest()
    {
        if (selectedQuest == null) return;

        questManager.StartQuest(selectedQuest, currentQuestGiver);
        
        if (currentQuestGiver != null)
        {
            currentQuestGiver.HideSpecialRequestImage();
            currentQuestGiver.ShowSpecialRequestTalkedImage();
        }
        
        questDetailPanel.SetActive(false);
        UpdateQuestList();
        ShowQuestNotification($"퀘스트 '{selectedQuest.questTitle}' 수락!");
    }
    
    private void DeclineQuest()
    {
        currentQuestGiver?.CustormerExit();
        questDetailPanel.SetActive(false);
        ShowQuestNotification("퀘스트를 거절했습니다.");
    }
    
    private void CompleteQuest()
    {
        if (selectedQuest == null) return;

        var activeQuest = questManager.GetActiveQuest(selectedQuest.questID);
        if (activeQuest == null) return;
        
        var questGiver = activeQuest.customer;
        
        questManager.CompleteQuest(selectedQuest);
        questGiver?.CustormerExit();
        
        questDetailPanel.SetActive(false);
        UpdateQuestList();
        ShowQuestNotification("퀘스트 완료: " + selectedQuest.questTitle);
    }
    
    private void CancelQuest()
    {
        if (selectedQuest == null) return;

        var activeQuest = questManager.GetActiveQuest(selectedQuest.questID);
        var questGiver = activeQuest?.customer;
        questGiver?.CustormerExit();

        questManager.CancelQuest(selectedQuest.questID);
        questDetailPanel.SetActive(false);
        UpdateQuestList();
        ShowQuestNotification($"퀘스트 '{selectedQuest.questTitle}' 취소!");
    }
    
    public void ShowQuestNotification(string message)
    {
        if (questNotification == null)
        {
            Debug.LogError("[QuestUI] 퀘스트 알림 실패: 'Quest Notification' GameObject가 인스펙터에 할당되지 않았습니다!");
            return;
        }
        if (notificationText == null)
        {
            Debug.LogError("[QuestUI] 퀘스트 알림 실패: 'Notification Text' 컴포넌트가 인스펙터에 할당되지 않았습니다!");
            return;
        }

        if (notificationText != null) notificationText.text = message;
        if (questNotification != null) questNotification.SetActive(true);
        
        CancelInvoke(nameof(HideQuestNotification));
        Invoke(nameof(HideQuestNotification), 3f);
    }
    
    private void HideQuestNotification()
    {
        if (questNotification != null)
        {
            questNotification.SetActive(false);
        }
    }
} 