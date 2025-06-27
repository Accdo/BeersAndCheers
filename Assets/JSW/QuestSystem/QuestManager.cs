using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

/// <summary>
/// 퀘스트 전체 관리(진행, 완료, UI 연동 등)
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    private InventoryManager inventory;
    private CustomerAI questGiver = null; // 퀘스트를 제공할 손님

    [Header("퀘스트 풀")]
    public List<QuestData> availableQuests = new List<QuestData>(); // 사용 가능한 퀘스트 목록

    // 현재 진행 중인 퀘스트
    private List<ActiveQuest> activeQuests = new List<ActiveQuest>();
    // 완료된 퀘스트
    private List<CompletedQuest> completedQuests = new List<CompletedQuest>();
    public CompletedQuest GetCompletedQuest(string questID) => completedQuests.FirstOrDefault(q => q.questData.questID == questID);
    // UI 연동용(선택)
    public QuestUI questUI;

    // 이벤트(필요시)
    public Action OnQuestAdded;
    public Action OnQuestCompleted;
    public Action OnQuestFailed;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inventory = FindAnyObjectByType<InventoryManager>();
        if (inventory == null)
        {
            Debug.LogError("InventoryManager not found in the scene!");
        }
    }

    // 랜덤 퀘스트 선택
    public QuestData GetRandomQuest()
    {
        if (this.availableQuests == null || this.availableQuests.Count == 0)
        {
            Debug.LogWarning("사용 가능한 퀘스트가 없습니다!");
            return null;
        }
        // 이미 진행 중인 퀘스트의 ID 목록을 가져옴
        var activeQuestIDs = activeQuests.Select(q => q.questData.questID).ToList();

        // 사용 가능한 퀘스트 목록에서 이미 진행 중인 퀘스트를 제외
        var questPool = this.availableQuests.Where(q => !activeQuestIDs.Contains(q.questID)).ToList();

        if (questPool.Count == 0)
        {
            Debug.LogWarning("모든 퀘스트가 이미 진행 중입니다!");
            return null;
        }

        // 랜덤 선택
        int randomIndex = UnityEngine.Random.Range(0, questPool.Count);
        return questPool[randomIndex];
    }
    public bool HasAvailableQuest()
    {
        if (this.availableQuests == null || this.availableQuests.Count == 0)
        {
            Debug.LogError("[QuestManager] 퀘스트 생성 실패: 'Available Quests' 리스트가 비어있습니다! 인스펙터에서 퀘스트 데이터를 할당해주세요.");
            return false;
        }

        var activeQuestIDs = activeQuests.Select(q => q.questData.questID).ToList();
        bool hasQuest = this.availableQuests.Any(q => !activeQuestIDs.Contains(q.questID));

        if (!hasQuest)
        {
            Debug.LogWarning($"[QuestManager] 퀘스트 생성 실패: 모든 퀘스트가 이미 진행 중입니다. (진행중인 퀘스트 수: {activeQuests.Count})");
        }

        return hasQuest;
    }

    // 퀘스트 시작 (랜덤 퀘스트 자동 선택)
    public bool StartRandomQuest(CustomerAI customer)
    {
        QuestData randomQuest = GetRandomQuest();
        if (randomQuest != null)
        {
            StartQuest(randomQuest, customer);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartQuest(QuestData questData, CustomerAI customer = null)
    {
        if (IsQuestActive(questData.questID))
            return;

        // 퀘스트를 시작하면, 더 이상 퀘스트를 줄 손님이 필요 없으므로 null 처리
        questGiver = null;

        var active = new ActiveQuest
        {
            questData = questData,
            customer = customer,
            startTime = Time.time,
            objectives = questData.objectives.Select(obj => new QuestObjectiveProgress
            {
                objectiveID = obj.objectiveID,
                currentAmount = 0,
                requiredAmount = GetRequiredAmount(obj),
                isCompleted = false
            }).ToList()
        };

        activeQuests.Add(active);
        
        // 손님에게 퀘스트를 성공적으로 주었음을 알림
        if (customer != null)
        {
            customer.ConfirmQuestGiven();
        }
        
        // 퀘스트 시작 시 현재 인벤토리에 있는 아이템들을 체크하여 목표 진행도 업데이트
        CheckCurrentInventoryForObjectives(active);
        
        OnQuestAdded?.Invoke();
        questUI?.UpdateQuestList();
    }

    // 퀘스트 시작 시 현재 인벤토리에 있는 아이템들을 체크하여 목표 진행도 업데이트
    private void CheckCurrentInventoryForObjectives(ActiveQuest quest)
    {
        if (inventory == null) return;
        
        foreach (var obj in quest.objectives)
        {
            var def = quest.questData.objectives.Find(o => o.objectiveID == obj.objectiveID);
            if (def != null && def.type == ObjectiveType.CollectItem && def.requiredItem != null)
            {
                int currentCount = GetItemCountFromInventory(def.requiredItem.itemName);
                
                obj.currentAmount = Mathf.Min(currentCount, obj.requiredAmount);
                
                if (obj.currentAmount >= obj.requiredAmount)
                    obj.isCompleted = true;
                    
                Debug.Log($"퀘스트 시작 시 {def.requiredItem.itemName} {currentCount}개 발견, 목표: {obj.requiredAmount}개");
            }
        }
    }

    // 인벤토리에서 특정 아이템의 개수를 직접 계산
    private int GetItemCountFromInventory(string itemName)
    {
        if (inventory == null) return 0;
        
        int totalCount = 0;
        
        // 백팩에서 검색
        if (inventory.backpack != null)
        {
            foreach (var slot in inventory.backpack.slots)
            {
                if (slot.itemName == itemName)
                {
                    totalCount += slot.count;
                }
            }
        }
        
        // 핫바에서 검색
        if (inventory.hotbar != null)
        {
            foreach (var slot in inventory.hotbar.slots)
            {
                if (slot.itemName == itemName)
                {
                    totalCount += slot.count;
                }
            }
        }
        
        return totalCount;
    }

    // 퀘스트 완료 조건을 체크하고 완료되면 자동으로 완료 처리
    public void CheckQuestCompletion()
    {
        // 복사본을 만들어서 순회 (중간에 activeQuests가 변경될 수 있으므로)
        var questsToCheck = activeQuests.ToList();

        foreach (var quest in questsToCheck)
        {
            if (IsQuestComplete(quest))
            {
                // 이미 완료된 퀘스트는 다시 처리하지 않음
                if (!completedQuests.Any(q => q.questData.questID == quest.questData.questID))
                {
                    CompleteQuest(quest.questData);
                }
            }
        }
    }

    // 퀘스트 완료 시 호출
    public void CompleteQuest(QuestData quest)
    {
        var questToComplete = GetActiveQuest(quest.questID);
        if (questToComplete == null) return; // 이미 완료된 퀘스트는 무시
        if (!IsQuestComplete(questToComplete)) return;

        activeQuests.Remove(questToComplete);

        RemoveQuestRequirements(questToComplete);
        GiveRewards(quest.rewards);

        completedQuests.Add(new CompletedQuest
        {
            questData = quest,
            customer = questToComplete.customer,
            completionTime = Time.time
        });

        OnQuestCompleted?.Invoke();
        questUI?.UpdateQuestList();

        if (GH_GameManager.instance.uiManager != null)
        {
            GH_GameManager.instance.uiManager.RefreshAll();
        }
      
    }

    // 퀘스트 완료 시 요구사항 아이템을 인벤토리에서 안전하게 제거
    private void RemoveQuestRequirements(ActiveQuest quest)
    {
        if (inventory == null) return;

        foreach (var obj in quest.objectives)
        {
            var def = quest.questData.objectives.Find(o => o.objectiveID == obj.objectiveID);
            if (def != null && def.type == ObjectiveType.CollectItem && def.requiredItem != null)
            {
                int amountToRemove = obj.requiredAmount;
                int actuallyRemoved = 0;

                for (int i = 0; i < amountToRemove; i++)
                {
                    int before = GetItemCountFromInventory(def.requiredItem.itemName);
                    inventory.RemoveItem(def.requiredItem.itemName);
                    int after = GetItemCountFromInventory(def.requiredItem.itemName);

                    if (after < before)
                        actuallyRemoved++;
                    else
                        break; // 더 이상 제거 불가
                }

                Debug.Log($"퀘스트 완료: {def.requiredItem.itemName} {actuallyRemoved}/{amountToRemove}개 제거됨");

                // 🔥 아이템이 부족하면 퀘스트 완료 중단!
                if (actuallyRemoved < amountToRemove)
                {
                    // 목표 미달성 처리
                    obj.isCompleted = false;
                    obj.currentAmount = GetItemCountFromInventory(def.requiredItem.itemName);
                    return; // 퀘스트 완료 중단
                }
            }
        }

        // UI 새로고침
        if (GH_GameManager.instance?.uiManager != null)
        {
            GH_GameManager.instance.uiManager.RefreshAll();
        }

        // 퀘스트 진행도 업데이트는 여기서 한 번만!
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UpdateAllQuestProgressFromInventory();
        }
    }

    // 퀘스트 취소
    public void CancelQuest(string questID)
    {
        var quest = GetActiveQuest(questID);
        if (quest == null) return;

        // 퀘스트 취소 시, 퀘스트 제공자도 초기화 (다시 퀘스트를 줄 수 있게)
        if (quest.customer == questGiver)
        {
            questGiver = null;
        }

        activeQuests.Remove(quest);
        OnQuestFailed?.Invoke();
        questUI?.UpdateQuestList();
    }

    // 이 손님이 퀘스트를 줄 수 있는지 확인하고 설정
    public bool TrySetQuestGiver(CustomerAI customer)
    {
        // 이 손님이 이미 활성화된 퀘스트를 줬는지 확인
        if (activeQuests.Any(q => q.customer == customer))
        {
            return false;
        }

        // 이미 퀘스트 줄 손님이 있거나, 가능한 퀘스트가 없으면 실패
        if (questGiver != null || !HasAvailableQuest())
        {
            return false;
        }

        questGiver = customer;
        return true;
    }

    // 아이템 수집/교환 목표 진행
    public void OnItemCollected(ItemData item, int amount = 1)
    {
        bool updated = false;
        
        foreach (var quest in activeQuests)
        {
            foreach (var obj in quest.objectives)
            {
                var def = quest.questData.objectives.Find(o => o.objectiveID == obj.objectiveID);
                if (def != null && def.type == ObjectiveType.CollectItem && def.requiredItem == item)
                {
                    obj.currentAmount += amount;
                    if (obj.currentAmount >= obj.requiredAmount)
                        obj.isCompleted = true;
                    updated = true;
                    
                    Debug.Log($"퀘스트 진행도 업데이트: {item.itemName} {obj.currentAmount}/{obj.requiredAmount}");
                }
            }
        }
        
        if (updated)
        {
            questUI?.UpdateQuestList();
            CheckQuestCompletion(); // 완료 조건 체크
        }
    }
    // 인벤토리에서 아이템 개수를 직접 체크하여 퀘스트 진행도 업데이트
    public void UpdateQuestProgressFromInventory()
    {
        if (inventory == null) return;
        
        foreach (var quest in activeQuests)
        {
            foreach (var obj in quest.objectives)
            {
                var def = quest.questData.objectives.Find(o => o.objectiveID == obj.objectiveID);
                if (def != null && def.type == ObjectiveType.CollectItem && def.requiredItem != null)
                {
                    // ItemManager를 통해 Item 객체를 가져와서 GetItemCount 사용
                    Item item = GH_GameManager.instance?.itemManager?.GetItemByName(def.requiredItem.itemName);
                    if (item != null)
                    {
                        int currentCount = inventory.GetItemCount(item, 0);
                        if (currentCount == -1) // -1이면 해당 아이템이 없음
                        {
                            currentCount = 0;
                        }
                        
                        obj.currentAmount = Mathf.Min(currentCount, obj.requiredAmount);
                        
                        if (obj.currentAmount >= obj.requiredAmount)
                            obj.isCompleted = true;
                    }
                }
            }
        }
        
        questUI?.UpdateQuestList();
        CheckQuestCompletion();
    }

    // 몬스터 사냥 목표 진행
    public void OnMonsterKilled(EnemyData killedEnemy, int amount = 1)
    {
        foreach (var quest in activeQuests)
        {
            foreach (var obj in quest.objectives)
            {
                var def = quest.questData.objectives.Find(o => o.objectiveID == obj.objectiveID);
                if (def != null && def.type == ObjectiveType.KillMonster && def.enemyData == killedEnemy)
                {
                    obj.currentAmount += amount;
                    if (obj.currentAmount >= obj.requiredAmount)
                        obj.isCompleted = true;
                }
            }
        }
        questUI?.UpdateQuestList();
    }

    // 인벤토리 변화 시 모든 퀘스트 진행도를 다시 계산
    public void UpdateAllQuestProgressFromInventory()
    {
        if (inventory == null) return;
        
        foreach (var quest in activeQuests)
        {
            foreach (var obj in quest.objectives)
            {
                var def = quest.questData.objectives.Find(o => o.objectiveID == obj.objectiveID);
                if (def != null && def.type == ObjectiveType.CollectItem && def.requiredItem != null)
                {
                    // 현재 인벤토리에서 실제 개수를 다시 계산
                    int currentCount = GetItemCountFromInventory(def.requiredItem.itemName);
                    
                    obj.currentAmount = Mathf.Min(currentCount, obj.requiredAmount);
                    
                    if (obj.currentAmount >= obj.requiredAmount)
                        obj.isCompleted = true;
                }
            }
        }
        
        questUI?.UpdateQuestList();
        CheckQuestCompletion();
    }

    // ======= 보조 함수 =======

    public List<ActiveQuest> GetActiveQuests() => activeQuests;
    public bool IsQuestActive(string questID) => activeQuests.Any(q => q.questData.questID == questID);
    public ActiveQuest GetActiveQuest(string questID) => activeQuests.FirstOrDefault(q => q.questData.questID == questID);
    public bool IsQuestComplete(ActiveQuest quest) => quest.objectives.All(o => o.isCompleted);

    private int GetRequiredAmount(QuestObjective obj)
    {
        switch (obj.type)
        {
            case ObjectiveType.CollectItem: return obj.requiredAmount;
            case ObjectiveType.KillMonster: return obj.killCount;
            case ObjectiveType.CookFood: return obj.foodCount;
            default: return 1;
        }
    }

    private void GiveRewards(List<QuestReward> rewards)
    {
        foreach (var reward in rewards)
        {
            switch (reward.type)
            {
                case RewardType.Item:
                    if (reward.rewardItem != null)
                    {
                        for (int i = 0; i < reward.itemAmount; i++)
                        {
                            inventory?.Add("Backpack", GH_GameManager.instance.itemManager.GetItemByName(reward.rewardItem.itemName));
                        }
                    }
                    break;
                case RewardType.Money:
                    if (GH_GameManager.instance != null && GH_GameManager.instance.goldManager != null)
                    {
                        GH_GameManager.instance.goldManager.AddMoney(reward.moneyAmount);
                    }
                    break;
                case RewardType.Satisfaction:
                    break;
                case RewardType.UnlockFood:
                    break;
            }
        }
    }


}

// ======= 퀘스트 진행/완료 데이터 구조 =======

[System.Serializable]
public class ActiveQuest
{
    public QuestData questData;
    public CustomerAI customer;
    public float startTime;
    public List<QuestObjectiveProgress> objectives = new List<QuestObjectiveProgress>();
}

[System.Serializable]
public class QuestObjectiveProgress
{
    public string objectiveID;
    public int currentAmount;
    public int requiredAmount;
    public bool isCompleted;
}

[System.Serializable]
public class CompletedQuest
{
    public QuestData questData;
    public CustomerAI customer;
    public float completionTime;
}