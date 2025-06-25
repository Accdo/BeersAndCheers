using UnityEngine;
using System.Collections.Generic;

public class MonsterKillTracker : MonoBehaviour
{
    public static MonsterKillTracker Instance { get; private set; }
    
    private Dictionary<EnemyData, int> killCounts = new Dictionary<EnemyData, int>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void OnMonsterKilled(EnemyData enemyData)
    {
        if (enemyData == null) return;

        if (!killCounts.ContainsKey(enemyData))
        {
            killCounts[enemyData] = 0;
        }
        
        killCounts[enemyData]++;
        
        // 퀘스트 매니저에 몬스터 사냥 이벤트 전달
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnMonsterKilled(enemyData);
        }
        
        Debug.Log($"몬스터 '{enemyData.EnemyName}' 사냥! 총 {killCounts[enemyData]}마리");
    }
    
    public int GetKillCount(EnemyData enemyData)
    {
        return (enemyData != null && killCounts.ContainsKey(enemyData)) ? killCounts[enemyData] : 0;
    }
} 