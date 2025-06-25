using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("퀘스트 기본 정보")]
    public string questID;
    public string questTitle;
    [TextArea(3, 5)]
    public string questDescription;
    public Sprite questIcon;
    
    [Header("퀘스트 타입")]
    public QuestType questType;
    
    [Header("퀘스트 목표")]
    public List<QuestObjective> objectives = new List<QuestObjective>();
    
    [Header("퀘스트 보상")]
    public List<QuestReward> rewards = new List<QuestReward>();
    
    [Header("퀘스트 설정")]
    public bool isRepeatable = false;
    public float timeLimit = 0f; // 0이면 제한 없음
    public int requiredLevel = 0;
    
    [Header("퀘스트 대화")]
    public DialogueScript questStartDialogue;
    public DialogueScript questCompleteDialogue;
    public DialogueScript questFailDialogue;

    /// <summary>
    /// 인스펙터에서 값이 변경될 때 모든 목표의 Description 업데이트
    /// </summary>
    private void OnValidate()
    {
        if (objectives != null)
        {
            // 목표 설명 자동 업데이트
            foreach (var objective in objectives)
            {
                if (objective != null)
                {
                    objective.UpdateDescription();
                }
            }

            // 첫 번째 목표의 아이콘을 퀘스트 아이콘으로 자동 설정
            // (몬스터 사냥 퀘스트는 EnemyData에 아이콘이 없어 수동 설정 필요)
            if (objectives.Count > 0 && objectives[0] != null)
            {
                var firstObjective = objectives[0];
                switch (firstObjective.type)
                {
                    case ObjectiveType.CollectItem:
                        if (firstObjective.requiredItem != null)
                            questIcon = firstObjective.requiredItem.icon;
                        break;
                    
                    // 몬스터 사냥은 EnemyData에 아이콘이 없으므로 자동 할당에서 제외
                    // case ObjectiveType.KillMonster:
                    //     break;
                        
                    case ObjectiveType.CookFood:
                        if (firstObjective.requiredFood != null)
                            questIcon = firstObjective.requiredFood.icon;
                        break;
                }
            }
        }
    }
}

[System.Serializable]
public class QuestObjective
{
    public string objectiveID;
    [HideInInspector]
    public string description;
    public ObjectiveType type;
    
    [Header("재료 교환 목표")]
    public ItemData requiredItem;
    public int requiredAmount = 1;
    
    [Header("몬스터 사냥 목표")]
    public EnemyData enemyData;
    public int killCount = 1;
    
    [Header("음식 제작 목표")]
    public FoodData requiredFood;
    public int foodCount = 1;

    public void UpdateDescription()
    {
        switch (type)
        {
            case ObjectiveType.CollectItem:
                if (requiredItem != null)
                {
                    description = $"{requiredItem.itemName} {requiredAmount}개 수집";
                }
                else
                {
                    description = "아이템 수집";
                }
                break;
                
            case ObjectiveType.KillMonster:
                if (enemyData != null)
                {
                    description = $"{enemyData.EnemyName} {killCount}마리 처치";
                }
                else
                {
                    description = "몬스터 처치";
                }
                break;
                
            case ObjectiveType.CookFood:
                if (requiredFood != null)
                {
                    description = $"{requiredFood.itemName} {foodCount}개 제작";
                }
                else
                {
                    description = "음식 제작";
                }
                break;
                
            default:
                description = "목표 달성";
                break;
        }
    }

    public void OnValidate()
    {
        UpdateDescription();
    }
}

[System.Serializable]
public class QuestReward
{
    public RewardType type;
    public ItemData rewardItem;
    public int itemAmount = 1;
    public int moneyAmount = 0;
    public float satisfactionBonus = 0f;
}

public enum QuestType
{
    ItemExchange,    // 재료 물물교환
    MonsterHunt,     // 몬스터 사냥
    FoodCooking      // 음식 제작
}

public enum ObjectiveType
{
    CollectItem,     // 아이템 수집/교환
    KillMonster,     // 몬스터 사냥
    CookFood         // 음식 제작
}

public enum RewardType
{
    Item,
    Money,
    Satisfaction
} 